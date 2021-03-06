﻿using Dist.Dme.Base.Conf;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.DAL.Context;
using Dist.Dme.DisCache.Define;
using Dist.Dme.DisCache.Impls;
using Dist.Dme.DisCache.Interfaces;
using Dist.Dme.DisCache.Redis;
using Dist.Dme.DisFS.Adapters.Mongo;
using Dist.Dme.Extensions;
using Dist.Dme.HSMessage.Conf;
using Dist.Dme.HSMessage.MQ.Kafka;
using Dist.Dme.HSMessage.Websocket.Fleck;
using Dist.Dme.Scheduler;
using Dist.Dme.Service.Impls;
using Dist.Dme.Service.Interfaces;
using Dist.Dme.Service.Scheduler;
using Dist.Dme.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using MongoDB.Driver;
using NLog;
using NLog.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dist.Dme.WebApi
{
    public class Startup
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

       
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            // 设置数据源，读取appsettings.json配置文件
            GlobalSystemConfig.DBConnectionString = this.Configuration.GetConnectionString("DataSource");
            // 注册缓存对象
            services.AddMemoryCache();
            IConfigurationSection connectionStringsSection = this.Configuration.GetSection("ConnectionStrings");
           IConfigurationSection cacheProviderSection = connectionStringsSection.GetSection("CacheProvider");
            if (cacheProviderSection != null)
            {
                try
                {
                    string type = cacheProviderSection.GetValue<string>("type");
                    if ("redis".Equals(type, StringComparison.OrdinalIgnoreCase))
                    {
                        // redis 分布式缓存
                        RedisCacheProvider provider = cacheProviderSection.GetSection("provider").Get<RedisCacheProvider>();
                        services.AddSingleton(typeof(ICacheService), new RedisCacheService(new RedisCacheOptions
                        {
                            Configuration = provider.HostName + ":" + provider.Port,
                            InstanceName = provider.InstanceName
                        }, provider.DataBase));
                    }
                    else if ("redis.r.w".Equals(type, StringComparison.OrdinalIgnoreCase))
                    {
                        // redis读写分离
                        RedisRWConfigInfo provider = cacheProviderSection.GetSection("provider").Get<RedisRWConfigInfo>();
                        RedisManager redisManager = new RedisManager(provider);
                        services.AddSingleton(typeof(ICacheService), new RedisRWCacheService(redisManager.GetClient()));
                    }
                }
                catch (Exception ex)
                {
                    LOG.Error(ex, "redis连接失败，准备启用MemoryCache服务");
                    SetCacheService(services);
                }
            }
            else
            {
                SetCacheService(services);
            }
            // mongo
            IConfigurationSection mongoSection = this.Configuration.GetSection("ConnectionStrings").GetSection("Mongo");
            if (mongoSection != null)
            {
                try
                {
                    // 注册mongo连接信息
                    MongodbHost mongohost = mongoSection.Get<MongodbHost>();
                    services.AddSingleton(typeof(MongodbHost), mongohost);
                    // IMongoClient mongoClient = new MongoClient(mongohost.Connection);
                    IMongoClient mongoClient = MongodbManager<object>.GetMongodbClient(mongohost.ConnectionString);
                    services.AddSingleton(typeof(IMongoClient), mongoClient);
                    IMongoDatabase mongoDatabase = mongoClient.GetDatabase(mongohost.DataBase);
                    services.AddSingleton(typeof(IMongoDatabase), mongoDatabase);
                }
                catch (Exception ex)
                {
                    LOG.Error(ex, "mongo连接失败");
                }
            }
            // 注册知识库
            services.AddSingleton<IRepository, Repository>();
            // 注册版本服务
            services.AddSingleton<IVersionService, VersionService>();
            // 注册用户服务
            services.AddSingleton<IUserService, UserService>();
            // 注册模型服务
            services.AddSingleton< IModelService, ModelService> ();
            // 注册数据源服务
            services.AddSingleton<IDataSourceService, DataSourceService>();
            // 注册算法服务
            services.AddSingleton<IAlgorithmService, AlgorithmService>();
            // 注册业务日志服务
            services.AddSingleton<ILogService, LogService>();
            // 注册任务服务
            services.AddSingleton<ITaskService, TaskService>();
            // 设置全局
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            ServiceFactory.CacheService = serviceProvider.GetService<ICacheService>();
            ServiceFactory.MongoHost = serviceProvider.GetService<MongodbHost>();
            ServiceFactory.MongoClient = serviceProvider.GetService<IMongoClient>();
            ServiceFactory.MongoDatabase = serviceProvider.GetService<IMongoDatabase>();
            // 消息相关
            IConfigurationSection messageSection = connectionStringsSection.GetSection("Message");
            if (messageSection != null)
            {
                // 消息
                IConfigurationSection mqSection = messageSection.GetSection("MQ");
                if (mqSection != null)
                {
                    KafkaSetting kafkaSetting = mqSection.Get<KafkaSetting>();
                    if (kafkaSetting.Switch)
                    {
                        KafkaConsumer.CreateConsumer(kafkaSetting.Opinion.GroupId, kafkaSetting.Opinion.Servers, kafkaSetting.Opinion.Topics);
                        KafkaConsumer.Start();
                        KafkaProducer.CreateProducer(kafkaSetting.Opinion.Servers);
                    }
                }
                // websocket
                IConfigurationSection websocketSection = messageSection.GetSection("Websocket");
                if (websocketSection != null)
                {
                    WebsocketSetting websocketSetting = websocketSection.Get<WebsocketSetting>();
                    WebsocketFleckServer.CreateWebsocketServer(websocketSetting.NodeId, websocketSetting.Port, websocketSetting.Host);
                }
            }
            // scheduler，注入参数设置
            IConfigurationSection schedulerSection = this.Configuration.GetSection("Scheduler");
            if (schedulerSection != null)
            {
                if (schedulerSection.GetValue<Boolean>("switch"))
                {
                    IConfigurationSection propertiesSection = schedulerSection.GetSection("properties");
                    var values = propertiesSection.GetChildren()
                    .Select(item => new KeyValuePair<string, string>(item.Key,
                    item.Value.Contains("$") ? Configuration.GetValue<string>(item.Value.Replace("${", "").Replace("}", "")) : item.Value))
                    .ToDictionary(x => x.Key, x => x.Value);

                    DmeQuartzScheduler<TaskRunnerJob>.SetSchedulerProperties(DataUtil.ToNameValueCollection(values));
                    // 调取开启。如果不开启，则不会执行。
                    DmeQuartzScheduler<TaskRunnerJob>.Start().GetAwaiter();
                }
            }

            // DemoScheduler.RunProOracle().Wait();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Dist Model Engine接口文档",
                    Description = "RESTful API for DME",
                    TermsOfService = "None",
                    Contact = new Contact { Name = "weifj", Email = "weifj@dist.com.cn", Url = "https://github.com/leoterry-ulrica/dme" }
                });
                //Set the comments path for the swagger json and ui.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "Dist.Dme.WebApi.xml");
                c.IncludeXmlComments(xmlPath);
                //  c.OperationFilter<HttpHeaderOperation>(); // 添加httpHeader参数
            });
            
            // 配置日志服务
            services.AddLogging();
        }

        private static void SetCacheService(IServiceCollection services)
        {
            // 如果为空，默认使用memorycache
            services.AddSingleton<IMemoryCache>(factory =>
            {
                var cache = new MemoryCache(new MemoryCacheOptions());
                return cache;
            });
            services.AddSingleton<ICacheService, MemoryCacheService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // 获取注册的服务
            ServiceLocator.Instance = app.ApplicationServices;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dist Model Engine API V1");
                c.ShowExtensions();
                //c.ShowRequestHeaders();
            });
            app.UseMvc();

            // log
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);//这是为了防止中文乱码
            loggerFactory.AddNLog();//添加NLog
        }
    }
}
