using Dist.Dme.DisCache.Interfaces;
using Dist.Dme.Service.Interfaces;
using Dist.Dme.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dist.Dme.WebApi.Controllers.Base
{
    public class CommonController : BaseController
    {
        protected IAlgorithmService algorithmService;
        protected ICacheService cacheService;
        protected IDataSourceService dataSourceService;
        protected IModelService modelService;
        protected ITaskService taskService;
        protected IUserService userService;
        protected IVersionService versionService;

        public CommonController()
        {
            // 如果直接使用ServiceLocator.Instance.GetService方法获取自定义服务是，使用Scoped方式注册服务的话这里获取失败。
            // hca.HttpContext.RequestServices.GetService<IMyService>();
            // IHttpContextAccessor hca = (IHttpContextAccessor)ServiceLocator.Instance.GetService(typeof(IHttpContextAccessor));
            this.algorithmService = (IAlgorithmService)ServiceLocator.Instance.GetService(typeof(IAlgorithmService));
            this.cacheService = (ICacheService)ServiceLocator.Instance.GetService(typeof(ICacheService));
            this.dataSourceService = (IDataSourceService)ServiceLocator.Instance.GetService(typeof(IDataSourceService));
            this.modelService = (IModelService)ServiceLocator.Instance.GetService(typeof(IModelService));
            this.taskService = (ITaskService)ServiceLocator.Instance.GetService(typeof(ITaskService));
            this.userService = (IUserService)ServiceLocator.Instance.GetService(typeof(IUserService));
            this.versionService = (IVersionService)ServiceLocator.Instance.GetService(typeof(IVersionService));
        }
    }
}
