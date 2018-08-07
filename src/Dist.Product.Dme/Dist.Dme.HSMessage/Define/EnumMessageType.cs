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
        /// 系统消息，每个客户端都能接收到
        /// </summary>
        [Description("系统消息")]
        SYSTEM = 0,
        /// <summary>
        /// 当前任务执行者才能接收到
        /// </summary>
        [Description("任务类型消息")]
        TASK = 1,
        /// <summary>
        /// 踢人消息
        /// </summary>
        [Description("踢人消息")]
        KICK = 2
    }
}
