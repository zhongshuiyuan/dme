using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.Model.Entity;
using Newtonsoft.Json.Linq;
using NLog;
using SqlSugar;
using System;
using System.Collections.Generic;

namespace Dist.Dme.RuleSteps
{
    /// <summary>
    /// 规则步骤元数据基类
    /// </summary>
    public abstract class BaseRuleStepMeta
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        // public abstract IRuleStepType RuleStepType { get; }
        // protected abstract EnumRuleStepTypes MyRuleStepType { get; }
        public abstract object InParams { get; }
        protected IRepository repository;
        protected DmeRuleStep step;
        //protected int modelId;
        //protected int versionId;
        //protected int ruleStepId;
        /// <summary>
        /// 数据源id
        /// </summary>
        protected string Source { get; set; }
        /// <summary>
        /// 真正在运行的时候，step才会被使用到
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="step"></param>
        public BaseRuleStepMeta(IRepository repository, DmeRuleStep step)
        {
            this.repository = repository;
            this.step = step;
        }
        /// <summary>
        /// 步骤需要的输入参数
        /// </summary>
        protected IDictionary<String, Property> InputParameters { get; set; } = new Dictionary<String, Property>() {
            [nameof(Source)] = new Property(nameof(Source), "数据源唯一Code", EnumValueMetaType.TYPE_STRING, "", "", "数据源唯一Code")
        };
        /// <summary>
        /// 获取步骤类型元数据
        /// </summary>
        /// <param name="enumRuleStepTypes"></param>
        /// <returns></returns>
        //protected object GetRuleStepTypeMeta(EnumRuleStepTypes enumRuleStepTypes)
        //{
        //    return new Dictionary<string, object>
        //    {
        //        ["value"] = (int)enumRuleStepTypes,
        //        ["name"] = EnumUtil.GetEnumDisplayName(enumRuleStepTypes),
        //        ["desc"] = EnumUtil.GetEnumDescription(enumRuleStepTypes)
        //    };
        //}
        /// <summary>
        /// 通用读取属性
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, Property> ReadAttributes()
        {
            IList<DmeRuleStepAttribute> attributes = repository.GetDbContext().Queryable<DmeRuleStepAttribute>().Where(rsa => rsa.RuleStepId == this.step.Id).ToList();
            if (attributes?.Count > 0)
            {
                IDictionary<string, Property> dictionary = new Dictionary<string, Property>();
                foreach (var item in attributes)
                {
                    dictionary[item.AttributeCode] = new Property(item.AttributeCode, item.AttributeCode, default(EnumValueMetaType), item.AttributeValue);
                }
                return dictionary;
            }
            return null;
        }

        public bool SaveAttributes(IDictionary<string, Property> attributes)
        {
            if (attributes?.Count == 0)
            {
                LOG.Info("属性个数为 0，不再执行后面的操作。");
                return true;
            }
            var db = repository.GetDbContext();
           
            if (0 == attributes?.Count)
            {
                return false;
            }
            // 先删除这个步骤的属性，再重新添加
            Boolean result = db.Ado.UseTran<Boolean>(() =>
            {
                // 删除的影响条目
                int deleteCount = db.Deleteable<DmeRuleStepAttribute>().Where(rsa => rsa.RuleStepId == this.step.Id).ExecuteCommand();
                LOG.Info($"删除规则[{this.step.Id}]下的{deleteCount} 条属性记录");
                // 保存步骤属性
                //DmeRuleStep dmeRuleStep = db.Queryable<DmeRuleStep>().Single(rs => rs.Id == this.step.Id);
                //if (null == dmeRuleStep)
                //{
                //    throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"规则[{this.step.Id}]没有找到数据库记录");
                //}
                // 不使用接口IList，Insertable<>批量插入存在问题
                List<DmeRuleStepAttribute> attributeEntities = new List<DmeRuleStepAttribute>();
                // 去重
                ISet<string> datasources = new HashSet<string>();
                foreach (var item in attributes)
                {
                    if (nameof(Source).Equals(item.Value.Name) && !string.IsNullOrEmpty(item.Value.DataSourceCode))
                    {
                        // 不需要在实体[DmeRuleStepAttribute]添加数据源
                        datasources.Add(item.Value.DataSourceCode);
                        continue;
                    }

                    DmeRuleStepAttribute dmeRuleStepAttribute = new DmeRuleStepAttribute
                    {
                        RuleStepId = this.step.Id,
                        ModelId = this.step.ModelId,
                        VersionId = this.step.VersionId,
                        AttributeCode = item.Key,
                        AttributeValue = item.Value.Value
                    };
                    
                    if (nameof(EnumValueMetaType.TYPE_FEATURECLASS).Equals(item.Value.DataTypeCode)
                        || nameof(EnumValueMetaType.TYPE_MDB_FEATURECLASS).Equals(item.Value.DataTypeCode)
                        || nameof(EnumValueMetaType.TYPE_SDE_FEATURECLASS).Equals(item.Value.DataTypeCode))
                    {
                        // 要素图层需要处理一下
                        dmeRuleStepAttribute.AttributeValue = "{\"name\":\"" + item.Value.Value + "\",\"source\":\"" + item.Value.DataSourceCode + "\"}";
                    }
                    attributeEntities.Add(dmeRuleStepAttribute);
                }
                if (attributeEntities.Count > 0)
                {
                    db.Insertable<DmeRuleStepAttribute>(attributeEntities).ExecuteCommand();
                }
                if (datasources.Count > 0)
                {
                    this.SaveDataSourceAttribute(db, datasources);
                }
                return true;
            }).Data;
            return result;
        }
        /// <summary>
        /// 保存数据源关联信息
        /// </summary>
        /// <param name="attributes"></param>
        protected void SaveDataSourceAttribute(SqlSugarClient db, ISet<string> datasources)
        {
            // 删除这个步骤原来关联的数据源
            db.Deleteable<DmeRuleStepDataSource>().Where(rsds => rsds.RuleStepId == step.Id).ExecuteCommand();
            List<DmeRuleStepDataSource> newRefDatasources = new List<DmeRuleStepDataSource>();
            foreach (var item in datasources)
            {
                DmeDataSource dmeDataSource = db.Queryable<DmeDataSource>().Single(ds => ds.SysCode == item);
                if (null == dmeDataSource)
                {
                    continue;
                }
                newRefDatasources.Add(new DmeRuleStepDataSource
                {
                    DataSourceId = dmeDataSource.Id,
                    ModelId = this.step.ModelId,
                    VersionId = this.step.VersionId,
                    RuleStepId = this.step.Id
                });
            }
            if (newRefDatasources.Count > 0)
            {
                db.Insertable<DmeRuleStepDataSource>(newRefDatasources);
            }
        }

        //public int SaveMeta(double x, double y,
        //    string stepName)
        //{
        //    int identity = -1;
        //    SqlSugarClient db = repository.GetDbContext();
        //    return db.Ado.UseTran<int>(() => 
        //    {
        //        if (-1 == this.step.Id)
        //        {
        //            LOG.Info($"步骤id为{this.step.Id}，为新创建步骤");
        //            // 查找当前步骤类型
        //            DmeRuleStepType dmeRuleStepType = db.Queryable<DmeRuleStepType>().Single(rst => rst.Code == nameof(this.MyRuleStepType));
        //            if (null == dmeRuleStepType)
        //            {
        //                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"步骤编码{nameof(this.MyRuleStepType)}不存在，请核实数据。");
        //            }
        //            DmeRuleStep dmeRuleStep = new DmeRuleStep
        //            {
        //                SysCode = GuidUtil.NewGuid(),
        //                ModelId = this.step.ModelId,
        //                VersionId = this.step.VersionId,
        //                Name = stepName,
        //                X = x,
        //                Y = y,
        //                StepTypeId = dmeRuleStepType.Id
        //            };
        //            identity = db.Insertable<DmeRuleStep>(dmeRuleStep).ExecuteReturnIdentity();
        //        }
        //        else
        //        {
        //            LOG.Info($"步骤id为{this.step.Id}，更新步骤信息");
        //            DmeRuleStep dmeRuleStep = db.Queryable<DmeRuleStep>().InSingle(this.step.Id);
        //            if(null == dmeRuleStep)
        //            {
        //                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"步骤id[{this.step.Id}]不存在，请核实数据。");
        //            }
        //            if (!string.IsNullOrEmpty(stepName))
        //            {
        //                // 如果名称不为空，才更新原来的名称
        //                dmeRuleStep.Name = stepName;
        //            }
        //            dmeRuleStep.X = x;
        //            dmeRuleStep.Y = y;
        //            db.Updateable<DmeRuleStep>(dmeRuleStep).ExecuteCommandHasChange();
        //            identity = dmeRuleStep.Id;
        //        }
        //        return identity;
        //    }).Data;
        //}
    }
}
