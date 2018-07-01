/* ----------------------------------------------------------
 * 文件名称：KafkaStreamsConsumerWindow.xaml.cs
 * 作者：秦建辉
 * 
 * 微信：splashcn
 * 
 * 博客：http://www.firstsolver.com/wordpress/
 * 
 * 开发环境：
 *      Visual Studio V2017
 *      .NET Framework 4.5.2
 *      librdkafka.redist 0.11.1
 *      Confluent.Kafka 0.11.1
 *      
 * 版本历史：
 *      V1.0    2017年11月14日
 *              KafkaStreams 流式计算结果演示
 *              
 * 控制台命令
 *      启动 ZooKeeper 服务器
 *          bin\windows\zookeeper-server-start.bat config\zookeeper.properties
 *      启动 Kafka 服务器
 *          bin\windows\kafka-server-start.bat config\server.properties
 *      发送消息
 *          bin\windows\kafka-console-producer.bat --broker-list localhost:9092 --topic test
 *      开启消费者
 *          bin\windows\kafka-console-consumer.bat --bootstrap-server localhost:9092 --topic test --from-beginning
------------------------------------------------------------ */
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace Splash
{
    public partial class KafkaStreamsConsumerWindow : Window
    {
        private bool IsListening = false;
        private volatile bool ShouldStop = false;
        private volatile ObservableCollection<TWordInformation> WordStatistics = new ObservableCollection<TWordInformation>();

        public KafkaStreamsConsumerWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataGridWordStatistics.ItemsSource = WordStatistics;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsListening)
            {
                ShouldStop = true;
            }
        }

        private void ButtonListen_Click(object sender, RoutedEventArgs e)
        {
            buttonListen.IsEnabled = false;
            if (IsListening)
            {
                ShouldStop = true;
            }
            else
            {
                ShouldStop = false;

                string Broker = textBoxBroker.Text.Trim();
                if (string.IsNullOrEmpty(Broker))
                {
                    MessageBox.Show("消息代理不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string Topic = textBoxTopic.Text.Trim();
                if (string.IsNullOrEmpty(Topic))
                {
                    MessageBox.Show("主题不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string GroupId = textBoxGroupId.Text.Trim();
                if (string.IsNullOrEmpty(GroupId))
                {
                    MessageBox.Show("分组ID不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ThreadPool.QueueUserWorkItem(StartListenServer, new TConsumerSetting() { Broker = Broker, Topic = Topic, GroupId = GroupId });
            }
        }

        private void StartListenServer(object state)
        {
            TConsumerSetting Setting = state as TConsumerSetting;
            try
            {
                var config = new Dictionary<string, object>
                {
                    { "group.id", Setting.GroupId },
                    { "bootstrap.servers", Setting.Broker },
                    { "auto.offset.reset", "earliest" }
                };

                using (var consumer = new Consumer<string, long>(config, new StringDeserializer(Encoding.UTF8), new LongDeserializer()))
                {
                    consumer.Subscribe(Setting.Topic);

                    SetStatus(true);
                    while (true)
                    {
                        if (ShouldStop) break;
                        if (consumer.Consume(out Message<string, long> msg, TimeSpan.FromSeconds(1)))
                        {
                            string key = msg.Key;
                            long value = msg.Value;
                            this.Dispatcher.Invoke(() => {
                                try
                                {
                                    TWordInformation Source = Enumerable.First<TWordInformation>(WordStatistics, s => key.Equals(s.Word));
                                    WordStatistics.Remove(Source);
                                    WordStatistics.Add(new TWordInformation(key, value));
                                }
                                catch (InvalidOperationException)
                                {   // 新单词
                                    WordStatistics.Add(new TWordInformation(key, value));
                                }
                            });
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                string ErrorMessage = exception.Message;
                if (string.IsNullOrEmpty(ErrorMessage)) ErrorMessage = exception.InnerException.Message;
                MessageBox.Show(ErrorMessage, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SetStatus(false);
            }
        }

        public void SetStatus(bool isListening)
        {
            this.Dispatcher.Invoke(new Action<bool>((status) =>
            {
                if (status)
                {
                    this.buttonListen.Content = "停止侦听";
                }
                else
                {
                    this.buttonListen.Content = "开启侦听";
                }
                this.IsListening = status;
                this.buttonListen.IsEnabled = true;
            }), isListening);
        }
    }

    public class TConsumerSetting
    {
        public string Broker;
        public string Topic;
        public string GroupId;
    }

    public class TWordInformation
    {
        /// <summary>
        /// 单词
        /// </summary>
        public string Word { get; set; }

        /// <summary>
        /// 次数
        /// </summary>
        public long Count { get; set; }

        // 构造函数
        public TWordInformation(string word, long count)
        {
            Word = word;
            Count = count;
        }
    }
}
