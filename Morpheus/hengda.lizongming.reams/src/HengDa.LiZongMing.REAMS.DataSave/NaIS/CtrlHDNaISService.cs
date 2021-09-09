using Aming.Core;
using Aming.DTU;
using Aming.DTU.Config;
using HengDa.LiZongMing.REAMS.Wpf.Dto;
using HengDa.LiZongMing.REAMS.Wpf.IotFrames;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using HengDa.LiZongMing.REAMS.Devices;
using RSNetDevice;
using RSNetDevice.Data;
using RSNetDevice.Model;
using HengDa.LiZongMing.REAMS.ZCQ;
using Volo.Abp.Threading;
using Newtonsoft.Json;
using HengDa.LiZongMing.REAMS.Devices.Dtos;

namespace HengDa.LiZongMing.REAMS.CtrlServices
{
    /// <summary>
    /// 碘化钠谱仪通讯类
    /// </summary>
    public class CtrlHDNaISService : ICtrlService, ISingletonDependency
    {
        private ILoggerFactory loggerFactory;
        private ILogger Logger;
        private UniMessageBase _uniMessage;
        /// <summary>
        /// 当前通讯方式对象
        /// </summary>
        public IDataEndPoint MyDataEndPoint { get; set; } = null;

        public int WaitTimeOUt { get; set; } = 30000;
        public BaseDto LastResponseData { get; set; }


        /// <summary>
        /// 将本端数据转发到别的端口的处理程序
        /// </summary>
        public event ReceivedUniMessageCallBack OnDataForwardTo;

        /// <summary>
        /// 将消息接收的事件传递到更多的地方处理，主要是
        /// </summary>
        public event ReceivedUniMessageCallBack OnUniMessageDataReceived;

        public Action<MqttClientConfig, UniMessageBase, SSBaseDto> OnUniMessageReceivedCallBack { get; set; } = null;
        public CtrlHDNaISService(ILoggerFactory loggerFactory, 
            IDataEndPoint config, 
            DeviceDto Device,
           UniMessageBase uniMessageBase)
        {
            this.Device = Device;
            this.loggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger<CtrlHDNaISService>();
            this._uniMessage = uniMessageBase;
        }



        public CtrlHDNaISService Init(ILoggerFactory loggerFactory, IDataEndPoint config)
        {
            switch (config.EndPointType)
            {
                case EndPointEnum.TCP:
                    {
                        var tsObj = ServiceFactory.Create(loggerFactory, config);
                        //tsObj.funcCheckPackFormat = this.CheckPackFormat;//定义粘包处理检查类
                        tsObj.OnUniMessageReceived += OnUniMessageReceived;
                        TranService = tsObj;
                        var t = Task.Run(async () =>
                        {
                            bool ok = await tsObj.TryOpenAsync();
                        });
                        Task.WaitAll(new[] { t }, 25000);//等他结束
                    }
                    this.MyDataEndPoint = config;
                    break;

                default:
                    //throw new NotImplementedException("不支持的方式"); //出错
                    break;
            }
            return this;
        }

        /// <summary>
        /// SerialPortService 对象
        /// </summary>
        public ITransmissionService TranService { get; set; }
        public DeviceDto Device { get; set; }

        /// <summary>
        /// 串口收到消息时触发
        /// </summary>
        /// <param name="config"></param>
        /// <param name="data"></param>
        public void PortDataReceived(UARTConfig config, UniMessageBase uniMessageBase)
        {

        }
        public async Task<IotResult<SSBaseDto>> CmdExec_Normal(ushort iMacNo, HDSSCmdCode cmd, byte[] data)
        {
            //传出一个ZCQ设备Dto对象
            IotResult<UniMessageBase> _cmdResult = null;
            string title = EnumHelper.GetEnumDescription<HDSSCmdCode>(cmd);
            HDSSFrame f = (HDSSFrame)new HDSSFrame().BuildFram(iMacNo, (byte)cmd, data);
            SSBaseDto dtoResult = new SSBaseDto();
            try
            {
               // _cmdResult = await TranService.SendAndWaitReply(f.Bytes, title, 30000);
                _cmdResult = await TranService.SendAsync(f.Bytes);
            }
            catch (Exception ex)
            {
                _cmdResult = new IotResult<UniMessageBase>(false, $"通讯出错：{ex.Message}({ex.Source})");
            }
            //返回DTO结果
            IotResult<SSBaseDto> _dataReturn = new IotResult<SSBaseDto>(_cmdResult.Ok, _cmdResult.Message);
            if (_cmdResult.Ok)
            {
                //此时正常，则开始解析数据
                f.DecodeFrame(_cmdResult.Result.Payload);
                _dataReturn.Result = f.DtoData;//此时读取解析后的DTO对象
                                               // _dataReturn.Result.SourceData = _cmdResult.Result.Payload;
            }
            else
            {
                //此时通讯出错，则返回一个出错的DTO
                _dataReturn.Result = new SSBaseDto()
                {
                    ErrMsg = _cmdResult.Message,
                    Sucessfully = false,
                    Cmd = cmd
                };
            }
            return _dataReturn;
        }
        public async Task<IotResult<SSBaseDto>> CmdExec_SourceData(string sTitle, byte[] data)
        {
            //传出一个ZCQ设备Dto对象
            IotResult<UniMessageBase> _cmdResult = null;
            SSBaseDto dtoResult = new SSBaseDto();
            try
            {
                _cmdResult = await TranService.SendAndWaitReply(data, sTitle, 30000);
            }
            catch (Exception ex)
            {
                _cmdResult = new IotResult<UniMessageBase>(false, $"通讯出错：{ex.Message}({ex.Source})");
            }
            //返回DTO结果
            IotResult<SSBaseDto> _dataReturn = new IotResult<SSBaseDto>(_cmdResult.Ok, _cmdResult.Message);
            if (_cmdResult.Ok)
            {
                HDSSFrame f = new HDSSFrame();
                //此时正常，则开始解析数据
                f.DecodeFrame(_cmdResult.Result.Payload);
                _dataReturn.Result = f.DtoData;//此时读取解析后的DTO对象
                //_dataReturn.Result.SourceData = _cmdResult.Result.Payload;
            }
            else
            {
                //此时通讯出错，则返回一个出错的DTO
                _dataReturn.Result = new SSBaseDto()
                {
                    ErrMsg = _cmdResult.Message,
                    Sucessfully = false,
                    Title = sTitle
                };
            }
            return _dataReturn;
        }

        #region 添加粘包处理

        /// <summary>
        /// 高压电离室的xml数据包，前面有5个头字节，每一个固定，后4字节是长度，再后面是xml的标准字符内容
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        public int CheckPackFormat(List<byte> buff)
        {
            if (buff.Count < 9) return 0;//小于3个肯定没接收完；
            if (240 != buff[0])
            {
                //Logger.LogDebug("收到的高压电离室的数据包没有前面的5个头字节");
                return 0;
            }
            int length = Aming.Core.StringHelper.BitToInt(buff.ToArray(), 1, 4);
            if (buff.Count >= length + 5)
            {
                //有完整内容
                return buff.Count;
            }
            //其他方式是非预期的，则直接都返回接受完了；
            return 0;
        }
        #endregion
        public async Task<IotResult<UniMessageBase>> CmdExec(HDSSCmdCode cmd, byte[] data = null)
        {
            IotResult<UniMessageBase> _cmdResult = null;
            string title = EnumHelper.GetEnumDescription<HDSSCmdCode>(cmd);
            HDSSFrame f = (HDSSFrame)new HDSSFrame().BuildFram((byte)cmd, data);
            return await CmdExecRaw(title, f.Bytes);
        }

        public async Task<IotResult<UniMessageBase>> CmdExecWaitReply(HDSSCmdCode cmd, byte[] data = null)
        {
            //传出一个ZCQ设备Dto对象
            string title = EnumHelper.GetEnumDescription<HDSSCmdCode>(cmd);
            HDSSFrame f = (HDSSFrame)new HDSSFrame().BuildFram((byte)cmd, data);
            return await CmdExecRawWaitReply(title, f.Bytes);
        }

        public async Task<IotResult<UniMessageBase>> CmdExecRawWaitReply(string title, byte[] data)
        {
            IotResult<UniMessageBase> _cmdResult = null;
            try
            {
                _cmdResult = await TranService.SendAndWaitReply(data, title);
                //_cmdResult = await TranService.SendAndWaitReply(f.Bytes, title, WaitTimeout);
            }
            catch (Exception ex)
            {
                _cmdResult = new IotResult<UniMessageBase>(false, $"通讯出错：{ex.Message}({ex.Source})");
            }
            return _cmdResult;
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



        /// <summary>
        /// 串口收到消息时触发
        /// </summary>
        /// <param name="config"></param>
        /// <param name="data"></param>
        public void OnUniMessageReceived(IDataEndPoint config, UniMessageBase msg)
        {
            //此时正常，则开始解析数据
            //F0 03 05 01 B1 00 FF 01 E6 D6 □ 测试数据
            HDNaISFrame f = (HDNaISFrame)new HDNaISFrame();
            f.DecodeFrame(msg.Payload);
            if (f.DtoData == null) return;
            this.LastResponseData = f.DtoData;
            try
            {
                //保存数据
                Task.Run(async () => { 
                var _serviceProvider = Aming.Tools.IocHelper.ServiceProvider;
                var  naIService = _serviceProvider.GetService<HDNaISDataService>();
                await  naIService.DataSave(this, msg, LastResponseData);
                });
            }
            catch (Exception ex)
            {
                Logger.LogError("请在这里 CtrlHDNSService.PortDataReceived添加处理代码");
            }
            if (OnDataForwardTo != null && !TranService.Config.ForwardToTopic.IsNullOrWhiteSpace())
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
            if (msg.Topic == this.TranService.Config.ForwardFromTopic)
            {
                this.TranService.SendUniMessage(msg);
            }
        }


    }
}
