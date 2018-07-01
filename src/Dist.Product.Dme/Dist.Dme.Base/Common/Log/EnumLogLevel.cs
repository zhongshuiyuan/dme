using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Dist.Dme.Base.Common.Log
{
    /// <summary>
    /// 日志级别枚举
    /// </summary>
    public enum EnumLogLevel
    {
        [Description("错误日志")]
        ERROR,
        [Description("最小日志")]
        MINIMAL,
        [Description("基本日志")]
        BASIC,
        [Description("详细日志")]
        DETAILED,
        [Description("调试日志")]
        DEBUG
    }
}
