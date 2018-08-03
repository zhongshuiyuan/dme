using Dist.Dme.Base.Framework;
using Dist.Dme.Model.DTO;
using Dist.Dme.Service.Interfaces;
using Dist.Dme.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Dist.Dme.WebApi.Controllers
{
    /// <summary>
    /// 版本服务
    /// </summary>
    [Route("api/versions")]
    public class VersionController : BaseController
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        public IVersionService VersionService { get; private set; }
        /// <summary>
        /// 自动注入（DI）
        /// </summary>
        /// <param name="versionService"></param>
        public VersionController(IVersionService versionService)
        {
            this.VersionService = versionService;
        }

        /// <summary>
        /// 获取当前版本信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/")]
        public Result GetCurrentVersion()
        {
            return base.Success(this.VersionService.GetCurrentVersion());
        }
        /// <summary>
        /// 更新版本号
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, HttpPut]
        [Route("v1/")]
        public Result UpdateVersion([FromBody]VersionDTO dto)
        {
            return base.Success(this.VersionService.UpdateVersion(dto));
        }
    }
}