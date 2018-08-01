using Dist.Dme.Base.Common;
using Dist.Dme.Base.Common.Page;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.DAL.Context;
using Dist.Dme.DisFS.Adapters.Mongo;
using Dist.Dme.DisFS.Collection;
using Dist.Dme.Extensions;
using Dist.Dme.Model.DTO;
using Dist.Dme.Model.Entity;
using Dist.Dme.Service.Interfaces;
using MongoDB.Driver;
using NLog;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

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
            switch (operation)
            {
                case 0:
                    LOG.Info($"停止任务[{taskCode}]");
                    task.Status = EnumUtil.GetEnumDisplayName(EnumSystemStatusCode.DME_STOP);
                    task.LastTime = DateUtil.CurrentTimeMillis;
                    db.Updateable<DmeTask>(task).UpdateColumns(t => new { t.Status, task.LastTime }).ExecuteCommand();
                    break;
                case 1:
                    // @TODO 有待完成
                    LOG.Info($"重启任务[{taskCode}]");
                    task.Status = EnumUtil.GetEnumDisplayName(EnumSystemStatusCode.DME_RUNNING);
                    task.LastTime = DateUtil.CurrentTimeMillis;
                    db.Updateable<DmeTask>(task).UpdateColumns(t => new { t.Status, task.LastTime }).ExecuteCommand();
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
    }
}
