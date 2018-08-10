using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Dist.Dme.HSMessage.Define;
using Dist.Dme.HSMessage.Websocket.Fleck;
using Newtonsoft.Json;
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
        private static Consumer<Null, string> consumer;
        private static string[] topics;
        private static Boolean isListening = false;

        /// <summary>
        /// 创建主题
        /// </summary>
        /// <param name="topic"></param>
        public static void CreateTopic(string topic)
        {
            List<string> topics = consumer.Subscription;
            topics.Add(topic);
            consumer.Unsubscribe();
            consumer.Subscribe(topics);
        }

        public static Consumer<Null, string> CreateConsumer(string groupId, string servers, string topics, int autoCommitIntervalMs = 3000, string autoOffsetReset = "earliest")
        {
            var conf = new Dictionary<string, object>
            {
              { "group.id", groupId },
              { "bootstrap.servers", servers },
              { "auto.commit.interval.ms", autoCommitIntervalMs },
              { "auto.offset.reset", autoOffsetReset }
            };
            consumer = new Consumer<Null, string>(conf, null, new StringDeserializer(Encoding.UTF8));
            KafkaConsumer.topics = topics.Split(",");
            Listen(consumer, KafkaConsumer.topics);
            return consumer;
        }
        /// <summary>
        /// 开启订阅和接收消息
        /// </summary>
        /// <param name="consumer"></param>
        /// <param name="topics"></param>
        private static void Listen(Consumer<Null, string> consumer, string[] topics)
        {
            consumer.OnMessage += (_, msg)
                    =>
            {
                if (Object.Equals(nameof(EnumMessageType.SYSTEM), msg.Topic))
                {
                    MessageBody messageBody = JsonConvert.DeserializeObject<MessageBody>(msg.Value);
                    // 系统消息，广播
                    WebsocketFleckServer.Broadcast(msg.Value);
                }
                else if (Object.Equals(nameof(EnumMessageType.TASK), msg.Topic))
                {
                    // 任务消息，发给指定人
                    MessageBody messageBody = JsonConvert.DeserializeObject<MessageBody>(msg.Value);
                    // @TODO
                    WebsocketFleckServer.SendMsgAsync(messageBody.To, messageBody);
                }
                else if (Object.Equals(nameof(EnumMessageType.KICK), msg.Topic))
                {
                    KickMessageBody kickMessageBody = JsonConvert.DeserializeObject<KickMessageBody>(msg.Value);
                    WebsocketFleckServer.KickSocket(kickMessageBody.NodeKey, kickMessageBody.AppId, kickMessageBody.AgentType);
                }
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
        public static async void Start()
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
        public static void Unsubscribe()
        {
            isListening = false;
            LOG.Info($"成功取消订阅所有主题");
        }
        /// <summary>
        /// 停止订阅指定专题
        /// </summary>
        /// <param name="topic"></param>
        public static void Unsubscribe(string topic)
        {
            List<string> topics = consumer.Subscription;
            List<string> newTopics = new List<string>();
            foreach (var item in topics)
            {
                if(!item.Equals(topic))
                {
                    newTopics.Add(item);
                }
            }
            consumer.Unsubscribe();
            consumer.Subscribe(newTopics);
            LOG.Info($"成功取消订阅主题[{topic}]，目前订阅主题[{string.Concat(newTopics)}]");
        }
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
