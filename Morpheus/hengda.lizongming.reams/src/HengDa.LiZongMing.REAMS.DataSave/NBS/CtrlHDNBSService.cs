using Aming.Core;
using Aming.DTU;
using Aming.DTU.Config;
using HengDa.LiZongMing.REAMS.Devices.Dtos;
using HengDa.LiZongMing.REAMS.NaI;
using HengDa.LiZongMing.REAMS.NBS;
using HengDa.LiZongMing.REAMS.Wpf.Dto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace HengDa.LiZongMing.REAMS.CtrlServices
{
    /// <summary>
    /// 控制超大容量采样器的具体硬件的类，方法是一些组合步骤
    /// </summary>
    public class CtrlHDNBSService : ICtrlService, ISingletonDependency
    {
        #region 可配置变量
        /// <summary>
        /// 将实时数据保存至历史数据的时间间隔
        /// </summary>
        public int RunStatusSaveHisInteralSeconds { get; set; } = 60;
        /// <summary>
        /// 设备站点号，标号设备在网络中的地址，具有同一网络内唯一性，当前协议通讯过程需要确认该值，默认为1
        /// 后期可以从数据库配置中读取
        /// </summary>
        public byte MacAdrNo { get; set; } = 1;
        #endregion
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
        
        public NBSBaseDto LastResponseData { get; set; }
        /// <summary>
        /// 将本端数据转发到别的端口的处理程序
        /// </summary>
        public event ReceivedUniMessageCallBack OnDataForwardTo;
        /// <summary>
        /// 将消息接收的事件传递到更多的地方处理，主要是
        /// </summary>
        public event ReceivedUniMessageCallBack OnUniMessageDataReceived;
        public Action<MqttClientConfig, UniMessageBase, NBSBaseDto> OnUniMessageReceivedCallBack { get; set; } = null;

        public CtrlHDNBSService(ILoggerFactory loggerFactory, string jsonConfig)
        {
            IDataEndPoint config = null;
            if (jsonConfig.Contains("MqttType"))
                config = JsonConvert.DeserializeObject<MqttClientConfig>(jsonConfig);
            else
                config = JsonConvert.DeserializeObject<UARTConfig>(jsonConfig);
            ////TODO：其它的请补全
            Init(loggerFactory, config);
        }
        public CtrlHDNBSService(ILoggerFactory loggerFactory, IDataEndPoint config)
        {
            Init(loggerFactory, config);
        }
        public CtrlHDNBSService Init(ILoggerFactory loggerFactory, IDataEndPoint config)
        {
            this.loggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger<CtrlHDNBSService>();

            this.MyDataEndPoint = config;
            var tsObj = ServiceFactory.Create(loggerFactory, config);
            tsObj.funcCheckPackFormat = SerialPortService.CheckPackFormatBy680D;
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
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<IotResult<UniMessageBase>> CmdExec(HDNBSCmdCode cmd, byte[] data = null)
        {
            //传出一个ZCQ设备Dto对象
            string title = EnumHelper.GetEnumDescription<HDNBSCmdCode>(cmd);
            HDNBSFrame f = (HDNBSFrame)new HDNBSFrame(this.MacAdrNo).BuildFram((byte)cmd, data);
            return await CmdExecRaw(title, f.Bytes);
        }
        public async Task<IotResult<NBSBaseDto>> CmdExec_Normal(ushort iMacNo, HDNBSCmdCode cmd, byte[] data)
        {
            //传出一个NBS设备Dto对象
            IotResult<UniMessageBase> _cmdResult = null;
            string title = EnumHelper.GetEnumDescription<HDNBSCmdCode>(cmd);
            HDNBSFrame f = (HDNBSFrame)new HDNBSFrame((byte)iMacNo).BuildFram((byte)cmd, data);
            NBSBaseDto dtoResult = new NBSBaseDto();
            try
            {
                _cmdResult = await TranService.SendAndWaitReply(f.Bytes, title);
            }
            catch(Exception ex)
            {
                _cmdResult = new IotResult<UniMessageBase>(false, $"通讯出错：{ex.Message}({ex.Source})");
            }
            //返回DTO结果
            IotResult<NBSBaseDto> _dataReturn = new IotResult<NBSBaseDto>();
            if (_cmdResult.Ok)
            {
                //此时正常，则开始解析数据
                f.DecodeFrame(_cmdResult.Result.Payload);
                _dataReturn.Result = f.DtoData;//此时读取解析后的DTO对象
            }
            else
            {
                //此时通讯出错，则返回一个出错的DTO
                _dataReturn.Result = new NBSBaseDto()
                {
                    ErrMsg = _cmdResult.Message,
                    Sucessfully = false,
                    Cmd = cmd
                };
            }
            return _dataReturn;
        }
        public async Task<IotResult<UniMessageBase>> CmdExecRaw(string title, byte[] data)
        {
            IotResult<UniMessageBase> _cmdResult = null;
            try
            {
                _cmdResult = await TranService.SendAsync(data, title);
            }
            catch (Exception ex)
            {
                _cmdResult = new IotResult<UniMessageBase>(false, $"通讯出错：{ex.Message}({ex.Source})");
            }
            return _cmdResult;
        }

        public void SendUniMessage(UniMessageBase msg)
        {
            if (msg.Topic == this.TranService.Config.ForwardFromTopic)
            {
                this.TranService.SendUniMessage(msg);
            }
        }
        #region 自动采集超大数据
        #region 公共属性
        /// <summary>
        /// 当前设备信息对象，其他对象可能回来读取该值
        /// </summary>
        public string DeviceDesc
        {
            get
            {
                return Device == null ? "device is empty" : $"{Device.ProductName},{Device.SN}";
            }
        }
        #endregion
        #region 数据采集过程
        public int DynamicTaskDelay = 3000;
        byte[] _Datas = null;
        int HDNBSCmdIndex = -1;
        HDNBSCmdCode[] HDNBSCmdCodes;
        private HDNBSCmdCode GetCurrentCmd()
        {
            if (this.HDNBSCmdCodes == null || this.HDNBSCmdCodes.Length == 0)
            {
                HDNBSCmdCodes = new HDNBSCmdCode[] { HDNBSCmdCode.瞬时数据查询, HDNBSCmdCode.报警记录查询,
                    HDNBSCmdCode.设定值查询, HDNBSCmdCode.运行状态查询,HDNBSCmdCode.NBS剩余时间,HDNBSCmdCode.NBS剩余体积
                    , HDNBSCmdCode.文件号查询,HDNBSCmdCode.读取文件
                };
            }
            this.HDNBSCmdIndex++;//递增1
            if (HDNBSCmdIndex < 0 || HDNBSCmdIndex >= this.HDNBSCmdCodes.Length)
                HDNBSCmdIndex = 0;
            if (this._FileNoReading == 0 && this.HDNBSCmdCodes[this.HDNBSCmdIndex] == HDNBSCmdCode.读取文件)
            {
                //此时文件号为空，无法读取文件，则跳过
                this.HDNBSCmdIndex++;
                return GetCurrentCmd();
            }
            if (this.HDNBSCmdCodes[this.HDNBSCmdIndex] == HDNBSCmdCode.读取文件)
            {
                //此时为读取文件，则参数定义成文件内容
                this._Datas = StringHelper.ShortToBytes(this._FileNoReading);
            }
            else
            {
                if (this._Datas != null && this._Datas.Length > 0)
                    this._Datas = new byte[0];
            }
            return this.HDNBSCmdCodes[this.HDNBSCmdIndex];
        }
        /// <summary>
        /// 开始超大容量气溶胶采样设备数据采集
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="device"></param>
        public async void StartAcquireAsync(DeviceDto device)
        {
            /*************
             
             * *************************/
            this.Device = device;//存储设备唯一标识，后面上传数据会用到
            //初始化设备连接
            var config = device.GetConfig();
            this._FileNoReading = 1;//初始化文件的读取序号
            while (true)
            {
                //确保端口打开，如果已经打开，TryOpenAsync直接会返回，不会重复去打开
                if (!await this.TranService.TryOpenAsync())
                {
                    //添加日志，错误内容TryOpenAsync已经写入了
                    Logger.LogDebug($"设备[{this.DeviceDesc}]打开失败。");
                    await Task.Delay(3000);//如果设备连接失败，可能是配置错了，也可能是设备未连接，休眠一下再尝试打开
                    continue;
                }
                HDNBSCmdCode cmd = this.GetCurrentCmd();
                //开始采集
                await this.CmdExec(cmd, this._Datas);
                await Task.Delay(this.DynamicTaskDelay);
            }
        }
        #endregion
        #region 同步文件记录
        /// <summary>
        /// 存储数据库中当前租户设备的最新的启示时间，要通过这个来判断数据库更新到哪里了
        /// </summary>
        DateTime? _MaxStartTimeInDB = null;
        public void UpdateMaxStartTimeInDB(DateTime? detTime, IServiceScope scope = null)
        {
            if (detTime != null)
                this._MaxStartTimeInDB = detTime;
            else
            {
                //此时从数据库更新
                if (scope == null)
                {
                    this.Logger.LogError("插入的IServiceScope为空，无法读取数据库最新的文件起始时间");
                    return;
                }
                NbsRecord NBSRecord;
                try
                {
                    INbsRecordRepository NBSRecords = scope.ServiceProvider.GetService<INbsRecordRepository>();
                    var query = NBSRecords.Where(m => m.DeviceId == this.Device.Id && m.StartTime.Length > 0)
                        .WhereIf(this.Device.TenantId != null, m => m.TenantId == this.Device.TenantId).OrderByDescending(m => m.StartTime);
                    NBSRecord = query.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, "读取数据库最大测试时间时出错。");
                    return;
                }
                if (NBSRecord != null)
                {
                    DateTime detStartTime;
                    if (DateTime.TryParse(NBSRecord.StartTime, out detStartTime))
                        this._MaxStartTimeInDB = detStartTime;
                }
                else
                {
                    //此时数据库里没有任何数据，则要全部从设备中同步
                    this._MaxStartTimeInDB = DateTime.MinValue;
                }
            }
        }
        public bool IsFileSave2Db(NBSFileInfoDto dto)
        {
            if (dto == null) return false;
            if (this._FileNoReading <= 0) return false;
            if (this._FileNoReading != dto.FileNo) return false;
            return true;
        }
        /// <summary>
        /// 设备中最大文件号
        /// </summary>
        int _MaxFileNo = 0;
        /// <summary>
        /// 读取文件的记录的文件号，读取时从1网上递增
        /// </summary>
        short _FileNoReading = 0;
        /// <summary>
        /// 当前已经保存的文件
        /// </summary>
        public bool NextFileInfoSaveCompeleted(NbsRecord record, IServiceScope scope)
        {
            if (record == null) return false;
            if (this._MaxStartTimeInDB == null)
            {
                this.UpdateMaxStartTimeInDB(null, scope);//从数据库更新已经存储的最新时间
            }
            if (this._FileNoReading != (short)record.FileNo) return false;//不是本软件发起的命令不用理会
            DateTime detDtoStarTime;
            if (this._MaxStartTimeInDB != null && DateTime.TryParse(record.StartTime, out detDtoStarTime))
            {
                if (this._MaxStartTimeInDB >= detDtoStarTime)
                {
                    //此时比较一下时间，如果已经传入的DTO时间要早于或等于数据库的最大时间，说明后面要查的都在数据库中了，则不用再继续循环下去了。
                    this._FileNoReading = 0;
                    return true;
                }
            }
            //此时还未到达设备最大序号的情况下我们继续往上递增，否则超过了，就不用了
            if (this._MaxFileNo > 0 && this._FileNoReading >= this._MaxFileNo)
            {
                this._FileNoReading = 0;
                return true;
            }
            else
            {
                this._FileNoReading++;
                return false;
            }
        }
        #endregion
        #region 数据反馈相应
        public void OnUniMessageReceived(IDataEndPoint config, UniMessageBase msg)
        {
            //此时正常，则开始解析数据
            HDNBSFrame f = (HDNBSFrame)new HDNBSFrame(this.MacAdrNo);
            f.DecodeFrame(msg.Payload);
            this.LastResponseData = f.DtoData;
            try
            {
                //保存数据
                var _serviceProvider = Aming.Tools.IocHelper.ServiceProvider;
                HDNBSDataService s = _serviceProvider.GetService<HDNBSDataService>();
                s.DataSave(this, msg, f.DtoData);
            }
            catch (Exception ex)
            {
                Logger.LogError("请在这里 CtrlHDNBSService.PortDataReceived添加处理代码");
            }
            //处理文件号查询
            if (f.DtoData is NBSFileNoDto dtoFileNo) this.ProFileNoDto(dtoFileNo);
            if (OnUniMessageReceivedCallBack != null) //传统的事件触发传出去
            {
                //var msg1 = new UniMessageBase { Payload = data, Topic = $"/DTU/{Config.Port}/from" };
                OnUniMessageReceivedCallBack(config as MqttClientConfig, msg, f.DtoData);
            }
        }
        public void ProFileNoDto(NBSFileNoDto dtoFileNo)
        {
            if (!dtoFileNo.Sucessfully) return;
            if (this._MaxFileNo != dtoFileNo.MaxFileNo)
            {
                //此时设备文件最大序号发生了改变，则可以认为完成了新的测试；
                //当然在设备中把最大文件号的文件删除了，估计也可以改变，但这个不影响数据保存
                if (this._FileNoReading == 0)
                {
                    //此时设备已经新增了设备文件，则要读取文件
                    this._FileNoReading = 1;
                    this._MaxStartTimeInDB = null;//这里要重新开启轮询，因为不清楚，是否中间错过了N次检测；
                    this.Logger.LogDebug($"设备中最大文件号从{this._MaxFileNo}改为{dtoFileNo.MaxFileNo}，软件启动历史文件读取。");
                    this._MaxFileNo = dtoFileNo.MaxFileNo;//更新最大文件号
                }
                else
                {
                    //TODO：补充说明，为什么大于0不更新
                    if (this._MaxFileNo == 0)
                        this._MaxFileNo = dtoFileNo.MaxFileNo;
                    //此时不用记录了，下次再获取最大文件号时，再传入进来，直至_FileNoReading!=0时；
                    this.Logger.LogDebug($"设备中最大文件号从{this._MaxFileNo}改为{dtoFileNo.MaxFileNo}，但由于正在读取历史文件记录[{this._FileNoReading}]，所以当前不更新最大文件号。");
                }
            }
        }

        #endregion
        #endregion
    }
}
