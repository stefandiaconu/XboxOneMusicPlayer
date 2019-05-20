﻿using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Media.Playback;
using Windows.Media.Core;

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
        MediaPlayer player;
        bool playing;
        static Button play;
        public MainPage()
        {
            this.InitializeComponent();
            player = new MediaPlayer();
            playing = false;
            play = new Button();
            init();
            
            //play.Click += PlayButton_Click;
        }

        private void init()
        {
            ConnectClient();
        }

        static MqttFactory factory = new MqttFactory();
        static IMqttClient mqttClient;

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets");
            Windows.Storage.StorageFile file = await folder.GetFileAsync("Subcarpati\\12 - Subcarpati - La cutari (cu Mara).mp3");

            player.AutoPlay = false;
            player.Source = MediaSource.CreateFromStorageFile(file);
            if (playing)
            {
                player.Source = null;
                playing = false;
            }
            else
            {
                player.Play();
                playing = true;
            }

            var message = new MqttApplicationMessageBuilder()
            .WithTopic("/milanfdl1899@gmail.com/milan")
            .WithPayload("Ok")
            .WithExactlyOnceQoS()
            .WithRetainFlag()
            .Build();

            await mqttClient.PublishAsync(message);
            Debug.WriteLine("Message published!");

            Debug.WriteLine("Press any key to exit.");
            Debug.WriteLine("");
        }

        public string subscribe()
        {

            string mes = "";
            mqttClient.Connected += async (s, e) =>
            {
                Debug.WriteLine("### CONNECTED WITH SERVER ###");

                // Subscribe to a topic
                await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("/milanfdl1899@gmail.com/milan").Build());
                await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("/milanfdl1899@gmail.com/test").Build());


                mqttClient.ApplicationMessageReceived += async (s1, e1) =>
                {
                    if (e1.ApplicationMessage.Topic.Equals("/milanfdl1899@gmail.com/milan"))
                    {
                        mes = Encoding.UTF8.GetString(e1.ApplicationMessage.Payload);
                        if (mes.Equals("OK"))
                        {
                            displayMessageAsync("Title", "Content", "Dialog");
                            Windows.Storage.StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets");
                            Windows.Storage.StorageFile file = await folder.GetFileAsync("Subcarpati\\12 - Subcarpati - La cutari (cu Mara).mp3");

                            player.AutoPlay = false;
                            player.Source = MediaSource.CreateFromStorageFile(file);
                            if (playing)
                            {
                                player.Source = null;
                                playing = false;
                            }
                            else
                            {
                                player.Play();
                                playing = true;
                            }
                            Debug.WriteLine("Playing!!!");
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

        public async void ConnectClient()
        {
            mqttClient = factory.CreateMqttClient();

            subscribe();

            var options = new MqttClientOptionsBuilder()
                //.WithTcpServer("broker.hivemq.com", 1883) // Port is optional
                .WithTcpServer("mqtt.dioty.co", 1883) // Port is optional
                .WithCredentials("milanfdl1899@gmail.com", "f8e50cb7")
                .Build();
            await mqttClient.ConnectAsync(options);


        }

        public static void displayMessageAsync(String title, String content, String dialogType)
        {
            Debug.WriteLine("Received");
        }
    }
}
