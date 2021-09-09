using Aming.Core;
using Aming.DTU;
using Aming.DTU.Config;
using HengDa.LiZongMing.REAMS.Wpf.Dto;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using HengDa.LiZongMing.REAMS.Devices;
using HengDa.LiZongMing.REAMS.ZCQ;
using Volo.Abp.Threading;
using HengDa.LiZongMing.REAMS.Devices.Dtos;
using HengDa.LiZongMing.REAMS.Wpf.IotFrames;

namespace HengDa.LiZongMing.REAMS.CtrlServices
{
    /// <summary>
    /// 控制气象6参的具体硬件的类，方法是一些组合步骤
    /// </summary>
    public class CtrlXPHService : ICtrlService, ISingletonDependency
    {
        #region 可配置变量
        /// <summary>
        /// 将实时数据保存至历史数据的时间间隔
        /// </summary>
        public int RunStatusSaveHisInteralSeconds { get; set; } = 60;
        #endregion
        private ILoggerFactory loggerFactory;
        private ILogger Logger;
        /// <summary>
        /// 气碘设备信息
        /// </summary>
        public DeviceDto Device { get; set; }
        /// <summary>
        /// 当前通讯方式对象
        /// </summary>
        public IDataEndPoint MyDataEndPoint { get; set; } = null;
        /// <summary>
        /// 超时时间
        /// </summary>
        //public int WaitTimeout { get; set; } = 30000;

        //public ZCQBaseDto LastResponseData { get; set; }

        /// <summary>
        /// 事件回调时，收到数据触发解析
        /// </summary>
        public Action<MqttClientConfig, UniMessageBase, ZCQBaseDto> OnUniMessageReceivedCallBack { get; set; } = null;
        public CtrlXPHService(ILoggerFactory loggerFactory, string jsonConfig)
        {
            IDataEndPoint config = null;
            if (jsonConfig.Contains("MqttType"))
                config = JsonConvert.DeserializeObject<MqttClientConfig>(jsonConfig);
            else
                config = JsonConvert.DeserializeObject<UARTConfig>(jsonConfig);
            ////TODO：其它的请补全
            Init(loggerFactory, config);
        }
        public CtrlXPHService(ILoggerFactory loggerFactory, IDataEndPoint config)
        {
            Init(loggerFactory, config);
        }
        public CtrlXPHService(ILoggerFactory loggerFactory, Device device)
        {

            Init(loggerFactory, device.GetConfig());
        }

        /// <summary>
        /// 将本端数据转发到别的端口的处理程序
        /// </summary>
        public event ReceivedUniMessageCallBack OnDataForwardTo ;

        /// <summary>
        /// 将消息接收的事件传递到更多的地方处理，主要是
        /// </summary>
        public event ReceivedUniMessageCallBack OnUniMessageDataReceived;


        /// <summary>
        /// 每个站点号一个等待记录，（考虑到一个串口多个485设备的情况）
        /// </summary>
        public static System.Collections.Concurrent.ConcurrentDictionary<string, XPHFrame> ListServices = new System.Collections.Concurrent.ConcurrentDictionary<string, XPHFrame>();


        public CtrlXPHService Init(ILoggerFactory loggerFactory, IDataEndPoint config)
        {
            this.loggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger<CtrlXPHService>();

            this.MyDataEndPoint = config;
            var tsObj = ServiceFactory.Create(loggerFactory, config);
            //tsObj.funcCheckPackFormat = SerialPortService.CheckPackFormatBy680D;//定义粘包处理检查类
            tsObj.OnUniMessageReceived += OnUniMessageReceived;
            this.TranService = tsObj;
            var t = Task.Run(async () =>
            {
                bool ok = await tsObj.TryOpenAsync();
            });
            Task.WaitAll(new Task[] { t }, 25000);//等他结束
            return this;
        }

        /// <summary>
        /// SerialPortService 对象
        /// </summary>
        public ITransmissionService TranService { get; set; }

        /// <summary>
        /// 串口收到消息时触发
        /// </summary>
        /// <param name="config"></param>
        /// <param name="data"></param>
        public void OnUniMessageReceived(IDataEndPoint config, UniMessageBase msg)
        {
            //此时正常，则开始解析数据
            XPHFrame f = new XPHFrame();
            f.DecodeFrame(msg.Payload);
            try
            {
                //保存数据
                var _serviceProvider = Aming.Tools.IocHelper.ServiceProvider;
                XPHDataService s = _serviceProvider.GetService<XPHDataService>();
                s.DataSave(this, msg, f.DtoDatas);
            }
            catch (Exception ex)
            {
                Logger.LogError("请在这里 CtrlHDZCQService.PortDataReceived添加处理代码");
            }

            //if (OnUniMessageReceivedCallBack != null) //传统的事件触发传出去
            //{
            //    //var msg1 = new UniMessageBase { Payload = data, Topic = $"/DTU/{Config.Port}/from" };
            //    OnUniMessageReceivedCallBack(config as MqttClientConfig, msg, f.DtoDatas);
            //}

            if (OnDataForwardTo != null && !TranService.Config.ForwardToTopic.IsNullOrWhiteSpace())
            {
                //单独线程中去转发到mqtt，不阻塞
                var t = Task.Run(() =>
                {

                    OnDataForwardTo(TranService.Config, msg);
                });
            }

        }

        public async Task<IotResult<UniMessageBase>> CmdExec(XPHCmdCode cmd, byte[] data = null)
        {
            //传出一个ZCQ设备Dto对象
            IotResult<UniMessageBase> _cmdResult = null;
            string title = EnumHelper.GetEnumDescription<XPHCmdCode>(cmd);
            XPHFrame f = (XPHFrame)new XPHFrame().BuildFram((byte)cmd, data);

            return await CmdExecRaw(title, f.Bytes);
            ////返回DTO结果
            //IotResult<ZCQBaseDto> _dataReturn = new IotResult<ZCQBaseDto>();
            //if (_cmdResult.Ok)
        }
      
        public async Task<IotResult<UniMessageBase>> CmdExecRaw(string title, byte[] data)
        {
            IotResult<UniMessageBase> _cmdResult = null;
            try
            {
                _cmdResult = await TranService.SendAsync(data, title);
                //_cmdResult = await TranService.SendAndWaitReply(f.Bytes, title, WaitTimeout);
            }
            catch (Exception ex)
            {
                _cmdResult = new IotResult<UniMessageBase>(false, $"通讯出错：{ex.Message}({ex.Source})");
            }
            return _cmdResult;
        }

        #region 自定义粘包和分包
        /// <summary>
        /// 判断是否接受完整
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        //public int CheckPackFormatBy680D(List<byte> buff)
        //{
        //    if (buff.Count < 4) return 0;//小于2个字节的则认为未传完
        //    int iLen = buff[1];
        //    iLen += 6;
        //    if (buff.Count >= iLen && (buff[0] == 0x68 && buff[3] == 0x68 && buff[iLen - 1] == 0x0D))
        //        return iLen;
        //    return 0;
        //}
        #endregion
        public void SendUniMessage(UniMessageBase msg)
        {
            if (msg.Topic == this.TranService.Config.ForwardFromTopic)
            {
                this.TranService.SendUniMessage(msg);
            }
        }
    }
}
