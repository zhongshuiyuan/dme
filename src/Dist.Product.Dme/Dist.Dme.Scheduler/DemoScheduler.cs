using Dist.Dme.Base.Conf;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dist.Dme.Scheduler
{
    public class DemoScheduler
    {
        private static void Main(string[] args)
        {
            RunProgramRunExample().GetAwaiter().GetResult();
            Console.WriteLine("Press any key to close the application");
            Console.ReadKey();
        }
        public static async Task RunProOracle()
        {
            NameValueCollection properties = new NameValueCollection
            {
                ["quartz.scheduler.instanceName"] = "DmeScheduler",
                ["quartz.scheduler.instanceId"] = "instance_one",
                ["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz",
                ["quartz.threadPool.threadCount"] = "5",
                // 设置为false才能使用UsingJobData自定义属性
                ["quartz.jobStore.useProperties"] = "true",
                // 数据源名称，自定义
                ["quartz.jobStore.dataSource"] = "default",
                // 表名前缀
                ["quartz.jobStore.tablePrefix"] = "DME_QRTZ_",
                // 注意：需要指定quartz.serializer.type，并且不能是json
                ["quartz.serializer.type"] = "json",
                // default要跟上面的数据源名称对应
                ["quartz.dataSource.default.connectionString"] = GlobalSystemConfig.DBConnectionString,// "Data Source=//192.168.1.166:1521/orcl;User Id=dme_scheduler;Password=pass";
                ["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.OracleDelegate, Quartz",
                // 注意：驱动名称
                // .NET Core version (nestandard2.0) only supports [OracleODPManaged], which is also supported by full framework version.
                // https://www.nuget.org/packages/Oracle.ManagedDataAccess/ - full framework
                // https://www.nuget.org/packages/Oracle.ManagedDataAccess.Core/2.12.0-beta3 - .NET Core
                ["quartz.dataSource.default.provider"] = "OracleODPManaged"
            };
            // cluster环境下，集群化后，某节点失效后，剩余的节点能保证job继续执行下去，instanceId设置为自动命名AUTO？
            // properties["quartz.jobStore.clustered"] = "true";
            // properties["quartz.scheduler.instanceId"] = "AUTO";

            ISchedulerFactory schedulerFactory = new StdSchedulerFactory(properties);
            IScheduler scheduler = await schedulerFactory.GetScheduler();

            // and start it off
            await scheduler.Start();

           var datamap = new JobDataMap();
            datamap.Put("ModelCode", "abc");

            // define the job and tie it to our HelloJob class
            IJobDetail jobDetail = JobBuilder.Create<DemoJob>()
                .WithIdentity("job2", "group1")
               .UsingJobData("abc", "123")
                .Build();
            
            // Trigger the job to run now, and then repeat every 10 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger2", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(10)            //在这里配置执行延时
                    .RepeatForever())
                .Build();

            // Tell quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(jobDetail, trigger);
        }
        public static async Task RunProgramRunExample()
        {
            try
            {
                // Grab the Scheduler instance from the Factory
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(props);
                IScheduler scheduler = await factory.GetScheduler();

                // and start it off
                await scheduler.Start();

                // define the job and tie it to our HelloJob class
                IJobDetail job = JobBuilder.Create<DemoJob>()
                    .WithIdentity("job1", "group1")
                    .Build();

                // Trigger the job to run now, and then repeat every 10 seconds
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger1", "group1")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(10)            //在这里配置执行延时
                        .RepeatForever())
                    .Build();

                // Tell quartz to schedule the job using our trigger
                await scheduler.ScheduleJob(job, trigger);

                // some sleep to show what‘s happening
                //                await Task.Delay(TimeSpan.FromSeconds(5));
                // and last shut down the scheduler when you are ready to close your program
                //                await scheduler.Shutdown();           

                //如果解除await Task.Delay(TimeSpan.FromSeconds(5))和await scheduler.Shutdown()的注释，
                //5秒后输出"Press any key to close the application"，
                //scheduler里注册的任务也会停止。


            }
            catch (SchedulerException se)
            {
                Console.WriteLine(se);
            }
        }
    }
}
