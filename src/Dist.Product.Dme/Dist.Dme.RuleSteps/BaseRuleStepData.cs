using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework;
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
    /// <summary>
    /// 步骤数据产生和保存基类
    /// </summary>
    public abstract class BaseRuleStepData
    {
        protected IRepository repository;
        protected int taskId;
        protected DmeRuleStep step;
        //protected int modelId;
        //protected int versionId;
        //protected int ruleStepId;
        /// <summary>
        /// 真正运行的时候，taskId和step才被使用到
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="taskId"></param>
        /// <param name="step"></param>
        public BaseRuleStepData(IRepository repository, int taskId, DmeRuleStep step)
        {
            this.repository = repository;
            this.taskId = taskId;
            this.step = step;
            //this.modelId = modelId;
            //this.versionId = versionId;
            //this.ruleStepId = ruleStepId;
         
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
                    RuleStepId = this.step.Id,
                    ResultCode = item.Key,
                    ResultType = EnumUtil.GetEnumName<EnumValueMetaType>(item.Value.DataType)
                };
                switch (item.Value.DataType)
                {
                    case (int)EnumValueMetaType.TYPE_BIGNUMBER:
                    case (int)EnumValueMetaType.TYPE_BINARY:
                    case (int)EnumValueMetaType.TYPE_BOOLEAN:
                    case (int)EnumValueMetaType.TYPE_INTEGER:
                    case (int)EnumValueMetaType.TYPE_NUMBER:
                    case (int)EnumValueMetaType.TYPE_SERIALIZABLE:
                    case (int)EnumValueMetaType.TYPE_STRING:
                    case (int)EnumValueMetaType.TYPE_TIMESTAMP:
                    case (int)EnumValueMetaType.TYPE_MDB_FEATURECLASS:
                        taskResult.ResultValue = item.Value.Value;
                        break;
                    case (int)EnumValueMetaType.TYPE_JSON:
                        // 存储到mongodb
                        TaskResultColl taskResultColl = new TaskResultColl
                        {
                            TaskId = this.taskId,
                            RuleStepId = this.step.Id,
                            Code = item.Key,
                            Value = JsonConvert.SerializeObject(item.Value.Value)
                        };
                        MongodbHelper<TaskResultColl>.Add(ServiceFactory.MongoDatabase, taskResultColl);
                        //taskResult.ResultValue = JsonConvert.SerializeObject(item.Value.Value);
                        break;
                    case (int)EnumValueMetaType.TYPE_DATE:
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
