using Dist.Dme.Base.Common;
using NLog;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Dist.Dme.Base.Utils
{
    /// <summary>
    /// 网络辅助类
    /// </summary>
    public sealed class NetAssist
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 对ip进行ping操作
        /// </summary>
        /// <param name="strIP"></param>
        /// <returns></returns>
        public static bool PingIP(string strIP)
        {
            try
            {
                PingOptions pingOption = new PingOptions
                {
                    DontFragment = true
                };
                Ping pingSender = new Ping();
                string data = "sendData:this is a test!";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 120;
                PingReply reply = pingSender.Send(strIP, timeout, buffer);
                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                LOG.Error("ping ip发生错误，详情：" + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 对ip和端口做ping测试
        /// </summary>
        /// <param name="strIP"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool PingPort(string strIP, int port)
        {
            bool strTelnet = false;
            try
            {
                IPAddress ip = IPAddress.None;
                bool isip = IPAddress.TryParse(strIP, out ip);
                if (!isip)
                {
                    strIP = Dns.GetHostEntry(strIP).AddressList[0].ToString();
                }

                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    SendTimeout = 120
                };

                isip = IPAddress.TryParse(strIP, out ip);

                if (isip)
                {
                    sock.Connect(ip, port);
                }
                else
                {
                    return true;
                }

                sock.Shutdown(SocketShutdown.Both);
                sock.Close();
                strTelnet = true;
            }
            catch (SocketException e)
            {
                if (e.ErrorCode != 10061)
                {
                }
                strTelnet = false;
            }
            return strTelnet;
        }

        /// <summary>
        /// 检查服务器和端口是否正常
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool Telnet(string host, int port, out string message)
        {
            message = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(host))
                {
                    message = "SDE服务器配地址未配置";
                    return false;
                }
                if (PingIP(host) == false)
                {
                    message = string.Format("无法连接SDE服务器：{0}!", host);
                    return false;
                }
                if (PingPort(host, port) == false)
                {
                    message = string.Format("无法连接SDE服务器：{0}:{1}!", host, port);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                LOG.Error("检测SDE连接发生错误，详情：" + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 获取IPV4地址
        /// </summary>
        /// <returns></returns>
        public static string GetLocalHost()
        {
            try
            {
                IPAddress[] arrIPAddresses = Dns.GetHostAddresses(Dns.GetHostName());
                foreach (IPAddress ip in arrIPAddresses)
                {
                    if (ip.AddressFamily.Equals(AddressFamily.InterNetwork))
                    {
                        return ip.ToString();
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                LOG.Error("获取本地host地址错误，详情：" + ex.Message);
                return "0.0.0.0";
            }
        }
        /// <summary>
        /// 是否一个合法ip地址
        /// </summary>
        /// <param name="ip"></param>
        /// <returns>true/false</returns>
        public static bool IsIP(string ip)
        {
            if (string.IsNullOrEmpty(ip))
            {
                return false;
            }
            String regexStr = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
            Regex guidReg = new Regex(regexStr);
            return guidReg.IsMatch(ip);
        }
        /// <summary>
        /// 获取user agent类型
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public static EnumUserAgentType GetUserAgentType(string userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
            {
                return EnumUserAgentType.UNKNOWN;
            }
            userAgent = userAgent.ToLower();
            string[] mobileAgents = { "iphone", "android", "phone", "mobile", "wap", "netfront", "java", "opera mobi","opera mini", "ucweb",
                "windows ce", "symbian", "series", "webos", "sony", "blackberry", "dopod", "nokia", "samsung", "palmsource", "xda", "pieplus",
                "meizu", "midp", "cldc", "motorola", "foma", "docomo", "up.browser", "up.link", "blazer", "helio", "hosin", "huawei", "novarra",
                "coolpad", "webos", "techfaith", "palmsource", "alcatel", "amoi", "ktouch", "nexian", "ericsson", "philips", "sagem", "wellcom",
                "bunjalloo", "maui", "smartphone", "iemobile", "spice", "bird", "zte-", "longcos", "pantech", "gionee", "portalmmm", "jig browser",
                "hiptop", "benq", "haier", "^lct", "320x320", "240x320", "176x220", "w3c ", "acs-", "alav", "alca", "amoi", "audi", "avan", "benq",
                "bird", "blac", "blaz", "brew", "cell", "cldc", "cmd-", "dang", "doco", "eric", "hipt", "inno", "ipaq", "java", "jigs", "kddi", "keji", "leno",
                "lg-c", "lg-d", "lg-g", "lge-", "maui", "maxo", "midp", "mits", "mmef", "mobi", "mot-", "moto", "mwbp", "nec-", "newt", "noki", "oper",
                "palm", "pana", "pant", "phil", "play", "port", "prox", "qwap", "sage", "sams", "sany", "sch-", "sec-", "send", "seri", "sgh-", "shar", "sie-",
                "siem", "smal", "smar", "sony", "sph-", "symb", "t-mo", "teli", "tim-", "tsm-", "upg1", "upsi", "vk-v", "voda", "wap-", "wapa", "wapi", "wapp",
                "wapr", "webc", "winw", "winw", "xda", "xda-", "googlebot-mobile" };
            for (int i = 0; i < mobileAgents.Length; i++)
            {
                if (userAgent.ToLower().IndexOf(mobileAgents[i], System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return EnumUserAgentType.MOBILE;
                }
            }
            if (userAgent.IndexOf("micromessenger") > 0)
            {
                return EnumUserAgentType.WEIXIN;
            }
            return EnumUserAgentType.BROWSER;
        }
    }
}
