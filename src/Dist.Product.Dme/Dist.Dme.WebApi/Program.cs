﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System.IO;

namespace Dist.Dme.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //NLog配置文件，如果没有用默认的名字，则使用以下方式加载
            // NLog.Web.NLogBuilder.ConfigureNLog("nlog.config");  

            IWebHostBuilder webHostBuilder = CreateWebHostBuilder(args);
            IWebHost webHost = webHostBuilder
                .UseKestrel(options =>
                {
                    //所有controller都不限制post的body大小
                    options.Limits.MaxRequestBodySize = null;
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .ConfigureLogging(logging =>
                {
                    //移除已经注册的其他日志处理程序
                    logging.ClearProviders();
                    //设置最小的日志级别
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog()
                .Build();
            webHost.Run();
        }
        /// <summary>
        /// 创建WebHostBuilder
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder().AddCommandLine(args)
               .Build();
            string host = config["host"];
            string port = config["port"];
            if (!string.IsNullOrEmpty(port))
            {
                // >dotnet xxx.dll  --host 127.0.0.1 --port 7321
                if (string.IsNullOrEmpty(host))
                {
                    host = "*";
                }
                return WebHost.CreateDefaultBuilder(args)
                .UseUrls($"http://{host}:{port}");
            }
            else
            {
                config = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json", optional: true)
                          .AddJsonFile("hosting.json", optional: true)
                          .Build();

                return new WebHostBuilder()
                    .UseConfiguration(config);
            }
        }
    }
}
