using System;
using System.Collections.Generic;
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
        /// 开启监听
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/start")]
        public Result StartListen([FromQuery]string topic)
        {
            if (string.IsNullOrEmpty(topic) && ServiceFactory.ConsumerClient != null) 
            {
                ServiceFactory.ConsumerClient.Start();
                return base.Success($"成功启动监控，主题[{ServiceFactory.MessageSetting.Opinion.Topics + (string.IsNullOrEmpty(topic) ? "" : "," + topic)}]......");
            }
            ServiceFactory.ConsumerClient = null;
            ServiceFactory.ConsumerClient = new ConsumerClient(
                ServiceFactory.MessageSetting.Opinion.GroupId, 
                ServiceFactory.MessageSetting.Opinion.Servers, 
                ServiceFactory.MessageSetting.Opinion.Topics + "," + topic);
            ServiceFactory.ConsumerClient.Start();
            // ConsumerClient.Start(topic);
            return base.Success($"成功启动监控，主题[{ServiceFactory.MessageSetting.Opinion.Topics + "," + topic}]......");
        }
        /// <summary>
        /// 停止监听
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/stop")]
        public Result StopListen()
        {
            ServiceFactory.ConsumerClient.Stop();
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
