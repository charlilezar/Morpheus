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
using Volo.Abp.Threading;

namespace Aming.DTU
{
    /// <summary>
    ///TcpClien通讯服务
    /// </summary>
    public class TcpClientService : ITransmissionService
    {

        public AsyncTcpSocketClient _client;
        public Microsoft.Extensions.Logging.ILogger Logger { get; set; }
        /// <summary>
        /// 通讯端口是否连接中
        /// </summary>
        public bool IsConnected { get { return (_client != null && _client.State == TcpSocketConnectionState.Connected); } }
        /// <summary>
        /// 配置
        /// </summary>
        public IDataEndPoint Config { get; set; } //MqttClientConfig
        public TcpClientConfig TcpClientConfig { get { return Config as TcpClientConfig; } }

        /// <summary>
        /// 是否需要检查包格式，在通讯分包和粘包时有用
        /// <return>一个包的有效长度</return>
        /// </summary>
        public Func<List<byte>, int> funcCheckPackFormat { get; set; } = null;

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
        public TcpClientService(ILoggerFactory loggerFactory, TcpClientConfig config)
        {
            this.Logger = loggerFactory.CreateLogger<TcpClientService>();
            this.Config = config;
        }


        public async Task<bool> TryOpenAsync()
        {
            if (this._client != null && (this._client.State == TcpSocketConnectionState.Connecting || this._client.State == TcpSocketConnectionState.Connected))
                return true;
            try
            {
                var tcpConfig = new AsyncTcpSocketClientConfiguration();
                //config.UseSsl = true;
                //config.SslTargetHost = "Cowboy";
                //config.SslClientCertificates.Add(new System.Security.Cryptography.X509Certificates.X509Certificate2(@"D:\\Cowboy.cer"));
                //config.SslPolicyErrorsBypassed = false;

                //config.FrameBuilder = new FixedLengthFrameBuilder(20000);
                tcpConfig.FrameBuilder = new RawBufferFrameBuilder();
                //tcpConfig.FrameBuilder = new LineBasedFrameBuilder();
                //config.FrameBuilder = new LengthPrefixedFrameBuilder();
                //config.FrameBuilder = new LengthFieldBasedFrameBuilder();

                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(TcpClientConfig.ServerIP), TcpClientConfig.ServerPort);
                _client = new AsyncTcpSocketClient(remoteEP, OnServerDataReceived, OnServerConnected, OnServerDisconnected, tcpConfig);
                //public AsyncTcpSocketClient(IPEndPoint remoteEP, Func<AsyncTcpSocketClient, byte[], int, int, Task> onServerDataReceived = null, Func<AsyncTcpSocketClient, Task> onServerConnected = null, Func<AsyncTcpSocketClient, Task> onServerDisconnected = null, AsyncTcpSocketClientConfiguration configuration = null);

                await _client.Connect();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
                return false;
            }

        }

        public async Task<IotResult<UniMessageBase>> SendAsync(byte[] data, string title = "")
        {
            IotResult<UniMessageBase> rs = null;
            try
            {
                if (_client == null || _client.State == TcpSocketConnectionState.Closed || _client.State == TcpSocketConnectionState.None)
                {
                    await TryOpenAsync();
                }
                if (_client != null || _client.State == TcpSocketConnectionState.Connected || _client.State == TcpSocketConnectionState.None)
                {

                    Logger.LogDebug($"# {title} 发送 \r\n{data.ByteToHexStr()}");
                    await _client.SendAsync(data);
                    await Task.Delay(10);
                    rs = new IotResult<UniMessageBase>(true, "发送成功");
                    return await Task.FromResult(rs);
                }
                else
                {
                    Logger.LogWarning($"{TcpClientConfig.ServerIP}:{TcpClientConfig.ServerPort}没有打开!不能发送数据!");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"{TcpClientConfig.ServerIP}:{TcpClientConfig.ServerPort}发送数据出错!");
            }
            rs = new IotResult<UniMessageBase>(false, "发送失败");
            return await Task.FromResult(rs);
        }

        public async Task<IotResult<UniMessageBase>> SendAndWaitReply备份(byte[] data, string title = "", int waitTimeout = 3000)
        {
            resultCompletionSource = new TaskCompletionSource<IotResult<UniMessageBase>>();

            #region 定义超时对象
            var ct = new CancellationTokenSource(waitTimeout);
            ct.Token.Register(() => resultCompletionSource.TrySetCanceled(), useSynchronizationContext: false);
            #endregion
            try
            {
                if (_client == null || _client.State == TcpSocketConnectionState.Closed || _client.State == TcpSocketConnectionState.None)
                {
                    await TryOpenAsync();
                }

                if (_client != null && _client.State == TcpSocketConnectionState.Connected)
                {

                    Logger.LogDebug($"# {title} 发送 \r\n{data.ByteToHexStr()}");

                    await _client.SendAsync(data);
                    await Task.Delay(10);
                    return await resultCompletionSource.Task;
                }
                else
                {
                    //此时窜口未打开，则直接取消异步等待
                    Logger.LogWarning($"{TcpClientConfig.ServerIP}:{TcpClientConfig.ServerPort}没有打开!不能发送数据!");
                    var rs = new IotResult<UniMessageBase>(false, $"{TcpClientConfig.ServerIP}:{TcpClientConfig.ServerPort}没有打开!不能发送数据!") { Result = null };
                    resultCompletionSource.SetResult(rs);
                }
            }
            catch (Exception ex)
            {
                ////此时应意外原因通讯出错，则直接终止

                Logger.LogError(ex, $"{TcpClientConfig.ServerIP}:{TcpClientConfig.ServerPort}发送数据出错!");
                var rs = new IotResult<UniMessageBase>(false, $"{TcpClientConfig.ServerIP}:{TcpClientConfig.ServerPort}发送数据出错!" + ex.Message) { Result = null };
                resultCompletionSource.SetResult(rs);
            }
            return await resultCompletionSource.Task;
        }
        public async Task<IotResult<UniMessageBase>> SendAndWaitReply(byte[] data, string title = "", int waitTimeout = 3000)
        {
            resultCompletionSource = new TaskCompletionSource<IotResult<UniMessageBase>>();

            #region 定义超时对象
            var ct = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken.None);
            ct.CancelAfter(waitTimeout);
            #endregion
            try
            {
                if (_client == null || _client.State == TcpSocketConnectionState.Closed || _client.State == TcpSocketConnectionState.None)
                {
                    await TryOpenAsync();
                }

                if (_client != null && _client.State == TcpSocketConnectionState.Connected)
                {

                    Logger.LogDebug($"# {title} 发送 \r\n{data.ByteToHexStr()}");
                    using (ct.Token.Register(() => SendAndWaitReplyTimeOut(this.resultCompletionSource, new Exception("通讯超时"), data, title), useSynchronizationContext: false))
                    {
                        await _client.SendAsync(data);
                        await Task.Delay(10);
                        return await resultCompletionSource.Task;
                    }
                }
                else
                {
                    //此时窜口未打开，则直接取消异步等待
                    Logger.LogWarning($"{TcpClientConfig.ServerIP}:{TcpClientConfig.ServerPort}没有打开!不能发送数据!");
                    //此时窜口未打开，则直接取消异步等待
                    SendAndWaitReplyTimeOut(this.resultCompletionSource, new Exception($"TCP/IP未连接关闭，无法通讯！"), data, title);
                }
            }
            catch (Exception ex)
            {
                ////此时应意外原因通讯出错，则直接终止

                Logger.LogError(ex, $"{TcpClientConfig.ServerIP}:{TcpClientConfig.ServerPort}发送数据出错!");
                SendAndWaitReplyTimeOut(this.resultCompletionSource, ex, data, title);
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
            return await SendAsync(msg.Payload, msg.ClientId);
        }

        public void Dispose()
        {
            if (_client != null && _client.State == TcpSocketConnectionState.Closed)
            {
                var t = _client.Close();
                Task.WaitAll(new Task[] { t }, 1000);
            }
            _client = null;
        }


        public async Task OnServerConnected(AsyncTcpSocketClient client)
        {
            Logger.LogDebug(string.Format("TCP server {0} has connected.", client.RemoteEndPoint));
            await Task.CompletedTask;
        }
        #region 添加粘包处理
        /// <summary>
        /// 最后一个包的接收时间
        /// </summary>
        public DateTime LastPackReceiveTime = DateTime.MinValue;
        /// <summary>
        /// 处理粘包时最大等待时间，单位：毫秒, 不同的设备可以更改时间，一般在200毫秒就够了
        /// </summary>
        public int SplitPackWaitingMilliSeconds = 500;
        List<byte> ReceiveBytes = new List<byte>();

        ///// <summary>
        ///// 以换行符为条件分包结束
        ///// </summary>
        ///// <returns>一个包的有效长度</returns>
        //public static int CheckPackFormatByNewLine(List<byte> buff)
        //{
        //    if (buff.Count < 2) return 0;//小于2个字节的则认为未传完
        //    for (int i = 0; i < buff.Count; i++)
        //    {
        //        if (buff[i] == 0x0D || buff[i] == 0x0A)   //换行符\r \n
        //        {
        //            if (i < buff.Count - 1 && buff[i + 1] == 0x0D)
        //                i++;
        //            return i + 1;
        //        }
        //    }
        //    return 0;
        //}
        ///// <summary>
        ///// 检查包的格式有效性,HD气碘使用的格式，68 NN NN 68 ...... CH OD 
        ///// </summary>
        ///// <returns>一个包的有效长度</returns>
        //public static int CheckPackFormatBy680D(List<byte> buff)
        //{
        //    if (buff.Count < 4) return 0;//小于2个字节的则认为未传完
        //    int iLen = buff[1];
        //    iLen += 6;
        //    if (buff.Count >= iLen && (buff[0] == 0x68 && buff[3] == 0x68 && buff[iLen - 1] == 0x0D))
        //        return iLen;
        //    return 0;
        //}
        #endregion

        public async Task OnServerDataReceived(AsyncTcpSocketClient client, byte[] dataBuff, int offset, int count)
        {
            try
            {
                byte[] data = new byte[count];
                Array.Copy(dataBuff, offset, data, 0, count); //只能从缓冲区取一段
                UniMessageBase msg;
                //开始处理粘包，处理方案和窜口处理一致
                if (funcCheckPackFormat != null)
                {//有分包粘包要考虑
                    if (ReceiveBytes.Count > 0 && (DateTime.Now - LastPackReceiveTime).TotalMilliseconds > this.SplitPackWaitingMilliSeconds)
                    {
                        Logger.LogDebug($"# 缓存区有无效数据将被抛弃。[{ReceiveBytes.ToArray().ByteToHexStr()}]");
                        ReceiveBytes.Clear();
                    }
                    else
                    {
                        ReceiveBytes.AddRange(data);
                        LastPackReceiveTime = DateTime.Now;
                    }
                    int fLen = funcCheckPackFormat(ReceiveBytes);
                    if (0 >= fLen)
                    {
                        msg = new UniMessageBase { Payload = data, Topic = $"/mingdtu/{TcpClientConfig.ServerIP}/{TcpClientConfig.ServerPort}/hex/up" };
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
                msg = new UniMessageBase { Payload = data, Topic = $"/mingdtu/{TcpClientConfig.ServerIP}/{TcpClientConfig.ServerPort}/hex/up" };
                if (msg.Payload.Length < 100)
                    Logger.LogDebug($"# {msg.Topic} 收完整数据 \r\n{msg.Payload.ByteToHexStr()}");//接收完整后再发送一次全部收到的数据，前面粘包处理时发送每次收到的
                else
                    Logger.LogDebug($"# {msg.Topic} 收完整数据 \r\n{msg.Payload.Length}字节");


                if (OnDataForwardTo != null)
                {
                    //单独线程中去转发，不阻塞
                    var t2 = Task.Run(() =>
                    {
                        OnDataForwardTo(Config, msg);
                    });
                }
                //单独线程中去处理，不阻塞
                var t1 = Task.Run(() =>
                {
                    if (resultCompletionSource != null && ((int)resultCompletionSource.Task.Status) < 4)
                    {
                        var rs = new IotResult<UniMessageBase>(true, "收到回应") { Result = msg };
                        resultCompletionSource.SetResult(rs);
                    }
                    else if (OnUniMessageReceived != null) //传统的事件触发传出去
                    {
                        //var msg1 = new UniMessageBase { Payload = data, Topic = $"/DTU/{Config.Port}/from" };
                        OnUniMessageReceived(Config, msg);
                    }
                });


            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"{TcpClientConfig.ServerIP}:{TcpClientConfig.ServerPort}数据解析时出错!");
            }
            await Task.CompletedTask;
        }

        public async Task OnServerDisconnected(AsyncTcpSocketClient client)
        {
            Logger.LogDebug(string.Format("TCP server {0} has disconnected.", client.RemoteEndPoint));
            await Task.CompletedTask;
        }
        #region 超时处理
        /// <summary>
        /// 定义一个无返回的委托，用于设置超市后如何处理数据，如果外部调用该类前不赋值该委托，
        /// 则默认调用调用函数SendAndWaitReply_TimeOutPro，该函数会抛出一个Excetpion对象，提示引发取消异步等待的原因。
        /// </summary>
        public Action<TaskCompletionSource<IotResult<UniMessageBase>>, Exception, byte[], string> SendAndWaitReplyTimeOut = SendAndWaitReply_TimeOutPro;
        private static void SendAndWaitReply_TimeOutPro(TaskCompletionSource<IotResult<UniMessageBase>> tcs, Exception ex, byte[] data, string sTitle)
        {
            if (tcs != null && (int)tcs.Task.Status < 4)
            {
                var rs = new IotResult<UniMessageBase>(false, $"TCP/IP执行出错：{ex.Message}({ex.Source})") { Result = null };
                tcs.SetResult(rs);
                //此时直接报错错误
                //Exception ex = new Exception(string.Format("执行[{0}]时超时，相关数据{1}。", sTitle, StringHelper.ByteToHexStr(data)));
                //Logger.LogWarning($"执行命令{sTitle}时取消了异步等待，原因:{ex.Message}({ex.Source})。");
                //tcs.SetException(ex);
            }
        }
        #endregion

    }
}

