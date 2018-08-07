using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.HSMessage.Conf
{
    /// <summary>
    /// websocket配置
    /// </summary>
    public class WebsocketSetting
    {
        /// <summary>
        /// 节点id，主要用于集群环境下，id值在同一个分布式环境下不能重复
        /// </summary>
        public int NodeId { get; set; }
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
    }
}
