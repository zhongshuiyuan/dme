using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dist.Dme.AECommon.Utils;
using Dist.Dme.Base.Conf;
using Dist.Dme.DisCache.Define;
using Dist.Dme.DisCache.Impls;
using Dist.Dme.DisCache.Interfaces;
using Dist.Dme.DisCache.Redis;
using Dist.Dme.Service.Impls;
using Dist.Dme.Service.Interfaces;
using log4net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;

namespace Dist.Dme.WebApi
{
    public class Startup
    {
        private static ILog LOG = LogManager.GetLogger(typeof(Startup));

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
            SysConfig.DBConnectionString = this.Configuration.GetConnectionString("DataSource");
            // 注册缓存对象
            services.AddMemoryCache();
           IConfigurationSection cacheProviderSection = this.Configuration.GetSection("CacheProvider");
            if (cacheProviderSection != null)
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
                    }, provider.Database));
                } else if ("redis.r.w".Equals(type, StringComparison.OrdinalIgnoreCase))
                {
                    // redis读写分裂
                    RedisRWConfigInfo provider = cacheProviderSection.GetSection("provider").Get<RedisRWConfigInfo>();
                    RedisManager redisManager = new RedisManager(provider);
                    services.AddSingleton(typeof(ICacheService), new RedisRWCacheService(redisManager.GetClient()));
                }
            }
            else
            {
                // 如果为空，默认使用memorycache
                services.AddSingleton<IMemoryCache>(factory =>
                {
                    var cache = new MemoryCache(new MemoryCacheOptions());
                    return cache;
                });
                services.AddSingleton<ICacheService, MemoryCacheService>();
            }
            // 注册用户服务
            services.AddSingleton<IUserService, UserService>();
            // 注册模型服务
            services.AddSingleton< IModelService, ModelService> ();
            // 注册数据源服务
            services.AddSingleton<IDataSourceService, DataSourceService>();
            // 注册算法服务
            services.AddSingleton<IAlgorithmService, AlgorithmService>();

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
          
        }
       
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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
        }
    }
}
