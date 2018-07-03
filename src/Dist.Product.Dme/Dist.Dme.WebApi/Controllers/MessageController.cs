using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dist.Dme.Base.Framework;
using Dist.Dme.HSMessage.Kafka;
using Dist.Dme.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Dist.Dme.WebApi.Controllers
{
    [Route("api/messages")]
    public class MessageController : BaseController
    {
       /// <summary>
       /// 开启监听
       /// </summary>
       /// <param name="topic"></param>
       /// <returns></returns>
        [HttpGet]
        [Route("listen/v1/start/{topic}")]
        public Result StartListen(string topic)
        {
            ConsumerClient.Start(topic);
            return base.Success($"成功启动监控，主题[{topic}]......");
        }
        /// <summary>
        /// 停止监听
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("listen/v1/stop/{topic}")]
        public Result StopListen(string topic)
        {
            ConsumerClient.Stop(topic);
            return base.Success($"成功停止监控，主题[{topic}]......");
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="topic">主题</param>
        /// <param name="msg">消息体</param>
        /// <returns></returns>
        [HttpPost]
        [Route("send/v1/{topic}")]
        public Result SendMsg(string topic ,[FromBody] string msg)
        {
            ProducerClinet.Send(topic, msg);
            return base.Success($"成功往主题[{topic}]发送消息......");
        }
    }
}
