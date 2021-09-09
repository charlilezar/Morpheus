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
using RSNetDevice;
using RSNetDevice.Data;
using RSNetDevice.Model;
using Volo.Abp.Threading;
using Newtonsoft.Json;
using HengDa.LiZongMing.REAMS.Devices.Dtos;
using Aming.Tools;

namespace HengDa.LiZongMing.REAMS.CtrlServices
{
    /// <summary>
    /// 动环监测站通讯类
    /// </summary>
    public class CtrlHDSSService : ICtrlService, ISingletonDependency
    {
        private ILoggerFactory loggerFactory;
        private ILogger Logger;
        private RSServer _rSServer;
        private SSRealDataDto _sSRealDataDto;
        private UniMessageBase _uniMessage;
        /// <summary>
        /// 当前通讯方式对象
        /// </summary>
        public IDataEndPoint MyDataEndPoint { get; set; } = null;

        public int WaitTimeOUt { get; set; } = 30000;
        public SSBaseDto LastResponseData { get; set; }


        /// <summary>
        /// 将本端数据转发到别的端口的处理程序
        /// </summary>
        public event ReceivedUniMessageCallBack OnDataForwardTo;

        /// <summary>
        /// 将消息接收的事件传递到更多的地方处理，主要是
        /// </summary>
        public event ReceivedUniMessageCallBack OnUniMessageDataReceived;

        public Action<MqttClientConfig, UniMessageBase, SSBaseDto> OnUniMessageReceivedCallBack { get; set; } = null;
        public CtrlHDSSService(ILoggerFactory loggerFactory,
            IDataEndPoint config,
            SSRealDataDto sSRealDataDto,
            DeviceDto Device,
           UniMessageBase uniMessageBase)
        //  ZcqRunStatus zcqRunStatus)
        {
            this.Device = Device;
            this.loggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger<CtrlHDSSService>();
            this._sSRealDataDto = sSRealDataDto;
            this._uniMessage = uniMessageBase;
            //this._ZcqRunStatus = zcqRunStatus;
            //Init(loggerFactory,config);
        }



        public CtrlHDSSService Init(ILoggerFactory loggerFactory, IDataEndPoint config)
        {


            switch (config.EndPointType)
            {
                case EndPointEnum.TCP:
                    {
                        var tsObj = ServiceFactory.Create(loggerFactory, config);
                        tsObj.funcCheckPackFormat = this.CheckPackFormat;//定义粘包处理检查类
                        tsObj.OnUniMessageReceived += OnUniMessageReceived;
                        TranService = tsObj;
                        var t = Task.Run(async () =>
                        {
                            await RSInit((TcpClientConfig)config);
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





        #region rsServe 初始化
        private async Task RSInit(TcpClientConfig config)
        {
            await Task.Run(() =>
            {
                try
                {
                    _rSServer = RSServer.Initiate(config.ServerIP, config.ServerPort);//初始化
                    _rSServer.OnReceiveLoginData += RsServer_OnReceiveLoginData;//登录帧数据接收处理
                    _rSServer.OnReceiveRealtimeData += RsServer_OnReceiveRealtimeData;//实时数据接收处理
                    _rSServer.OnReceiveStoreData += RsServer_OnReceiveStoreData;//存储数据接收处理
                    _rSServer.OnTelecontrolAck += RsServer_OnTelecontrolAck;//遥控指令应答处理
                    _rSServer.OnTimmingAck += RsServer_OnTimmingAck;//校时命令应答处理
                    _rSServer.OnReceiveParamIds += RsServer_OnReceiveParamIds;//接收设备参数列表处理
                    _rSServer.OnReceiveParam += RsServer_OnReceiveParam;//接收设备参数处理
                    _rSServer.OnReceiveWriteParamAck += RsServer_OnReceiveWriteParamAck;//下载参数指令应答处理
                    _rSServer.OnReceiveTransDataAck += RsServer_OnReceiveTransDataAck;//透传指令应答处理
                    bool res = _rSServer.Start();//启动监听服务
                }
                catch (Exception ex)
                {
                    Logger.LogError($"动环监测启动失败:{ex.Message}");
                }

            });
        }
        #endregion
        #region rsServer事件处理

        /// <summary>
        /// 测试遥控指令
        /// </summary>
        public void SendTelecontrol()
        {
            _rSServer.Telecontrol(10000000, 1, 0);
        }

        private void RsServer_OnReceiveLoginData(RSServer server, LoginData data)
        {
            //设备登录指令处理
            Logger.LogInformation("设备登录->设备编号：" + data.DeviceID + "\r\n");
        }
        private void RsServer_OnTimmingAck(RSServer server, TimmingAck ack)
        { //校时指令应答处理
            Logger.LogInformation("校时应答->设备编号：" + ack.DeviceID + "\t执行结果：" + ack.ExecStatus + "\r\n");
        }

        private void RsServer_OnTelecontrolAck(RSServer server, TelecontrolAck ack)
        {//遥控指令应答处理
            Logger.LogInformation("遥控应答->设备编号：" + ack.DeviceID + "\t继电器编号：" + ack.RelayID + "\t执行结果：" + ack.ExecStatus + "\r\n");
            //推送到mqtt
            //TODO

        }

        private void RsServer_OnReceiveStoreData(RSServer server, StoreData data)
        { //存储数据接收处理
            foreach (NodeData ndata in data.NodeList)//遍历节点数据。数据包括网络设备的数据以及各个节点数据。温湿度数据存放在节点数据中
            {
                string str = "存储数据->设备编号：" + data.DeviceID + "\t节点编号：" + ndata.NodeID + "\t温度：" + ndata.Tem + "\t湿度：" + ndata.Hum + "\t时间：" + ndata.RecordTime.ToString("yyyy-MM-dd HH:mm:ss") + "\t坐标类型：" + ndata.CoordinateType.ToString() + "\t经度：" + ndata.Lng + "\t维度：" + ndata.Lat;
                Logger.LogInformation(str + "\r\n");
            }
        }
        private static readonly object receivedProcess = new object();
        private void RsServer_OnReceiveRealtimeData(RSServer server, RealTimeData data)
        {
            //实时数据接收处理
            var porcTask = Task.Run(async () =>
              {
              List<SSRealDataDto> SSRealDataDtoList = new List<SSRealDataDto>();
              AsyncHelper.RunSync(async () => {
                  foreach (NodeData ndataT in data.NodeList)//遍历节点数据。数据包括网络设备的数据以及各个节点数据。温湿度数据存放在节点数据中
                  {
                      try
                      {
                          var ndata = ndataT;
                          string str = "实时数据->设备编号：" + data.DeviceID +
                          "\t经度：" + data.Lng +
                          "\t纬度：" + data.Lat +
                          "\t坐标类型：" + data.CoordinateType.ToString() +
                          "\t节点编号：" + ndata.NodeID +
                            "\t温度：" + ndata.Tem +
                            "\t湿度：" + ndata.Hum +
                            "\t继电器状态：" + data.RelayStatus +
                            "\t浮点型数据：" + ndata.FloatValue +
                            "\t32位有符号数据：" + ndata.SignedInt32Value +
                            "\t32位无符号数据：" + ndata.UnSignedInt32Value;
                          this._sSRealDataDto = new SSRealDataDto();
                          this._sSRealDataDto.DeviceID = data.DeviceID;
                          this._sSRealDataDto.Lng = data.Lng;
                          this._sSRealDataDto.Lat = data.Lat;
                          this._sSRealDataDto.CoordinateType = data.CoordinateType;
                          this._sSRealDataDto.Tem = ndata.Tem;
                          this._sSRealDataDto.Hum = ndata.Hum;
                          this._sSRealDataDto.NodeID = ndata.NodeID;
                          this._sSRealDataDto.RelayStatus = data.RelayStatus;
                          this._sSRealDataDto.FloatValue = ndata.FloatValue;
                          this._sSRealDataDto.SignedInt32Value = ndata.SignedInt32Value;
                          this._sSRealDataDto.UnSignedInt32Value = ndata.UnSignedInt32Value;
                          SSRealDataDtoList.Add(this._sSRealDataDto);
                          Logger.LogInformation($"{ ndata.NodeID } " + str + "\r\n");
                      }
                      catch (Exception ex)
                      {
                          Logger.LogError("请在这里 CtrlSSService.PortDataReceived添加处理代码");
                      }

                  }
              });
                  //保存数据
                  var currentDtoList = SSRealDataDtoList;
                  _uniMessage.ClientId = currentDtoList.FirstOrDefault().DeviceID.ToString();
                  _uniMessage.Topic = TranService.Config.ForwardToTopic;
                  byte[] databytes = System.Text.ASCIIEncoding.ASCII.GetBytes(JsonConvert.SerializeObject(currentDtoList));
                  _uniMessage.Payload = databytes;
                  var currentUniMsg = _uniMessage;
                  var SSBaseDtoList = SSRealDataDtoList;
                  var ssService = IocHelper.ServiceProvider.GetService<HDSSDataService>();
                  await ssService.DataSave(this, currentUniMsg, SSRealDataDtoList);
                  //转发
                  if (OnDataForwardTo != null && !TranService.Config.ForwardToTopic.IsNullOrWhiteSpace())
                  {
                      var tempdto = SSRealDataDtoList;
                      byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(JsonConvert.SerializeObject(tempdto));
                      var msg = new UniMessageBase { Payload = data, Topic = TranService.Config.ForwardToTopic };
                      OnDataForwardTo(TranService.Config, msg);
                      Logger.LogInformation($"send completed ");
                  }

              });
       
        }

        private void RsServer_OnReceiveParamIds(RSServer server, ParamIdsData data)
        { //设备参数列表接收处理            

            string str = "设备参数编号列表->设备编号：" + data.DeviceID + "\t参数总数量：" + data.TotalCount + "\t本帧参数数量：" + data.Count + "\r\n";
            foreach (int paramId in data.PararmIdList)//遍历设备中参数id编号
            {
                str += paramId + ",";
            }
            Logger.LogInformation(str + "\r\n");
        }

        private void RsServer_OnReceiveParam(RSServer server, ParamData data)
        {//设备参数接收处理

            string str = "设备参数->设备编号：" + data.DeviceID + "\r\n";

            foreach (ParamItem pararm in data.ParameterList)
            {
                str += "参数编号：" + pararm.ParamId + "\t参数描述：" + pararm.Description + "\t参数值：" + (pararm.ValueDescription == null ? pararm.Value : pararm.ValueDescription[pararm.Value]) + "\r\n";
            }
            Logger.LogInformation(str + "\r\n");
        }
        private void RsServer_OnReceiveWriteParamAck(RSServer server, WriteParamAck data)
        {//下载设备参数指令应答处理

            string str = "下载设备参数->设备编号：" + data.DeviceID + "\t参数数量：" + data.Count + "\t" + (data.Res ? "下载成功" : "下载失败");
            Logger.LogInformation(str + "\r\n");
        }

        private void RsServer_OnReceiveTransDataAck(RSServer server, TransDataAck data)
        {//透传指令应答处理

            string str = "数据透传->设备编号：" + data.DeviceID + "\t响应结果：" + data.Data + "\r\n字节数：" + data.TransDataLen;
            Logger.LogInformation(str + "\r\n");
        }

        #endregion




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
                _cmdResult = await TranService.SendAndWaitReply(f.Bytes, title, 30000);
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
            if (buff.Count < 10) return 0;//小于3个肯定没接收完；
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
            Logger.LogError("SS还没有写保存代码??");




            //TODO: 请在这里添加处理代码
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
