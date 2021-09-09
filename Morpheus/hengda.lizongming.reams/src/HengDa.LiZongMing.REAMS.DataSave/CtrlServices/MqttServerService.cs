using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet.Client.Receiving;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace MQTTnet.TestApp.NetCore
{
    public  class MqttServerService
    {
        public static IMqttServer mqttServer=null;
        //public static void RunEmptyServer()
        //{
        //    var mqttServer = new MqttFactory().CreateMqttServer();
        //    mqttServer.StartAsync(new MqttServerOptions()).GetAwaiter().GetResult();

        //    Console.WriteLine("Press any key to exit.");
        //    Console.ReadLine();
        //}

        public  async Task RunAsync()
        {
            try
            {
                if (mqttServer != null) {
                    if (mqttServer.IsStarted)
                    {
                      await  mqttServer.StopAsync();
                       await Task.Delay(1000);
                    }

                }
                var options = new MqttServerOptions
                {
                    ConnectionValidator = new MqttServerConnectionValidatorDelegate(p =>
                    {
                        //只有客户Client 要来验证(USER   PASS ),其它客户端不验证直接通过
                        if (p.ClientId == "Client")
                        {
                            if (p.Username != "USER" || p.Password != "PASS")
                            {
                                p.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                            }
                        }
                    }),

                    //数据在服务端怎么保存
                    //Storage = new RetainedMessageHandler(),

                    //几个拦截器功能演示
                    ApplicationMessageInterceptor = new MqttServerApplicationMessageInterceptorDelegate(context =>
                    {
                        if (MqttTopicFilterComparer.IsMatch(context.ApplicationMessage.Topic, "/myTopic/WithTimestamp/#"))
                        {
                            // Replace the payload with the timestamp. But also extending a JSON 
                            // based payload with the timestamp is a suitable use case.
                            context.ApplicationMessage.Payload = Encoding.UTF8.GetBytes(DateTime.Now.ToString("O"));
                        }

                        if (context.ApplicationMessage.Topic == "not_allowed_topic")
                        {
                            context.AcceptPublish = false;
                            context.CloseConnection = true;
                        }
                    }),
                    //几个订阅拦截器功能演示
                    SubscriptionInterceptor = new MqttServerSubscriptionInterceptorDelegate(context =>
                    {
                        if (context.TopicFilter.Topic.StartsWith("admin/foo/bar") && context.ClientId != "theAdmin")
                        {
                            context.AcceptSubscription = false;
                        }

                        if (context.TopicFilter.Topic.StartsWith("the/secret/stuff") && context.ClientId != "Imperator")
                        {
                            context.AcceptSubscription = false;
                            context.CloseConnection = true;
                        }
                    })
                };

                // Extend the timestamp for all messages from clients.
                // Protect several topics from being subscribed from every client.

                //var certificate = new X509Certificate(@"C:\certs\test\test.cer", "");
                //options.TlsEndpointOptions.Certificate = certificate.Export(X509ContentType.Cert);
                //options.ConnectionBacklog = 5;
                //options.DefaultEndpointOptions.IsEnabled = true;
                //options.TlsEndpointOptions.IsEnabled = false;

                 mqttServer = new MqttFactory().CreateMqttServer();

                mqttServer.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(e =>
                {
                    //MqttNetConsoleLogger.PrintToConsole(
                    //    $"'{e.ClientId}' reported '{e.ApplicationMessage.Topic}' > '{Encoding.UTF8.GetString(e.ApplicationMessage.Payload ?? new byte[0])}'",
                    //    ConsoleColor.Magenta);
                });


                mqttServer.ClientConnectedHandler = new MqttServerClientConnectedHandlerDelegate(e =>
                {
                    Console.Write("Client disconnected event fired.");
                });

                await mqttServer.StartAsync(options);

                //Console.WriteLine("Press any key to exit.");
                //Console.ReadLine();

                //await mqttServer.StopAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            //Console.ReadLine();
        }
    }
}
