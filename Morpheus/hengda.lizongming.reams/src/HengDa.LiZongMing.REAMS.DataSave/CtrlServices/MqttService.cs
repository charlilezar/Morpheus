using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Logging;

using Cowboy.Sockets;

using System.Net;
using Aming.DTU;
using Aming.DTU.Config;
using Aming.Core;
using HengDa.LiZongMing.REAMS.Wpf;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Protocol;

namespace Aming.DTU
{
    /// <summary>
    ///TcpClien通讯服务
    /// </summary>
    public class MqttService : ITransmissionService
    {
        MqttClient mqttClient = null;
        public string SubscribedTopic { get; set; }//已经订阅的内容
        public Microsoft.Extensions.Logging.ILogger Logger { get; set; }
        /// <summary>
        /// 通讯端口是否连接中(目前发现mqttClient.IsConnected无效，只有刚连接时是true，第二次来调用就变成false)
        /// </summary>
        //public bool IsConnected { get; set; } = false;
        public bool IsConnected { get { return (mqttClient != null && mqttClient.IsConnected); } }
        /// <summary>
        /// 配置
        /// </summary>
        public MqttConfig Config { get; set; }
        IDataEndPoint ITransmissionService.Config { get => Config; set => Config = (MqttConfig)value; }

        /// <summary>
        /// 事件回调时，收到数据触发解析
        /// </summary>
        public Action<MqttConfig, UniMessageBase> OnDataReceived = null;
        /// <summary>
        /// 事件回调变异步回调
        /// </summary>
        TaskCompletionSource<IotResult<UniMessageBase>> resultCompletionSource = null;
        public MqttService(ILoggerFactory loggerFactory, MqttConfig config)
        {
            this.Logger = loggerFactory.CreateLogger<MqttService>();
            this.Config = config;
        }

        public void Dispose()
        {
            if (this.mqttClient != null)
            {
                if (this.IsConnected)
                    this.mqttClient.DisconnectAsync();
                this.mqttClient.Dispose();
                this.mqttClient = null;
            }
        }

        public async Task OnServerConnected(MqttClient client)
        {
            Logger.LogDebug(string.Format("MQTT server {0} has connected.", client));
            await Task.CompletedTask;
        }
        #region 连接对象
        public void SubscribedTopicClear()
        {
            //清空已订阅的主题
            this.SubscribedTopic = string.Empty;
        }
        public async Task<bool> TryOpenAsync(bool autoCheckConnect)
        {
           
            if(autoCheckConnect)
            {
                //打开监听线程，确保一直连接中，因为可能远程客户端会有请求过来。
                //TODO:打开子线程监听
            }
            return  await TryOpenAsync();
        }
        public async Task<bool> TryOpenAsync()
        {
            if (mqttClient == null)
            {
                mqttClient = new MqttFactory().CreateMqttClient() as MqttClient;
                mqttClient.UseApplicationMessageReceivedHandler(this.OnServerDataReceived);
            }
            if (mqttClient.IsConnected) return true;//该属性居然无效的，一直会是false
            //if (this.IsConnected) return true;
            IMqttClientOptions Options = new MqttClientOptionsBuilder()
                      .WithTcpServer(Config.ServerIP, Config.ServerPort)
                      //.WithClientId(Guid.NewGuid().ToString().Substring(0, 5))
                      .WithClientId(Config.ClientId)
                      .WithCredentials(Config.UserName, Config.Password)
                      .WithCleanSession()
                      .Build();
            try
            {
                await mqttClient.ConnectAsync(Options);
                //this.IsConnected = true;
                await SubscribeAsync();//连接后直接订阅
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
            }
            return false;
        }
        public async Task<bool> SubscribeAsync()
        {
            if (!await TryOpenAsync()) return false;
            //开始订阅
            try
            {
                //Task<MQTTnet.Client.Subscribing.MqttClientSubscribeResult> task = mqttClient.SubscribeAsync(sTopic, MqttQualityOfServiceLevel.AtMostOnce);
                await mqttClient.SubscribeAsync(this.Config.SubTopic, MqttQualityOfServiceLevel.AtMostOnce);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
                return false;
            }
        }
        #endregion
        #region 添加粘包处理
        public Func<List<byte>, int> funcCheckPackFormat = CheckPackFormatByNewLine;
        List<byte> ReceiveBytes = new List<byte>();

        /// <summary>
        /// 以换行符为条件分包结束
        /// </summary>
        /// <returns>一个包的有效长度</returns>
        public static int CheckPackFormatByNewLine(List<byte> buff)
        {
            return buff.Count;
        }

        #endregion
        #region 发送/接收数据
        public async Task<IotResult<UniMessageBase>> SendAsync(byte[] data, string title = "")
        {
            IotResult<UniMessageBase> rs;
            try
            {
                if (await TryOpenAsync())
                {
                    string strData = data.ByteToHexStr();
                    Logger.LogDebug($"# {title} 发送 \r\n{strData}\r\n主题[{this.Config.PubTopicDown}]");
                    //此时正确打开着，则发送数据
                    await this.mqttClient.PublishAsync(this.Config.PubTopicDown, strData, MqttQualityOfServiceLevel.AtMostOnce);
                    await Task.Delay(10);
                    rs = new IotResult<UniMessageBase>(true, "发送成功");
                }
                else
                {
                    string strErr = $"MQTT[{Config.ServerIP}:{Config.ServerPort}]没有连上!不能发送数据!";
                    Logger.LogWarning(strErr);
                    rs = new IotResult<UniMessageBase>(false, strErr);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"{Config.ServerIP}:{Config.ServerPort}发送数据出错!");
                rs = new IotResult<UniMessageBase>(false, $"发送数据出错:{ex.Message}({ex.Source})");
            }
            return await Task.FromResult(rs);
        }

        public async Task<IotResult<UniMessageBase>> SendAndWaitReply(byte[] data, string title = "", int waitTimeout = 3000)
        {
            resultCompletionSource = new TaskCompletionSource<IotResult<UniMessageBase>>();
            IotResult<UniMessageBase> rs;
            #region 定义超时对象
            var ct = new CancellationTokenSource(waitTimeout);
            ct.Token.Register(() => resultCompletionSource.TrySetCanceled(), useSynchronizationContext: false);
            #endregion
            try
            {
                if (await TryOpenAsync())
                {
                    //此时成功连接
                    string strTopicDown = this.Config.GetPubTopicDown();
                    string strData = data.ByteToHexStr();
                    Logger.LogDebug($"# {title} 发送 \r\n{strData}\r\n主题[{strTopicDown}]");
                    //此时正确打开着，则发送数据
                    await this.mqttClient.PublishAsync(strTopicDown, strData, MqttQualityOfServiceLevel.AtMostOnce);
                    await Task.Delay(10);
                    rs = new IotResult<UniMessageBase>(true, "发送成功");
                    return await resultCompletionSource.Task;
                }
                else
                {
                    //此时没有连接上
                    //此时窜口未打开，则直接取消异步等待
                    string strMsg = $"MQTT[{Config.ServerIP}:{Config.ServerPort}]没有连接!不能发送数据!";
                    Logger.LogWarning(strMsg);
                    rs = new IotResult<UniMessageBase>(false, strMsg) { Result = null };
                    resultCompletionSource.SetResult(rs);
                }
            }
            catch (Exception ex)
            {
                ////此时应意外原因通讯出错，则直接终止
                //this.IsConnected = false;//很可能是断开了，重新连接
                Logger.LogError(ex, $"{Config.ServerIP}:{Config.ServerPort}发送数据出错!");
                rs = new IotResult<UniMessageBase>(false, $"MQTT[{Config.ServerIP}:{Config.ServerPort}]发送数据出错:{ex.Message}({ex.Source})") { Result = null };
                resultCompletionSource.SetResult(rs);
            }
            return await resultCompletionSource.Task;
        }
        public async Task OnServerDataReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            if (e == null || e.ApplicationMessage == null) return;
            try
            {
                UniMessageBase msg;
                if (string.Compare(e.ApplicationMessage.Topic, this.Config.GetPubTopicUp(), true) == 0)
                {
                    //此时为设备返回，则要返回给设备，否则是做其他相应，应该是远程客户端的通讯请求
                    byte[] data = GetMacDataFromMqttData(e.ApplicationMessage.Payload);
                    if (funcCheckPackFormat != null)
                    {
                        ReceiveBytes.AddRange(data);
                        int fLen = funcCheckPackFormat(ReceiveBytes);
                        if (0 >= fLen)
                        {
                            msg = new UniMessageBase { Payload = data, Topic = $"/DTU/{Config.ServerPort}/from" };
                            Logger.LogDebug($"# {msg.Topic} 收到 \r\n{msg.Payload.ByteToHexStr()}，但数据不全，等待下次接收。");
                            return; //长度不够或格式不符
                        }
                        else
                        {
                            //分包的数据已接收完整
                            data = ReceiveBytes.Take(fLen).ToArray();
                            if (ReceiveBytes.Count == fLen)
                                ReceiveBytes.Clear();
                            else
                                ReceiveBytes.RemoveRange(0, fLen);
                        }
                    }
                    msg = new UniMessageBase { Payload = data, Topic = $"/DTU/{Config.ServerPort}/from" };
                    Logger.LogDebug($"# {msg.Topic} 收完整数据 \r\n{msg.Payload.ByteToHexStr()}");//接收完整后再发送一次全部收到的数据，前面粘包处理时发送每次收到的
                    if (OnDataReceived != null)
                    {
                        OnDataReceived(Config, msg);
                    }
                    if (resultCompletionSource != null && ((int)resultCompletionSource.Task.Status) < 4)
                    {
                        var rs = new IotResult<UniMessageBase>(true, "收到回应") { Result = msg };
                        resultCompletionSource.SetResult(rs);
                    }
                    await Task.CompletedTask;
                }
                else
                {
                    #region 远程客户端数据请求

                    #endregion
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"{Config.ServerIP}:{Config.ServerPort}数据接收时出错!");
            }
        }
        public byte[] GetMacDataFromMqttData(byte[] data)
        {
            //将MQTT返回过来的data转成设备返回的格式，即1个字节一个byte，而MQTT的话是2个ASCII值存储一个字节
            return StringHelper.HexStrToHexByte(Encoding.UTF8.GetString(data));
        }
        #endregion
        public async Task OnServerDisconnected(AsyncTcpSocketClient client)
        {
            Logger.LogDebug(string.Format("TCP server {0} has disconnected.", client.RemoteEndPoint));
            await Task.CompletedTask;
        }
        
    }
}

