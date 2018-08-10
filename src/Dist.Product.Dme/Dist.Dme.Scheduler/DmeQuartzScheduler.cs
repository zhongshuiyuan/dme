using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Scheduler.Define;
using NLog;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;

namespace Dist.Dme.Scheduler
{
    /// <summary>
    /// dme调度器
    /// </summary>
    public class DmeQuartzScheduler<T> where  T : IJob
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 属性配置
        /// </summary>
        private static NameValueCollection SchedulerProperties { get; set; } = new NameValueCollection();
        private static ISchedulerFactory _schedulerFactory;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="properties"></param>
        public static void SetSchedulerProperties(NameValueCollection properties)
        {
            if (null == properties || 0 == properties.Count)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, "properties 不能为空");
            }
            DmeQuartzScheduler<T>.SchedulerProperties = properties;
            _schedulerFactory = new StdSchedulerFactory(properties);
        }
        /// <summary>
        /// 验证属性设置
        /// </summary>
        /// <returns></returns>
        private static Boolean CheckProperties()
        {
            if (null == SchedulerProperties || 0 == SchedulerProperties.Count)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, "SchedulerProperties 未被初始化");
            }
            return true;
        }
        /// <summary>
        /// 获取job唯一标识的名称
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static string GetJobIdentityName(string code)
        {
            return DmeSchedulerConstants.JOB_NAME_PREFIX + code;
        }
        /// <summary>
        /// 获取job唯一标识所属组名称
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private static string GetJobIdentifyGroup(string group)
        {
            return DmeSchedulerConstants.JOB_GROUP_PREFIX + group;
        }
        /// <summary>
        /// 获取trigger唯一标识的名称
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static string GetTriggerIdentityName(string code)
        {
            return DmeSchedulerConstants.TRIGGER_NAME_PREFIX + code;
        }
        /// <summary>
        /// 获取trigger唯一标识所属组名称
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private static string GetTriggerIdentifyGroup(string group)
        {
            return DmeSchedulerConstants.TRIGGER_GROUP_PREFIX + group;
        }
        /// <summary>
        /// 创建job detail
        /// </summary>
        /// <param name="code"></param>
        /// <param name="group"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        private static IJobDetail CreateJobDetail(string code, string group, IDictionary<string, object> dataMap, string description)
        {
            JobDataMap jobDataMap = new JobDataMap();
            if (dataMap?.Count > 0)
            {
                foreach (var item in dataMap)
                {
                    jobDataMap.Put(item.Key, item.Value);
                }
            }
            return JobBuilder.Create<T>()
              .WithIdentity(GetJobIdentityName(code), GetJobIdentifyGroup(group))
              .WithDescription(description)
              .UsingJobData(jobDataMap)
              .Build();
        }
        /// <summary>
        /// 创建cronExpression trigger
        /// </summary>
        /// <param name="code">唯一编码</param>
        /// <param name="group">组名</param>
        /// <param name="cronExpression">周期表达式</param>
        /// <param name="startNow">是否立刻启动</param>
        /// <param name="timeOffset">多少时间后启动</param>
        /// <returns></returns>
        private static ITrigger CreateTrigger(string code, string group, string cronExpression, bool startNow, DateTimeOffset timeOffset)
        {
            TriggerBuilder triggerBuilder = TriggerBuilder.Create()
                                         .WithIdentity(GetTriggerIdentityName(code), GetTriggerIdentifyGroup(group))
                                         // 运行模式
                                         .WithCronSchedule(cronExpression);
            if(startNow)
            {
                triggerBuilder.StartNow(); 
            }
            else
            {
                if (timeOffset != null)
                {
                    triggerBuilder.StartAt(timeOffset);
                }
            }
          
            return triggerBuilder.Build();
        }
        
        /// <summary>
        /// 创建秒数 trigger
        /// </summary>
        /// <param name="code"></param>
        /// <param name="group"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        private static ITrigger CreateTrigger(string code, string group, int seconds)
        {
            return TriggerBuilder.Create()
                                         .WithIdentity(GetTriggerIdentityName(code), GetTriggerIdentifyGroup(group))
                                          .WithSimpleSchedule(x => x
                                            //在这里配置执行延时
                                            .WithIntervalInSeconds(seconds)
                                            .RepeatForever())
                                         .Build();
        }
        /// <summary>
        /// 开启scheduler
        /// </summary>
        /// <returns></returns>
        public static async Task Start()
        {
            CheckProperties();

            IScheduler scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.Start();
        }
        /// <summary>
        /// 关闭scheduler
        /// </summary>
        /// <returns></returns>
        public static async Task Shutdown()
        {
            CheckProperties();

            IScheduler scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.Shutdown();
        }
        /// <summary>
        /// 新建job，以cronExpression调度
        /// </summary>
        /// <param name="code">任务编码</param>
        /// <param name="group">模型分类</param>
        /// <param name="cronExpression">周期表达式</param>
        /// <param name="dataMap">扩展参数</param>
        /// <param name="startNow">是否现在启动</param>
        /// <param name="timeOffset">多少时间后启动</param>
        /// <param name="description">描述</param>
        /// <returns></returns>
        public static async Task NewJob(string code, string group, string cronExpression, IDictionary<string, object> dataMap, bool startNow, DateTimeOffset timeOffset, string description = "")
        {
            CheckProperties();

            IScheduler scheduler = await _schedulerFactory.GetScheduler();
          
            IJobDetail jobDetail = CreateJobDetail(code, group, dataMap, description);
            ITrigger trigger = CreateTrigger(code, group, cronExpression, startNow, timeOffset);

            await scheduler.ScheduleJob(jobDetail, trigger);
            await scheduler.Start();
        }
       
        /// <summary>
        /// 新建job，以间隔秒数调度
        /// </summary>
        /// <param name="code">唯一编码</param>
        /// <param name="group">组名称</param>
        /// <param name="intervalSeconds">秒数间隔</param>
        /// <param name="dataMap">扩展参数</param>
        /// <param name="description">描述</param>
        /// <returns></returns>
        public static async Task NewJob(string code, string group, int intervalSeconds, IDictionary<string, object> dataMap, string description = "")
        {
            CheckProperties();

            IScheduler scheduler = await _schedulerFactory.GetScheduler();
          
            IJobDetail jobDetail = CreateJobDetail(code, group, dataMap, description);
            ITrigger trigger = CreateTrigger(code, group, intervalSeconds);

            await scheduler.ScheduleJob(jobDetail, trigger);
            await scheduler.Start();
        }
        /// <summary>
        /// 删除job
        /// </summary>
        /// <param name="code">唯一编码</param>
        /// <param name="group">job所属组</param>
        /// <returns></returns>
        public static async Task<Boolean> DeleteJob(string code, string group)
        {
            IScheduler scheduler = await _schedulerFactory.GetScheduler();

            var trigger = new TriggerKey(GetTriggerIdentityName(code), GetTriggerIdentifyGroup(group));
            // 停止触发器
            await scheduler.PauseTrigger(trigger);
            // 移除触发器
            await scheduler.UnscheduleJob(trigger); 
            await scheduler.DeleteJob(JobKey.Create(GetJobIdentityName(code), GetJobIdentifyGroup(group)));

            return true;
        }
        /// <summary>
        /// 暂停单个job
        /// </summary>
        /// <param name="code">job唯一编码</param>
        /// <param name="group">job所属组</param>
        /// <returns></returns>
        public static async Task<Boolean> PauseJob(string code, string group)
        {
            IScheduler scheduler = await _schedulerFactory.GetScheduler();
            JobKey jobKey = JobKey.Create(GetJobIdentityName(code), GetJobIdentifyGroup(group));
            if (await scheduler.CheckExists(jobKey))
            {
                // 暂停单个任务
                await scheduler.PauseJob(jobKey);
            }
            else
            {
                LOG.Warn($"job[{code}]不存在");
            }
            return true;
        }
        /// <summary>
        /// 暂停所有job
        /// </summary>
        /// <returns></returns>
        public static async Task<Boolean> PauseAllJob()
        {
            IScheduler scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.PauseAll();
        
            return true;
        }
        /// <summary>
        /// 重启指定job
        /// </summary>
        /// <param name="code"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static async Task<Boolean> ResumeJob(string code, string group)
        {
            IScheduler scheduler = await _schedulerFactory.GetScheduler();
            JobKey jobKey = JobKey.Create(GetJobIdentityName(code), GetJobIdentifyGroup(group));
            if (await scheduler.CheckExists(jobKey))
            {
                // 暂停单个任务
                await scheduler.ResumeJob(jobKey);
            }
            else
            {
                LOG.Warn($"job[{code}]不存在");
            }
            return true;
        }
        /// <summary>
        /// 重启所有job
        /// </summary>
        /// <returns></returns>
        public static async Task<Boolean> ResumeAllJob()
        {
            IScheduler scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.ResumeAll();
    
            return true;
        }
        /// <summary>
        /// 更新job的运算周期
        /// </summary>
        /// <param name="code"></param>
        /// <param name="group"></param>
        /// <param name="cronExpression"></param>
        /// <param name="startNow">是否立刻运行</param>
        /// <param name="timeOffset">多少时间后执行</param>
        /// <returns></returns>
        public static async Task<Boolean> UpdateJobSchedule(string code, string group, string cronExpression, bool startNow, DateTimeOffset timeOffset)
        {
            IScheduler scheduler = await _schedulerFactory.GetScheduler();
            TriggerKey triggerKey = new TriggerKey(GetTriggerIdentityName(code));
            ITrigger trigger = CreateTrigger(code, group, cronExpression, startNow, timeOffset);
            await scheduler.RescheduleJob(triggerKey, trigger);

            return true;
        }
    }
}
