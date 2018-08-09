using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Service.Interfaces;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Dist.Dme.Service.Scheduler
{
    /// <summary>
    /// 任务执行器调度job
    /// </summary>
    public class TaskRunnerJob : IJob
    {
        private ITaskService _taskService;
        /// <summary>
        /// 任务唯一编码
        /// </summary>
        public readonly static string TASK_CODE_KEY = "TaskCode";

        public TaskRunnerJob(ITaskService taskService)
        {
            this._taskService = taskService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            // 注意MergedJobDataMap表示把Job和trigger的UsingJobData属性合并，并且trigger里面的key会覆盖job相同的key
            // 区别于JobDetail.JobDataMap
            JobDataMap dataMap = context.MergedJobDataMap;
            if (!dataMap.ContainsKey(TASK_CODE_KEY))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"task run job 缺失参数 {TASK_CODE_KEY}");
            }
     
            string taskCodeValue = dataMap.GetString("TaskCode");
            await _taskService.RunTaskScheduleAsync(taskCodeValue);
        }
    }
}
