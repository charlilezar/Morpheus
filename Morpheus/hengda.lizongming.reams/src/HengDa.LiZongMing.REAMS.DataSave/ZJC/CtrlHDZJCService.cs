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
using Microsoft.Extensions.DependencyInjection;
using HengDa.LiZongMing.REAMS.ZJC.Dtos;

namespace HengDa.LiZongMing.REAMS.CtrlServices
{
    /// <summary>
    /// 控制干湿沉降硬件通讯类
    /// </summary>
    public class CtrlHDZJCService : ICtrlService, ISingletonDependency
    {
        #region 可配置变量
        /// <summary>
        /// 设备站点号，modbus通讯必备，要与硬件设置的站点号一致，否则设备不返回数据
        /// </summary>
        public ushort MacNo { get; set; } = 1;
        /// <summary>
        /// 每天的降雨明细记录遍历间隔，默认5分钟
        /// </summary>
        public int TodayRainDataReadIntervalSeconds { get; set; } = 300;
        /// <summary>
        /// 每天的干沉降明细记录遍历间隔，默认5分钟
        /// </summary>
        public int TodayDryDataReadIntervalSeconds { get; set; } = 300;
        public int RunStatusSaveHisIntervalSeconds { get; set; } = 60;
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
        public Action<MqttClientConfig, UniMessageBase, ZJCBaseDto> OnUniMessageReceivedCallBack { get; set; } = null;


        /// <summary>
        /// 将本端数据转发到别的端口的处理程序
        /// </summary>
        public event ReceivedUniMessageCallBack OnDataForwardTo;

        /// <summary>
        /// 将消息接收的事件传递到更多的地方处理，主要是
        /// </summary>
        public event ReceivedUniMessageCallBack OnUniMessageDataReceived;

        public CtrlHDZJCService(ILoggerFactory loggerFactory, string jsonConfig)
        {
            IDataEndPoint config = null;
            if (jsonConfig.Contains("MqttType"))
                config = JsonConvert.DeserializeObject<MqttClientConfig>(jsonConfig);
            else
                config = JsonConvert.DeserializeObject<UARTConfig>(jsonConfig);
            ////TODO：其它的请补全
            Init(loggerFactory, config);
        }
        public CtrlHDZJCService(ILoggerFactory loggerFactory, IDataEndPoint config)
        {
            Init(loggerFactory, config);
        }

        public CtrlHDZJCService Init(ILoggerFactory loggerFactory, IDataEndPoint config)
        {

            this.loggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger<CtrlHDZJCService>();

            this.MyDataEndPoint = config;
            var tsObj = ServiceFactory.Create(loggerFactory, config);
            tsObj.funcCheckPackFormat = SerialPortService.CheckPackFormatByModbus;//定义Modbus下的粘包处理
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
        public async Task<bool> SaveDevicesData(HDZJCCmdCode cmd)
        {
            return await SaveDevicesData(cmd,new byte[] { });
        }
        public async Task<bool> SaveDevicesData(HDZJCCmdCode cmd, byte[] data)
        {
            //通过异步等待方式读取DTO数据，这里没有办法，气碘返回数据中可以分辨出是什么命令
            //但干湿沉降找不出，返回内容为，设备编码，功能码（仅能识别出读数据还是写数据），返回字节长度，请求的地址数据，校验码
            IotResult<ZJCBaseDto> result = await CmdExec_WaitReply(cmd, data);
            if (!IsDeviceReturnValid(cmd,result)) return false;
            if (result.Result.Cmd==HDZJCCmdCode.实时数据)
            {
                //此时读取数量
                SetDryRecordCnt(((ZJCRealDataDto)result.Result).DryDepositionCount);
            }
            //此时可以保存数据了
            return await this.HDZJCDataService.DataSave(this, null, result.Result);
        }
        private bool IsDeviceReturnValid(HDZJCCmdCode cmd, IotResult<ZJCBaseDto> result)
        {
            //判断收到的数据是否正确
            if (result == null)
            {
                Logger.LogError($"干湿沉降命令[{EnumHelper.GetEnumDescription<HDZJCCmdCode>(cmd)}]请求失败，发回了空对象。");
                return false;
            }
            if (!result.Ok)
            {
                Logger.LogError($"干湿沉降命令[{EnumHelper.GetEnumDescription<HDZJCCmdCode>(cmd)}]请求出错：{result.Message}");
                return false;
            }
            if (result.Result == null)
            {
                Logger.LogError($"干湿沉降命令[{EnumHelper.GetEnumDescription<HDZJCCmdCode>(cmd)}]请求失败，发回了空DTO。");
                return false;
            }
            if (!result.Result.Sucessfully)
            {
                Logger.LogError($"干湿沉降命令[{EnumHelper.GetEnumDescription<HDZJCCmdCode>(cmd)}]解析出错：{result.Result.ErrMsg}");
                return false;
            }
            return true;
        }
        public async Task<IotResult<ZJCBaseDto>> CmdExec_WaitReply( HDZJCCmdCode cmd, byte[] data)
        {
            //传出一个ZCQ设备Dto对象
            IotResult<UniMessageBase> _cmdResult = null;
            string title = EnumHelper.GetEnumDescription<HDZJCCmdCode>(cmd);
            HDZJCFrame f = (HDZJCFrame)new HDZJCFrame().BuildFram(this.MacNo, (byte)cmd, data);
            ZJCBaseDto dtoResult = new ZJCBaseDto();
            try
            {
                _cmdResult = await TranService.SendAndWaitReply(f.Bytes, title, 30000);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "干湿沉降通讯出错");
                _cmdResult = new IotResult<UniMessageBase>(false, $"干湿沉降通讯出错：{ex.Message}({ex.Source})");
            }
            //返回DTO结果
            IotResult<ZJCBaseDto> _dataReturn = new IotResult<ZJCBaseDto>(_cmdResult.Ok, _cmdResult.Message);
            if (_cmdResult.Ok)
            {
                //此时正常，则开始解析数据
                f.DecodeFrame(_cmdResult.Result.Payload);
                _dataReturn.Result = f.DtoData;//此时读取解析后的DTO对象
                _dataReturn.Result.Raw = _cmdResult.Result.Payload;
            }
            else
            {
                //此时通讯出错，则返回一个出错的DTO
                _dataReturn.Result = new ZJCBaseDto()
                {
                    ErrMsg = _cmdResult.Message,
                    Sucessfully = false,
                };
            }
            return _dataReturn;
        }
        #region 同步干沉降记录
        List<ZJCDryDepositionDataDto> ZJCDryDepositionDataDtos = null;//干沉降DTO记录集
        DateTime? DryMaxIndexUpdatedTime = null;
        /// <summary>
        /// 记录当前最后一条干沉降索引
        /// </summary>
        ushort DryMaxIndex = 0;
        public void SetDryRecordCnt(ushort iCnt)
        {
            //如果超过0表示还在循环遍历记录
            if (DryMaxIndex > 0) return;
            if (this.DryMaxIndexUpdatedTime == null)
                this.DryMaxIndex = iCnt;//赋值后就会遍历并存入数据库中
            else
            {
                TimeSpan span = DateTime.Now - (DateTime)DryMaxIndexUpdatedTime;
                //如果上次更新时间还未到达预期的
                if (span.TotalSeconds < this.TodayDryDataReadIntervalSeconds)
                    return;
                this.DryMaxIndex = iCnt;//赋值后就会遍历并存入数据库中
            }
        }
        
        public void DryRecordExist()
        {
            //通知发送的索引已经存在了
            Logger.LogDebug($"干沉降记录，已经存在，将最大索引号从{this.DryMaxIndex}变更为0。");
            this.DryMaxIndex = 0;
        }
        public async Task ReadDryRecordFromDevice()
        {
            if (this.DryMaxIndex <= 0) return;//此时不用去查询了，估计时间还未到
            Logger.LogDebug($"开始读取干沉降索引{this.DryMaxIndex}的数据。");
            #region 设置干沉降索引
            byte[] bs = StringHelper.UShortToBytes(this.DryMaxIndex);
            HDZJCCmdCode cmd = HDZJCCmdCode.设置干沉降条号索引条号;
            IotResult<ZJCBaseDto> result = await CmdExec_WaitReply(cmd, bs);
            if (!IsDeviceReturnValid(cmd, result)) return;//返回出错的话，不用再读取后面的数据了；
            #endregion
            #region 读取干沉降记录
            await Task.Delay(1000);//给设备一点时间，根据之前的测试不能太快，否则设备信息还未写入地址
            cmd = HDZJCCmdCode.干沉降记录查询;
            result = await CmdExec_WaitReply(cmd, bs);
            if (!IsDeviceReturnValid(cmd, result)) return;//返回出错的话，不用再读取后面的数据了；
            ZJCDryDepositionDataDto dto = (ZJCDryDepositionDataDto)result.Result;
            bool blExist;
            if (!this.HDZJCDataService.DryRecordExist(this, dto, out blExist))
            {
                //此时判断出错，则不对本次数据读取做任何操作，可能和数据库连接中断了
                return;
            }
            if (blExist || this.DryMaxIndex <= 1)
            {
                //此时已经存在该干沉降记录了，或者已经遍历到最后一个索引了，我们将找出来的数据保存至数据库
                if (ZJCDryDepositionDataDtos != null && this.ZJCDryDepositionDataDtos.Count > 0)
                {
                    #region 存储日志
                    if (blExist)
                    {
                        this.Logger.LogDebug($"干沉降记录,索引={this.DryMaxIndex},时间=[{dto.StartTime.ToString("yyyy-MM-dd HH:mm:ss")},{dto.EndTime.ToString("yyyy-MM-dd HH:mm:ss")}]已经存在于数据库中，终止遍历，之前{this.ZJCDryDepositionDataDtos.Count}行数据一起存入数据库。");
                    }
                    else
                    {
                        this.Logger.LogDebug($"干沉降记录,索引={this.DryMaxIndex},时间=[{dto.StartTime.ToString("yyyy-MM-dd HH:mm:ss")},{dto.EndTime.ToString("yyyy-MM-dd HH:mm:ss")}]，由于已经遍历到最小索引，终止遍历，之前{this.ZJCDryDepositionDataDtos.Count}行数据一起存入数据库。");
                    }
                    #endregion
                    if (!this.HDZJCDataService.DryRecordSave(this, this.ZJCDryDepositionDataDtos))
                    {
                        return;
                    }
                    this.ZJCDryDepositionDataDtos.Clear();
                }
                else
                {
                    //存储日志
                    this.Logger.LogDebug($"干沉降记录[Exist={blExist}],索引={this.DryMaxIndex},时间=[{dto.StartTime.ToString("yyyy-MM-dd HH:mm:ss")},{dto.EndTime.ToString("yyyy-MM-dd HH:mm:ss")}]，终止遍历，但无数据需要存入数据库。");
                }
                //强记录复位成0，等待下次再查询
                this.DryMaxIndex = 0;
            }
            else
            {
                //存入缓存，然后一起提交
                if (this.ZJCDryDepositionDataDtos == null)
                    this.ZJCDryDepositionDataDtos = new List<ZJCDryDepositionDataDto>();
                this.ZJCDryDepositionDataDtos.Add(dto);
                this.Logger.LogDebug($"干沉降记录,索引={this.DryMaxIndex},时间=[{dto.StartTime.ToString("yyyy-MM-dd HH:mm:ss")},{dto.EndTime.ToString("yyyy-MM-dd HH:mm:ss")}]需要存入数据库，暂先存入缓存对象。");
                //读取上一条干沉降记录
                this.DryMaxIndex--;
            }
            #endregion
        }
        #endregion
        #region 同步湿沉降记录
        List<ZJCRainDataDto> ZJCRainDataDto = null;
        DateTime? _MaxRainRecordTime = null;
        private void InitMaxRainRecordTime()
        {
            /**********
             * 初始化当前已经读取到的降雨时间
             * 注意，如果数据库中没有任何记录，则应该是第一次同步，我们直接读取1个月前的数据就可以了
             * *********************/
            if (this._MaxRainRecordTime != null) return;//如果已经赋值过了，则不用再读取了
            DateTime? detDB = this.ReadMaxRainRecordTimeFromDB();
            if (detDB == null)
                detDB = DateTime.Now.AddMonths(-1);//是在么有数据则遍历1个月以前的
            this._MaxRainRecordTime = (DateTime)detDB;
        }
        private DateTime? ReadMaxRainRecordTimeFromDB()
        {
            //从数据库读取最新的降雨记录
            return DateTime.Parse("2019-12-01");//TODO:设备没有2021的降雨数据，暂时返回一个比较早的
            return null;
        }
        private bool ReadRainDataDetailCompeleted(ZJCRainDataDto dto)
        {
            //判断是否已经将某一时间索引下的降雨明细读取完毕了，完毕的话返回True。
            //规则：当返回的数据不再变化时就表示都已经被读取了，这个是这个设备的特点。
            //如何判断是否数据不变更：降雨起始时间和降雨结束时间
            if (this.ZJCRainDataDto == null || this.ZJCRainDataDto.Count == 0) return false;
            return ZJCRainDataDto.Find((ZJCRainDataDto temp) => { return temp.StartTime == dto.StartTime && temp.EndTime == dto.EndTime; }) != null;
        }

        public async Task<bool> SaveRainRecord()
        {
            DateTime detData = (DateTime)this._MaxRainRecordTime;
            byte[]  data = new byte[4];
            data[0] = (byte)(detData.Year - 2000);//注意：2021年，存储的是21；
            data[1] = (byte)detData.Month;//月份
            data[2] = (byte)detData.Day;//日
            data[3] = 0x00;//最后一个无效；
            //读取湿沉降记录
            Logger.LogDebug($"设置降雨明细日期索引为：{detData.ToString("yyyy-MM-dd")}。");
            HDZJCCmdCode cmd = HDZJCCmdCode.设置降雨明细日期索引;
            IotResult<ZJCBaseDto> result = await CmdExec_WaitReply(cmd, data);
            if (!IsDeviceReturnValid(cmd, result)) return false;//返回出错的话，不用再读取后面的数据了；
            this.ZJCRainDataDto = new List<ZJCRainDataDto>();//接受下面遍历出来的数据
            cmd = HDZJCCmdCode.降雨记录查询;
            ZJCRainDataDto dto;
            int iCnt = 0;
            while (true)
            {
                iCnt++;
                Logger.LogDebug($"日期索引为：{detData.ToString("yyyy-MM-dd")}的降雨明细读取第{iCnt}次。");
                await Task.Delay(1000);//给设备一点时间，根据之前的测试,不能太快去请求，否则设备信息还未写入地址
                //此时设置成功，则读取寄存器地址
                result = await CmdExec_WaitReply(cmd, data);
                if (!IsDeviceReturnValid(cmd, result)) return false;//返回出错的话，当前时间索引下的降雨明细停止遍历，待下次再继续遍历，因为我们不能跳过这个数据；
                if (iCnt >= 100)
                {
                    //这里做个限制，担心设备程序有异常，不停的切换时间来返回的话，那就会进入死循环了，也有肯能遍历完成后，又改成有从第一行开始遍历了，这样也可能会死循环
                    Logger.LogError($"干湿沉降降雨记录遍历超过了100次，请检测原因。当前返回数据{StringHelper.ByteToHexStr(result.Result.Raw)}");
                    return false;
                }
                try
                {
                    dto = (ZJCRainDataDto)result.Result;
                }
                catch(Exception ex)
                {
                    Logger.LogError(ex, "干湿沉降结果强制转化成降雨记录DTO时失败。");
                    return false;
                }
                if(dto.InvalidData)
                {
                    Logger.LogDebug($"干湿沉降降雨记录返回无效数据，可以认为时间[{detData.ToString("yyyy-MM-dd")}]下无降雨记录，无效数据描述：{dto.ErrMsg}；设备反馈数据:{StringHelper.ByteToHexStr(dto.Raw)}");
                    //此时为无效数据，出现这种情况只能是设备写入地址的数据不是正常的我们时间格式，有可能是0，也有可能是其他数值，但不是时间的；
                    //这种情况我们可以认定为该日期下没有明显，否则设备会写入正常数据的；
                    return true;
                }
                if (this.ReadRainDataDetailCompeleted(dto))
                {
                    Logger.LogDebug($"日期索引为：{detData.ToString("yyyy-MM-dd")}的降雨明细读取完成，开始保存到数据库。");
                    //此时已经读取完毕，则保存数据
                    if (!await this.HDZJCDataService.SaveRainDataDetail(this, ZJCRainDataDto))
                        return false;//保存失败，则直接返回，下次继续存储该时间索引下的降雨明细
                    return true;
                }
                else
                {
                    //此时还未读取，则继续往下遍历
                    this.ZJCRainDataDto.Add(dto);
                    Logger.LogDebug($"成功读取到干湿沉降降雨明细数据：起始时间={dto.StartTime.ToString("yyyy-MM-dd")}，结束日期={dto.EndTime.ToString("yyyy-MM-dd")}。");
                }
            }
        }
        public async Task ReadRainRecordFromDevice()
        {
            this.InitMaxRainRecordTime();// 初始化降雨最大时间
            //从设备读取降雨明细，并存入数据库
            TimeSpan timeSpan = DateTime.Now - (DateTime)this._MaxRainRecordTime;
            //如果时间还未超过指定时间，不要去读降雨记录了，这个值默认是5分钟，不用这么实时
            if (timeSpan.TotalSeconds < this.TodayRainDataReadIntervalSeconds) return;
            if(await this.SaveRainRecord())
            {
                //此时保存成功，则更新当前最大降雨记录时间
                //如果我们查询的时间是今天以前的，昨天或更早的，直接加1天就可以了，因为这个设备的时间索引就是以天为单位的，也就是它的明细是指1天内的
                //降雨明细，如果时间是今天的那我们就按照
                DateTime detPreRecord = (DateTime)this._MaxRainRecordTime;
                if (detPreRecord.Year<DateTime.Now.Year || detPreRecord.Month < DateTime.Now.Month || detPreRecord.Day < DateTime.Now.Day)
                {
                    this._MaxRainRecordTime = detPreRecord.AddDays(1);//直接增加一天即可
                }
                else
                {
                    //更新为当前时间，用于下次遍历前是否应到了TodayRainDataReadIntervalSeconds设定的时间
                    this._MaxRainRecordTime = DateTime.Now;
                }
            }
        }
        #endregion
        #region 采集干湿沉降数据
        HDZJCDataService HDZJCDataService
        {
            get
            {
                return Aming.Tools.IocHelper.ServiceProvider.GetService<HDZJCDataService>();
            }
        }
        int HDZJCCmdIndex = 0;
        public int DynamicTaskDelay = 500;
        HDZJCCmdCode[] HDZJCCmdCodes = new HDZJCCmdCode[] { HDZJCCmdCode.实时数据, HDZJCCmdCode.查询指定干沉降索引号的干沉降记录, HDZJCCmdCode.查询指定时间的降雨记录明细 };


        private HDZJCCmdCode GetCurrentCmd()
        {
            if (HDZJCCmdIndex < 0 || HDZJCCmdIndex >= this.HDZJCCmdCodes.Length)
                HDZJCCmdIndex = 0;
            return this.HDZJCCmdCodes[HDZJCCmdIndex++];
        }
        public async void StartAcquireAsync(DeviceDto device)
        {
            /**************
             * 干湿沉降读取逻辑
             * 实时数据：每隔一定时间一次性读取对应地址；
             * 干沉降记录读取：
             * 1、软件加载时读取数据库中最新记录的索引号和StartTime、EndTime；然后根据这个索引号去设备读取是否数据一致，不一致的重新定义为1，从头开始读取；一般
             *    这种情况是清空历史记录；
             * 2、如果是一致的就将该地址+1，然后再去读取，如果出来的是有效数据，则继续加一，如果不是的等待一定时间后继续读取原来的序号；
             * 3、这里还要继续读取索引号-1的情况，如果是出来的不是有效数据，则表示历史记录被清空了。应该重新再同步了。
             * 4、所以我们要读取索引号的一前一后值，确保数据是一致的。
             * 降雨记录：
             * 1、系统中存储降雨记录的规则：
             * 1.1、一天可能存在多条降雨记录；
             * 1.2、不是每天都有降雨记录，估计1个月不下雨就1个月没有记录；
             * 2、系统存储逻辑（方案一）
             * 2.1、系统加载时从数据库中读取降雨记录最新时间，如果该记录为空，可能是第一次运行，则从一个月前开始读取；
             * 2.2、这里注意：不是每天都有降雨记录的，所以这个会导致每次开启软件都会遍历好多天，所以超过1个月的就只遍历1个月时间；
             * 3、系统存储逻辑（方案二）
             * 3.1、方案一种不是很理想，如果我们在数据库端添加一张表来存储我读取过哪一天的记录了，至于是否有数据不要紧，都存储下来。相当于在数据库中添加一张降雨信息主表，存储
             *      读取时间，总降雨时间，总降雨量。如果没有降雨记录，则降雨量和时间就为空0；
             * 3.2、采集软件打开时就去查找主表的最新时间，然后继续往后查找降雨记录；
             * 4、系统存储逻辑（方案三）
             * 4.1、采集软件打开直接遍历3天前的数据；
             * 5、由于一天当中可能存在多条降雨记录，所以最好每个1小时去查询设备，看看是否新增了降雨明细，这样历史数据库中的数据就较为实时了，而不是等第二天才能看到昨天的记录；
             * **************/
            this.Device = device;//存储设备唯一标识，后面上传数据会用到
            //初始化设备连接
            var config = device.GetConfig();
            while (true)
            {
                //确保端口打开，如果已经打开，TryOpenAsync直接会返回，不会重复去打开
                if (!await this.TranService.TryOpenAsync())
                {
                    //添加日志，错误内容TryOpenAsync已经写入了
                    Logger.LogDebug($"设备[{this.Device.Name}]打开失败。");
                    await Task.Delay(3000);//如果设备连接失败，可能是配置错了，也可能是设备未连接，休眠一下再尝试打开
                    continue;
                }
                //读取当前要执行的命令
                HDZJCCmdCode cmd = this.GetCurrentCmd();
                Logger.LogDebug($"开始执行干湿沉降命令[{EnumHelper.GetEnumDescription<HDZJCCmdCode>(cmd)}]");
                //开始采集
                if (cmd == HDZJCCmdCode.查询指定干沉降索引号的干沉降记录)
                    await this.ReadDryRecordFromDevice();
                else if (cmd == HDZJCCmdCode.查询指定时间的降雨记录明细)
                    await this.ReadRainRecordFromDevice();
                else if (cmd == HDZJCCmdCode.实时数据)
                    await this.SaveDevicesData(cmd);
                await Task.Delay(this.DynamicTaskDelay);
            }
        }

        public void OnUniMessageReceived(IDataEndPoint config, UniMessageBase msg)
        {
            //TODO：

            Logger.LogError("ZJC还没有写保存代码??");

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

        #endregion
    }
}
