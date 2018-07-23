using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.DisFS.Adapters.Mongo;
using Dist.Dme.DisFS.Collection;
using Dist.Dme.Model.Entity;
using Dist.Dme.RuleSteps.MongoDBOutput.DTO;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.RuleSteps.MongoDBOutput
{
    public class MongoDBOutputStepData : BaseRuleStepData, IRuleStepData
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        private MongoDBOutputStepMeta ruleStepMeta;

        public MongoDBOutputStepData(IRepository repository, int taskId, DmeRuleStep step) :
         base(repository, taskId, step)
        {
            ruleStepMeta = new MongoDBOutputStepMeta(repository, step);
        }
        public IRuleStepMeta RuleStepMeta => ruleStepMeta;

        public Result Run()
        {
            IDictionary<string, Property> attributes = this.ruleStepMeta.ReadAttributes();
            string database = Convert.ToString(attributes[nameof(this.ruleStepMeta.Database)].Value);
            string collection = Convert.ToString(attributes[nameof(this.ruleStepMeta.Collection)].Value);
            IList<MongoFieldDTO> mongoFields = (IList<MongoFieldDTO>)attributes[nameof(this.ruleStepMeta.MongoFields)].Value;
            if(0 == mongoFields?.Count)
            {
                return new Result(EnumSystemStatusCode.DME_FAIL, "没有输出的字段信息，停止计算", null);
            }
            DmeDataSource dmeDataSource = (DmeDataSource)attributes[nameof(this.ruleStepMeta.Source)].Value;
            MongodbHost mongodbHost = JsonConvert.DeserializeObject<MongodbHost>(dmeDataSource.Connection);
            if (!string.IsNullOrEmpty(database))
            {
                mongodbHost.DataBase = database;
            }
            mongodbHost.Collection = collection;
            JObject json = new JObject
            {
                { "TaskId", this.taskId },
                { "RuleStepId", this.step.Id }
            };
            foreach (var item in mongoFields)
            {
                string fieldName = item.Name;
                if (0 == item.IsNeedPrecursor)
                {
                    if (0 == item.IsUseName)
                    {
                        fieldName = item.NewName;
                    }
                    json.Add(fieldName, JToken.FromObject(item.ConstantValue));
                }
                else
                {
                    // 前驱参数
                    if (string.IsNullOrEmpty(item.Name)
                      || !item.Name.ToString().Contains(":"))
                    {
                        throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"步骤[{step.SysCode}]的前驱参数[{item.Name}]无效");
                    }
                    string preStepName = item.Name.Split(":")[0];
                    string preAttributeName = item.Name.Split(":")[1];
                    Property property = base.GetStepAttributeValue(preStepName, preAttributeName);
                    if (null == property)
                    {
                        throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"步骤名[{preStepName}]的参数[{preAttributeName}]无效");
                    }
                    if (0 == item.IsUseName)
                    {
                        fieldName = item.NewName;
                    }
                    else
                    {
                        fieldName = preAttributeName;
                    }
                    json.Add(fieldName, JToken.FromObject(property.Value));
                }  
            }
            BsonDocument document = BsonDocument.Parse(json.ToJson());
            int count = MongodbHelper<BsonDocument>.Add(mongodbHost, document);

            return new Result(EnumSystemStatusCode.DME_SUCCESS, $"模型[{step.ModelId}]的步骤[{step.SysCode}]运算完毕", null);
        }

        public bool SaveAttributes(IDictionary<string, Property> attributes)
        {
            return ruleStepMeta.SaveAttributes(attributes);
        }
    }
}
