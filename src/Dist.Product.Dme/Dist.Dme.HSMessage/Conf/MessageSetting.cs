using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.HSMessage.Conf
{
    public class MessageSetting
    {
        public string Type { get; set; }
        public Opinion Opinion { get; set; }
    }
    public class Opinion
    {
        /// <summary>
        /// 是否开启
        /// </summary>
        public Boolean Switch{ get; set;}
        public string GroupId { get; set; }
        public string Servers { get; set; }
        public string Topics { get; set; }
        public int AutoCommitIntervalMs { get; set; }
    }
}
