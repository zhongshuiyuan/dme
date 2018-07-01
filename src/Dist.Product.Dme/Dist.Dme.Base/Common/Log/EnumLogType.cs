using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Dist.Dme.Base.Common.Log
{
    /// <summary>
    /// 日志类型枚举
    /// </summary>
    public enum EnumLogType
    {
        [Description("登录")]
        LOGIN,
        [Description("登出")]
        LOGOUT,
        [Description("泛指操作")]
        ACTION,
        [Description("实体日志")]
        ENTITY
    }
}
