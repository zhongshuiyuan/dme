using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Dist.Dme.Base.Utils;
using Dist.Dme.DisFS.Adapters.Mongo;
using Dist.Dme.DisFS.Collection;
using Dist.Dme.Extensions;
using Dist.Dme.HSMessage.Define;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dist.Dme.HSMessage.MQ.Kafka
{
    /// <summary>
    /// 生产者
    /// </summary>
    public class KafkaProducer
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        private static Producer<Null, string> producer;

        //static ProducerClinet()
        //{
        //    var config = new Dictionary<string, object>
        //    {
        //        { "bootstrap.servers", ProducerSetting.Servers }
        //    };
        //    _producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8));
        //}
        public static Producer<Null, string> CreateProducer(string servers)
        {
            var config = new Dictionary<string, object>
            {
                { "bootstrap.servers", servers }
            };
            producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8));
            return producer;
        }
        /// <summary>
        /// 通用的消息发送
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Task<Boolean> Send(string topic, string message)
        {
            return Task.Run(() => {
               
                var dr = producer.ProduceAsync(topic, null, message);
                if (dr.Result.Error.HasError)
                {
                    LOG.Error($" 发送失败，详情[{dr.Result.Error.Reason}]");
                    return false;
                }
                LOG.Info($"成功发送 '{dr.Result.Value}' to: {dr.Result.TopicPartitionOffset}");
                return true;
            });
        }
            /// <summary>
            /// 发送消息
            /// </summary>
            /// <param name="topic">主题</param>
            /// <param name="message"></param>
            /// <returns>是否发送成功，true/false</returns>
            public static Task<Boolean> Send(string topic, MessageBody message)
           {
             return Task.Run(() => {
                 if (string.IsNullOrWhiteSpace(message.SysCode))
                 {
                     message.SysCode = GuidUtil.NewGuid();
                 }
                 // 发送之前先持久化
                 MessageColl messageCollection = new MessageColl
                 {
                     SysCode = message.SysCode,
                     From = message.From,
                     To = message.To,
                     ChannelType = (int)message.ChannelType,
                     MsgType = (int)message.MessageType,
                     Payload = message.Payload,
                     SendTime = DateUtil.CurrentTimeMillis,
                     Delivered = (int)EnumDeliverType.UNDELIVERED,
                     Read = (int)EnumReadType.UNREAD
                 };

                 MongodbHelper<MessageColl>.Add(ServiceFactory.MongoDatabase, messageCollection);

                 var dr = producer.ProduceAsync(topic, null, JsonConvert.SerializeObject(message));
                if (dr.Result.Error.HasError)
                {
                    LOG.Error($" 发送失败，详情[{dr.Result.Error.Reason}]");
                    return false;
                }
                 messageCollection.Delivered = (int)EnumDeliverType.DELIVERED;
                 MongodbHelper<MessageColl>.Update(ServiceFactory.MongoDatabase, messageCollection, messageCollection._id);
                LOG.Info($"成功发送 '{dr.Result.Value}' to: {dr.Result.TopicPartitionOffset}");
                return true;
            });
        }
        /// <summary>
        /// 测试demo
        /// </summary>
        public static void Main()
        {
            var config = new Dictionary<string, object>
            {
                { "bootstrap.servers", "localhost:9092" }
            };

            using (var producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8)))
            {
                var dr = producer.ProduceAsync("my-topic", null, "test message text").Result;
                // 已送达
                Console.WriteLine($"Delivered '{dr.Value}' to: {dr.TopicPartitionOffset}");
            }
        }
    }
}
