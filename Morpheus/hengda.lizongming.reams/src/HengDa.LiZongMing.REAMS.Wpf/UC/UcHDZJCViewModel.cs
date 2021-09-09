using Aming.Core;
using Aming.DTU.Config;
using HengDa.LiZongMing.REAMS.CtrlServices;
using HengDa.LiZongMing.REAMS.Iot;
using Microsoft.Extensions.Logging;
using HengDa.LiZongMing.REAMS.Wpf.Dto;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;

namespace HengDa.LiZongMing.REAMS.Wpf.UC
{
    public class UcHDZJCViewModel: Screen
    {
        const string EXCUTINGVIEW = "执行中...";
        const string EXCUTEVIEW = "执行";
        private ILoggerFactory loggerFactory;
        private ILogger Logger;
        private readonly IServiceProvider ServiceProvider;
        private IWindowManager windowManager;
        #region 发送命令所需参数
        /// <summary>
        /// 降雨明细搜索的时间索引，精确到年月日就可以了
        /// </summary>
        public DateTime RainDetailSearchDate { get; set; }
        /// <summary>
        /// 故障明细搜索的时间索引，精确到年月日就可以了
        /// </summary>
        public DateTime AlarmDetailSearchDate { get; set; }
        /// <summary>
        /// 降雨明细查询的索引日期
        /// </summary>
        public ushort RainDetailStartIndex { get; set; }
        /// <summary>
        /// 故障明细查询的索引日期
        /// </summary>
        public ushort DryDepDetailStartIndex { get; set; }
        /// <summary>
        /// 要查询干沉降的数据行数
        /// </summary>
        public int DryDepDetailSearchCnt { get; set; }
        public bool AutoExcute { get; set; } = false;//是否自动执行，如果该变量为真的话，会一直执行命令
        public string ExcuteStateView { get; set; } = EXCUTEVIEW;
        public List<NameValue<HDZJCCmdCode>> CmdCodeList { get; set; }
        /// <summary>
        /// 当前要执行的命令
        /// </summary>
        public HDZJCCmdCode CmdCode_Selected { get; set; }
        /// <summary>
        /// 开始采样参数
        /// </summary>
        public string CmdDatas { get; set; } = "01";
        /// <summary>
        /// 系统支持的通讯模式
        /// </summary>
        public List<NameValue<EndPointEnum>> CommunicationMode { get; set; }
        /// <summary>
        ///  已设定的通讯模式
        /// </summary>
        public EndPointEnum CommunicationMode_Selected { get; set; } = EndPointEnum.TCP;
        /// <summary>
        /// 设备序号
        /// </summary>
        public ushort MacNo { get; set; } = 1;
        #endregion
        #region DTO数据
        /// <summary>
        /// 获取的降雨记录集
        /// </summary>
        public DataGridResult<Dto.ZJCRainDataDto> _MyZJCRainDataDtos { get; set; } = new DataGridResult<ZJCRainDataDto>();
        /// <summary>
        /// 干沉降记录集
        /// </summary>
        public DataGridResult<Dto.ZJCDryDepositionDataDto> _MyZJCDryDepositionDataDtos { get; set; } = new DataGridResult<ZJCDryDepositionDataDto>(30);
        /// <summary>
        /// 故障明细
        /// </summary>
        public DataGridResult<Dto.ZJCAlarmDataDto> _MyZJCAlarmDataDtos { get; set; } = new DataGridResult<ZJCAlarmDataDto>(50);

        #endregion
        #region MQTT监听
        //TODO:临时关闭
        //public string MqttConnectedStatus { get; set; } = "断开";
        //public void StartMqttServerListen()
        //{
        //    if (this._MqttServerListen == null)
        //    {
        //        this._MqttServerListen = new Aming.DTU.MqttListen(this.loggerFactory, new MqttConfig()
        //        {
        //            ServerIP = "api.hengda.show",
        //            ServerPort = 1883,
        //            UserName = "88888888",
        //            Password = "88888888",
        //            PubTopicUp = "/mingdtu/dtu/GanShi001/hex/up",
        //            PubTopicDown = "/mingdtu/dtu/GanShi001/hex/down"
        //        });
        //        this._MqttServerListen.SendText2MQTTServer = this.CreateText2MQTTServer;
        //        this._MqttServerListen.ConnectedChangedNotice = this.MqttServerConnectedChanged;
        //    }
        //    string strErr;
        //    if (this._MqttServerListen.StartListenning(out strErr))
        //    {
        //        this.MyDtoShow.SetMsg("MQTT监听已启动");
        //    }
        //    else
        //    {
        //        this.MyDtoShow.SetMsg($"MQTT监听失败：{strErr}");
        //    }
        //}
        //public void StopMqttServerListen()
        //{
        //    if (MqttConnectedStatus == "断开")
        //        StartMqttServerListen();//此时用户是要打开链接
        //    else
        //    {
        //        //此时要断开链接
        //        if (this._MqttServerListen == null) return;
        //        this._MqttServerListen.StopListenning(1);
        //    }
        //}
        //Aming.DTU.MqttListen _MqttServerListen { get; set; }
        //public async Task<string> CreateText2MQTTServer(byte[] data)
        //{
        //    //这里反正测试，直接读取实时数据即可
        //    IotResult<ZJCBaseDto> rs = await service.CmdExec_SourceData("MQTT客户端请求", data);
        //    if (rs!=null && rs.Result!=null)
        //    {
        //        this.MyDtoShow.SetDotValuesOrder(rs.Result);//TODO:临时测试
        //        return rs.Result.SourceData.ByteToHexStr(); //Newtonsoft.Json.JsonConvert.SerializeObject((ZJCRealDataDto)rs.Result);
        //    }
        //    this.MyDtoShow.SetMsg("返回为空！");//TODO:临时测试
        //    return string.Empty;
        //}
        //public void MqttServerConnectedChanged(Aming.DTU.MqttListen listen,bool blNewStatus)
        //{
        //    this.MqttConnectedStatus = blNewStatus ? "连接中" : "断开";
        //}
        #endregion
        public Dto.DtoShowEntity MyDtoShow { get; set; }//用于获取的DTO信息转换成List<MyViewEntity>形式

        public UcHDZJCViewModel(IServiceProvider serviceProvider, IWindowManager windowManager, ILoggerFactory loggerFactory)
        {
            this.ServiceProvider = serviceProvider;
            this.windowManager = windowManager;
            this.loggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger<UcHDZJCViewModel>();
            CmdCodeList = EnumHelper.GetEnumListNameValue<HDZJCCmdCode>();
            CommunicationMode = EnumHelper.GetEnumListNameValue<EndPointEnum>();
            MyDtoShow = new DtoShowEntity(35);
            //临时关闭
            //StartMqttServerListen();
            LoadConfigAndConn();
        }

        /// <summary>
        /// 从保存的配置文件打开服务设备
        /// </summary>
        /// <returns></returns>
        public bool LoadConfigAndConn()
        {
            //判断已创建的通讯方式和当前选中的是否一致，是的话就，就不用再创建了
            if (this.service != null && this.service.MyDataEndPoint != null && this.service.MyDataEndPoint.EndPointType == this.CommunicationMode_Selected)
            {
                Logger.LogDebug("通讯方式：" + EnumHelper.GetEnumDescription(this.service.MyDataEndPoint.EndPointType));
                return true;
            }
            IDataEndPoint config;
            if (this.CommunicationMode_Selected == EndPointEnum.TCP)
            {
                config = new Aming.DTU.Config.TcpClientConfig()
                {
                    ServerIP = "192.168.1.202",
                    ServerPort = 4196
                };
                Logger.LogDebug("初始化通讯方式：" + EnumHelper.GetEnumDescription(EndPointEnum.TCP));
            }
            else if (this.CommunicationMode_Selected == EndPointEnum.UART)
            {
                config = new Aming.DTU.Config.UARTConfig()
                {
                    Port = "COM3"
                };
                Logger.LogDebug("初始化通讯方式：" + EnumHelper.GetEnumDescription(EndPointEnum.UART));
            }
            else if (this.CommunicationMode_Selected == EndPointEnum.MQTT)
            {
                config = new Aming.DTU.Config.MqttClientConfig()
                {
                    ServerIP = "api.hengda.show",
                    ServerPort = 1883,
                    UserName = "88888888",
                    Password = "88888888",
                    SubTopic = "/mingdtu/dtu/3c6105156c3c/hex/up",
                    PubTopicUp = "/mingdtu/dtu/3c6105156c3c/hex/up",
                    PubTopicDown = "/mingdtu/dtu/3c6105156c3c/hex/down"
                    //SubTopic = "/mingdtu/dtu/GanShi001/hex/up",
                    //PubTopicUp = "/mingdtu/dtu/GanShi001/hex/up",
                    //PubTopicDown = "/mingdtu/dtu/GanShi001/hex/down"
                };
                Logger.LogDebug("初始化通讯方式：" + EnumHelper.GetEnumDescription(EndPointEnum.MQTT));
            }
            else
            {
                Logger.LogDebug("暂不支持通讯方式：" + EnumHelper.GetEnumDescription(this.CommunicationMode_Selected));
                this.MyDtoShow.Msg = "目前通信方式只支持TCP、UART、MQTT。";
                return false;
            }
            service = new CtrlHDZJCService(loggerFactory, config);
            return true;

        }
        public async Task StartCmdExec()
        {
            Logger.LogDebug($"设备连接状态：{this.MacConnectedStatus}");
            if (this.ExcuteStateView == EXCUTINGVIEW)
            {
                this.MyDtoShow.SetMsg("正在执行中，不能再启动执行。");
                return;
            }
            this.ExcuteStateView = EXCUTINGVIEW;
            while (true)
            {
                try
                {
                    await CmdExec();
                }
                catch (Exception ex)
                {
                    this.MyDtoShow.SetMsg("执行出错：" + ex.Message + "(" + ex.Source + ")");
                    break;
                }
                if (!AutoExcute) break;
                else
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
            this.ExcuteStateView = EXCUTEVIEW;
        }

        public async Task CmdExec()
        {
            Logger.LogDebug("命令执行开始");
            LoadConfigAndConn();
            IotResult<ZJCBaseDto> rs;
            if (this.CmdCode_Selected == HDZJCCmdCode.查询指定时间的降雨记录明细)
            {
                #region 组合查询_获取降雨记录明细
                this._MyZJCRainDataDtos.Clear();//清空记录集，准备接受指定时间段内的所有明细
                rs = await CmdExec(HDZJCCmdCode.设置降雨明细日期索引);
                if (rs.Ok && rs.Result != null && rs.Result.Sucessfully)
                {
                    bool blGoon;
                    //查询故障数据，直至最后一条：反馈的数据无效时说明该数据是最后一条
                    do
                    {
                        await Task.Delay(1000);//给点时间写入
                        rs = await CmdExec(HDZJCCmdCode.降雨记录查询);
                        blGoon = rs != null && rs.Result != null && ((Dto.ZJCRainDataDto)rs.Result).Sucessfully
                           && !((Dto.ZJCRainDataDto)rs.Result).InvalidData;
                        if (blGoon)
                        {
                            blGoon = this._MyZJCRainDataDtos.DataCount == 0 || this._MyZJCRainDataDtos[this._MyZJCRainDataDtos.DataCount - 1].StartTime !=
                                ((Dto.ZJCRainDataDto)rs.Result).StartTime;
                        }
                        //有效数据则添加
                        if (blGoon)
                            this._MyZJCRainDataDtos.AddData((Dto.ZJCRainDataDto)rs.Result);
                        //TODO:这里如何处理通讯失败的数据？如果查询过程中通讯出错了，如何通知用户是因出错中断了？
                    }
                    while (blGoon);
                }
                #endregion
            }
            else if (this.CmdCode_Selected == HDZJCCmdCode.查询指定干沉降索引号的干沉降记录)
            {
                this.AutoExcute = false;
                #region 组合查询_干沉降记录
                this._MyZJCDryDepositionDataDtos.Clear();//清空记录集，准备接受指定时间段内的所有明细
                rs = await CmdExec(HDZJCCmdCode.设置干沉降条号索引条号);
                if (rs.Ok && rs.Result != null && rs.Result.Sucessfully)
                {
                    //根据指定查询调试，进行查询，如果还未到条数，设备就反馈无效数了，则提前退出循环
                    for (int i = 0; i < this.DryDepDetailSearchCnt; i++)
                    {
                        await Task.Delay(1000);//给点时间写入
                        rs = await CmdExec(HDZJCCmdCode.干沉降记录查询);
                        if (rs != null && rs.Result != null && ((Dto.ZJCDryDepositionDataDto)rs.Result).Sucessfully
                           && !((Dto.ZJCDryDepositionDataDto)rs.Result).InvalidData)
                        {
                            if(this._MyZJCDryDepositionDataDtos.DataCount>0 && this._MyZJCDryDepositionDataDtos[this._MyZJCDryDepositionDataDtos.DataCount-1].StartTime==
                                ((Dto.ZJCDryDepositionDataDto)rs.Result).StartTime)
                            {
                                //与上一时间一样的，则表示查询终止了
                                break;
                            }
                            //此时为正确解析，且为有效数，则添加
                            this._MyZJCDryDepositionDataDtos.AddData((Dto.ZJCDryDepositionDataDto)rs.Result);
                            this.DryDepDetailStartIndex++;
                        }
                        else break;//此时为解析出错，或者数据无效，退出循环
                    }
                }
                #endregion
            }
            else if (this.CmdCode_Selected == HDZJCCmdCode.查询指定时间的故障记录明细)
            {
                this.AutoExcute = false;
                #region 组合查询_指定时间的故障明细
                this._MyZJCAlarmDataDtos.Clear();
                rs = await CmdExec(HDZJCCmdCode.设置故障查询日期索引);
                if (rs.Ok && rs.Result != null && rs.Result.Sucessfully)
                {
                    bool blGoon;
                    //查询故障数据，直至最后一条：反馈的数据无效时说明该数据是最后一条
                    do
                    {
                        await Task.Delay(1000);//留足一点时间给设备写入；
                        rs = await CmdExec(HDZJCCmdCode.故障记录查询);
                        blGoon = rs != null && rs.Result != null && ((Dto.ZJCAlarmDataDto)rs.Result).Sucessfully
                            && !((Dto.ZJCAlarmDataDto)rs.Result).InvalidData;
                        //有效数据则添加
                        if (blGoon)
                            this._MyZJCAlarmDataDtos.AddData((Dto.ZJCAlarmDataDto)rs.Result);
                    }
                    while (blGoon);
                }
                #endregion
            }
            else
            {
                //此时为普通信息，直接显示结果
                rs = await CmdExec(this.CmdCode_Selected);
                this.MyDtoShow.SetDotValuesOrder(rs.Result);
                if (this.AutoExcute)
                {
                    //结束联系执行
                    if (!rs.Ok || rs.Result == null || !rs.Result.Sucessfully)
                        this.AutoExcute = false;
                }
                return;
            }
            if (rs != null)
            {
                this.MyDtoShow.SetMsg($"返回结果：{rs.Ok}\r\n消息：{rs.Message}\r\n解析结果：{(rs.Result == null ? "NULL" : rs.Result.Sucessfully)}\r\n解析异常消息：{(rs.Result == null ? string.Empty : rs.Result.ErrMsg)}");
            }
            Logger.LogDebug("命令执行结束");
        }
        public async Task<IotResult<ZJCBaseDto>> CmdExec(HDZJCCmdCode cmd)
        {
            DateTime detCmdStart = DateTime.Now;
            byte[] data;
            if (cmd == 0x00)
            {
                this.AutoExcute = false;
                return new IotResult<ZJCBaseDto>(false, "请选择命令");
            }
            if (cmd == HDZJCCmdCode.设置降雨明细日期索引)
            {
                #region 设置降雨明细日期索引
                /**********
                 * 地址0062H，0063H，
                 * 地址0062H：D8-D15:年 D0-D7：月
                 * 0063H：D8-D15:日
                 * *************/
                data = new byte[4];
                data[0] = (byte)(this.RainDetailSearchDate.Year - 2000);//注意：2021年，存储的是21；
                data[1] = (byte)this.RainDetailSearchDate.Month;//月份
                data[2] = (byte)this.RainDetailSearchDate.Day;//日
                data[3] = 0x00;//最后一个无效；
                #endregion
            }
            else if (cmd == HDZJCCmdCode.设置故障查询日期索引)
            {
                #region 设置降雨明细日期索引
                /**********
                 * 地址0062H，0063H，
                 * 地址0062H：D8-D15:年 D0-D7：月
                 * 0063H：D8-D15:日
                 * *************/
                data = new byte[4];
                data[0] = (byte)(this.AlarmDetailSearchDate.Year - 2000);//注意：2021年，存储的是21；
                data[1] = (byte)this.AlarmDetailSearchDate.Month;//月份
                data[2] = (byte)this.AlarmDetailSearchDate.Day;//日
                data[3] = 0x00;//最有一个无效；
                #endregion
            }
            else if (cmd == HDZJCCmdCode.设置降雨明条号细索引)
            {
                #region 设置降雨明条号细索引
                data = StringHelper.UShortToBytes(this.RainDetailStartIndex);//该函数调用的返回刚好2个字节，且高位在前，低位在后
                #endregion
            }
            else if (cmd == HDZJCCmdCode.设置干沉降条号索引条号)
            {
                #region 设置降雨明条号细索引
                data = StringHelper.UShortToBytes(this.DryDepDetailStartIndex);//该函数调用的返回刚好2个字节，且高位在前，低位在后
                #endregion
            }
            else if (cmd == HDZJCCmdCode.自定义地址查询)
            {
                #region 自定义地址查询
                data = new byte[2];
                data[0] = (byte)this.DryDepDetailStartIndex;
                data[1] = (byte)this.DryDepDetailSearchCnt;
                #endregion
            }
            else
            {
                data = new byte[] { };
            }
            IotResult<ZJCBaseDto> rs = await service.CmdExec_WaitReply(cmd, data);
            Logger.LogDebug($"完成[{EnumHelper.GetEnumDescription(this.CommunicationMode_Selected)}]通讯的[{EnumHelper.GetEnumDescription(cmd)}]命令：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")},共耗时{(DateTime.Now - detCmdStart).TotalMilliseconds.ToString("#########0.###")}毫秒");
            return rs;
            //IotResult<ZJCBaseDto> rs = await service.CmdExec_Normal(this.MacNo, cmd, data);
            //this.MyDtoShow.SetDotValuesOrder(rs.Result);
        }
        CtrlHDZJCService service { get; set; } = null;
        
        public string MacConnectedStatus
        {
            get
            {
                //return this.service != null && this.service.TranService != null && this.service.TranService.IsConnected;
                if (this.service != null && this.service.TranService != null && this.service.TranService.IsConnected)
                    return "已连接";
                return "断开";
            }
        }
        public void DisConnectInstrument()
        {
            this.service?.TranService?.Dispose();
        }

    }
}
