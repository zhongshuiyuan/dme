using Dist.Dme.HSMessage.Define;
using Fleck;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
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
        //客户端url以及其对应的Socket对象字典
        private static IDictionary<string, IWebSocketConnection> client_Sockets = new Dictionary<string, IWebSocketConnection>();
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
                        string clientId = GetClientId(socket);

                        client_Sockets.Add(clientId, socket);
                        LOG.Info(DateTime.Now.ToString() + "|服务器：和客户端网页:" + clientId + " 建立WebSock连接！");
                    };
                    socket.OnClose = () =>  //连接关闭事件
                    {
                        // string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                        string clientId = GetClientId(socket);
                        //如果存在这个客户端,那么对这个socket进行移除
                        if (client_Sockets.ContainsKey(clientId))
                        {
                            //注:Fleck中有释放
                            //关闭对象连接 
                            //if (dic_Sockets[clientUrl] != null)
                            //{
                            //dic_Sockets[clientUrl].Close();
                            //}
                            client_Sockets.Remove(clientId);
                        }
                        LOG.Info(DateTime.Now.ToString() + "|服务器：和客户端:" + clientId + " 断开WebSock连接！");
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
        private static string GetClientId(IWebSocketConnection socket)
        {
            string clientId;
            if (socket.ConnectionInfo.Path.Equals("/"))
            {
                clientId = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
            }
            else
            {
                clientId = socket.ConnectionInfo.Path.Substring(socket.ConnectionInfo.Path.LastIndexOf("/") + 1);
            }

            return clientId;
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
                string fromClientId = GetClientId(socket);
                MessageBody messageBody = JsonConvert.DeserializeObject<MessageBody>(message);
                // string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                LOG.Info(DateTime.Now.ToString() + "|服务器：【收到】来客户端网页:" + fromClientId + "的信息：\n" + message);
                if (client_Sockets.ContainsKey(messageBody.To))
                {
                    LOG.Info($"客户端[{messageBody.To}]在线，发送在线消息");
                    IWebSocketConnection desSocket = client_Sockets[messageBody.To];
                    MessageBody desMsgBody = new MessageBody
                    {
                        From = fromClientId,
                        To = messageBody.To,
                        Type = messageBody.Type,
                        Payload = messageBody.Payload
                    };
                    LOG.Info(DateTime.Now.ToString() + $"|服务器：【发出】消息，来源：{fromClientId}，目的：{messageBody.To}\n，内容：{messageBody.Payload}");
                    desSocket.Send(JsonConvert.SerializeObject(desMsgBody));
                }
                else
                {
                    LOG.Info($"客户端[{messageBody.To}]离线，发送离线消息");
                }
                
                return true;
            });
        }

        /// <summary>
        /// 关闭与客户端的所有的连接
        /// </summary>
        public void CloseAll()
        {
            foreach (var item in client_Sockets.Values)
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
                foreach (var clientConn in client_Sockets.Values)
                {
                    if (clientConn.IsAvailable)
                    {
                        clientConn.Send(message);
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
            foreach (var item in client_Sockets.Keys)
            {
                clients.Add(item);
            }
            return clients;
        }
    }
}
