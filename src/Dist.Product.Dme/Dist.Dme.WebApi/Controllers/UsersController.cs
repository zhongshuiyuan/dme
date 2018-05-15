using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Utils;
using Dist.Dme.DisCache.Interfaces;
using Dist.Dme.Model.DTO;
using Dist.Dme.Model.Entity;
using Dist.Dme.Service.Impls;
using Dist.Dme.Service.Interfaces;
using Dist.Dme.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Dist.Dme.WebApi.Controllers
{
    /// <summary>
    /// 数据源服务
    /// </summary>
    [Route("api/users")]
    public class UsersController : BaseController
    {
        public IUserService UserService { get; private set; }
        public ICacheService CacheService { get; private set; }
        /// <summary>
        /// 自动注入（DI）
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="cacheService"></param>
        public UsersController(IUserService userService, ICacheService cacheService)
        {
            this.UserService = userService;
            this.CacheService = cacheService;
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/")]
        public Result ListUsers()
        {
            return base.Success(this.UserService.ListUsers());
        }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/cache/{key}/{value}")]
        public Result AddCache(string key, string value)
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