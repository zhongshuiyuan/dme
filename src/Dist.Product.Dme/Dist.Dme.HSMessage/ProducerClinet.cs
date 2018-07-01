using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Dist.Dme.HSMessage.Conf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.HSMessage
{
    public class ProducerClinet
    {
        private static Producer<Null, string> _producer;

        static ProducerClinet()
        {
            var config = new Dictionary<string, object>
            {
                { "bootstrap.servers", ProducerSetting.Servers }
            };
            _producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8));
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        /// <returns>是否发送成功，true/false</returns>
        public static Boolean Send(string topic, string message)
        {
            if (string.IsNullOrEmpty(topic))
            {
                topic = ProducerSetting.Topic;
            }
            var dr = _producer.ProduceAsync(topic, null, message).Result;
            Console.WriteLine($"Delivered '{dr.Value}' to: {dr.TopicPartitionOffset}");
            return true;
        }
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
