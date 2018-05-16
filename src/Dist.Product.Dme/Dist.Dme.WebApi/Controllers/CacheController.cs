using Dist.Dme.Base.Framework;
using Dist.Dme.DisCache.Interfaces;
using Dist.Dme.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Dist.Dme.WebApi.Controllers
{
    /// <summary>
    /// 数据源服务
    /// </summary>
    [Route("api/cache")]
    public class CacheController : BaseController
    {
        public ICacheService CacheService { get; private set; }
        /// <summary>
        /// 自动注入（DI）
        /// </summary>
        /// <param name="cacheService"></param>
        public CacheController(ICacheService cacheService)
        {
            this.CacheService = cacheService;
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/cache/{key}/{value}")]
        public Result AddCache(string key, object value)
        {
            return base.Success(this.CacheService.Add(key, value));
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/cache/{key}")]
        public Result GetCache(string key)
        {
            return base.Success(this.CacheService.Get<object>(key));
        }
    }
}