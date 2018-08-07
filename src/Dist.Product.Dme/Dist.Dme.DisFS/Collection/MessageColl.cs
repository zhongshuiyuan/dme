using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.DisFS.Collection
{
    /// <summary>
    /// 消息集合类
    /// </summary>
    public class MessageColl
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 系统编码
        /// </summary>
        public string SysCode { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// 目标
        /// </summary>
        public string To { get; set; }
        /// <summary>
        /// 频道类型，群消息：0，讨论组消息：1，个人对个人消息：2
        /// </summary>
        public int ChannelType { get; set; }
        /// <summary>
        /// 消息类型，0：系统消息；1：任务消息
        /// </summary>
        public int MsgType { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Payload { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        public long SendTime { get; set; }
        /// <summary>
        /// 是否送达，0：否；1：是
        /// </summary>
        public int Delivered { get; set; }
        /// <summary>
        /// 是否已读，0：否；1：是
        /// </summary>
        public int Read { get; set; }
        /// <summary>
        /// 已读时间
        /// </summary>
        public long ReadTime { get; set; }
    }
}
