using Dist.Dme.Base.Common;
using Dist.Dme.Base.Common.Log;
using Dist.Dme.Base.Common.Page;
using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Framework.Collections;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.DAL.Context;
using Dist.Dme.DisFS.Adapters.Mongo;
using Dist.Dme.DisFS.Collection;
using Dist.Dme.Extensions;
using Dist.Dme.HSMessage.Define;
using Dist.Dme.HSMessage.MQ.Kafka;
using Dist.Dme.Model.DTO;
using Dist.Dme.Model.Entity;
using Dist.Dme.RuleSteps;
using Dist.Dme.Scheduler;
using Dist.Dme.Service.Interfaces;
using Dist.Dme.Service.Scheduler;
using MongoDB.Driver;
using NLog;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dist.Dme.Service.Impls
{
    public class TaskService : BaseBizService, ITaskService
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 自动注入参数
        /// </summary>
        /// <param name="repository"></param>
        public TaskService(IRepository repository)
        {
            base.Repository = repository;
        }
        public object ListTaskPage(int pageIndex, int pageSize)
        {
            SqlSugarClient db = base.Repository.GetDbContext();
            int totalNumber = 0;
            IList<DmeTask> tasks = db.Queryable<DmeTask>().OrderBy(t => t.CreateTime, OrderByType.Desc).ToPageList(pageIndex, pageSize, ref totalNumber);
            if (0 == tasks?.Count)
            {
                return 0;
            }
            IList<TaskRespDTO> taskRespDTOs = new List<TaskRespDTO>();

            foreach (var item in tasks)
            {
                TaskRespDTO dto = new TaskRespDTO
                {
                    Task = item,
                    Model = db.Queryable<DmeModel>().Single(m => m.Id == item.ModelId),
                    ModelVersion = db.Queryable<DmeModelVersion>().Single(mv => mv.Id == item.VersionId)
                };
                taskRespDTOs.Add(dto);
            }
            SimplePage<TaskRespDTO> page = new SimplePage<TaskRespDTO>(pageIndex, pageSize, totalNumber, taskRespDTOs);
            return page;
        }
        public object GetTaskResult(string taskCode, int ruleStepId)
        {
            SqlSugarClient db = base.Repository.GetDbContext();
            DmeTask task = db.Queryable<DmeTask>().Single(t => t.SysCode == taskCode);
            if (null == task)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"任务不存在[{taskCode}]");
            }
            // 查询任务的结果输出
            IList<DmeTaskResult> taskResults = null;
            if (-1 == ruleStepId)
            {
                // 全部步骤
                taskResults = db.Queryable<DmeTaskResult>().Where(tr => tr.TaskId == task.Id).ToList();
            }
            else
            {
                // 指定步骤
                taskResults = db.Queryable<DmeTaskResult>().Where(tr => tr.TaskId == task.Id && tr.RuleStepId == ruleStepId).ToList();
            }
            if (null == taskResults || 0 == taskResults.Count)
            {
                return null;
            }
            IList<TaskResultRespDTO> taskResultRespDTOs = new List<TaskResultRespDTO>();
            TaskResultRespDTO temp = null;
            foreach (var item in taskResults)
            {
                temp = new TaskResultRespDTO
                {
                    RuleStepId = item.RuleStepId,
                    Code = item.ResultCode,
                    Type = item.ResultType
                };
                // 解析步骤类型
                DmeRuleStep ruleStep = db.Queryable<DmeRuleStep>().Single(rs => rs.Id == item.RuleStepId);

                //   Value = item.ResultValue
                EnumValueMetaType @enum = EnumUtil.GetEnumObjByName<EnumValueMetaType>(temp.Type);
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
                        temp.Value = item.ResultValue;
                        break;
                    case EnumValueMetaType.TYPE_DATE:
                        // 要求格式：yyyy-MM-dd hh:mm:ss 
                        temp.Value = Convert.ToDateTime(item.ResultValue?.ToString());
                        break;
                    case EnumValueMetaType.TYPE_BOOLEAN:
                        temp.Value = Boolean.Parse(item.ResultValue?.ToString());
                        break;
                    case EnumValueMetaType.TYPE_SERIALIZABLE:
                        break;
                    case EnumValueMetaType.TYPE_BINARY:
                        break;
                    case EnumValueMetaType.TYPE_MDB_FEATURECLASS:
                        break;
                    case EnumValueMetaType.TYPE_STRING_LIST:
                        temp.Value = item.ResultValue?.ToString().Split(";");
                        break;
                    case EnumValueMetaType.TYPE_SDE_FEATURECLASS:
                        break;
                    case EnumValueMetaType.TYPE_FEATURECLASS:
                        break;
                    case EnumValueMetaType.TYPE_JSON:
                        // 从mongo中获取
                        var filter = Builders<TaskResultColl>.Filter.And(
                            Builders<TaskResultColl>.Filter.Eq("TaskId", item.TaskId),
                            Builders<TaskResultColl>.Filter.Eq("RuleStepId", item.RuleStepId),
                            Builders<TaskResultColl>.Filter.Eq("Code", item.ResultCode));
                        IList<TaskResultColl> colls = MongodbHelper<TaskResultColl>.FindList(ServiceFactory.MongoDatabase, filter);
                        if (colls != null && colls.Count > 0)
                        {
                            temp.Value = colls[0].Value;
                        }
                        break;
                    default:
                        break;
                }

                taskResultRespDTOs.Add(temp);
            }
            return taskResultRespDTOs;
        }
        public object OperateTask(string taskCode, int operation)
        {
            var db = Repository.GetDbContext();
            DmeTask task = db.Queryable<DmeTask>().Single(t => t.SysCode == taskCode);
            if (null == task)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"任务[{taskCode}]不存在");
            }
            DmeModel model = db.Queryable<DmeModel>().InSingle(task.ModelId);
            switch (operation)
            {
                case 0:
                    LOG.Info($"停止任务[{taskCode}]");
                    task.Status = EnumUtil.GetEnumDisplayName(EnumSystemStatusCode.DME_STOP);
                    task.LastTime = DateUtil.CurrentTimeMillis;
                    db.Updateable<DmeTask>(task).UpdateColumns(t => new { t.Status, task.LastTime }).ExecuteCommand();
                    DmeQuartzScheduler<TaskRunnerJob>.PauseJob(task.SysCode, model.ModelTypeCode);
                    break;
                case 1:
                    // @TODO 有待完成
                    LOG.Info($"重启任务[{taskCode}]");
                    task.Status = EnumUtil.GetEnumDisplayName(EnumSystemStatusCode.DME_RUNNING);
                    task.LastTime = DateUtil.CurrentTimeMillis;
                    db.Updateable<DmeTask>(task).UpdateColumns(t => new { t.Status, task.LastTime }).ExecuteCommand();
                    DmeQuartzScheduler<TaskRunnerJob>.ResumeJob(task.SysCode, model.ModelTypeCode);
                    break;
                case -1:
                    LOG.Info($"删除任务[{taskCode}]");
                    // 删除与任务相关的步骤记录和结果记录
                    int count = db.Deleteable<DmeTaskRuleStep>(trs => trs.TaskId == task.Id).ExecuteCommand();
                    LOG.Info($"删除任务[{taskCode}]关联的[{count}]个步骤记录");
                    count = db.Deleteable<DmeTaskResult>(tr => tr.TaskId == task.Id).ExecuteCommand();
                    LOG.Info($"删除任务[{taskCode}]关联的[{count}]个结果记录");
                    // 从mongo中获取
                    var filter = Builders<TaskResultColl>.Filter.And(
                        Builders<TaskResultColl>.Filter.Eq("TaskId", task.Id));
                    MongodbHelper<TaskResultColl>.DeleteMany(ServiceFactory.MongoDatabase, filter);
                    db.Deleteable<DmeTask>(task).ExecuteCommand();
                    DmeQuartzScheduler<TaskRunnerJob>.DeleteJob(task.SysCode, model.ModelTypeCode);
                    break;
                default:
                    break;
            }
            return true;
        }
        public object GetTask(string taskCode)
        {
            var db = base.Repository.GetDbContext();
            DmeTask task = db.Queryable<DmeTask>().Single(t => t.SysCode == taskCode);
            if(null == task)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"任务[{taskCode}]不存在");
            }
            return task;
        }
        public async Task<DmeTask> CreateTaskAsync(NewTaskReqDTO dto)
        {
            // 验证数据库是否存在指定模型版本信息
            SqlSugarClient db = base.Repository.GetDbContext();
            // 查询模型版本
            DmeModelVersion modelVersion = db.Queryable<DmeModelVersion>().Single(mv => mv.SysCode == dto.ModelVersionCode);
            if (null == modelVersion)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"模型的版本[{dto.ModelVersionCode}]不存在");
            }
            // Single方法，如果查询数据库多条数据，会抛出异常
            DmeModel model = db.Queryable<DmeModel>().Single(m => m.Id == modelVersion.ModelId);
            if (null == model)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"模型[{modelVersion.ModelId}]不存在");
            }
            // 获取模型类型
            DmeModelType modelType = db.Queryable<DmeModelType>().InSingle(model.ModelTypeId);
            // 查找关联的算法信息
            IList<DmeRuleStep> ruleSteps = db.Queryable<DmeRuleStep>().Where(rs => rs.ModelId == model.Id && rs.VersionId == modelVersion.Id).ToList();
            if (0 == ruleSteps?.Count)
            {
                LOG.Warn($"模型[{model.SysCode}]的版本[{dto.ModelVersionCode}]下没有可执行步骤，停止运行");
                return null;
            }
            DmeTask newTask = db.Ado.UseTran<DmeTask>(() =>
            {
                // 每执行一次模型，生成一次任务
                newTask = new DmeTask
                {
                    SysCode = GuidUtil.NewGuid(),
                    CreateTime = DateUtil.CurrentTimeMillis,
                    Status = EnumUtil.GetEnumDisplayName(EnumSystemStatusCode.DME_RUNNING),
                    ModelId = model.Id,
                    VersionId = modelVersion.Id,
                    Remark = dto.Remark,
                    NodeServer = NetAssist.GetLocalHost()
                };
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    newTask.Name = "task-" + newTask.CreateTime;
                }
                else
                {
                    newTask.Name = dto.Name;
                }
               
                newTask.LastTime = newTask.CreateTime;
                newTask = db.Insertable<DmeTask>(newTask).ExecuteReturnEntity();
                return newTask;
            }).Data;

            // 创建job
            await DmeQuartzScheduler<TaskRunnerJob>.NewJob(
                newTask.SysCode,
                modelType.SysCode, 
                dto.CronExpression, 
                new Dictionary<string, object> { { TaskRunnerJob .TASK_CODE_KEY, newTask .SysCode} }, 
                1 == dto.StartNow, 
                DateTimeOffset.Now.AddSeconds(dto.InSeconds), 
                dto.Remark);
            return newTask;
        }
        private Task RunTaskAsyncEx(SqlSugarClient db, DmeModel model, DmeModelVersion modelVersion, DmeTask task, IList<DmeRuleStep> ruleSteps)
        {
            return Task.Run(() =>
            {
                return db.Ado.UseTran<DmeTask>(() =>
                {
                    try
                    {
                        // 查询步骤前后依赖关系
                        // 形成链表
                        IList<DmeRuleStepHop> hops = db.Queryable<DmeRuleStepHop>().Where(rsh => rsh.ModelId == model.Id && rsh.VersionId == modelVersion.Id).OrderBy("STEP_FROM_ID").ToList();
                        IList<RuleStepLinkedListNode<DmeRuleStep>> rulestepLinkedList = this.GetRuleStepNodeLinkedList(db, model, modelVersion, ruleSteps);
                        foreach (var item in rulestepLinkedList)
                        {
                            // 开始计算步骤
                            this.RunRuleStepNode(db, task, item);
                        }
                        // 完成模型计算
                        task.Status = EnumUtil.GetEnumDisplayName(EnumSystemStatusCode.DME_SUCCESS);
                        task.LastTime = DateUtil.CurrentTimeMillis;
                        db.Updateable<DmeTask>(task).ExecuteCommand();
                        // @TODO 发送模型计算成功的消息
                        KafkaProducer.Send(nameof(EnumMessageType.TASK), new MessageBody()
                        {
                            // 创建者一致
                            From = "123",
                            To = "123",
                            ChannelType = EnumChannelType.P2P,
                            MessageType = EnumMessageType.TASK,
                            Payload = $"模型[{task.SysCode}]计算完成，状态[{task.Status}]"
                        });
                        return task;
                    }
                    catch (Exception ex)
                    {
                        // 更改任务执行的状态
                        if (ex is BusinessException)
                        {
                            task.Status = EnumUtil.GetEnumDisplayName(EnumUtil.GetEnumObjByValue<EnumSystemStatusCode>(((BusinessException)ex).Code));
                        }
                        else
                        {
                            task.Status = EnumUtil.GetEnumDisplayName(EnumSystemStatusCode.DME_ERROR);
                        }
                        task.LastTime = DateUtil.CurrentTimeMillis;
                        db.Updateable<DmeTask>(task).ExecuteCommand();
                        // 添加日志
                        this.LogService.AddLogAsync(Base.Common.Log.EnumLogType.ENTITY, Base.Common.Log.EnumLogLevel.ERROR, nameof(DmeTask), task.SysCode, "", ex, "", NetAssist.GetLocalHost());
                        throw ex;
                    }
                });
            });
        }
        /// <summary>
        /// 构建步骤的链表信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ruleSteps"></param>
        /// <returns>多个链表</returns>
        private IList<RuleStepLinkedListNode<DmeRuleStep>> GetRuleStepNodeLinkedList(SqlSugarClient db, DmeModel model, DmeModelVersion modelVersion, IList<DmeRuleStep> ruleSteps)
        {
            IList<RuleStepLinkedListNode<DmeRuleStep>> newLinkedSteps = new List<RuleStepLinkedListNode<DmeRuleStep>>();
            // 一次性构建步骤实体字典
            IDictionary<int, RuleStepLinkedListNode<DmeRuleStep>> ruleStepDic = new Dictionary<int, RuleStepLinkedListNode<DmeRuleStep>>();
            foreach (var subStep in ruleSteps)
            {
                ruleStepDic[subStep.Id] = new RuleStepLinkedListNode<DmeRuleStep>(subStep);
                newLinkedSteps.Add(ruleStepDic[subStep.Id]);
            }
            //IList<RuleStepLinkedListNode<DmeRuleStep>> multiLinkedList = new List<RuleStepLinkedListNode<DmeRuleStep>>();
            IList<DmeRuleStepHop> hops = db.Queryable<DmeRuleStepHop>().Where(rsh => rsh.ModelId == model.Id && rsh.VersionId == modelVersion.Id).OrderBy(rsh => rsh.StepFromId).ToList();
            if (0 == hops?.Count)
            {
                return newLinkedSteps;
            }

            IDictionary<int, RuleStepLinkedListNode<DmeRuleStep>> newRuleStepLinkedNodeDic = new Dictionary<int, RuleStepLinkedListNode<DmeRuleStep>>();
            // 已经使用的步骤id集合
            IList<int> usedStepIds = new List<int>();
            RuleStepLinkedListNode<DmeRuleStep> linkedStepFromNode = null;
            RuleStepLinkedListNode<DmeRuleStep> linkedStepToNode = null;
            // 反过来构建？
            foreach (var hop in hops)
            {
                if (!ruleStepDic.ContainsKey(hop.StepFromId) || !ruleStepDic.ContainsKey(hop.StepToId) || 0 == hop.Enabled)
                {
                    continue;
                }
                linkedStepFromNode = ruleStepDic[hop.StepFromId];
                linkedStepToNode = ruleStepDic[hop.StepToId];
                linkedStepFromNode.Next.Add(linkedStepToNode);
                linkedStepToNode.Previous.Add(linkedStepFromNode);
            }
            return newLinkedSteps;
        }
      
        /// <summary>
        /// 运行步骤节点
        /// </summary>
        /// <param name="db"></param>
        /// <param name="task"></param>
        /// <param name="node"></param>
        private void RunRuleStepNode(SqlSugarClient db, DmeTask task, RuleStepLinkedListNode<DmeRuleStep> node)
        {
            // 先计算前置节点
            if (node.Previous?.Count > 0)
            {
                foreach (var item in node.Previous)
                {
                    RunRuleStepNode(db, task, item);
                }
            }
            DmeTaskRuleStep dmeTaskRuleStep = null;
            try
            {
                // 先判断任务的状态，是否被停止
                DmeTask taskStatus = db.Queryable<DmeTask>().Single(t => t.SysCode == task.SysCode);
                if (null == taskStatus || taskStatus.Status.Equals(EnumUtil.GetEnumDisplayName(EnumSystemStatusCode.DME_STOP)))
                {
                    LOG.Info($"任务[{task.SysCode}]不存在或者已被停止");
                    return;
                }
                dmeTaskRuleStep = this.GetTaskRuleStep(db, task, node.Value, out string cacheKey);
                if (dmeTaskRuleStep != null && EnumUtil.GetEnumDisplayName(EnumSystemStatusCode.DME_SUCCESS).Equals(dmeTaskRuleStep.Status))
                {
                    // 释放
                    dmeTaskRuleStep = null;
                    LOG.Info($"任务[{task.SysCode}]下的步骤[{node.Value.SysCode}]已被计算过，并且状态为[success]");
                    return;
                }
                // 如果前置节点没有了，则计算当前节点内容
                DmeRuleStepType ruleStepTypeTemp = db.Queryable<DmeRuleStepType>().Single(rst => rst.Id == node.Value.StepTypeId);
                IRuleStepData ruleStepData = RuleStepFactory.GetRuleStepData(ruleStepTypeTemp.Code, this.Repository, task, node.Value);
                if (null == ruleStepData)
                {
                    throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"步骤工厂无法创建编码为[{ruleStepTypeTemp.Code}]的流程实例节点");
                }
                dmeTaskRuleStep = new DmeTaskRuleStep
                {
                    SysCode = GuidUtil.NewGuid(),
                    TaskId = task.Id,
                    RuleStepId = node.Value.Id,
                    Status = EnumUtil.GetEnumDisplayName(EnumSystemStatusCode.DME_RUNNING),
                    CreateTime = DateUtil.CurrentTimeMillis,
                    LastTime = DateUtil.CurrentTimeMillis
                };
                dmeTaskRuleStep = db.Insertable<DmeTaskRuleStep>(dmeTaskRuleStep).ExecuteReturnEntity();
                // 任务步骤创建成功后，把相关信息记录在缓存中
                ServiceFactory.CacheService.AddAsync(cacheKey, dmeTaskRuleStep, 60);
                Result stepResult = ruleStepData.Run();
                UpdateRuleStep(db, dmeTaskRuleStep, cacheKey, stepResult);
                // 然后计算下一个步骤
                if (node?.Next.Count > 0)
                {
                    foreach (var item in node.Next)
                    {
                        RunRuleStepNode(db, task, item);
                    }
                }
            }
            catch (Exception ex)
            {
                dmeTaskRuleStep.Status = EnumUtil.GetEnumDisplayName(EnumSystemStatusCode.DME_ERROR);
                dmeTaskRuleStep.LastTime = DateUtil.CurrentTimeMillis;
                // 只更新状态和最后时间
                db.Updateable<DmeTaskRuleStep>(dmeTaskRuleStep).UpdateColumns(ts => new { ts.Status, ts.LastTime }).ExecuteCommand();
                this.LogService.AddLogAsync(Base.Common.Log.EnumLogType.ENTITY, EnumLogLevel.ERROR, nameof(DmeTaskRuleStep), dmeTaskRuleStep.SysCode, "", ex, "", NetAssist.GetLocalHost());
            }
        }
        /// <summary>
        /// 更新任务和步骤信息到数据和缓存
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dmeTaskRuleStep"></param>
        /// <param name="cacheKey"></param>
        /// <param name="stepResult"></param>
        private static void UpdateRuleStep(SqlSugarClient db, DmeTaskRuleStep dmeTaskRuleStep, string cacheKey, Result stepResult)
        {
            dmeTaskRuleStep.Status = EnumUtil.GetEnumDisplayName(stepResult.Code);
            dmeTaskRuleStep.LastTime = DateUtil.CurrentTimeMillis;
            // 只更新状态和最后时间
            db.Updateable<DmeTaskRuleStep>(dmeTaskRuleStep).UpdateColumns(ts => new { ts.Status, ts.LastTime }).ExecuteCommand();
            // 刷新缓存
            ServiceFactory.CacheService.ReplaceAsync(cacheKey, dmeTaskRuleStep);
        }
        /// <summary>
        /// 从缓存或者db中获取任务步骤信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="task"></param>
        /// <param name="ruleStep"></param>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        private DmeTaskRuleStep GetTaskRuleStep(SqlSugarClient db, DmeTask task, DmeRuleStep ruleStep, out string cacheKey)
        {
            // 先从缓存查找
            cacheKey = HashUtil.Hash_2_MD5_32($"{task.SysCode}_{ruleStep.SysCode}");
            DmeTaskRuleStep dmeTaskRuleStep = null;
            try
            {
                dmeTaskRuleStep = ServiceFactory.CacheService.Get<DmeTaskRuleStep>(cacheKey);
                if (dmeTaskRuleStep != null)
                {
                    LOG.Info($"缓存中获取到任务步骤信息，任务id[{dmeTaskRuleStep.TaskId}]，步骤id[{dmeTaskRuleStep.RuleStepId}]");
                    return dmeTaskRuleStep;
                }
            }
            catch (Exception ex)
            {
                LOG.Warn("从缓存中获取任务步骤信息失败，详情：" + ex.Message);
            }
            // 从数据库中查找
            dmeTaskRuleStep = db.Queryable<DmeTaskRuleStep>().Single(tr => tr.TaskId == task.Id && tr.RuleStepId == ruleStep.Id);
            return dmeTaskRuleStep;
        }
        public async Task RunTaskScheduleAsync(string taskCode)
        {
            // @TODO 运行任务
            var db = Repository.GetDbContext();
            DmeTask task = db.Queryable<DmeTask>().Single(t => t.SysCode == taskCode);
            if (null == task)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"任务[{taskCode}]不存在");
            }
            // 查询模型版本
            DmeModelVersion modelVersion = db.Queryable<DmeModelVersion>().InSingle(task.VersionId);//.Single(mv => mv.SysCode == dto.ModelVersionCode);
            if (null == modelVersion)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"模型的版本[{task.VersionId}]不存在");
            }
            // Single方法，如果查询数据库多条数据，会抛出异常
            DmeModel model = db.Queryable<DmeModel>().InSingle(task.ModelId);
            if (null == model)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"模型[{task.ModelId}]不存在");
            }
            // 查找关联的算法信息
            IList<DmeRuleStep> ruleSteps = db.Queryable<DmeRuleStep>().Where(rs => rs.ModelId == model.Id && rs.VersionId == modelVersion.Id).ToList();
            if (0 == ruleSteps?.Count)
            {
                LOG.Warn($"模型[{model.SysCode}]的版本[{modelVersion.SysCode}]下没有可执行步骤，停止运行");
                return;
            }
            try
            {
                // 清理掉任务之前的过程结果
                // 包括DME_TASK_RESULT、DME_TASK_RULESTEP和mongo TaskResultColl
                db.Ado.UseTran(() => {
                    int count = db.Deleteable<DmeTaskRuleStep>().Where(trs => trs.TaskId == task.Id).ExecuteCommand();
                    LOG.Info($"删除任务关联的步骤记录[{count}]条");
                    db.Deleteable<DmeTaskResult>().Where(tr => tr.TaskId == task.Id).ExecuteCommand();
                    LOG.Info($"删除任务结果记录[{count}]条");
                    var filter = Builders<TaskResultColl>.Filter.And(
                        Builders<TaskResultColl>.Filter.Eq("TaskCode", task.SysCode));
                    DeleteResult deleteResult = MongodbHelper<TaskResultColl>.DeleteMany(ServiceFactory.MongoDatabase, filter);
                    LOG.Info($"删除mongo记录[{deleteResult.DeletedCount}]条");
                });
                // 此时不阻塞，返回类型为Task，为了能捕获到线程异常信息
                await RunTaskAsyncEx(db, model, modelVersion, task, ruleSteps);
            }
            catch (Exception ex)
            {
                LOG.Error(ex, ex.Message);
                // 更改任务状态
                task.Status = EnumUtil.GetEnumDisplayName(EnumSystemStatusCode.DME_ERROR);
                task.LastTime = DateUtil.CurrentTimeMillis;
                db.Updateable<DmeTask>(task).UpdateColumns(t => new { task.Status, task.LastTime }).ExecuteCommand();
            }
        }
    }
}
