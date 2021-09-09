using Aming.Core;
using Aming.DTU;
using Aming.DTU.Config;
using HengDa.LiZongMing.REAMS.Devices.Dtos;
using HengDa.LiZongMing.REAMS.Wpf.Dto;
using HengDa.LiZongMing.REAMS.Wpf.IotFrames;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

 namespace HengDa.LiZongMing.REAMS.CtrlServices
{
    /// <summary>
    /// 感雨器通讯类
    /// </summary>
    public class CtrlHDRNSService : ICtrlService, ISingletonDependency
    {
        private ILoggerFactory loggerFactory;
        private ILogger Logger;



        /// <summary>
        /// 设备信息
        /// </summary>
        public DeviceDto Device { get; set; }
        /// <summary>
        /// 当前通讯方式对象
        /// </summary>
        public IDataEndPoint MyDataEndPoint { get; set; } = null;


        /// <summary>
        /// 将本端数据转发到别的端口的处理程序
        /// </summary>
        public event ReceivedUniMessageCallBack OnDataForwardTo;

        /// <summary>
        /// 将消息接收的事件传递到更多的地方处理，主要是
        /// </summary>
        public event ReceivedUniMessageCallBack OnUniMessageDataReceived;

        public CtrlHDRNSService(ILoggerFactory loggerFactory, string jsonConfig)
        {
            IDataEndPoint config = null;
            if (jsonConfig.Contains("MqttType"))
                config = JsonConvert.DeserializeObject<MqttClientConfig>(jsonConfig);
            else
                config = JsonConvert.DeserializeObject<UARTConfig>(jsonConfig);
            ////TODO：其它的请补全
            Init(loggerFactory, config);
        }
        public CtrlHDRNSService(ILoggerFactory loggerFactory, IDataEndPoint config)
        {
            Init(loggerFactory, config);
        }



        public CtrlHDRNSService Init(ILoggerFactory loggerFactory, IDataEndPoint config)
        {

            this.loggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger<CtrlHDRNSService>();

            this.MyDataEndPoint = config;
            var tsObj = ServiceFactory.Create(loggerFactory, config);
            tsObj.funcCheckPackFormat = SerialPortService.CheckPackFormatByModbus;//定义Modbus下的粘包处理
            tsObj.OnUniMessageReceived += OnUniMessageReceived;
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
            //TODO: 请在这里添加处理代码
            Logger.LogError("请在这里 CtrlHDZCQService.PortDataReceived添加处理代码");

            if (OnDataForwardTo != null && !TranService.Config.ForwardToTopic.IsNullOrWhiteSpace())
            {
                //单独线程中去转发到mqtt，不阻塞
                var t = Task.Run(() =>
                {

                    OnDataForwardTo(TranService.Config, msg);
                });
            }
        }

        public async Task<IotResult<RNSBaseDto>> CmdExec(ushort iMacNo,HDRNSCmdCode cmd, byte[] data)
        {
            //传出一个ZCQ设备Dto对象
            IotResult<UniMessageBase> _cmdResult = null;
            string title = EnumHelper.GetEnumDescription<HDRNSCmdCode>(cmd);
            HDRNSFrame f = (HDRNSFrame)new HDRNSFrame().BuildFram(iMacNo, (byte)cmd, data);
            RNSBaseDto dtoResult = new RNSBaseDto();
            try
            {
                _cmdResult = await TranService.SendAndWaitReply(f.Bytes, title, 30000);
            }
            catch(Exception ex)
            {
                _cmdResult = new IotResult<UniMessageBase>(false, $"通讯出错：{ex.Message}({ex.Source})");
            }
            //返回DTO结果
            IotResult<RNSBaseDto> _dataReturn = new IotResult<RNSBaseDto>(_cmdResult.Ok, _cmdResult.Message);
            if (_cmdResult.Ok)
            {
                //此时正常，则开始解析数据
                f.DecodeFrame(_cmdResult.Result.Payload);
                _dataReturn.Result = f.DtoData;//此时读取解析后的DTO对象
            }
            else
            {
                //此时通讯出错，则返回一个出错的DTO
                _dataReturn.Result = new RNSBaseDto()
                {
                    ErrMsg = _cmdResult.Message,
                    Sucessfully = false,
                    Cmd = cmd
                };
            }
            return _dataReturn;
        }

        public void SendUniMessage(UniMessageBase msg)
        {
            if (msg.Topic == this.TranService.Config.ForwardFromTopic)
            {
                this.TranService.SendUniMessage(msg);
            }
        }
    }
}
