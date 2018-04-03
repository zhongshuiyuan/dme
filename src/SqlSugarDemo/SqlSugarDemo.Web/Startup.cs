using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugarDemo.Busines.Base;

namespace SqlSugarDemo.Web
{
    public class Startup
    {
        readonly private IConfiguration _Configuration;
        public Startup(IConfiguration configuration)
        {
            this._Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            SugarBase.DB_ConnectionString = this._Configuration.GetConnectionString("Sugar");   //为数据库连接字符串赋值
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default1",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
