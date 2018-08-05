using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Dist.Dme.HSMessage.Define
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public enum EnumMessageType
    {
        /// <summary>
        /// 群消息
        /// </summary>
        [Description("群消息")]
        GROUP = 0,
        /// <summary>
        /// 讨论组消息
        /// </summary>
        [Description("讨论组消息")]
        DISCUSSION = 1,
        /// <summary>
        /// 个人对个人消息
        /// </summary>
        [Description("个人对个人消息")]
        [DisplayName("P2P")]
        P2P = 2
    }
}
