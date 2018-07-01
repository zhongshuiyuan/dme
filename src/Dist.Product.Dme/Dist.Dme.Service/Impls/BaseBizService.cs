using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.Service.Interfaces;
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
        public IRepository Repository { get; set; }
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
