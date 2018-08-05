using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.DisFS.Adapters.Mongo;
using Dist.Dme.DisFS.Collection;
using Dist.Dme.Extensions;
using Dist.Dme.Model.Entity;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Newtonsoft.Json;
using NLog;
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
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        protected IRepository repository;
        protected int taskId;
        protected DmeTask task;
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
        //public BaseRuleStepData(IRepository repository, int taskId, DmeRuleStep step)
        //{
        //    this.repository = repository;
        //    this.taskId = taskId;
        //    this.step = step;
        //    //this.modelId = modelId;
        //    //this.versionId = versionId;
        //    //this.ruleStepId = ruleStepId;
         
        //}
        public BaseRuleStepData(IRepository repository, DmeTask task, DmeRuleStep step)
        {
            this.repository = repository;
            this.task = task;
            this.step = step;
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
                            TaskCode = this.task.SysCode,
                            RuleStepId = this.step.Id,
                            RuleStepCode = this.step.SysCode,
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

        public Property GetStepAttributeValue(string otherStepName, string attributeCode)
        {
            // 
            var db = repository.GetDbContext();
            DmeRuleStep otherStep = db.Queryable<DmeRuleStep>().Single(rs => rs.ModelId == step.ModelId && rs.VersionId == step.VersionId && rs.Name == otherStepName);
            if (null == otherStep)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"步骤[{otherStepName}]在模型[{step.ModelId}]版本[{step.VersionId}]下不存在");
            }
            DmeRuleStepType otherStepType = db.Queryable<DmeRuleStepType>().Single(rst => rst.Id == otherStep.StepTypeId);
            if (null == otherStepType)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"步骤类型id[{otherStep.StepTypeId}]不存在");
            }
            IRuleStepData ruleStepData = RuleStepFactory.GetRuleStepData(otherStepType.Name, repository, this.task, otherStep);
            IDictionary<string, Property> inputParameters = ruleStepData.RuleStepMeta.InParams;
            IDictionary<string, Property> outputParameters = ruleStepData.RuleStepMeta.OutParams;
            Property property = null;
            if (inputParameters.ContainsKey(attributeCode))
            {
                // 从步骤属性表查找
                property = inputParameters[attributeCode];
                DmeRuleStepAttribute dmeRuleStepAttribute = db.Queryable<DmeRuleStepAttribute>().Single(rsa => rsa.RuleStepId == otherStep.Id && rsa.RowIndex == 0 && rsa.AttributeCode == attributeCode);
                property.Value = dmeRuleStepAttribute.AttributeValue;
            }
            else if (outputParameters.ContainsKey(attributeCode))
            {
                // 从步骤结果表查找
                property = outputParameters[attributeCode];
                DmeTaskResult dmeTaskResult = db.Queryable<DmeTaskResult>().Single(tr => tr.TaskId == this.taskId && tr.RuleStepId == otherStep.Id && tr.ResultCode == attributeCode);

                EnumValueMetaType @enum = EnumUtil.GetEnumObjByName<EnumValueMetaType>(property.DataTypeCode);
                switch (@enum)
                {
                    case EnumValueMetaType.TYPE_UNKNOWN:
                    case EnumValueMetaType.TYPE_NUMBER:
                    case EnumValueMetaType.TYPE_STRING:
                    case EnumValueMetaType.TYPE_INTEGER:
                    case EnumValueMetaType.TYPE_BIGNUMBER:
                    case EnumValueMetaType.TYPE_TIMESTAMP:
                    case EnumValueMetaType.TYPE_INET:
                    case EnumValueMetaType.TYPE_LOCAL_FILE:
                    case EnumValueMetaType.TYPE_GDB_PATH:
                    case EnumValueMetaType.TYPE_FOLDER:
                        property.Value = dmeTaskResult.ResultValue;
                        break;
                    case EnumValueMetaType.TYPE_DATE:
                        // 要求格式：yyyy-MM-dd hh:mm:ss 
                        property.Value = Convert.ToDateTime(dmeTaskResult.ResultValue?.ToString());
                        break;
                    case EnumValueMetaType.TYPE_BOOLEAN:
                        property.Value = Boolean.Parse(dmeTaskResult.ResultValue?.ToString());
                        break;
                    case EnumValueMetaType.TYPE_SERIALIZABLE:
                        break;
                    case EnumValueMetaType.TYPE_BINARY:
                        break;
                    case EnumValueMetaType.TYPE_MDB_FEATURECLASS:
                        break;
                    case EnumValueMetaType.TYPE_STRING_LIST:
                        property.Value = dmeTaskResult.ResultValue?.ToString().Split(";");
                        break;
                    case EnumValueMetaType.TYPE_SDE_FEATURECLASS:
                        break;
                    case EnumValueMetaType.TYPE_FEATURECLASS:
                        break;
                    case EnumValueMetaType.TYPE_JSON:
                        // 从mongo中获取
                        var filter = Builders<TaskResultColl>.Filter.And(
                            Builders<TaskResultColl>.Filter.Eq("TaskCode", this.task.SysCode),
                            Builders<TaskResultColl>.Filter.Eq("RuleStepCode", otherStep.SysCode),
                            Builders<TaskResultColl>.Filter.Eq("Code", attributeCode));
                        IList<TaskResultColl> colls = MongodbHelper<TaskResultColl>.FindList(ServiceFactory.MongoDatabase, filter);
                        if (colls != null && colls.Count > 0)
                        {
                            property.Value = colls[0].Value;
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                LOG.Error($"属性编码[{attributeCode}]无效");
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"属性编码[{attributeCode}]无效");
            }
            return property;
        }
 
    }
}
