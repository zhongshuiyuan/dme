using Dist.Dme.Base.Framework;
using Dist.Dme.Service.Interfaces;
using Dist.Dme.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Dist.Dme.WebApi.Controllers
{
    /// <summary>
    /// 用户服务
    /// </summary>
    [Route("api/users")]
    public class UsersController : BaseController
    {
        public IUserService UserService { get; private set; }
        /// <summary>
        /// 自动注入（DI）
        /// </summary>
        /// <param name="userService"></param>
        public UsersController(IUserService userService)
        {
            this.UserService = userService;
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
    }
}