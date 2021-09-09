using Aming.Core;
using Aming.DTU.Config;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Threading;

namespace Aming.DTU
{
    /// <summary>
    /// 串口通讯服务
    /// </summary>
    public class SerialPortService : ITransmissionService
    {
        public ILogger Logger { get; set; }
        public SerialPort sp = null;
        /// <summary>
        /// 通讯端口是否连接中
        /// </summary>
        public bool IsConnected { get { return sp != null && sp.IsOpen; } }
        public string[] PortList { get; set; } = SerialPort.GetPortNames();

        /// <summary>
        /// 是否在等待返回信息
        /// </summary>
        //public bool IsWaitReply { get { return (resultCompletionSource != null && !resultCompletionSource.Task.IsCompleted); } }

        /// <summary>
        /// 配置
        /// </summary>
        public IDataEndPoint Config { get; set; } //MqttClientConfig
        public UARTConfig UARTConfig { get { return Config as UARTConfig; } }

        /// <summary>
        /// 事件回调时，收到数据触发解析
        /// </summary>
        public event ReceivedUniMessageCallBack OnUniMessageReceived = null;

        /// <summary>
        /// 将本端数据转发到别的端口的处理程序
        /// </summary>
        public event ReceivedUniMessageCallBack OnDataForwardTo = null;

        /// <summary>
        /// 是否需要检查包格式，在通讯分包和粘包时有用
        /// <return>一个包的有效长度</return>
        /// </summary>
        public Func<List<byte>, int> funcCheckPackFormat { get; set; } = null;

        /// <summary>
        /// 事件回调变异步回调
        /// </summary>
        TaskCompletionSource<IotResult<UniMessageBase>> resultCompletionSource = null;
        public SerialPortService(ILoggerFactory loggerFactory, UARTConfig config)
        {
            this.Logger = loggerFactory.CreateLogger<SerialPortService>();
            this.Config = config;
        }
        public async Task<bool> TryOpenAsync()
        {

            try
            {
                if (sp != null && sp.IsOpen)
                {
                    //已经打开了。
                }
                else
                {
                    sp = new SerialPort((string)UARTConfig.Port,
                        UARTConfig.BaudRate,
                        Parity.None);
                    sp.DataReceived += PortDataReceived;
                    //_port.ReceivedBytesThreshold = 1; //原来是这
                    //_port.ReceivedBytesThreshold = 1024;
                    sp.ReadTimeout = 40;
                    sp.Open();
                }
                Logger.LogInformation($"打开端口{UARTConfig.Port}!");
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"打开端口{UARTConfig.Port}出错!");
            }
            return await Task.FromResult(false);

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

        /// <summary>
        /// 以换行符为条件分包结束
        /// </summary>
        /// <returns>一个包的有效长度</returns>
        public static int CheckPackFormatByNewLine(List<byte> buff)
        {
            if (buff.Count < 2) return 0;//小于2个字节的则认为未传完
            for (int i = 0; i < buff.Count; i++)
            {
                if (buff[i] == 0x0D || buff[i] == 0x0A)   //换行符\r \n
                {
                    if (i < buff.Count - 1 && buff[i + 1] == 0x0D)
                        i++;
                    return i + 1;
                }
            }
            return 0;
        }
        /// <summary>
        /// 检查包的格式有效性,HD气碘使用的格式，68 NN NN 68 ...... CH OD 
        /// </summary>
        /// <returns>一个包的有效长度</returns>
        public static int CheckPackFormatBy680D(List<byte> buff)
        {
            if (buff.Count < 4) return 0;//小于2个字节的则认为未传完
            int iLen = buff[1];
            iLen += 6;
            if (buff.Count >= iLen && (buff[0] == 0x68 && buff[3] == 0x68 && buff[iLen - 1] == 0x0D))
                return iLen;
            return 0;
        }

        /// <summary>
        /// 恒达在用的Modbus格式。没有起始和结束标记。如果返回为0表示等待接收，否则接收完成
        /// </summary>
        /// <param name="buff"></param>
        /// <returns>从ReceiveBytes中提取的数据长度</returns>
        public static int CheckPackFormatByModbus(List<byte> buff)
        {
            if (buff.Count < 3) return 0;//小于3个肯定没接收完；
            if (0x03 == buff[1])
            {
                //此时为查询的,则规则就是第三个字节为返回的字节数量
                //总字节数量为：设备地址（1byte）+功能码(1byte)+字节数量(1byte)+返回的字节+校验码(2个字节)
                int iCnt = buff[2] + 5;
                if (buff.Count >= iCnt)
                    return buff.Count;
                return 0;//此时还未收满
            }
            else if (0x10 == buff[1])
            {
                //此时为多寄存器写入，返回固定是8个字节的；
                if (buff.Count >= 8) return buff.Count;
                return 0;
            }
            //其他方式是非预期的，则直接都返回接受完了；
            return buff.Count;
        }
        #endregion

        #region 超时处理
        ///// <summary>
        ///// 定义一个无返回的委托，用于设置超市后如何处理数据，如果外部调用该类前不赋值该委托，
        ///// 则默认调用调用函数SendAndWaitReply_TimeOutPro，该函数会抛出一个Excetpion对象，提示引发取消异步等待的原因。
        ///// </summary>
        //public Action<TaskCompletionSource<IotResult<UniMessageBase>>, Exception, byte[], string> SendAndWaitReplyTimeOut = null;
        //private void SendAndWaitReply_TimeOutPro(TaskCompletionSource<IotResult<UniMessageBase>> tcs, Exception ex, byte[] data, string sTitle)
        //{
        //    if (tcs != null && (int)tcs.Task.Status < 4)
        //    {
        //        //此时直接报错错误
        //        //Exception ex = new Exception(string.Format("执行[{0}]时超时，相关数据{1}。", sTitle, StringHelper.ByteToHexStr(data)));
        //        Logger.LogWarning($"执行命令{sTitle}时取消了异步等待，原因:{ex.Message}({ex.Source})。");
        //        tcs.SetException(ex);
        //    }
        //}
        #endregion
        void PortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // 常见的形式，没有分包粘包要考虑
                Task.Delay(20); //实际情况总被分包了，所以这里等一小会，临时性简单解决
                var rxlen = sp.BytesToRead;
                byte[] data = new byte[rxlen];
                sp.Read(data, 0, rxlen);
                //这个日志改由粘包处理时发送Logger.LogDebug($"串口收到 \r\n{data.ByteToHexStr()}");

                UniMessageBase msg;
                if (funcCheckPackFormat != null)
                { //有分包粘包要考虑
                    if(ReceiveBytes.Count>0 && (DateTime.Now- LastPackReceiveTime).TotalMilliseconds> this.SplitPackWaitingMilliSeconds)
                    {
                        Logger.LogDebug($"# 缓存区有无效数据将被抛弃。[{ReceiveBytes.ToArray().ByteToHexStr()}]");
                        ReceiveBytes.Clear();
                    }
                    else{
                        ReceiveBytes.AddRange(data);
                        LastPackReceiveTime=DateTime.Now;
                    }
                    //Logger.LogDebug($"# 缓存数据[{ReceiveBytes.ToArray().ByteToHexStr()}]。");
                    int fLen = funcCheckPackFormat(ReceiveBytes);
                    if (0 >= fLen)
                    {
                        msg = new UniMessageBase { Payload = data, Topic = $"/mingdtu/uart/{UARTConfig.Port}/hex/up" };
                        Logger.LogDebug($"# {msg.Topic} 收到 \r\n{msg.Payload.ByteToHexStr()}，但数据不全，等待下次接收。");
                        return; //长度不够或格式不符
                    }
                    else
                    {
                        //分包的数据已
                        data = ReceiveBytes.Take(fLen).ToArray();
                        if (ReceiveBytes.Count == fLen)
                            ReceiveBytes.Clear();
                        else
                            ReceiveBytes.RemoveRange(0, fLen);

                    }
                }
                msg = new UniMessageBase { Payload = data, Topic = $"/mingdtu/uart/{UARTConfig.Port}/hex/up" };
                Logger.LogDebug($"# {msg.Topic} 收到 \r\n{msg.Payload.ByteToHexStr()}");//接收完整后再发送一次全部收到的数据，前面粘包处理时发送每次收到的

                if (OnDataForwardTo != null)
                {
                    //单独线程中去转发，不阻塞
                    var t2 = Task.Run(() =>
                    {
                        OnDataForwardTo(UARTConfig, msg);
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
                        OnUniMessageReceived(UARTConfig, msg);
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"{UARTConfig.Port}数据解析时出错!");
            }

        }

        public async Task<IotResult<UniMessageBase>> SendAsync(byte[] data, string title = "")
        {
            IotResult<UniMessageBase> rs = null;
            try
            {
                if (sp == null || !sp.IsOpen)
                {
                    await TryOpenAsync();
                }
                if (sp != null && sp.IsOpen)
                {
                    Logger.LogDebug($"# {title} 发送 \r\n{data.ByteToHexStr()}");
                    sp.Write(data, 0, data.Length);
                    await Task.Delay(30);
                    rs = new IotResult<UniMessageBase>(true, "发送成功");
                    return await Task.FromResult(rs);
                }
                else
                {
                    Logger.LogWarning($"{UARTConfig.Port}没有打开!不能发送数据!");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"{UARTConfig.Port}发送数据出错!");
            }
            rs = new IotResult<UniMessageBase>(false, "发送失败");
            return await Task.FromResult(rs);
        }
        public async Task<IotResult<UniMessageBase>> SendAndWaitReply(byte[] data, string title = "", int waitTimeout = 3000)
        {
            resultCompletionSource = new TaskCompletionSource<IotResult<UniMessageBase>>();
            #region 定义超时对象
            var ct = new CancellationTokenSource(waitTimeout);
            ct.Token.Register(() => resultCompletionSource.TrySetCanceled(), useSynchronizationContext: false);
            #endregion
            try
            {
                if (sp == null || !sp.IsOpen)
                {
                    await TryOpenAsync();//这里不用考虑，取消异步等待，因为的下面的代码会处理
                }
                if (sp != null && sp.IsOpen)
                {
                    Logger.LogDebug($"# {title} 发送 \r\n{data.ByteToHexStr()}");
                    sp.Write(data, 0, data.Length);
                    await Task.Delay(30);
                    return await resultCompletionSource.Task;
                }
                else
                {
                    //此时窜口未打开，则直接取消异步等待
                    Logger.LogWarning($"{UARTConfig.Port}没有打开!不能发送数据!");
                    var rs = new IotResult<UniMessageBase>(false, $"窜口{UARTConfig.Port}没有打开!不能发送数据!") { Result = null };
                    resultCompletionSource.SetResult(rs);
                }
            }
            catch (Exception ex)
            {
                //此时应意外原因通讯出错，则直接终止
                Logger.LogError(ex, $"{UARTConfig.Port}发送数据出错：{ex.Message}({ex.Source})");
                var rs = new IotResult<UniMessageBase>(false, $"窜口{UARTConfig.Port}发送数据出错{ex.Message}({ex.Source})。") { Result = null };
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
            return await SendAsync(msg.Payload, msg.ClientId);
        }

        public void Dispose()
        {
            if (sp != null && sp.IsOpen)
            {
                //已经打开了。
                sp.Dispose();
            }
            sp = null;
        }


    }
}
