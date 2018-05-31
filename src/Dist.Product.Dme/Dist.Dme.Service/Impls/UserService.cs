using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.DAL.Context;
using Dist.Dme.Model;
using Dist.Dme.Model.DTO;
using Dist.Dme.Model.Entity;
using Dist.Dme.Service.Interfaces;
using log4net;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Service.Impls
{
    /// <summary>
    /// 用户服务
    /// </summary>
    public class UserService : BaseBizService, IUserService
    {
        private static ILog LOG = LogManager.GetLogger(typeof(UserService));

        public UserService(IRepository repository)
        {
            base.Repository = repository;
        }
        public IList<UserInfoDTO> ListUsers()
        {
            List<DmeUser> users = base.Repository.GetDbContext().Queryable<DmeUser>().OrderBy(u => u.LoginName, OrderByType.Asc).ToList();
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
