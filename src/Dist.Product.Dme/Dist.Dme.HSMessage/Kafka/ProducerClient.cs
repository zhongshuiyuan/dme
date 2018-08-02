using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Dist.Dme.HSMessage.Kafka.Conf;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.HSMessage.Kafka
{
    public class ProducerClient
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        private static Producer<Null, string> _producer;
        private Producer<Null, string> producer;

        //static ProducerClinet()
        //{
        //    var config = new Dictionary<string, object>
        //    {
        //        { "bootstrap.servers", ProducerSetting.Servers }
        //    };
        //    _producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8));
        //}
        public ProducerClient(string servers)
        {
            var config = new Dictionary<string, object>
            {
                { "bootstrap.servers", ProducerSetting.Servers }
            };
            this.producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8));
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="topic">主题</param>
        /// <param name="message"></param>
        /// <returns>是否发送成功，true/false</returns>
        public Boolean Send(string topic, string message)
        {
            var dr = this.producer.ProduceAsync(topic, null, message).Result;
            LOG.Info($"Delivered '{dr.Value}' to: {dr.TopicPartitionOffset}");
            return true;
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
                Console.WriteLine($"Delivered '{dr.Value}' to: {dr.TopicPartitionOffset}");
            }
        }
    }
}
