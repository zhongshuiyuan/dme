using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Dist.Dme.HSMessage.Conf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dist.Dme.HSMessage.Kafka
{
    public class ConsumerClient
    {
        private static bool _isListening = false;
        private static Consumer<Null, string> _consumer;
        static ConsumerClient()
        {
            var conf = new Dictionary<string, object>
            {
              { "group.id", ConsumerSetting.GroupId },
              { "bootstrap.servers", ConsumerSetting.Servers },
              { "auto.commit.interval.ms", ConsumerSetting.AutoCommitIntervalMs },
              { "auto.offset.reset", ConsumerSetting.AutoOffsetReset }
            };
            _consumer = new Consumer<Null, string>(conf, null, new StringDeserializer(Encoding.UTF8));
        }
        /// <summary>
        /// 启动订阅消费
        /// </summary>
        /// <param name="topic">主题</param>
        public static async void Start(string topic)
        {
            _isListening = true;
            await Task.Run(()=> 
            {
                while (_isListening)
                {
                    _consumer.OnMessage += (_, msg)
                  => Console.WriteLine($"Read '{msg.Value}' from: {msg.TopicPartitionOffset}");

                    _consumer.OnError += (_, error)
                      => Console.WriteLine($"Error: {error}");

                    _consumer.OnConsumeError += (_, msg)
                      => Console.WriteLine($"Consume error ({msg.TopicPartitionOffset}): {msg.Error}");

                    _consumer.Subscribe(topic);

                    while (true)
                    {
                        _consumer.Poll(TimeSpan.FromMilliseconds(100));
                    }
                }
            }); 
        }
        /// <summary>
        /// 停止消费
        /// </summary>
        public static void Stop()
        {
            _isListening = false;
        }
        public static void Main()
        {
            var conf = new Dictionary<string, object>
            {
              { "group.id", "test-consumer-group" },
              { "bootstrap.servers", "localhost:9092" },
              { "auto.commit.interval.ms", 5000 },
              { "auto.offset.reset", "earliest" }
            };

            using (var consumer = new Consumer<Null, string>(conf, null, new StringDeserializer(Encoding.UTF8)))
            {
                consumer.OnMessage += (_, msg)
                  => Console.WriteLine($"Read '{msg.Value}' from: {msg.TopicPartitionOffset}");

                consumer.OnError += (_, error)
                  => Console.WriteLine($"Error: {error}");

                consumer.OnConsumeError += (_, msg)
                  => Console.WriteLine($"Consume error ({msg.TopicPartitionOffset}): {msg.Error}");

                consumer.Subscribe("my-topic");

                while (true)
                {
                    consumer.Poll(TimeSpan.FromMilliseconds(100));
                }
            }
        }
    }
}
