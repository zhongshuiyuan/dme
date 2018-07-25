using Dist.Dme.Base.Framework;
using Dist.Dme.DisCache.Interfaces;
using Dist.Dme.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;

namespace Dist.Dme.WebApi.Controllers
{
    /// <summary>
    /// 数据源服务
    /// </summary>
    [Route("api/caches")]
    public class CacheController : BaseController
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
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
        /// <param name="keyValuePair">使用动态类型dynamic，传过来是个json字符串，键值对</param>
        /// <returns></returns>
        /// 需要使用FromBody
        [HttpPost]
        [Route("v1/")]
        public Result AddCache([FromBody]dynamic keyValuePair)
        {
            string key = Convert.ToString(keyValuePair.key);
            object value = keyValuePair.value;
            LOG.Info($"添加缓存，key:{key}, value:{value}");
            return base.Success(this.CacheService.Add(key, value));
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/{key}")]
        public Result GetCache(string key)
        {
            return base.Success(this.CacheService.Get<object>(key));
        }
    }
}