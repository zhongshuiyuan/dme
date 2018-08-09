using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dist.Dme.Base.Framework;
using Dist.Dme.Model.DTO;
using Dist.Dme.Model.Entity;
using Dist.Dme.Service.Interfaces;
using Dist.Dme.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Dist.Dme.WebApi.Controllers
{
    /// <summary>
    /// 任务控制器
    /// </summary>
    [Route("api/tasks")]
    public class TaskController : BaseController
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        public ITaskService TaskService { get; private set; }
        public TaskController(ITaskService taskService)
        {
            TaskService = taskService;
        }
        /// <summary>
        /// 获取任务
        /// </summary>
        /// <param name="taskCode">任务唯一标识符</param>
        /// <returns>返回任务实体</returns>
        [HttpGet]
        [Route("v1/{taskCode}")]
        public Result GetTask(string taskCode)
        {
            return base.Success(this.TaskService.GetTask(taskCode));
        }
        /// <summary>
        /// 分页获取任务清单，以创建时间倒序
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/{pageIndex}/{pageSize}")]
        public Result ListTask(int pageIndex, int pageSize)
        {
            return base.Success(this.TaskService.ListTaskPage(pageIndex, pageSize));
        }
        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("v1")]
        public async Task<Result> CreateTask([FromBody] NewTaskReqDTO dto)
        {
            DmeTask task = await this.TaskService.CreateTaskAsync(dto);
            return base.Success(task, "任务已经创建完毕......");
        }
        /// <summary>
        /// 获取任务指定步骤或所有步骤的计算结果
        /// </summary>
        /// <param name="taskCode">任务编码</param>
        /// <param name="ruleStepId">步骤id，若传入-1，表示获取所有步骤的计算结果</param>
        /// <returns></returns>
        [HttpGet]
        [Route("result/v1/{taskCode}/{ruleStepId}")]
        public Result GetTaskOutput(string taskCode, int ruleStepId)
        {
            return base.Success(this.TaskService.GetTaskResult(taskCode, ruleStepId));
        }
        /// <summary>
        /// 停止任务
        /// </summary>
        /// <param name="taskCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/stop/{taskCode}")]
        public Result StopTask(string taskCode)
        {
            return base.Success(this.TaskService.OperateTask(taskCode, 0));
        }
        /// <summary>
        /// 重启任务
        /// </summary>
        /// <param name="taskCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/restart/{taskCode}")]
        public Result RestartTask(string taskCode)
        {
            return base.Success(this.TaskService.OperateTask(taskCode, 1));
        }
        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="taskCode"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("v1/{taskCode}")]
        public Result DeleteTask(string taskCode)
        {
            return base.Success(this.TaskService.OperateTask(taskCode, -1));
        }
    }
}
