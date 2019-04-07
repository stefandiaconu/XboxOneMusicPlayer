using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using MQTTnet;
using MQTTnet.Client;
using System.Text;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace XboxOneMusicPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            init();
        }

        private void init()
        {
            ConnectClient();

        }

        static MqttFactory factory = new MqttFactory();
        static IMqttClient mqttClient;

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            var message = new MqttApplicationMessageBuilder()
            .WithTopic("testtopic/milan")
            .WithPayload("Ok")
            .WithExactlyOnceQoS()
            .WithRetainFlag()
            .Build();

            await mqttClient.PublishAsync(message);
            Debug.WriteLine("Message published!");

            Debug.WriteLine("Press any key to exit.");
            Debug.WriteLine("");
        }

        public static string subscribe()
        {

            string mes = "";
            mqttClient.Connected += async (s, e) =>
            {
                Debug.WriteLine("### CONNECTED WITH SERVER ###");

                // Subscribe to a topic
                await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("testtopic/milan").Build());
                await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("testtopic/test").Build());


                mqttClient.ApplicationMessageReceived += (s1, e1) =>
                {
                    if (e1.ApplicationMessage.Topic.Equals("testtopic/milan"))
                    {
                        mes = Encoding.UTF8.GetString(e1.ApplicationMessage.Payload);
                        if (mes.Equals("Ok"))
                        {
                            displayMessageAsync("Title", "Content", "Dialog");
                        }

                    }

                    Debug.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                    Debug.WriteLine($"+ Topic = {e1.ApplicationMessage.Topic}");
                    Debug.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e1.ApplicationMessage.Payload)}");
                    mes = Encoding.UTF8.GetString(e1.ApplicationMessage.Payload);
                    Debug.WriteLine(mes);
                    Debug.WriteLine($"+ QoS = {e1.ApplicationMessage.QualityOfServiceLevel}");
                    Debug.WriteLine($"+ Retain = {e1.ApplicationMessage.Retain}");
                    Debug.WriteLine("");

                };

                Debug.WriteLine(s.ToString());

                Debug.WriteLine("### SUBSCRIBED ###");
            };

            return mes;
        }

        public static async void ConnectClient()
        {
            mqttClient = factory.CreateMqttClient();

            subscribe();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("broker.hivemq.com", 1883) // Port is optional
                .Build();
            await mqttClient.ConnectAsync(options);


        }

        public static void displayMessageAsync(String title, String content, String dialogType)
        {
            Debug.WriteLine("Received");
        }
    }
}
