using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Dist.Dme.HSMessage.Define
{
    /// <summary>
    /// 频道类别
    /// </summary>
    public enum EnumChannelType
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
        P2P = 2
    }
}
