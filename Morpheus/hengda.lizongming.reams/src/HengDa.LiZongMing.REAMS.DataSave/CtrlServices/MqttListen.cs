using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Logging;

//using Cowboy.Sockets;

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
using PropertyChanged;

namespace Aming.DTU
{
    [AddINotifyPropertyChangedInterface]
    /// <summary>
    ///TcpClien通讯服务
    /// </summary>
    public class MqttListen :MyListenBase
    {
        public Action<MqttListen, bool> ConnectedChangedNotice = null;
        MqttClient mqttClient = null;
        public string SubscribedTopic { get; set; }//已经订阅的内容
        public Microsoft.Extensions.Logging.ILogger Logger { get; set; }
        /// <summary>
        /// 通讯端口是否连接中(目前发现如果直接应用mqttClient对象的连接属性，连接状态变化几次后，WPF界面绑定值就不会更新了。目前不知道原因)
        /// </summary>
        public bool IsConnected { get; set; } = false;
        //public bool IsConnected { get { return (mqttClient != null && mqttClient.IsConnected); } }
        /// <summary>
        /// 配置
        /// </summary>
        public MqttConfig Config { get; set; }

        /// <summary>
        /// 事件回调时，收到数据触发解析
        /// </summary>
        public Action<MqttConfig, UniMessageBase> OnDataReceived = null;
        public MqttListen(ILoggerFactory loggerFactory, MqttConfig config)
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
                if (this.ConnectedChangedNotice != null)
                    ConnectedChangedNotice(this, false);
            }
        }

        #region 监听过程，确保MQTT监听正常运行
        public override void ListeningProData()
        {
            //判断是否要断开了，如果断开则重新连接，并记录日志
            this.TryOpenAsync();
            if (this.IsConnected != (this.mqttClient != null && this.mqttClient.IsConnected))
            {
                this.IsConnected = !this.IsConnected; //更新值，否则多次状态更改后WPF就不会更新状态了
                if (this.ConnectedChangedNotice != null)
                    this.ConnectedChangedNotice(this, this.IsConnected);
            }
            Thread.Sleep(1000);
        }
        public override void StopListenning(short iName)
        {
            //此时关闭了监听
            this.Dispose();
            base.StopListenning(iName);
        }
        #endregion
        #region 设置MQTT服务器参数
        public void SetMqttConfig(MqttConfig config)
        {
            this.Config = config;
        }
        #endregion
        #region 连接MQTT对象
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
                await mqttClient.SubscribeAsync(this.Config.GetPubTopicDown(), MqttQualityOfServiceLevel.AtMostOnce);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
                return false;
            }
        }
        #endregion
        #region 发送/接收数据
        public  Func<byte[], Task<string>> SendText2MQTTServer = null;
        public async Task OnServerDataReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            if (e == null || e.ApplicationMessage == null) return;
            try
            {
                UniMessageBase msg;
                if (string.Compare(e.ApplicationMessage.Topic, this.Config.GetPubTopicDown(), true) == 0)
                {
                    //此时为应该是远程客户端的通讯请求
                    byte[] data = GetMacDataFromMqttData(e.ApplicationMessage.Payload);
                    string strResponse;
                    if (this.SendText2MQTTServer != null)
                        strResponse = await SendText2MQTTServer(data);
                    else
                        strResponse = "unkown";
                    await this.mqttClient.PublishAsync(this.Config.GetPubTopicUp(), strResponse, MqttQualityOfServiceLevel.AtMostOnce);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"{Config.ServerIP}:{Config.ServerPort}数据解析时出错!");
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

