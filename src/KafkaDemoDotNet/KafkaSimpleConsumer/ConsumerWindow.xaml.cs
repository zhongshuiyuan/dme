/* ----------------------------------------------------------
 * 文件名称：ConsumerWindow.xaml.cs
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
 *      V1.0    2017年10月23日
 *              Kafka Consumer 示例
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
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Splash
{
    public partial class ConsumerWindow : Window
    {
        private bool IsListening = false;
        private volatile bool ShouldStop = false;

        public ConsumerWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsListening)
            {
                ShouldStop = true;
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            richTextBoxLog.Document.Blocks.Clear();
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

                ThreadPool.QueueUserWorkItem(StartListenServer, new ConsumerSetting() { Broker = Broker, Topic = Topic, GroupId = GroupId });
            }
        }

        private void StartListenServer(object state)
        {
            ConsumerSetting Setting = state as ConsumerSetting;
            try
            {
                var config = new Dictionary<string, object>
                {
                    { "group.id", Setting.GroupId },
                    { "bootstrap.servers", Setting.Broker }
                };

                using (var consumer = new Consumer<Null, string>(config, null, new StringDeserializer(Encoding.UTF8)))
                {
                    consumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(Setting.Topic, 0, 0) });

                    SetStatus(true);
                    while (true)
                    {
                        if (ShouldStop) break;
                        if (consumer.Consume(out Message<Null, string> msg, TimeSpan.FromSeconds(1)))
                        {
                            AppendTextToLog($"Topic: {msg.Topic}\r\nPartition: {msg.Partition}\r\nOffset: {msg.Offset}\r\nMessage: {msg.Value}");
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

        private void AppendTextToLog(string content)
        {
            this.Dispatcher.BeginInvoke(new Action<string>((message) => {
                Run run = new Run(message)
                {
                    Background = new SolidColorBrush(Colors.LightGray)
                };
                richTextBoxLog.Document.Blocks.Add(new Paragraph(run));
            }), content);
        }
    }

    public class ConsumerSetting
    {
        public string Broker;
        public string Topic;
        public string GroupId;
    }
}
