using Confluent.Kafka;
using Confluent.Kafka.Serialization;
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
        private Producer<Null, string> producer;

        //static ProducerClinet()
        //{
        //    var config = new Dictionary<string, object>
        //    {
        //        { "bootstrap.servers", ProducerSetting.Servers }
        //    };
        //    _producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8));
        //}
        public KafkaProducer(string servers)
        {
            var config = new Dictionary<string, object>
            {
                { "bootstrap.servers", servers }
            };
            this.producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8));
        }
            /// <summary>
            /// 发送消息
            /// </summary>
            /// <param name="topic">主题</param>
            /// <param name="message"></param>
            /// <returns>是否发送成功，true/false</returns>
            public Task<Boolean> Send(string topic, string message)
           {
             return Task.Run(() => {
                var dr = this.producer.ProduceAsync(topic, null, message);
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
