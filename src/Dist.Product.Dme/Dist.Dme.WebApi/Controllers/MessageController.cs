using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Dist.Dme.Base.Framework;
using Dist.Dme.Extensions;
using Dist.Dme.HSMessage.Define;
using Dist.Dme.HSMessage.MQ.Kafka;
using Dist.Dme.HSMessage.Websocket.Fleck;
using Dist.Dme.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;

namespace Dist.Dme.WebApi.Controllers
{
    [Route("api/messages")]
    public class MessageController : BaseController
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 创建topic
        /// </summary>
        /// <param name="topic">主题标识符</param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/topic")]
        public Result CreateTopic([Required][FromQuery]string topic)
        {
            //if (string.IsNullOrEmpty(topic) && ServiceFactory.KafkaConsumer != null) 
            //{
            //    ServiceFactory.KafkaConsumer.Start();
            //    return base.Success($"成功创建channel[{ServiceFactory.HSMessageSetting.Opinion.Topics + (string.IsNullOrEmpty(topic) ? "" : "," + topic)}]......");
            //}
            KafkaConsumer.CreateTopic(topic);
            // ConsumerClient.Start(topic);
            return base.Success($"成功创建channel[{topic}]......");
        }
        /// <summary>
        /// 停止订阅所有
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/unsubscribe")]
        public Result Unsubscribe()
        {
            KafkaConsumer.Unsubscribe();
            return base.Success($"成功停止监控......");
        }
        /// <summary>
        /// 停止订阅某个主题
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/unsubscribe/topic")]
        public Result UnsubscribeTopic([Required][FromQuery]string topic)
        {
            KafkaConsumer.Unsubscribe(topic);
            return base.Success($"成功停止订阅主题[{topic}]......");
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="topic">主题</param>
        /// <param name="msg">消息体</param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/send/{topic}")]
        public async Task<Result> SendMsgAsync(string topic ,[FromBody] MessageBody msg)
        {
            bool result = await KafkaProducer.Send(topic, msg);
            if (result)
            {
               return Success($"成功往主题[{topic}]发送消息[{msg}]......");
            }
            return base.Fail($"往主题[{topic}]发送消息失败，具体原因请管理员查看日志......");
        }
        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/broadcast")]
        public Result Broadcast(string msg)
        {
            Task.Run(() => {
                WebsocketFleckServer.Broadcast(msg);
            });
            return base.Success($"已广播消息：{msg}");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/clients")]
        public Result ListConnectClients()
        {
            return base.Success(WebsocketFleckServer.ListClients());
        }
    }
}
