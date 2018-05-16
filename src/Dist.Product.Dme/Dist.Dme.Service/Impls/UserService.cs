using Dist.Dme.Base.Utils;
using Dist.Dme.DAL.Context;
using Dist.Dme.Model;
using Dist.Dme.Model.DTO;
using Dist.Dme.Model.Entity;
using Dist.Dme.Service.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Service.Impls
{
    /// <summary>
    /// 用户服务
    /// </summary>
    public class UserService : AbstractContext, IUserService
    {
        private static ILog LOG = LogManager.GetLogger(typeof(UserService));
        public IList<UserInfoDTO> ListUsers()
        {
            List<DmeUser> users = base.DmeUserDb.GetList();
            if (null == users || 0 == users.Count)
            {
                return null;
            }
            IList<UserInfoDTO> userDtos = new List<UserInfoDTO>();
            foreach (var u in users)
            {
                userDtos.Add(ClassValueCopier<UserInfoDTO>.Copy(u));
            }
            return userDtos;
        }
    }
}
