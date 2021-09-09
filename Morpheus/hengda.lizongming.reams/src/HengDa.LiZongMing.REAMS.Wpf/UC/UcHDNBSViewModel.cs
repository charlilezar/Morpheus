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
using System.ComponentModel;
using PropertyChanged;

namespace HengDa.LiZongMing.REAMS.Wpf.UC
{
    [AddINotifyPropertyChangedInterface]
    public class UcHDNBSViewModel : Screen
    {
        const string EXCUTINGVIEW = "执行中...";
        const string EXCUTEVIEW = "执行";
        private ILoggerFactory loggerFactory;
        private ILogger Logger;
        private readonly IServiceProvider ServiceProvider;
        private IWindowManager windowManager;
        #region 发送命令所需参数
        /// <summary>
        /// 采样体积设定
        /// </summary>
        public int SamplingVSet { get; set; }
        /// <summary>
        /// 采样时长设置，分钟部分
        /// </summary>
        public short SampllingTimeLongSetMinutes { get; set; }
        /// <summary>
        /// 采样时长设置，小时部分
        /// </summary>
        public short SampllingTimeLongSetHours { get; set; }
        /// <summary>
        /// 瞬时流量设定
        /// </summary>
        public decimal SampllingFlowRateSet { get; set; }
        /// <summary>
        /// 滤膜ID
        /// </summary>
        public string LuMoID { get; set; } = string.Empty;
        /// <summary>
        /// 时间设置所需参数
        /// </summary>
        public DateTime InstrumentTimeSet { get; set; }
        /// <summary>
        /// 查询的文件号
        /// </summary>
        public ushort FileNo { get; set; } = 1;
        //public bool CmdParameter1_Visibility { get; set; }
        //public bool CmdParameter2_Visibility { get; set; }
        public bool AutoExcute { get; set; } = false;//是否自动执行，如果该变量为真的话，会一直执行命令
        public string ExcuteStateView { get; set; } = EXCUTEVIEW;
        public List<NameValue<HDNBSCmdCode>> CmdCodeList { get; set; }
        /// <summary>
        /// 当前要执行的命令
        /// </summary>
        public HDNBSCmdCode CmdCode_Selected { get; set; }
        /// <summary>
        /// 设备控制，01启动，02停止，03暂停
        /// </summary>
        public byte InstController { get; set; } = 0x01;
        /// <summary>
        /// 设备运行模式，00定量；01定时
        /// </summary>
        public string WorkMode { get; set; } = "00";
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
        public ushort MacNo { get; set; }
        #endregion
        public Dto.DtoShowEntity MyDtoShow { get; set; }//用于获取的DTO信息转换成List<MyViewEntity>形式

        public UcHDNBSViewModel(IServiceProvider serviceProvider, IWindowManager windowManager, ILoggerFactory loggerFactory)
        {
            this.ServiceProvider =serviceProvider;
            this.windowManager = windowManager;
            this.loggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger<UcHDNBSViewModel>();
            CmdCodeList = EnumHelper.GetEnumListNameValue<HDNBSCmdCode>();
            CommunicationMode = EnumHelper.GetEnumListNameValue<EndPointEnum>();
            MyDtoShow = new DtoShowEntity();
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
                    ServerIP = "192.168.1.244",
                    ServerPort = 503
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
                    ServerIP = "",
                    ServerPort = 502,
                    ServerUrl = ""
                };
                Logger.LogDebug("初始化通讯方式：" + EnumHelper.GetEnumDescription(EndPointEnum.MQTT));
            }
            else
            {
                Logger.LogDebug("暂不支持通讯方式：" + EnumHelper.GetEnumDescription(this.CommunicationMode_Selected));
                this.MyDtoShow.Msg = "目前通信方式只支持TCP、UART、MQTT。";
                return false;
            }
            service = new CtrlHDNBSService(loggerFactory, config);

            //var config = new Aming.DTU.Config.TcpClientConfig()
            //{
            //    ServerPort= 8881,
            //    ServerIP = "192.168.1.82"
            //    //ServerIP = "192.168.1.82"
            //};

            //service = new CtrlHDNBSService(loggerFactory, config);
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
                catch(Exception ex)
                {
                    this.MyDtoShow.SetMsg("执行出错：" + ex.Message + "(" + ex.Source + ")");
                    break;
                }
                if (!AutoExcute) break;
                else
                {
                    System.Threading.Thread.Sleep(300);
                }
            }
            this.ExcuteStateView = EXCUTEVIEW;
        }

        public async Task CmdExec()
        {
            Logger.LogDebug("命令执行开始");
            LoadConfigAndConn();
            byte[] data;
            #region 获取参数内容
            if(CmdCode_Selected==0x00)
            {
                this.AutoExcute = false;
                this.MyDtoShow.SetMsg("请选择命令！");
                return;
            }
            if (CmdCode_Selected == HDNBSCmdCode.采样启停控制)
            {
                //新增1
                #region 采样启停控制
                if (InstController!=1 && InstController!= 2 && InstController != 3)
                {
                    this.AutoExcute = false;
                    this.MyDtoShow.SetMsg("请指定设备控制方式！");
                    return;
                }
                data = new byte[] { InstController };
                #endregion
            }
            else if (CmdCode_Selected == HDNBSCmdCode.时间设置)
            {
                //代码已核实2
                #region 时间设置
                
                data = new byte[6];
                //把获取的时间的年月日时分秒转换成对应的byte，每个都是对应一个byte，所以我们取它低位就可以了
                byte[] bsTemp;
                //年份，2021的话，取21就可以了
                bsTemp = StringHelper.ShortToBytes((short)(this.InstrumentTimeSet.Year - 2000));
                data[0] = bsTemp[1];
                //取月份
                bsTemp = StringHelper.ShortToBytes((short)this.InstrumentTimeSet.Month);
                data[1] = bsTemp[1];//这里取低位就可以了；
                //取日期
                bsTemp = StringHelper.ShortToBytes((short)this.InstrumentTimeSet.Day);
                data[2] = bsTemp[1];
                //取小时
                bsTemp = StringHelper.ShortToBytes((short)this.InstrumentTimeSet.Hour);
                data[3] = bsTemp[1];
                //取分钟
                bsTemp = StringHelper.ShortToBytes((short)this.InstrumentTimeSet.Minute);
                data[4] = bsTemp[1];
                //取秒
                bsTemp = StringHelper.ShortToBytes((short)this.InstrumentTimeSet.Second);
                data[5] = bsTemp[1];
                #endregion
            }
            else if (CmdCode_Selected == HDNBSCmdCode.NBS运行模式设置)
            {
                //新增3
                #region 运行模式设置
                if (this.WorkMode != "00" && this.WorkMode != "01")
                {
                    this.AutoExcute = false;
                    this.MyDtoShow.SetMsg("请指定设备设备运行模式！");
                    return;
                }
                data = this.WorkMode.HexStrToHexByte();//返回应该是1个字节
                #endregion
            }
            else if (CmdCode_Selected == HDNBSCmdCode.滤膜编号设定)
            {
                //代码已核实4
                #region 滤膜ID写入
                /**************
                 * 滤膜编号为19位的ASCII码字符串，该字符串只能包含阿拉伯数字0到9 ，且必须是19位，若不足19位，
                 * 则需要前面用‘0’补全。若收到的编号不是所规定的格式，可能导致操作失败
                 * ***********/
                if (this.LuMoID.Length == 0)
                {
                    this.AutoExcute = false;
                    this.MyDtoShow.SetMsg("请输入滤膜ID！");
                    return;
                }
                if (this.LuMoID.Length > 19)
                {
                    this.AutoExcute = false;
                    this.MyDtoShow.SetMsg("滤膜ID不能超过19位！");
                    return;
                }
                char[] arrID = this.LuMoID.PadLeft(19, '0').ToCharArray();
                data = new byte[19];//传入固定是19位的；
                byte bValue;
                for (int i = 0; i < arrID.Length; i++)
                {
                    bValue = (byte)arrID[i];
                    if (bValue != 0x00 && (bValue < 48 || bValue > 57))
                    {
                        this.AutoExcute = false;
                        this.MyDtoShow.SetMsg("滤膜ID每一位必须是0~9的数字！");
                        return;
                    }
                    data[i] = bValue;
                }
                #endregion
            }
            else if (CmdCode_Selected == HDNBSCmdCode.采样流量设定)
            {
                //代码已核实5
                #region 采样瞬时流量设定
                // 根据协议要求，设备中该值得存储范围是4800-10000，对应的实际值范围是480-1000
                if (this.SampllingFlowRateSet < 480M || this.SampllingFlowRateSet > 1000M)
                {
                    this.AutoExcute = false;
                    this.MyDtoShow.SetMsg("请正确设置流量，必须是480~1000！");
                    return;
                }
                short iValue = (short)(this.SampllingFlowRateSet * 10);//实际值=设备存储值/10；
                data = StringHelper.ShortToBytes(iValue);
                #endregion
            }
            else if (CmdCode_Selected == HDNBSCmdCode.采样时长设定)
            {
                //代码已核实6
                #region 采样时长设定
                data = new byte[3];
                //读取小时部分，气溶胶的范围是0-99；
                byte[] bsTmp = StringHelper.ShortToBytes(this.SampllingTimeLongSetHours);
                data[0] = bsTmp[0];
                data[1] = bsTmp[1];
                //读取分钟部分
                data[2] =(byte)this.SampllingTimeLongSetMinutes;//分钟只取一位就可以了
                #endregion
            }
            else if (CmdCode_Selected == HDNBSCmdCode.采样体积设定)
            {
                //代码已核实7
                #region 采样体积设定
                int iValue = this.SamplingVSet * 10;//输入的值需要乘以10
                byte[] bsValue = StringHelper.IntToBit(iValue);
                //注意StringHelper.IntToBit获取的字节是高位在后，低位在前的，所以要转换一下
                data = new byte[4];
                data[0] = bsValue[3];
                data[1] = bsValue[2];
                data[2] = bsValue[1];
                data[3] = bsValue[0];
                #endregion
            }
            else if (CmdCode_Selected == HDNBSCmdCode.读取文件)
            {
                //代码已核实16
                #region 读取文件
                data = StringHelper.UShortToBytes(this.FileNo);
                #endregion
            }
            else
            {
                data = new byte[] { };
            }
            #endregion
            IotResult<NBSBaseDto> rs = await service.CmdExec_Normal(this.MacNo,CmdCode_Selected, data);
            this.MyDtoShow.SetDotValuesOrder(rs.Result);
            Logger.LogDebug("命令执行结束");
            OnPropertyChanged("MacConnectedStatus");
        }

        CtrlHDNBSService service { get; set; } = null;
        /// <summary>
        /// 界面忙
        /// </summary>
        public bool IsBusy
        {
            get;
            set;
        }
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
            OnPropertyChanged("MacConnectedStatus");
        }

    }
}
