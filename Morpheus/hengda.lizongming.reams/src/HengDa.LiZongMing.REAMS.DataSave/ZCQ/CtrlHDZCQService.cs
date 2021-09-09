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
namespace HengDa.LiZongMing.REAMS.CtrlServices
{
    /// <summary>
    /// 控制气碘采样器的具体硬件的类，方法是一些组合步骤
    /// </summary>
    public class CtrlHDZCQService : ICtrlService, ISingletonDependency
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
        public int WaitTimeout { get; set; } = 30000;
        public ZCQBaseDto LastResponseData { get; set; }
        /// <summary>
        /// 事件回调时，收到数据触发解析
        /// </summary>
        public Action<MqttClientConfig, UniMessageBase, ZCQBaseDto> OnUniMessageReceivedCallBack { get; set; } = null;
        //public CtrlHDZCQService(ILoggerFactory loggerFactory, string jsonConfig)
        //{
        //    IDataEndPoint config = null;
        //    if (jsonConfig.Contains("MqttType"))
        //        config = JsonConvert.DeserializeObject<MqttClientConfig>(jsonConfig);
        //    else
        //        config = JsonConvert.DeserializeObject<UARTConfig>(jsonConfig);
        //    ////TODO：其它的请补全
        //    Init(loggerFactory, config);
        //}
        public CtrlHDZCQService(ILoggerFactory loggerFactory, IDataEndPoint config)
        {
            Init(loggerFactory, config);
        }
        public CtrlHDZCQService(ILoggerFactory loggerFactory, Device device)
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



        public CtrlHDZCQService Init(ILoggerFactory loggerFactory, IDataEndPoint config)
        {
            this.loggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger<CtrlHDZCQService>();

            this.MyDataEndPoint = config;
            var tsObj = ServiceFactory.Create(loggerFactory, config);
            tsObj.funcCheckPackFormat = SerialPortService.CheckPackFormatBy680D;//定义粘包处理检查类
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

        public async Task<IotResult<UniMessageBase>> CmdExec(HDZCQCmdCode cmd, byte[] data = null)
        {
            //传出一个ZCQ设备Dto对象
            IotResult<UniMessageBase> _cmdResult = null;
            string title = EnumHelper.GetEnumDescription<HDZCQCmdCode>(cmd);
            HDZCQFrame f = (HDZCQFrame)new HDZCQFrame().BuildFram((byte)cmd, data);

            return await CmdExecRaw(title, f.Bytes);
            ////返回DTO结果
            //IotResult<ZCQBaseDto> _dataReturn = new IotResult<ZCQBaseDto>();
            //if (_cmdResult.Ok)
        }
        public async Task<IotResult<UniMessageBase>> CmdExecWaitReply(HDZCQCmdCode cmd, byte[] data = null)
        {
            //传出一个ZCQ设备Dto对象
            string title = EnumHelper.GetEnumDescription<HDZCQCmdCode>(cmd);
            HDZCQFrame f = (HDZCQFrame)new HDZCQFrame().BuildFram((byte)cmd, data);
            return await CmdExecRawWaitReply(title, f.Bytes);
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
        public async Task<IotResult<UniMessageBase>> CmdExecRawWaitReply(string title, byte[] data)
        {
            IotResult<UniMessageBase> _cmdResult = null;
            try
            {
                _cmdResult = await TranService.SendAndWaitReply(data, title);
                //当异步等待时，数据接收函数中未调用当前OnUniMessageReceived，所以在这里调用，并存储数据；
                this.OnUniMessageReceived(this.MyDataEndPoint, _cmdResult.Result);
                //_cmdResult = await TranService.SendAndWaitReply(f.Bytes, title, WaitTimeout);
            }
            catch (Exception ex)
            {
                _cmdResult = new IotResult<UniMessageBase>(false, $"通讯出错：{ex.Message}({ex.Source})");
            }
            return _cmdResult;
        }
        #region 自定义粘包和分包
        ///// <summary>
        ///// 判断是否接受完整
        ///// </summary>
        ///// <param name="buff"></param>
        ///// <returns></returns>
        //public int CheckPackFormatBy680D(List<byte> buff)
        //{
        //    if (buff.Count < 4) return 0;//小于2个字节的则认为未传完
        //    int iLen = buff[1];
        //    iLen += 6;
        //    if (buff.Count >= iLen && (buff[0] == 0x68 && buff[3] == 0x68 && buff[iLen - 1] == 0x0D))
        //        return iLen;
        //    return 0;
        //}
        ///// <summary>
        ///// 将多个粘在一起的包分成单独有效的数据包，这个MQTT通讯上会用到，TCP和串口一般不会出现这种情况
        ///// </summary>
        ///// <param name="bs"></param>
        ///// <returns></returns>
        //public List<byte[]> SplitPackBy680D(byte[] bs)
        //{
        //    List<byte[]> list = new List<byte[]>();
        //    /***********
        //     * 气碘通讯编码规则有个特点就是0x68开始和0D结束；
        //     * 结束符号一定是0x0D，但0D不一定是结束符号，有些数据比如269这个值就能分解成0x01和0x0D；
        //     * 也不能直接以0x0D 0x68作为确定是粘包了，以为16位整型3432就是直接分解成 0x0D 0x68；所以还是按照编码组成规则去考虑
        //     * 规则就是处理分包时的校验规则：0x68后面第的第一位表示字节长度，总的字节长度是该值+6；所以我们就按照这个思路去处理，另外起始编码一定是0x68
        //     * **********/
        //    int iLen;
        //    int iStart, iCnt;
        //    iStart = 0;//起始搜索固定-1，后面FindIndex用到；
        //    while (true)
        //    {
        //        iStart = Array.FindIndex<byte>(bs, iStart, m => m == 0x68);
        //        if (iStart < 0) break;
        //        if (bs.Length < iStart + 6)
        //        {
        //            //日志
        //            Logger.LogDebug($"已成功获取了{list.Count}个包，最后的数据包长度不足，异常数据起始位置{iStart}，全部数据[{StringHelper.ByteToHexStr(bs)}]");
        //            break;//此时后面就不用解析了，这个数据肯定不完整，传到后面去也是无效的，这里以日志以日志形式抛出就可以了
        //        }
        //        if (bs[iStart + 3] != 0x68)
        //        {
        //            Logger.LogDebug($"已成功获取了{list.Count}个包，但该包的0x68后面第3个字节不是0x68，该包小段舍弃，定位到最近一个0D处继续读取，异常数据起始位置{iStart}，全部数据[{StringHelper.ByteToHexStr(bs)}]");
        //            //此时问题来了，按道理是0x68 xx xx 0x68的，但现在后第四位不是0x68的，只能说是前面的被截取了，这个时候我们要舍弃它
        //            //把起始位置定位到最近一个0D
        //            iStart = Array.FindIndex<byte>(bs, iStart, m => m == 0x0D);
        //            if (iStart < 0) break;//没有则退出了
        //            continue;
        //        }
        //        iLen = bs[iStart + 1];//获取单个包的长度值；
        //        iCnt = iLen + 6;
        //        if (bs.Length < iStart + iCnt)
        //        {
        //            Logger.LogDebug($"已成功获取了{list.Count}个包，拆包时数据包长度不足{iCnt}，异常数据起始位置{iStart}，全部数据[{StringHelper.ByteToHexStr(bs)}]");
        //            break;//此时后面就不用解析了，这个数据肯定不完整，传到后面去也是无效的，这里以日志以日志形式抛出就可以了
        //        }
        //        list.Add(bs.Skip(iStart).Take(iCnt).ToArray());//这个时候一个完整的包就取到了
        //        iStart += iCnt;//继续下个包获取
        //    }
        //    this.Logger.LogDebug($"拆包执行完毕，获取了{list.Count}个包，原始数据[{StringHelper.ByteToHexStr(bs)}]");
        //    return list;
        //}
        #endregion
        #region 采集气碘数据
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
        int HDZCQCmdIndex = -1;
        HDZCQCmdCode[] HDZCQCmdCodes;
        private HDZCQCmdCode GetCurrentCmd()
        {
            if (this.HDZCQCmdCodes == null || this.HDZCQCmdCodes.Length == 0)
            {
                HDZCQCmdCodes = new HDZCQCmdCode[] { HDZCQCmdCode.瞬时数据查询, HDZCQCmdCode.故障查询,
                    HDZCQCmdCode.设置信息查询, HDZCQCmdCode.工作状态查询, HDZCQCmdCode.文件号查询,HDZCQCmdCode.读取文件 };
            }
            this.HDZCQCmdIndex++;//递增1
            if (HDZCQCmdIndex < 0 || HDZCQCmdIndex >= this.HDZCQCmdCodes.Length)
                HDZCQCmdIndex = 0;
            if( this._FileNoReading == 0  && this.HDZCQCmdCodes[this.HDZCQCmdIndex]== HDZCQCmdCode.读取文件)
            {
                //此时文件号为空，无法读取文件，则跳过
                this.HDZCQCmdIndex++;
                return GetCurrentCmd();
            }
            if (this.HDZCQCmdCodes[this.HDZCQCmdIndex] == HDZCQCmdCode.读取文件)
            {
                //此时为读取文件，则参数定义成文件内容
                this._Datas = StringHelper.ShortToBytes(this._FileNoReading);
            }
            else
            {
                if (this._Datas != null && this._Datas.Length > 0)
                    this._Datas = new byte[0];
            }
            return this.HDZCQCmdCodes[this.HDZCQCmdIndex];
        }
        /// <summary>
        /// 开始气碘数据采集,请在新Task中运行
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="device"></param>
        public async void StartAcquireAsync(DeviceDto device)
        {
            /*************
             *  * 气碘设备数据采集逻辑
            * 1、该设备需要人驱动后或根据自定义时间后才会采集，通常是空闲状态的，但不用管它工作状态，只要抓数据就可以了；
            * 2、实时读取最大文件号，当有增加时，则去读取一次历史记录，这里要注意可能不只1个历史记录待读取的，这个后面的程序会涉及；
            * 2.1、实时状态4次读取：当前最大文件号、设置值、瞬时状态、报警状态、作业状态，分别读取；这些数据是实时更新远程数据库的表结构，但最大文件号不用更新数据库；
            * 2.2、如果设备是空闲状态的，则读取频率低一点，如果是作业中的则读取频率高一点。这个目前没去实现，后期如果有必要再添加该功能，只要根据工作状态，动态改变DynamicTaskDelay就可以了；
            * 3、历史数据采集：读取历史记录的是最为关键的，分为软件启动时的补数据和实时读取文件记录；
            * 3.1、软件启动时判断是否少了数据，方法：从数据库中读取该设备最新的记录，根据测试启动时间来判断，注意这里不能通过 FileNo来判断，因为这个值在设备中是变动的，实际上后期在
            *       使用过程中，数据库里的FileNo基本是1；
            * 3.2、在上3.1中通过测试起始时间作为数据判断，那么我们要确保存入的数据是连续的，就是不要存入一条2021-01-01 01:01后直接存储最新时间的，跳过了中间的，这样软件启动时判断不了，除非
            *       在设备中一条一条去查再打数据库里判断究竟有没有保存，这样肯定不行；
            * 3.3  所以第一次软件启动，同步所有数据时要全部读取完成后再退出，否则就会丢失还未读取的数据，因为下次不会再读取了（原因就是软件启动时我读取的是最大的StartTime值），这是一个bug；
            * 3.4  为了修复3.3，改成以下方式，读取文件按正常规则来，但把数据存入一个List中，等全部读取完后，再通过事务一起提交到数据库中；
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
                HDZCQCmdCode cmd = this.GetCurrentCmd();
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
                ZcqRecord zcqRecord;
                try
                {
                    IZcqRecordRepository zcqRecords = scope.ServiceProvider.GetService<IZcqRecordRepository>();
                    var query = zcqRecords.Where(m => m.DeviceId == this.Device.Id && m.StartTime.Length > 0)
                        .WhereIf(this.Device.TenantId != null, m => m.TenantId == this.Device.TenantId).OrderByDescending(m => m.StartTime);
                    zcqRecord = query.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, "读取数据库最大测试时间时出错。");
                    return;
                }
                if (zcqRecord != null)
                {
                    DateTime detStartTime;
                    if (DateTime.TryParse(zcqRecord.StartTime, out detStartTime))
                        this._MaxStartTimeInDB = detStartTime;
                }
                else
                {
                    //此时数据库里没有任何数据，则要全部从设备中同步
                    this._MaxStartTimeInDB = DateTime.MinValue;
                }
            }
        }
        public bool IsFileSave2Db(ZCQFileInfoDto dto)
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
        public bool NextFileInfoSaveCompeleted(ZcqRecord record, IServiceScope scope)
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
            HDZCQFrame f = (HDZCQFrame)new HDZCQFrame();
            f.DecodeFrame(msg.Payload);
            this.LastResponseData = f.DtoData;
            try
            {
                //保存数据
                var _serviceProvider = Aming.Tools.IocHelper.ServiceProvider;
                HDZCQDataService s = _serviceProvider.GetService<HDZCQDataService>();
                
                s.DataSave(this, msg, f.DtoData);
            }
            catch (Exception ex)
            {
                Logger.LogError("请在这里 CtrlHDZCQService.PortDataReceived添加处理代码");
            }
            //处理文件号查询
            if (f.DtoData is ZCQFileNoDto dtoFileNo) this.ProFileNoDto(dtoFileNo);
            if (OnUniMessageReceivedCallBack != null) //传统的事件触发传出去
            {
                //var msg1 = new UniMessageBase { Payload = data, Topic = $"/DTU/{Config.Port}/from" };
                OnUniMessageReceivedCallBack(config as MqttClientConfig, msg, f.DtoData);
            }
        }
        public void ProFileNoDto(ZCQFileNoDto dtoFileNo)
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

        public void SendUniMessage(UniMessageBase msg)
        {
            if (msg.Topic == this.TranService.Config.ForwardFromTopic)
            {
                this.TranService.SendUniMessage(msg);
            }
        }
        #endregion
        #endregion
    }
}
