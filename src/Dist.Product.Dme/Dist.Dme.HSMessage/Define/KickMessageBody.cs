using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.HSMessage.Define
{
    /// <summary>
    /// 踢人的消息体
    /// </summary>
    public class KickMessageBody
    {
        /// <summary>
        /// 节点key
        /// </summary>
        public string NodeKey { get; set; }
        /// <summary>
        /// 客户端类型（设备类型）
        /// </summary>
        public string AgentType { get; set; }
        /// <summary>
        /// 客户端id
        /// </summary>
        public string AppId { get; set; }
    }
}
