using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.HSMessage.Define
{
    /// <summary>
    /// 消息接收体
    /// </summary>
    public class MessageReceivedBody
    {
        /// <summary>
        /// 接收方
        /// </summary>
        public string To { get; set; }
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
