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
        public string GroupId { get; set; }
        public string Servers { get; set; }
        public string Topics { get; set; }
        public int AutoCommitIntervalMs { get; set; }
    }
}
