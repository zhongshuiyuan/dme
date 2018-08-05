using Dist.Dme.Base.Common;
using Dist.Dme.Base.Utils;
using Dist.Dme.HSMessage.Define;
using Fleck;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Dist.Dme.HSMessage.Websocket.Fleck
{
    /// <summary>
    /// 集成实现websocket的Fleck框架
    /// </summary>
    public class WebsocketFleckServer
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// appId和user-agent作为key，socket作为value
        /// </summary>
        private static IDictionary<string, IWebSocketConnection> appIdAgent_Sockets = new Dictionary<string, IWebSocketConnection>();
        /// <summary>
        ///  appId和user-agent作为key，MessageAuthKey作为value
        /// </summary>
        private static IDictionary<string, MessageAuthKey> appIdAgent_AuthKey = new Dictionary<string, MessageAuthKey>();
        /// <summary>
        /// 同一个appId下有多少个不同的端同时在线
        /// </summary>
        private static IDictionary<string, HashSet<string>> appId_Agent = new Dictionary<string, HashSet<string>>();
        private WebSocketServer webSocketServer;
        public WebsocketFleckServer(int port = 30000, string host = "0.0.0.0")
        {
            if (string.IsNullOrEmpty(host))
            {
                host = "0.0.0.0";
            }
            //创建，监听所有的的地址
            webSocketServer = new WebSocketServer($"ws://{host}:{port}")
            {
                //出错后进行重启
                RestartAfterListenError = true
            };
            webSocketServer.ListenerSocket.NoDelay = true;
            StartAsync();
        }
        /// <summary>
        /// 开始监听
        /// </summary>
        private Task StartAsync()
        {
            return Task.Run(() => {
                webSocketServer.Start(socket =>
                {
                    socket.OnOpen = () =>   //连接建立事件
                    {
                        //获取客户端网页的url
                        MessageAuthKey messageAuthKey = GetAppAuthKey(socket);
                        if (null == messageAuthKey)
                        {
                            return;
                        }
                        appIdAgent_Sockets.Add(messageAuthKey.AppId + "_" + messageAuthKey.AgentType  , socket);
                        appIdAgent_AuthKey.Add(messageAuthKey.AppId + "_" + messageAuthKey.AgentType  , messageAuthKey);
                        if (appId_Agent.ContainsKey(messageAuthKey.AppId))
                        {
                            appId_Agent[messageAuthKey.AppId].Add(messageAuthKey.AgentType);
                        }
                        else
                        {
                            appId_Agent[messageAuthKey.AppId] = new HashSet<string>() { messageAuthKey.AgentType};
                        }
                        LOG.Info(DateTime.Now.ToString() + $"|服务器：和客户端网页:{messageAuthKey.AgentType}，{messageAuthKey.AppId} 建立WebSock连接！");
                    };
                    socket.OnClose = () =>  //连接关闭事件
                    {
                        // string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                        MessageAuthKey messageAuthKey = GetAppAuthKey(socket);
                        if (null == messageAuthKey)
                        {
                            return;
                        }
                        //如果存在这个客户端,那么对这个socket进行移除
                        if (appIdAgent_Sockets.ContainsKey(messageAuthKey.AppId + "_" + messageAuthKey.AgentType ))
                        {
                            //注:Fleck中有释放
                            //关闭对象连接 
                            //if (dic_Sockets[clientUrl] != null)
                            //{
                            //dic_Sockets[clientUrl].Close();
                            //}
                            appIdAgent_Sockets.Remove(messageAuthKey.AppId  + "_" + messageAuthKey.AgentType);
                            appIdAgent_AuthKey.Remove(messageAuthKey.AppId + "_" + messageAuthKey.AgentType );
                            appId_Agent.Remove(messageAuthKey.AppId);
                        }
                        LOG.Info(DateTime.Now.ToString() + $"|服务器：和客户端:{messageAuthKey.AgentType}，{messageAuthKey.AppId}断开WebSock连接！");
                    };
                    socket.OnMessage = message =>
                    {
                        // 接受客户端网页消息事件
                        SendAsync(socket, message);
                    };
                    socket.OnBinary = file => {
                        // 传入文件
                        string path = ("D:/test.txt");
                        //创建一个文件流
                        FileStream fs = new FileStream(path, FileMode.Create);

                        //将byte数组写入文件中
                        fs.Write(file, 0, file.Length);
                        //所有流类型都要关闭流，否则会出现内存泄露问题
                        fs.Close();
                    };
                });
            });
        }
        /// <summary>
        /// 获取客户端ID
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        private static MessageAuthKey GetAppAuthKey(IWebSocketConnection socket)
        {
            MessageAuthKey messageAuthKey = null;
            NameValueCollection queryParams = UriUtil.GetQueryString(socket.ConnectionInfo.Path);
            string appKey = queryParams["appKey"];
            string appId = queryParams["appId"];
            // string authKeyStr =socket.ConnectionInfo.Path.Substring(socket.ConnectionInfo.Path.LastIndexOf("?") + 1);
            string callbackMsg = "";
            if (string.IsNullOrEmpty(appKey) || string.IsNullOrEmpty(appId))
            {
                callbackMsg = $"appKey或appId 信息不存在，连接被拒绝";
                LOG.Error(callbackMsg);
                socket.Send(callbackMsg);
                socket.Close();
                return null;
            }
            try
            {
                // 获取agent类型，支持多端同时连接
                EnumUserAgentType enumUserAgentType = NetAssist.GetUserAgentType(socket.ConnectionInfo.Headers["user-agent"]);
                string agentType = EnumUtil.GetEnumName<EnumUserAgentType>((int)enumUserAgentType);

                if (appIdAgent_AuthKey.ContainsKey(appId + "_" + agentType))
                {
                    return appIdAgent_AuthKey[appId + "_" + agentType];
                }
                messageAuthKey = new MessageAuthKey
                {
                    AppId = appId,
                    AppKey = appKey,
                    AgentType = agentType
                };
           
                // @TODO 验证appkey的有限性和appId是否唯一
                return messageAuthKey;
            }
            catch (Exception ex)
            {
                LOG.Error(ex, ex.Message);
                callbackMsg = $"连接信息缺失，连接被拒绝。详情：{ex.Message}";
                socket.Send(callbackMsg);
                socket.Close();
                return null;
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private Task<Boolean> SendAsync(IWebSocketConnection socket, string message)
        {
            return Task.Run(() => {
                MessageAuthKey fromClientId = GetAppAuthKey(socket);
                if (null == fromClientId)
                {
                    return false;
                }
                MessageReceivedBody messageBody = JsonConvert.DeserializeObject<MessageReceivedBody>(message);
                // string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                LOG.Info(DateTime.Now.ToString() + $"|服务器：【收到】来客户端:{fromClientId.AppId}的信息：\n" + message);
                // 判断同一个appId下多端
                if (appId_Agent.ContainsKey(messageBody.To))
                {
                    HashSet<string> agents = appId_Agent[messageBody.To];
                    foreach (var agent in agents)
                    {
                        // 说明肯定在线
                        LOG.Info($"客户端[{messageBody.To}]在线，设备类型[{agent}]，发送在线消息");
                        IWebSocketConnection desSocket = appIdAgent_Sockets[messageBody.To + "_" + agent];

                        MessageSendBody desSendMsgBody = new MessageSendBody
                        {
                            From = fromClientId.AppId,
                            Type = messageBody.Type,
                            Payload = messageBody.Payload
                        };
                        LOG.Info(DateTime.Now.ToString() + $"|服务器：【发出】消息，来源：{fromClientId.AppId}，目的：{messageBody.To}\n，内容：{messageBody.Payload}");
                        desSocket.Send(JsonConvert.SerializeObject(desSendMsgBody));
                    }
                }
                else
                {
                    LOG.Info($"客户端[{messageBody.To}]离线，发送离线消息");
                    // TODO
                }

                return true;
            });
        }

        /// <summary>
        /// 关闭与客户端的所有的连接
        /// </summary>
        public void CloseAll()
        {
            foreach (var item in appIdAgent_Sockets.Values)
            {
                if (item != null)
                {
                    item.Close();
                }
            }
        }
        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="message">消息体</param>
        public void Broadcast(string message)
        {
            LOG.Info($"广播消息：{message}");
            Task.Run(() => {
                foreach (var socket in appIdAgent_Sockets.Values)
                {
                    if (socket.IsAvailable)
                    {
                        socket.Send(message);
                    }
                }
            });
        }
        /// <summary>
        /// 获取所有客户端信息
        /// </summary>
        /// <returns></returns>
        public IList<string> ListClients()
        {
            List<string> clients = new List<string>();
            foreach (var item in appIdAgent_Sockets.Keys)
            {
                clients.Add(item);
            }
            return clients;
        }
    }
}
