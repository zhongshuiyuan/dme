using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.Service.Interfaces;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Service.Impls
{
    /// <summary>
    /// 业务基类
    /// </summary>
    public abstract class BaseBizService : IBizService
    {
        /// <summary>
        /// 存储库
        /// </summary>
        protected IRepository Repository { get; set; }
        protected SqlSugarClient Db => Repository.GetDbContext();
        /// <summary>
        /// 日志服务
        /// </summary>
        public ILogService LogService { get; set; }

        public bool IsBizGuid(string code)
        {
            string regexStr = @"^(\{{0,1}([0-9a-fA-F]){8}([0-9a-fA-F]){4}([0-9a-fA-F]){4}([0-9a-fA-F]){4}([0-9a-fA-F]){12}\}{0,1})$";
            return GuidUtil.IsGUID(regexStr, code);
        }
    }
}
