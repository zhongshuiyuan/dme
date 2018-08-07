using Dist.Dme.Base.Common;
using Dist.Dme.Base.Utils;
using Dist.Dme.DisFS.Adapters.Mongo;
using Dist.Dme.DisFS.Collection;
using Dist.Dme.Extensions;
using Dist.Dme.HSMessage.Define;
using Dist.Dme.HSMessage.MQ.Kafka;
using Fleck;
using MongoDB.Driver;
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
        private static object _LOCK_OBJ = new object();
        /// <summary>
        /// 节点唯一id，用于分布式环境下
        /// </summary>
        private static int _nodeId;
        /// <summary>
        /// socket节点key前缀
        /// </summary>
        private static string _NODE_KEY_PREFIX = "DME_SOCKET_NODE_";
        /// <summary>
        /// appId和user-agent作为key，socket作为value
        /// </summary>
        private static IDictionary<string, IDictionary<string, IWebSocketConnection>> appId_Agent_SocketsMap = new Dictionary<string, IDictionary<string, IWebSocketConnection>>();
        /// <summary>
        ///  appId和user-agent作为key，MessageAuthKey作为value
        /// </summary>
        private static IDictionary<string, IDictionary<string, MessageAuthKey>> appId_Agent_AuthKeyMap = new Dictionary<string, IDictionary<string, MessageAuthKey>>();

        private static WebSocketServer _webSocketServer = null;
        public static void CreateWebsocketServer(int nodeId = 0, int port = 30000, string host = "0.0.0.0")
        {
            WebsocketFleckServer._nodeId = nodeId;
            if (string.IsNullOrEmpty(host))
            {
                host = "0.0.0.0";
            }
            //创建，监听所有的的地址
            _webSocketServer = new WebSocketServer($"ws://{host}:{port}")
            {
                //出错后进行重启
                RestartAfterListenError = true
            };
            _webSocketServer.ListenerSocket.NoDelay = true;
            StartAsync();
        }
        /// <summary>
        /// 开始监听
        /// </summary>
        private static Task StartAsync()
        {
            return Task.Run(() => {
                _webSocketServer.Start(socket =>
                {
                    socket.OnOpen = () =>   //连接建立事件
                    {
                        //获取客户端信息
                        MessageAuthKey messageAuthKey = GetAppAuthKey(socket);
                        if (null == messageAuthKey)
                        {
                            return;
                        }
                        if (AddAgentSocket(socket, messageAuthKey))
                        {
                            LOG.Info(DateTime.Now.ToString() + $"|服务器：和客户端:{messageAuthKey.AgentType}，{messageAuthKey.AppId} 建立WebSock连接！");
                        }
                    };
                    socket.OnClose = () =>  //连接关闭事件
                    {
                        // string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                        MessageAuthKey messageAuthKey = GetAppAuthKey(socket);
                        if (null == messageAuthKey)
                        {
                            return;
                        }
                        if(RemoveAgentSocket(messageAuthKey))
                        {
                            LOG.Info(DateTime.Now.ToString() + $"|服务器：和客户端:{messageAuthKey.AgentType}，{messageAuthKey.AppId}断开WebSock连接！");
                        }
                    };
                    socket.OnMessage = message =>
                    {
                        // 接受客户端消息事件
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
        /// 踢人
        /// </summary>
        /// <param name="nodeKey">节点key</param>
        /// <param name="appId">appId</param>
        /// <param name="agentType">设备类型</param>
        public static void KickSocket(string nodeKey, string appId, string agentType)
        {
            if (!Object.Equals(GetNodeKey(), nodeKey))
            {
                LOG.Info($"当前node为{GetNodeKey()}，跟kick node[{nodeKey}]信息不匹配，忽略剔除");
                return;
            }
            if (!appId_Agent_SocketsMap.ContainsKey(appId))
            {
                LOG.Info($"当前node没有找到appId[{appId}]，忽略剔除");
                return;
            }
           IDictionary<string, IWebSocketConnection> agentSocketMap =  appId_Agent_SocketsMap[appId];
            if (!agentSocketMap.ContainsKey(agentType))
            {
                LOG.Info($"当前node，appId[{appId}]下没有找到agent[{agentType}]，忽略剔除");
                return;
            }
            agentSocketMap[agentType].Close();
            LOG.Info($"当前node，appId[{appId}]下找到agent[{agentType}]，剔除完成");
        }

        /// <summary>
        /// 获取节点唯一key
        /// </summary>
        /// <returns></returns>
        private static string GetNodeKey()
        {
            return _NODE_KEY_PREFIX + _nodeId;
        }
        /// <summary>
        /// 添加某类客户端连接
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="messageAuthKey"></param>
        private static Boolean AddAgentSocket(IWebSocketConnection socket, MessageAuthKey messageAuthKey)
        {
            // 上锁
            lock (_LOCK_OBJ)
            {
                // 先判断分布式缓存中是否已经有对应的socket
                IDictionary<string, HashSet<string>>  nodeSocketMap = ServiceFactory.CacheService.Get<IDictionary<string, HashSet<string>>>(messageAuthKey.AppId);
                if (nodeSocketMap != null)
                {
                    // 当前用户id下跟多少个node建立了socket连接
                    foreach (var node_agents in nodeSocketMap)
                    {
                        // 排除当前节点
                        if (Object.Equals(node_agents.Key, GetNodeKey()))
                        {
                            continue;
                        }
                        HashSet<string> agentTypes = node_agents.Value;
                        foreach (var agentType in agentTypes)
                        {
                            if (Object.Equals(messageAuthKey.AgentType, agentType))
                            {
                                // 说明需要踢掉其它节点同一个设备的socket
                                // 发消息
                                KafkaProducer.Send(nameof(EnumMessageType.KICK), JsonConvert.SerializeObject(new KickMessageBody() {
                                    AppId = messageAuthKey.AppId,
                                    AgentType = agentType,
                                    NodeKey = node_agents.Key
                                }));
                            }
                        }
                    }
                }

                IDictionary<string, IWebSocketConnection> agentSocket = null;
                if (appId_Agent_SocketsMap.ContainsKey(messageAuthKey.AppId))
                {
                    agentSocket = appId_Agent_SocketsMap[messageAuthKey.AppId];
                    if (agentSocket.ContainsKey(messageAuthKey.AgentType))
                    {
                        // 强制踢人
                        // TODO 后续应该需要客户端主动触发确认才踢掉
                        agentSocket[messageAuthKey.AgentType].Close();
                    }
                    agentSocket[messageAuthKey.AgentType] = socket;
                }
                else
                {
                    agentSocket = new Dictionary<string, IWebSocketConnection>
                    {
                        [messageAuthKey.AgentType] = socket
                    };
                    appId_Agent_SocketsMap.Add(messageAuthKey.AppId, agentSocket);
                }
                IDictionary<string, MessageAuthKey> agentAuthkey = null;
                if (appId_Agent_AuthKeyMap.ContainsKey(messageAuthKey.AppId))
                {
                    agentAuthkey = appId_Agent_AuthKeyMap[messageAuthKey.AppId];
                    if (!agentAuthkey.ContainsKey(messageAuthKey.AgentType))
                    {
                        agentAuthkey[messageAuthKey.AgentType] = messageAuthKey;
                    }
                }
                else
                {
                    agentAuthkey = new Dictionary<string, MessageAuthKey>
                    {
                        [messageAuthKey.AgentType] = messageAuthKey
                    };
                    appId_Agent_AuthKeyMap.Add(messageAuthKey.AppId, agentAuthkey);
                }

                return true;
            } 
        }

        /// <summary>
        /// 移除某类客户端连接
        /// </summary>
        /// <param name="messageAuthKey"></param>
        private static Boolean RemoveAgentSocket(MessageAuthKey messageAuthKey)
        {
            //如果存在这个客户端,那么对这个socket进行移除
            if (appId_Agent_SocketsMap.ContainsKey(messageAuthKey.AppId)
            && appId_Agent_SocketsMap[messageAuthKey.AppId].ContainsKey(messageAuthKey.AgentType))
            {
                //注:Fleck中有释放
                //关闭对象连接 
                //if (dic_Sockets[clientUrl] != null)
                //{
                //dic_Sockets[clientUrl].Close();
                //}
                appId_Agent_SocketsMap[messageAuthKey.AppId].Remove(messageAuthKey.AgentType);
                appId_Agent_AuthKeyMap[messageAuthKey.AppId].Remove(messageAuthKey.AgentType);
            }
            return true;
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
                EnumUserAgentType enumUserAgentType = NetAssist.GetUserAgentType(socket.ConnectionInfo.Headers["User-Agent"]);
                string agentType = EnumUtil.GetEnumName<EnumUserAgentType>((int)enumUserAgentType);

                if (appId_Agent_AuthKeyMap.ContainsKey(appId))
                {
                    if (appId_Agent_AuthKeyMap[appId].ContainsKey(agentType))
                    {
                        return appId_Agent_AuthKeyMap[appId][agentType];
                    }
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
        private static Task<Boolean> SendAsync(IWebSocketConnection socket, string message)
        {
            return Task.Run(() => {
                MessageAuthKey fromClientId = GetAppAuthKey(socket);
                if (null == fromClientId)
                {
                    return false;
                }
                MessageBody messageBody = JsonConvert.DeserializeObject<MessageBody>(message);
                // string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                LOG.Info(DateTime.Now.ToString() + $"|服务器：【收到】来客户端:{fromClientId.AppId}的信息：\n" + message);
                // 判断同一个appId下多端
                if (appId_Agent_SocketsMap.ContainsKey(messageBody.To))
                {
                    IDictionary<string, IWebSocketConnection> agents = appId_Agent_SocketsMap[messageBody.To];
                    foreach (var agent in agents)
                    {
                        // 说明肯定在线
                        LOG.Info($"客户端[{messageBody.To}]在线，设备类型[{agent.Key}]，发送在线消息");
                        IWebSocketConnection desSocket = agent.Value;

                        MessageBody desSendMsgBody = new MessageBody
                        {
                            From = fromClientId.AppId,
                            To = messageBody.To,
                            ChannelType = messageBody.ChannelType,
                            Payload = messageBody.Payload
                        };
                        LOG.Info(DateTime.Now.ToString() + $"|服务器：【发出】消息，来源：{fromClientId.AppId}，目的：{messageBody.To}\n，内容：{messageBody.Payload}");
                        desSocket.Send(JsonConvert.SerializeObject(desSendMsgBody));
                    }
                }
                else
                {
                    LOG.Info($"客户端[{messageBody.To}]离线，发送离线消息");
                    // TODO 保存离线消息
                }
                return true;
            });
        }
        /// <summary>
        /// 对外开放发送消息
        /// </summary>
        /// <param name="appId">客户端标识符id，如果为空，则认为是广播模式，发送给所有客户端；否则单点发送</param>
        /// <param name="message">消息内容</param>
        public static void SendMsgAsync(string appId, string message)
        {
            if (string.IsNullOrWhiteSpace(appId))
            {
                Broadcast(message);
                return;
            }
            else
            {
                // 发给指定的客户端
                if (appId_Agent_SocketsMap.ContainsKey(appId))
                {

                }
                else
                {
                    LOG.Info($"客户端[{appId}]离线，发送离线消息");
                    // TODO 保存离线消息
                }
            }
        }
        /// <summary>
        /// 关闭与客户端的所有的连接
        /// </summary>
        public void CloseAll()
        {
            foreach (var agent in appId_Agent_SocketsMap.Values)
            {
                foreach (var socket in agent.Values)
                {
                    socket.Close();
                }
            }
        }
        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="message">消息体</param>
        public static Task Broadcast(string message)
        {
            LOG.Info($"广播消息：{message}");
            MessageBody messageBody = JsonConvert.DeserializeObject<MessageBody>(message);
            return Task.Run(() => {
                foreach (var agent in appId_Agent_SocketsMap)
                {
                    foreach (var socket in agent.Value.Values)
                    {
                        try
                        {
                            if (socket.IsAvailable)
                            {
                                socket.Send(message);
                                // 设置为已读
                                try
                                {
                                    // 从mongo中获取
                                    var filter = Builders<MessageColl>.Filter.And(
                                        Builders<MessageColl>.Filter.Eq("SysCode", messageBody.SysCode));
                                    IList<MessageColl> colls = MongodbHelper<MessageColl>.FindList(ServiceFactory.MongoDatabase, filter);
                                    if (colls?.Count > 0)
                                    {
                                        colls[0].Read = (int)EnumReadType.READ;
                                        colls[0].ReadTime = DateUtil.CurrentTimeMillis;
                                        MongodbHelper<MessageColl>.Update(ServiceFactory.MongoDatabase, colls[0], colls[0]._id);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LOG.Error($"更新消息已读状态出错，详情：{ex.StackTrace + "\r\n" + ex.Message}");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LOG.Error($"发送到[{agent.Key}]的消息失败，详情：{ex.StackTrace + "\r\n" + ex.Message}");
                        }
                    }
                }
            });
        }
        /// <summary>
        /// 获取所有客户端信息
        /// </summary>
        /// <returns></returns>
        public static IList<string> ListClients()
        {
            List<string> clients = new List<string>();
            foreach (var item in appId_Agent_SocketsMap.Keys)
            {
                clients.Add(item);
            }
            return clients;
        }
    }
}
