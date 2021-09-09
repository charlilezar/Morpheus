using Aming.Core;
using Aming.DTU;
using Aming.DTU.Config;
using HengDa.LiZongMing.REAMS.Devices.Dtos;
using HengDa.LiZongMing.REAMS.Wpf.Dto;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace HengDa.LiZongMing.REAMS.CtrlServices
{
    /// <summary>
    /// 控制转发专用的mqtt服务器连接
    /// </summary>
    public class CtrlForwardMqttServer : ICtrlService, ISingletonDependency
    {
        private ILoggerFactory loggerFactory;
        private ILogger Logger;


        /// <summary>
        /// 将本端数据转发到别的端口的处理程序
        /// </summary>
        public event ReceivedUniMessageCallBack OnDataForwardTo;

        /// <summary>
        /// 将消息接收的事件传递到更多的地方处理，主要是
        /// </summary>
        public event ReceivedUniMessageCallBack OnUniMessageDataReceived;

        /// <summary>
        /// 当前通讯方式对象
        /// </summary>
        public IDataEndPoint MyDataEndPoint { get; set; } = null;
        /// <summary>
        /// 气碘设备信息
        /// </summary>
        public DeviceDto Device { get; set; }


        public CtrlForwardMqttServer(ILoggerFactory loggerFactory, string jsonConfig)
        {
            IDataEndPoint config = null;
            if (jsonConfig.Contains("MqttType"))
                config = JsonConvert.DeserializeObject<MqttClientConfig>(jsonConfig);
            else
                config = JsonConvert.DeserializeObject<UARTConfig>(jsonConfig);
            ////TODO：其它的请补全
            Init(loggerFactory, config);
        }
        public CtrlForwardMqttServer(ILoggerFactory loggerFactory, IDataEndPoint config)
        {
            //  Init(loggerFactory, config);
            this.loggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger<CtrlForwardMqttServer>();
        }
        public CtrlForwardMqttServer Init(ILoggerFactory loggerFactory, IDataEndPoint config)
        {
            this.MyDataEndPoint = config;
            var tsObj = ServiceFactory.Create(loggerFactory, config);
            //tsObj.funcCheckPackFormat = SerialPortService.CheckPackFormatByModbus;//定义Modbus下的粘包处理
            tsObj.OnUniMessageReceived += OnUniMessageReceived;
            this.TranService = tsObj;
            var t = Task.Run(async () =>
            {
                bool ok = await tsObj.TryOpenAsync();
            });
            Task.WaitAll(new Task[] { t }, 25000);//等他结束
            return this;

        }

        private void TsObj_OnDataForwardTo(IDataEndPoint config, UniMessageBase msg)
        {
            //TODO Forward has done,next oper

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
            if (OnUniMessageDataReceived != null)
            {
                Task.Run(() =>
                {
                    OnUniMessageDataReceived(TranService.Config, msg);
                });

            }

            if (OnDataForwardTo != null)
            {
                //单独线程中去转发到mqtt，不阻塞
                var t = Task.Run(() =>
                {

                    OnDataForwardTo(TranService.Config, msg);
                });
            }

        }

        public void SendUniMessage(UniMessageBase msg)
        {
            //if (msg.Topic == this.TranService.Config.ForwardFromTopic)
            {
                this.TranService.SendUniMessage(msg);
            }
        }
    }
}
