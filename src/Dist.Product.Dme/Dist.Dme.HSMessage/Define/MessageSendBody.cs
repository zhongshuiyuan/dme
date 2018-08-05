using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.HSMessage.Define
{
    /// <summary>
    /// 消息发送体
    /// </summary>
    public class MessageSendBody
    {
        /// <summary>
        /// 发送方
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public EnumMessageType Type { get; set; }
        /// <summary>
        /// 具体内容
        /// </summary>
        public string Payload { get; set; }
    }
}
