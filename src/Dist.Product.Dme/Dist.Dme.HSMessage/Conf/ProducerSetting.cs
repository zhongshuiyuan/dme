using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.HSMessage.Conf
{
    /// <summary>
    /// 提供方配置
    /// </summary>
    public class ProducerSetting
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        public static string Servers { get; set; } = "localhost:9092";
        /// <summary>
        /// 主题
        /// </summary>
        public static string Topic { get; set; }
    }
}
