/* ----------------------------------------------------------
 * 文件名称：ProducerWindow.xaml.cs
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
 *              Kafka Producer 示例
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
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Splash
{
    public partial class ProducerWindow : Window
    {
        public ProducerWindow()
        {
            InitializeComponent();

            string Message = "《卜算子·咏梅》\r\n风雨送春归，\r\n飞雪迎春到。\r\n已是悬崖百丈冰，\r\n犹有花枝俏。\r\n\r\n俏也不争春，\r\n只把春来报。\r\n待到山花烂漫时，\r\n她在丛中笑。";
            textBoxMessage.Text = Message;
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            richTextBoxLog.Document.Blocks.Clear();
        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            buttonSubmit.IsEnabled = false;
            try
            {
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

                string Message = textBoxMessage.Text;
                if (string.IsNullOrEmpty(Message))
                {
                    MessageBox.Show("消息内容不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var config = new Dictionary<string, object> { { "bootstrap.servers", Broker } };
                using (var producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8)))
                {
                    var deliveryReport = producer.ProduceAsync(Topic, null, Message);
                    deliveryReport.ContinueWith(task =>
                    {
                        AppendTextToLog($"Producer: {producer.Name}\r\nTopic: {Topic}\r\nPartition: {task.Result.Partition}\r\nOffset: {task.Result.Offset}");
                    });

                    producer.Flush(TimeSpan.FromSeconds(10));
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
                buttonSubmit.IsEnabled = true;
            }
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
}
