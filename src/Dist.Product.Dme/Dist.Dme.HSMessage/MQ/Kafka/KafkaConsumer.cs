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
    /// 消费者
    /// </summary>
    public class KafkaConsumer
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        // private static IDictionary<string, bool> _topicListenMap = new Dictionary<string, bool>();
        // private static Consumer<Null, string> _consumer;
        private Consumer<Null, string> consumer;
        private string[] topics;
        private Boolean isListening = false;

        /// <summary>
        /// 创建主题
        /// </summary>
        /// <param name="topic"></param>
        public void CreateTopic(string topic)
        {
            List<string> topics = this.consumer.Subscription;
            topics.Add(topic);
            this.consumer.Unsubscribe();
            this.consumer.Subscribe(topics);
        }

        //static ConsumerClient()
        //{
        //    var conf = new Dictionary<string, object>
        //    {
        //      { "group.id", ConsumerSetting.GroupId },
        //      { "bootstrap.servers", ConsumerSetting.Servers },
        //      { "auto.commit.interval.ms", ConsumerSetting.AutoCommitIntervalMs },
        //      { "auto.offset.reset", ConsumerSetting.AutoOffsetReset }
        //    };
        //    _consumer = new Consumer<Null, string>(conf, null, new StringDeserializer(Encoding.UTF8));
        //}
        public KafkaConsumer(string groupId, string servers, string topics, int autoCommitIntervalMs = 3000, string autoOffsetReset = "earliest")
        {
            var conf = new Dictionary<string, object>
            {
              { "group.id", groupId },
              { "bootstrap.servers", servers },
              { "auto.commit.interval.ms", autoCommitIntervalMs },
              { "auto.offset.reset", autoOffsetReset }
            };
            this.consumer = new Consumer<Null, string>(conf, null, new StringDeserializer(Encoding.UTF8));
            this.topics = topics.Split(",");
            this.Listen(consumer, this.topics);
        }
        /// <summary>
        /// 开启订阅和接收消息
        /// </summary>
        /// <param name="consumer"></param>
        /// <param name="topics"></param>
        private void Listen(Consumer<Null, string> consumer, string[] topics)
        {
            consumer.OnMessage += (_, msg)
                    =>
            {
                LOG.Info($"Read '{msg.Value}' from: {msg.TopicPartitionOffset}");
            };
            consumer.OnError += (_, error)
              =>
            {
                LOG.Error($"Error: {error}");
            };

            consumer.OnConsumeError += (_, msg)
              =>
            {
                LOG.Error($"Consume error ({msg.TopicPartitionOffset}): {msg.Error}");
            };

            consumer.Subscribe(topics);
        }
        /// <summary>
        /// 开始
        /// </summary>
        public async void Start()
        {
            isListening = true;
            await Task.Run(() =>
            {
                while (isListening)
                {
                    consumer.Poll(TimeSpan.FromMilliseconds(100));
                }
            });
        }
        /// <summary>
        /// 停止所有订阅
        /// </summary>
        public void Unsubscribe()
        {
            isListening = false;
            LOG.Info($"成功取消订阅所有主题");
        }
        /// <summary>
        /// 停止订阅指定专题
        /// </summary>
        /// <param name="topic"></param>
        public void Unsubscribe(string topic)
        {
            List<string> topics = this.consumer.Subscription;
            List<string> newTopics = new List<string>();
            foreach (var item in topics)
            {
                if(!item.Equals(topic))
                {
                    newTopics.Add(item);
                }
            }
            this.consumer.Unsubscribe();
            this.consumer.Subscribe(newTopics);
            LOG.Info($"成功取消订阅主题[{topic}]，目前订阅主题[{string.Concat(newTopics)}]");
        }
        /// <summary>
        /// 启动订阅消费
        /// </summary>
        /// <param name="topic">主题</param>
        //public static async void Start(string topic)
        //{
        //    await Task.Run(()=> 
        //    {
        //        _topicListenMap[topic] = true;
        //        while (_topicListenMap[topic])
        //        {
        //            _consumer.OnMessage += (_, msg)
        //          => {
        //                  Console.WriteLine($"Read '{msg.Value}' from: {msg.TopicPartitionOffset}");
        //              };

        //            _consumer.OnError += (_, error)
        //              =>
        //            {
        //                Console.WriteLine($"Error: {error}");
        //            };

        //            _consumer.OnConsumeError += (_, msg)
        //              =>
        //            {
        //                Console.WriteLine($"Consume error ({msg.TopicPartitionOffset}): {msg.Error}");
        //            };

        //            _consumer.Subscribe(_topicListenMap.Keys);

        //            while (_topicListenMap[topic])
        //            {
        //                _consumer.Poll(TimeSpan.FromMilliseconds(100));
        //            }
        //        }
        //    }); 
        //}
        /// <summary>
        /// 停止消费
        /// </summary>
        //public static void Stop(string topic)
        //{
        //    _topicListenMap.Remove(topic);
        //    // 重新订阅
        //    _consumer.Unsubscribe();
        //    _consumer.Subscribe(_topicListenMap.Keys);
        //}
        /// <summary>
        /// 测试demo
        /// </summary>
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
