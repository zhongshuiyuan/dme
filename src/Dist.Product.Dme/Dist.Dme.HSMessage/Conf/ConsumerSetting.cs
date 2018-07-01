using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.HSMessage.Conf
{
    /// <summary>
    /// 消费方配置
    /// </summary>
    public class ConsumerSetting
    {
        /// <summary>
        /// 组id
        /// </summary>
        public static string GroupId { get; set; } = "consumer-group";
        /// <summary>
        /// 服务器地址
        /// </summary>
        public static string Servers { get; set; } = "localhost:9092";
        /// <summary>
        /// 拉取信息间隔（毫秒）
        /// </summary>
        public static int AutoCommitIntervalMs { get; set; } = 5000;
        /// <summary>
        /// 自动偏移量设置
        /// earliest 
        /// 当各分区下有已提交的offset时，从提交的offset开始消费；无提交的offset时，从头开始消费
        /// latest
        /// 当各分区下有已提交的offset时，从提交的offset开始消费；无提交的offset时，消费新产生的该分区下的数据
        /// none
        /// topic各分区都存在已提交的offset时，从offset后开始消费；只要有一个分区不存在已提交的offset，则抛出异常
        /// </summary>
        public static string AutoOffsetReset { get; set; } = "earliest";
    }
}
