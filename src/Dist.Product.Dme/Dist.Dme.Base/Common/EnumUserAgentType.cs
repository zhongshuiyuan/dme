using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Dist.Dme.Base.Common
{
    /// <summary>
    /// 客户端设备类型
    /// </summary>
    public enum EnumUserAgentType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [DisplayName("unknown")]
        UNKNOWN = 0,
        /// <summary>
        /// PC端
        /// </summary>
        [DisplayName("pc")]
        PC = 1,
        /// <summary>
        /// 浏览器端
        /// </summary>
        [DisplayName("browser")]
        BROWSER = 2,
        /// <summary>
        /// 移动端
        /// </summary>
        [DisplayName("mobile")]
        MOBILE = 3,
        /// <summary>
        /// 微信端
        /// </summary>
        [DisplayName("weixin")]
        WEIXIN = 4
    }
}
