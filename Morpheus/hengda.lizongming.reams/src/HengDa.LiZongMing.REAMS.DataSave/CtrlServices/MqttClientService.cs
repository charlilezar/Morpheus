using Aming.Core;
using Aming.DTU;
using Aming.DTU.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Protocol;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Threading;

namespace Aming.DTU
{
    public class MqttClientService : ITransmissionService
    {
        /// <summary>
        /// 转发时专用的mqtt客户端
        /// </summary>
        public static MqttClientService ForwardMqttClientService = null;

        //private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public ILogger Logger { get; set; }

        public IMqttClient client = null;
        //public static ConcurrentDictionary<string, IMqttClient> MqttClientList = new ConcurrentDictionary<string, IMqttClient>();
        public static object locked = new object();

        public IDataEndPoint Config { get; set; } //MqttClientConfig
        public MqttClientConfig MqttClientConfig { get { return Config as MqttClientConfig; } }
        public bool IsConnected { get { return (client != null && client.IsConnected); } }

        public Func<byte[], List<byte[]>> FuncSplitPack = null;//拆包过程，MQTT很大概率会很多包黏在一起，所以调用该类时要传入次方法


        public ConcurrentBag<string> SubTopicList = new ConcurrentBag<string>();


        /// <summary>
        /// 是否需要检查包格式，在通讯分包和粘包时有用
        /// <return>一个包的有效长度</return>
        /// </summary>
        public Func<List<byte>, int> funcCheckPackFormat { get; set; } = null; //TODO:需要增加，暂时未完成

        /// <summary>
        /// 事件回调时，收到数据触发解析
        /// </summary>
        public event ReceivedUniMessageCallBack OnUniMessageReceived = null;

        /// <summary>
        /// 将本端数据转发到别的端口的处理程序
        /// </summary>
        public event ReceivedUniMessageCallBack OnDataForwardTo = null;

        /// <summary>
        /// 事件回调变异步回调
        /// </summary>
        TaskCompletionSource<IotResult<UniMessageBase>> resultCompletionSource = null;


        public MqttClientService(ILoggerFactory loggerFactory, MqttClientConfig config)
        {
            this.Logger = loggerFactory.CreateLogger<MqttClientService>();
            this.Config = config;

            if (MqttClientConfig.EnableForward)
            {
                if (MqttClientConfig.ForwardToTopic == MqttClientConfig.GetSubTopicUp())
                {
                    Logger.LogWarning("Mqtt的上传主题和转发主题相同，这可能会造成消息无限循环错误，请仔细核对。修改服务器或主题，或者关闭转发功能。");
                }
            }
        }

        public async Task<bool> TryOpenAsync()
        {
            if (this.IsConnected) return true;
            MqttClientOptions clientOptions = null;
            MqttClientConfig config = MqttClientConfig;
            try
            {
                lock (locked)
                {
                    //MqttNetConsoleLogger.ForwardToConsole();
                    //string key = $"{config.ServerIP}_{config.ServerPort}_{config.ClientId}_{config.UserName}_{config.Password}";
                    //if (MqttClientList.ContainsKey(key))
                    //{
                    //    client = MqttClientList[key];
                    //    //TODO: 这里还要添加更多的订阅主题等，稍后再加
                    //    return true;
                    //}
                    var factory = new MqttFactory();
                    client = factory.CreateMqttClient();
                    //if (MqttClientList.ContainsKey(key))
                    //{
                    //    MqttClientList[key] = client;
                    //}
                    //else
                    //    MqttClientList.TryAdd(key, client);
                }
                if (clientOptions == null)
                {
                    clientOptions = new MqttClientOptions
                    {
                        ChannelOptions = new MqttClientTcpOptions
                        {
                            Server = config.ServerIP,       //  "127.0.0.1",
                            Port = config.ServerPort         //1883
                        },
                        ClientId = config.ClientId + $"_{MqttClientConfig.r.Next(0000, 9999)}",
                        Credentials = new MqttClientCredentials()
                        {
                            Username = config.UserName,
                            Password = System.Text.Encoding.ASCII.GetBytes(config.Password)
                        },
                        CommunicationTimeout = new TimeSpan(0, 3, 0),



                    };
                }


                client.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(e =>
                {
                    string m = @$"### RECEIVED APPLICATION MESSAGE ###
Topic = {e.ApplicationMessage.Topic}
Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}
QoS = {e.ApplicationMessage.QualityOfServiceLevel}
Retain = {e.ApplicationMessage.Retain}
";
                    Logger.LogDebug(m);
                    //add test log
                    var utf8Str = System.Text.Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    var getMsg = StringHelper.HexStringToString(utf8Str, Encoding.UTF8);
                    Logger.LogWarning($"convertmsg tranlate{getMsg}");
                    try
                    {
                        //if (config.ClientId == e.ClientId)
                        //    return; //自己发的消息自己不处理
                        UniMessageBase msg;
                        //if (funcCheckPackFormat != null)
                        //{ //有分包粘包要考虑
                        //    ReceiveBytes.AddRange(data);
                        //    int fLen = funcCheckPackFormat(ReceiveBytes);
                        //    if (0 >= fLen)
                        //    {
                        //        msg = new UniMessageBase { Payload = data, Topic = $"/DTU/{Config.Port}/from" };
                        //        Logger.LogDebug($"# {msg.Topic} 收到 \r\n{msg.Payload.ByteToHexStr()}，但数据不全，等待下次接收。");
                        //        return; //长度不够或格式不符
                        //    }
                        //    else
                        //    {
                        //        //分包的数据已
                        //        data = ReceiveBytes.Take(fLen).ToArray();
                        //        if (ReceiveBytes.Count == fLen)
                        //            ReceiveBytes.Clear();
                        //        else
                        //            ReceiveBytes.RemoveRange(0, fLen);
                        //    }
                        //}
                        //此时为应该是远程客户端的通讯请求
                        byte[] data = ConvertDataFromMqtt(e.ApplicationMessage.Topic, e.ApplicationMessage.Payload);
                        List<byte[]> listSplited = new List<byte[]>();
                        if (this.FuncSplitPack == null)
                            listSplited.Add(data);
                        else
                            listSplited = this.FuncSplitPack(data);//这里有可能存在多个包
                        foreach (byte[] bsSplitd in listSplited)
                        {
                            #region 处理收到的数据
                            msg = new UniMessageBase { Payload = bsSplitd, Topic = e.ApplicationMessage.Topic, ClientId = e.ClientId };
                            Logger.LogDebug($"# {msg.Topic} 收到 \r\n{msg.Payload.ByteToHexStr()}");//接收完整后再发送一次全部收到的数据，前面粘包处理时发送每次收到的

                            if (OnDataForwardTo != null)
                            {
                                //单独线程中去转发，不阻塞
                                var t1 = Task.Run(() =>
                               {
                                   msg = new UniMessageBase { Payload = bsSplitd, Topic = e.ApplicationMessage.Topic, ClientId = e.ClientId };
                                   OnDataForwardTo(config, msg);
                               });
                            }
                            //单独线程中去处理，不阻塞
                            var t = Task.Run(() =>
                              {
                                  if (resultCompletionSource != null && ((int)resultCompletionSource.Task.Status) < 4)
                                  {
                                      var rs = new IotResult<UniMessageBase>(true, "收到回应") { Result = msg };
                                      resultCompletionSource.SetResult(rs);
                                  }
                                  else if (OnUniMessageReceived != null) //传统的事件触发传出去
                                   {
                                       //var msg1 = new UniMessageBase { Payload = data, Topic = $"/DTU/{Config.Port}/from" };
                                       OnUniMessageReceived(config, msg);
                                  }
                              });
                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "处理收到的MQTT消息时失败！");
                    }
                });

                client.ConnectedHandler = new MqttClientConnectedHandlerDelegate(async e =>
                {
                    Logger.LogDebug("### CONNECTED WITH SERVER ###");

                    //var topic = MqttClientConfig.GetSubTopicUp();
                    await SubscribeAsync();

                });

                client.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(async e =>
                {

                    Logger.LogDebug("### DISCONNECTED FROM SERVER ###" + e?.Exception?.Message);
                    await Task.Delay(TimeSpan.FromSeconds(5));

                    try
                    {
                        await client.ConnectAsync(clientOptions);
                    }
                    catch
                    {
                        Logger.LogDebug("### RECONNECTING FAILED ###");
                    }
                });

                try
                {
                    await SubscribeAsync(MqttClientConfig.GetSubTopicUp()); //先把订阅的主题弄出来（要等连上后才会订阅）

                    await client.ConnectAsync(clientOptions);
                    //订阅主题，用于接受反馈的消息
                    //await client.SubscribeAsync(config.SubTopic);
                }
                catch (Exception exception)
                {
                    Logger.LogDebug("### CONNECTING FAILED ###" + Environment.NewLine + exception);
                }

                Logger.LogDebug("### WAITING FOR APPLICATION MESSAGES ###");

                //while (true)
                //{
                //    //Console.ReadLine();
                //    //发一次消息测试一下
                //    await client.SubscribeAsync(new MqttTopicFilter { Topic = "test", QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce });
                //    //string data = $"{{\"text\":\"Hello World {DateTime.Now.ToString()}\"}}";
                //    byte[] data = Encoding.UTF8.GetBytes($"{{\"text\":\"mqtt 中转插件已经上线 {DateTime.Now.ToString()}\"}}");
                //    //if (Config.Charset == "gbk" || Config.Charset == "gb2312" || Config.Charset == "gb18030")
                //    //    data = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("gbk"), data);
                //    var applicationMessage = new MqttApplicationMessageBuilder()
                //        .WithTopic(config.GetSubTopicUp())
                //        .WithPayload(data)
                //        .WithAtLeastOnceQoS()
                //        .Build();

                //    await client.PublishAsync(applicationMessage);
                //}
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
            }
            return await Task.FromResult(false);
        }
        public async Task SubscribeAsync(string addTopic = "")
        {
            if (!addTopic.IsNullOrWhiteSpace())
            {
                SubTopicList.Locking(() =>
                {
                    if (!SubTopicList.Contains(addTopic))
                        SubTopicList.Add(addTopic);
                });

            }
            if (client.IsConnected)
            {
                foreach (var topic in SubTopicList)
                {
                    if (!string.IsNullOrWhiteSpace(topic))
                    {
                        await client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());
                        Logger.LogDebug("### SUBSCRIBED ### " + topic);
                    }
                }
                Logger.LogDebug("### END SUBSCRIBED ###");
            }
        }
        /// <summary>
        /// 将MQTT返回过来的data转成设备通讯的原始的格式，根据是否启用了
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] ConvertDataFromMqtt(string topic, byte[] data)
        {
            string raw2 = StringHelper.GetTopicItem(topic, -2);
            if (raw2.ToLower() == "hex")
            {
                return StringHelper.HexStrToHexByte(Encoding.UTF8.GetString(data));
            }
            else if (raw2.ToLower() == "base64")
            {
                return Convert.FromBase64String(Encoding.UTF8.GetString(data));
            }
            else
                return data;
        }
        public byte[] ConvertDataToMqtt(string topic, byte[] data, string title)
        {
            //从topic的倒数第二段识别是否要转换编码
            string raw2 = StringHelper.GetTopicItem(topic, -2);
            if (raw2.ToLower() == "hex")
            {
                string hexstring = data.ByteToHexStr();
                Logger.LogDebug($"# {title} 发送 \r\n{hexstring}");
                return Encoding.UTF8.GetBytes(hexstring);
            }
            else if (raw2.ToLower() == "base64")
            {
                string b64string = Convert.ToBase64String(data);
                Logger.LogDebug($"# {title} 发送 \r\n{b64string}");
                return Encoding.UTF8.GetBytes(b64string);
            }
            else
                return data;
        }
        public async Task<IotResult<UniMessageBase>> SendAsync(string data, string title = "")
        {
            return await SendAsync(Encoding.UTF8.GetBytes(data), title);
        }
        public async Task<IotResult<UniMessageBase>> SendAsync(byte[] data, string title = "")
        {
            IotResult<UniMessageBase> rs = null;
            try
            {

                if (client == null || !client.IsConnected)
                {
                    await TryOpenAsync();
                }
                if (client != null && client.IsConnected)
                {
                    string topic = MqttClientConfig.GetPubTopicDown();
                    //将内容编码
                    byte[] hexdata = ConvertDataToMqtt(topic, data, title);

                    var applicationMessage = new MqttApplicationMessageBuilder()
                        .WithTopic(topic)
                        .WithPayload(hexdata)
                        .WithAtLeastOnceQoS()
                        .Build();

                    await client.PublishAsync(applicationMessage);
                    //await Task.Delay(10);
                    rs = new IotResult<UniMessageBase>(true, "发送成功");
                    return await Task.FromResult(rs);
                }
                else
                {
                    Logger.LogWarning($"{MqttClientConfig.ServerIP} 服务器没有连接!不能发送数据!");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"{MqttClientConfig.ServerIP}发送数据出错!");
            }
            rs = new IotResult<UniMessageBase>(false, "发送失败");
            return await Task.FromResult(rs);
        }


        //static Encoding gbk = Encoding.GetEncoding("gbk");


        public async Task<IotResult<UniMessageBase>> SendAndWaitReply(byte[] data, string title = "", int waitTimeout = 3000)
        {
            if (waitTimeout < 10000)
                waitTimeout = 10000;
            resultCompletionSource = new TaskCompletionSource<IotResult<UniMessageBase>>();
            #region 定义超时对象
            var ct = new CancellationTokenSource(waitTimeout); //mqtt的等待时间需要10秒以上
            ct.Token.Register(() => resultCompletionSource.TrySetCanceled(), useSynchronizationContext: false);
            #endregion
            try
            {
                if (client == null || !client.IsConnected)
                {
                    await TryOpenAsync();
                }
                if (client != null && client.IsConnected)
                {
                    string topic = MqttClientConfig.GetPubTopicDown();
                    //将内容编码
                    byte[] hexdata = ConvertDataToMqtt(topic, data, title);

                    var applicationMessage = new MqttApplicationMessageBuilder()
                        .WithTopic(topic)
                        .WithPayload(hexdata)
                        .WithAtLeastOnceQoS()
                        .Build();

                    await client.PublishAsync(applicationMessage);
                    return await resultCompletionSource.Task;

                }
                else
                {
                    Logger.LogWarning($"{MqttClientConfig.ServerIP} 服务器没有连接!不能发送数据!");
                    //此时窜口未打开，则直接取消异步等待
                    var rs = new IotResult<UniMessageBase>(false, $"服务器{MqttClientConfig.ServerIP}没有连接!不能发送数据!") { Result = null };
                    resultCompletionSource.SetResult(rs);
                }

            }
            catch (Exception ex)
            {
                //此时应意外原因通讯出错，则直接终止
                Logger.LogError(ex, $"服务器{MqttClientConfig.ServerIP}发送数据出错：{ex.Message}({ex.Source})");
                var rs = new IotResult<UniMessageBase>(false, $"服务器{MqttClientConfig.ServerIP}发送数据出错{ex.Message}({ex.Source})。") { Result = null };
                resultCompletionSource.SetResult(rs);
            }
            return await resultCompletionSource.Task;
        }

        /// <summary>
        /// 转发的消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task<IotResult<UniMessageBase>> SendUniMessage(UniMessageBase msg)
        {
            //var restlt = ConvertDataToMqtt(msg.Topic, msg.Payload,"");
            IotResult<UniMessageBase> rs = null;
            try
            {

                if (client == null || !client.IsConnected)
                {
                    await TryOpenAsync();
                }
                if (client != null && client.IsConnected)
                {
                    //将内容编码
                    byte[] hexdata = ConvertDataToMqtt(msg.Topic, msg.Payload, msg.ClientId);

                    var applicationMessage = new MqttApplicationMessageBuilder()
                        .WithTopic(msg.Topic)
                        .WithPayload(hexdata)
                        .WithAtLeastOnceQoS()
                        .Build();

                    await client.PublishAsync(applicationMessage);
                    await Task.Delay(10);
                }
                else
                {
                    Logger.LogWarning($"{MqttClientConfig.ServerIP} 服务器没有连接!不能发送数据!");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"{MqttClientConfig.ServerIP}发送数据出错!");
            }
            rs = new IotResult<UniMessageBase>(false, "发送失败");
            return await Task.FromResult(rs);
        }

        public void Dispose()
        {
            if (client == null || !client.IsConnected)
            {
                if (this.IsConnected)
                {
                    this.client.DisconnectAsync();
                    Thread.Sleep(1000);
                }
                this.client.Dispose();
                this.client = null;

            }
        }

    }
}
