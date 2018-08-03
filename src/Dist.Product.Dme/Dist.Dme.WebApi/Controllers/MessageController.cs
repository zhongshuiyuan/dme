using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Dist.Dme.Base.Framework;
using Dist.Dme.Extensions;
using Dist.Dme.HSMessage.Kafka;
using Dist.Dme.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Dist.Dme.WebApi.Controllers
{
    [Route("api/messages")]
    public class MessageController : BaseController
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 订阅主题
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/subscibe")]
        public Result Subscibe([FromQuery]string topic)
        {
            if (string.IsNullOrEmpty(topic) && ServiceFactory.ConsumerClient != null) 
            {
                ServiceFactory.ConsumerClient.Start();
                return base.Success($"成功启动监控，主题[{ServiceFactory.HSMessageSetting.Opinion.Topics + (string.IsNullOrEmpty(topic) ? "" : "," + topic)}]......");
            }
            ServiceFactory.ConsumerClient = null;
            ServiceFactory.ConsumerClient = new ConsumerClient(
                ServiceFactory.HSMessageSetting.Opinion.GroupId, 
                ServiceFactory.HSMessageSetting.Opinion.Servers, 
                ServiceFactory.HSMessageSetting.Opinion.Topics + "," + topic);
            ServiceFactory.ConsumerClient.Start();
            // ConsumerClient.Start(topic);
            return base.Success($"成功启动监控，主题[{ServiceFactory.HSMessageSetting.Opinion.Topics + (string.IsNullOrEmpty(topic) ? "" : "," + topic)}]......");
        }
        /// <summary>
        /// 停止订阅所有
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/unsubscribe")]
        public Result Unsubscribe()
        {
            ServiceFactory.ConsumerClient.Stop();
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
            ServiceFactory.ConsumerClient.Stop(topic);
            return base.Success($"成功停止监控......");
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="topic">主题</param>
        /// <param name="msg">消息体</param>
        /// <returns></returns>
        [HttpPost]
        [Route("send/v1/{topic}")]
        public Result SendMsg(string topic ,[FromQuery] string msg)
        {
            Boolean finished = ServiceFactory.ProducerClient.Send(topic, msg);
            if (finished)
            {
                return base.Success($"成功往主题[{topic}]发送消息......");
            }
            return base.Fail($"往主题[{topic}]发送消息失败，具体原因请管理员查看日志......");
        }
    }
}
