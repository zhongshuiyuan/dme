using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.Model.Entity;
using log4net;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.RuleSteps
{
    /// <summary>
    /// 规则步骤元数据基类
    /// </summary>
    public abstract class BaseRuleStepMeta
    {
        private static ILog LOG = LogManager.GetLogger(typeof(BaseRuleStepMeta));

        // public abstract IRuleStepType RuleStepType { get; }
        protected abstract EnumRuleStepTypes MyRuleStepType { get; }
        public abstract object InParams { get; }
        protected IRepository repository;
        protected DmeRuleStep step;
        //protected int modelId;
        //protected int versionId;
        //protected int ruleStepId;
        /// <summary>
        /// 数据源id集合
        /// </summary>
        protected IList<string> DatasourceIds { get; set; }

        public BaseRuleStepMeta(IRepository repository, DmeRuleStep step)
        {
            this.repository = repository;
            this.step = step;
        }
        /// <summary>
        /// 步骤需要的输入参数
        /// </summary>
        protected IDictionary<String, Property> InputParameters { get; set; } = new Dictionary<String, Property>() {
            [nameof(DatasourceIds)] = new Property(nameof(DatasourceIds), "数据源唯一Code集合", EnumValueMetaType.TYPE_JSON_ARRAY, "", "[\"datasourceCode1\", \"datasourceCode2\"]", "数据源唯一Code集合")
        };
        /// <summary>
        /// 获取步骤类型元数据
        /// </summary>
        /// <param name="enumRuleStepTypes"></param>
        /// <returns></returns>
        protected object GetRuleStepTypeMeta(EnumRuleStepTypes enumRuleStepTypes)
        {
            return new Dictionary<string, object>
            {
                ["value"] = (int)enumRuleStepTypes,
                ["name"] = EnumUtil.GetEnumDisplayName(enumRuleStepTypes),
                ["desc"] = EnumUtil.GetEnumDescription(enumRuleStepTypes)
            };
        }
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
        // 参数值封装成Property
        // public abstract IDictionary<string, object> ReadAttributesEx();
        public bool SaveAttributes(IDictionary<string, object> attributes)
        {
            if (attributes?.Count == 0)
            {
                LOG.Info("属性个数为 0，不再执行后面的操作。");
                return true;
            }
            var db = repository.GetDbContext();
            if (attributes.ContainsKey(nameof(DatasourceIds)))
            {
                // 解析步骤关联的数据源
                JArray array = JArray.Parse(attributes[nameof(DatasourceIds)]?.ToString());
                // 同步删除已解析的参数
                attributes.Remove(nameof(DatasourceIds));
                this.DatasourceIds = array.ToObject<List<string>>();
                if (this.DatasourceIds?.Count > 0)
                {
                    db.Ado.UseTran(() =>
                    {
                        // 删除这个步骤原来关联的数据源
                        db.Deleteable<DmeRuleStepDataSource>().Where(rsds => rsds.RuleStepId == step.Id).ExecuteCommand();
                        List<DmeRuleStepDataSource> newRefDatasources = new List<DmeRuleStepDataSource>();
                        DmeDataSource tempDatasource = null;
                        // 为了去重之用
                        IList<string> repeatItem = new List<string>();
                        foreach (var item in this.DatasourceIds)
                        {
                            tempDatasource = db.Queryable<DmeDataSource>().Single(ds => ds.SysCode == item);
                            if (null == tempDatasource || repeatItem.Contains(item))
                            {
                                continue;
                            }
                            DmeRuleStepDataSource dmeRuleStepDataSource = new DmeRuleStepDataSource
                            {
                                DataSourceId = tempDatasource.Id,
                                ModelId = this.step.ModelId,
                                VersionId = this.step.VersionId,
                                RuleStepId = this.step.Id
                            };
                            newRefDatasources.Add(dmeRuleStepDataSource);
                        }
                        db.Insertable<DmeRuleStepDataSource>(newRefDatasources);
                    });
                }
            }
            
            // 先删除这个步骤的属性，再重新添加
            return db.Ado.UseTran<Boolean>(() => 
            {
                // 删除的影响条目
                int deleteCount = db.Deleteable<DmeRuleStepAttribute>().Where(rsa => rsa.RuleStepId == this.step.Id).ExecuteCommand();
                LOG.Info($"删除规则[{this.step.Id}]下的{deleteCount} 条属性记录");
                // 保存步骤属性
                DmeRuleStep dmeRuleStep = db.Queryable<DmeRuleStep>().Single(rs => rs.Id == this.step.Id);
                if (null ==  dmeRuleStep)
                {
                    throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"规则[{this.step.Id}]没有找到数据库记录");
                }
                // 不使用接口IList，Insertable<>批量插入存在问题
                List<DmeRuleStepAttribute> attributeEntities = new List<DmeRuleStepAttribute>();
                foreach (var item in attributes)
                {
                    DmeRuleStepAttribute dmeRuleStepAttribute = new DmeRuleStepAttribute
                    {
                        RuleStepId = this.step.Id,
                        ModelId = dmeRuleStep.ModelId,
                        VersionId = dmeRuleStep.VersionId,
                        AttributeCode = item.Key,
                        AttributeValue = item.Value
                    };
                    attributeEntities.Add(dmeRuleStepAttribute);
                }
                db.Insertable<DmeRuleStepAttribute>(attributeEntities).ExecuteCommand();
                return true;
            }).Data;
        }
        public int SaveMeta(double x, double y,
            string stepName)
        {
            // 步骤名如果为空，则使用当前步骤类型默认的名称
            if (string.IsNullOrEmpty(stepName))
            {
                stepName = EnumUtil.GetEnumDisplayName(this.MyRuleStepType);// this.RuleStepType.Name;
            }
            int identity = -1;
            SqlSugarClient db = repository.GetDbContext();
            return db.Ado.UseTran<int>(() => 
            {
                if (-1 == this.step.Id)
                {
                    LOG.Info($"步骤id为{this.step.Id}，为新创建步骤");
                    // 查找当前步骤类型
                    DmeRuleStepType dmeRuleStepType = db.Queryable<DmeRuleStepType>().Single(rst => rst.Code == nameof(this.MyRuleStepType));
                    if (null == dmeRuleStepType)
                    {
                        throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"步骤编码{nameof(this.MyRuleStepType)}不存在，请核实数据。");
                    }
                    DmeRuleStep dmeRuleStep = new DmeRuleStep
                    {
                        SysCode = GuidUtil.NewGuid(),
                        ModelId = this.step.ModelId,
                        VersionId = this.step.VersionId,
                        Name = stepName,
                        X = x,
                        Y = y,
                        StepTypeId = dmeRuleStepType.Id
                    };
                    identity = db.Insertable<DmeRuleStep>(dmeRuleStep).ExecuteReturnIdentity();
                }
                else
                {
                    LOG.Info($"步骤id为{this.step.Id}，更新步骤信息");
                    DmeRuleStep dmeRuleStep = db.Queryable<DmeRuleStep>().InSingle(this.step.Id);
                    if(null == dmeRuleStep)
                    {
                        throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"步骤id[{this.step.Id}]不存在，请核实数据。");
                    }
                    if (!string.IsNullOrEmpty(stepName))
                    {
                        // 如果名称不为空，才更新原来的名称
                        dmeRuleStep.Name = stepName;
                    }
                    dmeRuleStep.X = x;
                    dmeRuleStep.Y = y;
                    db.Updateable<DmeRuleStep>(dmeRuleStep).ExecuteCommandHasChange();
                    identity = dmeRuleStep.Id;
                }
                return identity;
            }).Data;
        }
    }
}
