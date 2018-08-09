using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dist.Dme.Model.DTO;
using Dist.Dme.Model.Entity;

namespace Dist.Dme.Service.Interfaces
{
    public interface ITaskService
    {
        /// <summary>
        /// 获取任务清单，以创建时间倒序
        /// </summary>
        /// <returns></returns>
        object ListTaskPage(int pageIndex, int pageSize);
        /// <summary>
        /// 获取任务步骤的执行结果
        /// </summary>
        /// <param name="taskCode"></param>
        /// <param name="ruleStepId"></param>
        /// <returns></returns>
        object GetTaskResult(string taskCode, int ruleStepId);
        /// <summary>
        /// 操作任务
        /// </summary>
        /// <param name="taskCode">任务唯一编码</param>
        /// <param name="operation">操作符号，0：停止；1：重启；-1：删除</param>
        /// <returns></returns>
        object OperateTask(string taskCode, int operation);
        /// <summary>
        /// 根据唯一编码获任务实体
        /// </summary>
        /// <param name="taskCode"></param>
        /// <returns></returns>
        object GetTask(string taskCode);
        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<DmeTask> CreateTaskAsync(NewTaskReqDTO dto);
        /// <summary>
        /// 调度任务运行
        /// </summary>
        /// <param name="taskCode">任务唯一编码</param>
        Task RunTaskScheduleAsync(string taskCode);
    }
}
