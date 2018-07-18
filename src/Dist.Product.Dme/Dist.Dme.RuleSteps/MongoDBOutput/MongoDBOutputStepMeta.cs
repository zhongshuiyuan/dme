using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.Model.Entity;
using Dist.Dme.RuleSteps.MongoDBOutput.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dist.Dme.RuleSteps.MongoDBOutput
{
    /// <summary>
    /// mongodb
    /// </summary>
    [RuleStepTypeAttribute(Name = "MongoDBOutput", DisplayName = "mongo输出", Description = "mongo输出")]
    public class MongoDBOutputStepMeta : BaseRuleStepMeta, IRuleStepMeta
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 数据库
        /// </summary>
        public string Database { get; set; }
        /// <summary>
        /// 集合类
        /// </summary>
        public string Collection { get; set; }
        /// <summary>
        /// 文档字段json数组
        /// </summary>
        public IList<MongoFieldDTO> MongoFields { get; set; } = new List<MongoFieldDTO>();

        public MongoDBOutputStepMeta(IRepository repository, DmeRuleStep step)
        : base(repository, step)
        {
        }
        // protected override EnumRuleStepTypes MyRuleStepType => EnumRuleStepTypes.MongodbOutput;
        public string RuleStepName { get; set; } = "mongo输出";

        public object RuleStepType
        {
            get
            {
                var attribute = (RuleStepTypeAttribute)this.GetType().GetCustomAttributes(typeof(RuleStepTypeAttribute), false).FirstOrDefault();
                return attribute;
            }
        }

        public override object InParams
        {
            get
            {
                // base.InputParameters.Clear();
                base.InputParameters[nameof(Database)] = new Property(nameof(Database), "mongo数据库", EnumValueMetaType.TYPE_STRING, null, null, "mongo数据库实例名称");
                base.InputParameters[nameof(Collection)] = new Property(nameof(Collection), "mongo文档类", EnumValueMetaType.TYPE_STRING, null, null, "mongo集合类名称");
                base.InputParameters[nameof(MongoFields)] = new Property(nameof(MongoFields), "mongo输出字段", EnumValueMetaType.TYPE_JSON_ARRAY, MongoFields, MongoFields, "保存字段JSON数组");
                return base.InputParameters;
            }
        }
        /// <summary>
        /// 覆写父类的方法
        /// </summary>
        /// <returns></returns>
        public new IDictionary<string, Property> ReadAttributes()
        {
            IDictionary<string, Property> atts = new Dictionary<string, Property>();
            // 数据源步骤的标识符属性不会存在数据模型[DME_RULESTEP_ATTRIBUTE]，而是存在数据模型[DME_RULESTEP_DATASOURCE]
            var db = base.repository.GetDbContext();
            // 选择数据源信息
            IList<DmeDataSource> dataSources = db.Queryable<DmeRuleStepDataSource, DmeDataSource>(
                (rsds, ds) => rsds.DataSourceId == ds.Id && rsds.RuleStepId == step.Id && nameof(EnumDataSourceType.MONGODB) == ds.Type)
                .Select<DmeDataSource>((rsds, ds) => ds).ToList();
            if (dataSources?.Count > 0)
            {
                foreach (var item in dataSources)
                {
                    atts[nameof(base.Source)] = new Property(nameof(base.Source), item.Name, EnumValueMetaType.TYPE_STRING, item.SysCode, "", "", null, 1, 0, 1, item.SysCode);
                    // 一个数据源有且仅有一个关联
                    break;
                }
            }
            // 获取Database、Collection、AuthenticationUser、AuthenticationPassword和MongoFields
            //IList<DmeRuleStepAttribute> dmeRuleStepAttributes = db.Queryable<DmeRuleStepAttribute>().Where(rsa => rsa.ModelId == step.ModelId && rsa.VersionId == step.VersionId && rsa.RuleStepId == step.Id).ToList();
            //if (0 == dmeRuleStepAttributes?.Count)
            //{
            //    throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"缺失配置项[{nameof(Database)}]、[{nameof(Collection)}]和[{nameof(MongoFields)}]");
            //}
            ReadSingleAttribute(atts, db, nameof(Database), "mongo数据库");
            ReadSingleAttribute(atts, db, nameof(Collection), "mongo文档类");
            // 输出字段
            int fieldCount = db.Queryable<DmeRuleStepAttribute>().Count(rsa => rsa.ModelId == step.ModelId && rsa.VersionId == step.VersionId && rsa.RuleStepId == step.Id && rsa.AttributeCode == "name");
            if (fieldCount > 0)
            {
                for (int i = 0; i < fieldCount; i++)
                {
                    MongoFieldDTO field = new MongoFieldDTO();
                    this.MongoFields.Add(field);
                    IList<DmeRuleStepAttribute> list
                        = db.Queryable<DmeRuleStepAttribute>().Where(rsa => rsa.ModelId == step.ModelId && rsa.VersionId == step.VersionId && rsa.RuleStepId == step.Id && rsa.RowIndex == i).ToList();
                    field.IsNeedPrecursor = list[0].IsNeedPrecursor;
                    foreach (var item in list)
                    {
                        switch (item.AttributeCode)
                        {
                            case nameof(field.Name):
                                field.Name = item.AttributeValue.ToString();
                                break;
                            case nameof(field.IsUseName):
                                field.IsUseName = int.Parse(item.AttributeValue.ToString());
                                break;
                            case nameof(field.NewName):
                                field.NewName = item.AttributeValue.ToString();
                                break;
                            case nameof(field.ConstantValue):
                                field.ConstantValue = item.AttributeValue;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            atts[nameof(MongoFields)] = new Property(nameof(MongoFields), "mongo输出字段", EnumValueMetaType.TYPE_JSON_ARRAY, MongoFields, MongoFields, "保存字段JSON数组");

            return atts;
        }

        private DmeRuleStepAttribute ReadSingleAttribute(IDictionary<string, Property> atts, SqlSugarClient db, string attributeCode, string desc)
        {
            DmeRuleStepAttribute att = db.Queryable<DmeRuleStepAttribute>().Single(rsa => rsa.ModelId == step.ModelId && rsa.VersionId == step.VersionId && rsa.RuleStepId == step.Id && rsa.AttributeCode == attributeCode);
            if (att != null)
            {
                atts[attributeCode] = new Property(attributeCode, desc, EnumValueMetaType.TYPE_STRING, att.AttributeValue, "", "", null, 1, 0, 1);
            }
            else
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"缺失配置项[{attributeCode}]");
            }

            return att;
        }
        /// <summary>
        /// 覆写基类方法
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public new bool SaveAttributes(IDictionary<string, Property> attributes)
        {
            if (attributes?.Count == 0)
            {
                LOG.Info("属性个数为 0，不再执行后面的操作。");
                return true;
            }
            if (!attributes.ContainsKey(nameof(Database)))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"缺失属性[{nameof(Database)}]");
            }
            if (!attributes.ContainsKey(nameof(Collection)))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"缺失属性[{nameof(Collection)}]");
            }
            if (!attributes.ContainsKey(nameof(MongoFields)))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"缺失属性[{nameof(MongoFields)}]");
            }
           
            var db = repository.GetDbContext();
            db.Ado.UseTran(() =>
            {
                // 删除的影响条目
                int deleteCount = db.Deleteable<DmeRuleStepAttribute>().Where(rsa => rsa.RuleStepId == this.step.Id).ExecuteCommand();
                LOG.Info($"共删除[{deleteCount}]条属性记录");
               
                this.Database = attributes[nameof(Database)].Value.ToString();
                this.SaveStepAttribute(db, 0, nameof(this.Database), this.Database, attributes[nameof(Database)].IsNeedPrecursor);
                this.Collection = attributes[nameof(Collection)].Value.ToString();
                this.SaveStepAttribute(db, 0, nameof(this.Collection), this.Collection, attributes[nameof(Collection)].IsNeedPrecursor);

                this.MongoFields = JsonConvert.DeserializeObject<IList<MongoFieldDTO>>(attributes[nameof(MongoFields)].ToString());
                for (int i = 0; i < this.MongoFields.Count; i++)
                {
                    MongoFieldDTO field = this.MongoFields[i];
                    this.SaveStepAttribute(db, i, nameof(field.Name), field.Name, field.IsNeedPrecursor);
                    this.SaveStepAttribute(db, i, nameof(field.IsUseName), field.IsUseName, field.IsNeedPrecursor);
                    this.SaveStepAttribute(db, i, nameof(field.NewName), field.NewName, field.IsNeedPrecursor);
                    this.SaveStepAttribute(db, i, nameof(field.ConstantValue), field.ConstantValue, field.IsNeedPrecursor);
                }
                ISet<string> datasources = new HashSet<string>();
                foreach (var item in attributes.Values)
                {
                    if (string.IsNullOrEmpty(item.DataSourceCode))
                    {
                        continue;
                    }
                    datasources.Add(item.DataSourceCode);
                }
              base.SaveDataSourceAttribute(db, datasources);
            });
            return true;
        }

        private void SaveStepAttribute(SqlSugarClient db, int i, string fieldName, object fieldValue, int isNeedPrecursor)
        {
            DmeRuleStepAttribute attribute = new DmeRuleStepAttribute
            {
                ModelId = step.ModelId,
                VersionId = step.VersionId,
                RuleStepId = step.Id,
                RowIndex = i,
                IsNeedPrecursor = isNeedPrecursor,
                AttributeCode = fieldName,
                AttributeValue = fieldValue
            };
            db.Insertable<DmeRuleStepAttribute>(attribute).ExecuteCommand();
        }
    }
}
