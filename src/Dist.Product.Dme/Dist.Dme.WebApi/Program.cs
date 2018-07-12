using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace Dist.Dme.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //NLog配置文件，如果没有用默认的名字，则使用以下方式加载
            NLog.Web.NLogBuilder.ConfigureNLog("nlog.config");  
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                //.UseKestrel()
                //.UseContentRoot(Directory.GetCurrentDirectory())
                //.UseUrls("http://*:7321")
                .UseIISIntegration()
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders(); //移除已经注册的其他日志处理程序
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace); //设置最小的日志级别
                })
                .UseNLog()
                .Build();
    }
}
