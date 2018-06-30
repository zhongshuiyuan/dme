using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.DisFS.Adapters.Mongo;
using Dist.Dme.DisFS.Collection;
using Dist.Dme.Extensions;
using Dist.Dme.Model.Entity;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dist.Dme.RuleSteps
{
    public abstract class BaseRuleStepData
    {
        protected IRepository repository;
        protected int taskId;
        protected int modelId;
        protected int versionId;
        protected int ruleStepId;

        public BaseRuleStepData(IRepository repository, int taskId, int modelId, int versionId, int ruleStepId)
        {
            this.repository = repository;
            this.taskId = taskId;
            this.modelId = modelId;
            this.versionId = versionId;
            this.ruleStepId = ruleStepId;
         
        }
        /// <summary>
        /// 异步保存输出
        /// </summary>
        /// <param name="outputParams"></param>
        protected void SaveOutput(IDictionary<string, Property> outputParams)
        {
            // 保存计算结果
            foreach (var item in outputParams)
            {
                DmeTaskResult taskResult = new DmeTaskResult
                {
                    TaskId = this.taskId,
                    RuleStepId = this.ruleStepId,
                    ResultCode = item.Key,
                    ResultType = EnumUtil.GetEnumName<ValueMetaType>(item.Value.DataType)
                };
                switch (item.Value.DataType)
                {
                    case (int)ValueMetaType.TYPE_BIGNUMBER:
                    case (int)ValueMetaType.TYPE_BINARY:
                    case (int)ValueMetaType.TYPE_BOOLEAN:
                    case (int)ValueMetaType.TYPE_INTEGER:
                    case (int)ValueMetaType.TYPE_NUMBER:
                    case (int)ValueMetaType.TYPE_SERIALIZABLE:
                    case (int)ValueMetaType.TYPE_STRING:
                    case (int)ValueMetaType.TYPE_TIMESTAMP:
                    case (int)ValueMetaType.TYPE_MDB_FEATURECLASS:
                        taskResult.ResultValue = item.Value.Value;
                        break;
                    case (int)ValueMetaType.TYPE_JSON:
                        // 存储到mongodb
                        TaskResultColl taskResultColl = new TaskResultColl
                        {
                            TaskId = this.taskId,
                            RuleStepId = this.ruleStepId,
                            Code = item.Key,
                            Value = JsonConvert.SerializeObject(item.Value.Value)
                        };
                        MongodbHelper<TaskResultColl>.Add(ServiceFactory.MongoHost, taskResultColl);
                        //taskResult.ResultValue = JsonConvert.SerializeObject(item.Value.Value);
                        break;
                    case (int)ValueMetaType.TYPE_DATE:
                        // 日期类型，转换成毫秒存储
                        taskResult.ResultValue = ((DateTime)item.Value.Value).Millisecond;
                        break;
                    default:
                        break;
                }
                this.repository.GetDbContext().Insertable<DmeTaskResult>(taskResult).ExecuteCommand();
            }
        }
    }
}
