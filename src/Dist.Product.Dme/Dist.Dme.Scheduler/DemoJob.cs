using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dist.Dme.Scheduler
{
    public class DemoJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            //任务主体，这里强制要求必须是异步方法，如果不想用异步可以使用quartz 2.x版本
            await Console.Out.WriteLineAsync("Greetings from HelloJob!");
        }
    }
}
