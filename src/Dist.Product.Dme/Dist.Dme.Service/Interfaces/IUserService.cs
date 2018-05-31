using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Model.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Service.Interfaces
{
    /// <summary>
    /// 用户服务
    /// </summary>
    public interface IUserService : IBizService
    {
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        IList<UserInfoDTO> ListUsers();
    }
}
