using Dist.Dme.Base.Common.Log;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Service.Interfaces
{
    public interface ILogService
    {
        /// <summary>
        /// 异步添加日志
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="logLevel"></param>
        /// <param name="objectType"></param>
        /// <param name="objectId"></param>
        /// <param name="userCode"></param>
        /// <param name="address"></param>
        /// <param name="apps"></param>
        /// <param name="remark"></param>
        /// <param name="ex"></param>
        void AddLogAsync(EnumLogType logType, EnumLogLevel logLevel, string objectType, string objectId, string remark = "", Exception ex = null, string userCode = "", string address = "", string apps = "DME");
    }
}
