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
using System.IO;
using HengDa.LiZongMing.REAMS.Devices;
using Newtonsoft.Json;

namespace HengDa.LiZongMing.REAMS.Wpf.UC
{
    [AddINotifyPropertyChangedInterface]
    public class UcHDZCQViewModel : Screen
    {
        const string EXCUTINGVIEW = "执行中...";
        const string EXCUTEVIEW = "执行";
        private ILoggerFactory loggerFactory;
        private ILogger Logger;
        private readonly IServiceProvider ServiceProvider;
        private IWindowManager windowManager;
        private  CtrlHDZCQService service { get; set; } = null;
        #region MQTT监听
        //TODO:临时关掉
        //public string MqttConnectedStatus { get; set; } = "断开";
        //public void StartMqttServerListen()
        //{
        //    if (this._MqttServerListen == null)
        //    {
        //        
        //        this._MqttServerListen = new Aming.DTU.MqttListen(this.loggerFactory, new MqttConfig()
        //        {
        //            ServerIP = "api.hengda.show",
        //            ServerPort = 1883,
        //            UserName = "88888888",
        //            Password = "88888888",
        //            PubTopicUp = "/mingdtu/dtu/ZCQ001/hex/up",
        //            PubTopicDown = "/mingdtu/dtu/ZCQ001/hex/down"
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
        ////Aming.DTU.MqttListen _MqttServerListen { get; set; }
        //public async Task<string> CreateText2MQTTServer(byte[] data)
        //{
        //    //这里反正测试，直接读取实时数据即可
        //    IotResult<ZCQBaseDto> rs = await service.CmdExecRaw("MQTT客户端请求",data);
        //    if (rs != null && rs.Result != null)
        //    {
        //        this.MyDtoShow.SetDotValuesOrder(rs.Result);//TODO:临时测试
        //        return rs.Result.SourceData.ByteToHexStr(); //Newtonsoft.Json.JsonConvert.SerializeObject((ZJCRealDataDto)rs.Result);
        //    }
        //    this.MyDtoShow.SetMsg("返回为空！");//TODO:临时测试
        //    return string.Empty;
        //}
        //public void MqttServerConnectedChanged(Aming.DTU.MqttListen listen, bool blNewStatus)
        //{
        //    this.MqttConnectedStatus = blNewStatus ? "连接中" : "断开";
        //}
        #endregion
        #region 发送命令所需参数
        public string CmdParameter1 { get; set; } = "0";//发送领的参数1
        public string CmdParameter2 { get; set; } = "0";//发送领的参数2


        //public bool CmdParameter1_Visibility { get; set; }
        //public bool CmdParameter2_Visibility { get; set; }
        public bool AutoExcute { get; set; } = false;//是否自动执行，如果该变量为真的话，会一直执行命令
        public string ExcuteStateView { get; set; } = EXCUTEVIEW;
        public List<NameValue<HDZCQCmdCode>> CmdCodeList { get; set; }
        /// <summary>
        /// 当前要执行的命令
        /// </summary>
        public HDZCQCmdCode CmdCode_Selected { get; set; }
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
        public EndPointEnum CommunicationMode_Selected { get; set; } = EndPointEnum.MQTT;
        #endregion
        public Dto.DtoShowEntity MyDtoShow { get; set; }//用于获取的DTO信息转换成List<MyViewEntity>形式

        public UcHDZCQViewModel(IServiceProvider serviceProvider, 
            IWindowManager windowManager, 
            ILoggerFactory loggerFactory, 
            CtrlHDZCQService ctrlHDZCQService)
        {
            this.service = ctrlHDZCQService;
            this.ServiceProvider = serviceProvider;
            this.windowManager = windowManager;
            this.loggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger<UcHDZCQViewModel>();
            CmdCodeList = EnumHelper.GetEnumListNameValue<HDZCQCmdCode>();
            CommunicationMode = EnumHelper.GetEnumListNameValue<EndPointEnum>();
            MyDtoShow = new DtoShowEntity();

            //重点，
            Aming.Tools.IocHelper.ServiceProvider = serviceProvider;
            //TODO:临时关掉
            //StartMqttServerListen();
            //LoadConfigAndConn();
            LoadConfigByJson( "devices_config/devices_zcq.json");
        }


        private  Task LoadConfigByJson(string jsonFile = "devices_config/devices_zcq.json")
        {

            var json = File.ReadAllText(jsonFile);
            var device = JsonConvert.DeserializeObject<Device>(json);


            var config = device.GetConfig();
           // service = new CtrlHDZCQService(loggerFactory, config);
            service.OnUniMessageReceivedCallBack = (config, msg,dto) =>
            {
                //开始显示数据
                this.MyDtoShow.SetDotValuesOrder(dto);
            };
            return Task.CompletedTask;
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
                    ServerIP = "api.hengda.show",
                    ServerPort = 1883,
                    UserName = "88888888",
                    Password = "88888888",
                    //SubTopic = "/mingdtu/dtu/3c6105156c3c/hex/up",
                    //PubTopicUp = "/mingdtu/dtu/3c6105156c3c/hex/up",
                    //PubTopicDown = "/mingdtu/dtu/3c6105156c3c/hex/down"
                    SubTopic = "/mingdtu/dtu/ZCQ001/hex/up",
                    PubTopicUp = "/mingdtu/dtu/ZCQ001/hex/up",
                    PubTopicDown = "/mingdtu/dtu/ZCQ001/hex/down"
                };
                Logger.LogDebug("初始化通讯方式：" + EnumHelper.GetEnumDescription(EndPointEnum.MQTT));
            }
            else
            {
                Logger.LogDebug("暂不支持通讯方式：" + EnumHelper.GetEnumDescription(this.CommunicationMode_Selected));
                this.MyDtoShow.Msg = "目前通信方式只支持TCP、UART、MQTT。";
                return false;
            }
            //关闭原来的连接
            if (this.service != null && this.service.TranService != null )
            {
                this.service.TranService.Dispose();
                this.service = null;
            }
            service = new CtrlHDZCQService(loggerFactory, config);

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
                    System.Threading.Thread.Sleep(300);
                }
            }
            this.ExcuteStateView = EXCUTEVIEW;
        }

        public async Task CmdExec()
        {
             await this.CmdExec(this.CmdCode_Selected);
        }
        public async Task CmdExec(HDZCQCmdCode cmd)
        {
            DateTime detCmdStart = DateTime.Now;
            Logger.LogDebug("命令执行开始");
            //LoadConfigAndConn(); 不应该每次都建立连接
            byte[] data;
            #region 获取参数内容
            if (CmdCode_Selected == 0x00)
            {
                this.AutoExcute = false;
                this.MyDtoShow.SetMsg("请选择命令！");
                return ;
            }
            if (CmdCode_Selected == HDZCQCmdCode.读取文件)
            {
                short iValue;
                if (!short.TryParse(this.CmdParameter1, out iValue))
                {
                    this.AutoExcute = false;
                    this.MyDtoShow.SetMsg("请正确设置文件序号！");
                    return ;
                }
                data = StringHelper.ShortToBytes(iValue);
            }
            else if (CmdCode_Selected == HDZCQCmdCode.开始采样)
            {
                if (CmdDatas != "00" && CmdDatas != "01")
                {
                    this.AutoExcute = false;
                    this.MyDtoShow.SetMsg("请指定采样方式！");
                    return ;
                }
                data = CmdDatas.HexStrToHexByte();
            }
            else if (CmdCode_Selected == HDZCQCmdCode.采样瞬时流量设定)
            {
                #region 采样瞬时流量设定
                short iValue;
                if (!short.TryParse(this.CmdParameter1, out iValue))
                {
                    this.AutoExcute = false;
                    this.MyDtoShow.SetMsg("请正确设置流量！");
                    return ;
                }
                if (iValue < 20 || iValue > 260)
                {
                    this.AutoExcute = false;
                    this.MyDtoShow.SetMsg("请正确设置流量，必须是20~260！");
                    return ;
                }
                data = StringHelper.ShortToBytes(iValue);
                #endregion
            }
            else if (CmdCode_Selected == HDZCQCmdCode.采样时间设定)
            {
                #region 采样时间设定
                data = new byte[3];
                short iValue;
                //读取小时部分
                if (!short.TryParse(this.CmdParameter1, out iValue) || iValue < 0 || iValue > 999)
                {
                    this.AutoExcute = false;
                    this.MyDtoShow.SetMsg("请正确设置小时，必须是0~999！");
                    return ;
                }
                byte[] bsTmp = StringHelper.ShortToBytes(iValue);
                data[0] = bsTmp[0];
                data[1] = bsTmp[1];
                //读取分钟部分
                if (!short.TryParse(this.CmdParameter2, out iValue) || iValue < 0 || iValue > 60)
                {
                    this.AutoExcute = false;
                    this.MyDtoShow.SetMsg("请正确设置分钟，必须是0~60！");
                    return ;
                }
                data[2] = (byte)iValue;//分钟只取一位就可以了，而且只要低位就满足了
                #endregion
            }
            else if (CmdCode_Selected == HDZCQCmdCode.定量采样量设定)
            {
                #region 定量采样量设定
                int iValue;
                if (!int.TryParse(this.CmdParameter1, out iValue) || iValue < 0 || iValue > 980)
                {
                    this.AutoExcute = false;
                    this.MyDtoShow.SetMsg("请输入采样量！");
                    return ;
                }
                iValue = iValue * 10;//输入的值需要乘以10
                byte[] bsValue = StringHelper.IntToBit(iValue);
                //注意StringHelper.IntToBit获取的字节是高位在后，低位在前的，所以要转换一下
                data = new byte[4];
                data[0] = bsValue[3];
                data[1] = bsValue[2];
                data[2] = bsValue[1];
                data[3] = bsValue[0];
                #endregion
            }
            else if (CmdCode_Selected == HDZCQCmdCode.时间设置)
            {
                #region 时间设置
                DateTime detValue;
                if (!DateTime.TryParse(this.CmdParameter1, out detValue))
                {
                    this.AutoExcute = false;
                    this.MyDtoShow.SetMsg("请正确输入时间！");
                    return ;
                }
                data = new byte[6];
                //把获取的时间的年月日时分秒转换成对应的byte，每个都是对应一个byte，所以我们取它低位就可以了
                byte[] bsTemp;
                //年份，2021的话，取21就可以了
                bsTemp = StringHelper.ShortToBytes((short)(detValue.Year - 2000));
                data[0] = bsTemp[1];
                //取月份
                bsTemp = StringHelper.ShortToBytes((short)detValue.Month);
                data[1] = bsTemp[1];//这里取低位就可以了；
                //取日期
                bsTemp = StringHelper.ShortToBytes((short)detValue.Day);
                data[2] = bsTemp[1];
                //取小时
                bsTemp = StringHelper.ShortToBytes((short)detValue.Hour);
                data[3] = bsTemp[1];
                //取分钟
                bsTemp = StringHelper.ShortToBytes((short)detValue.Minute);
                data[4] = bsTemp[1];
                //取秒
                bsTemp = StringHelper.ShortToBytes((short)detValue.Second);
                data[5] = bsTemp[1];
                #endregion
            }
            else if (CmdCode_Selected == HDZCQCmdCode.滤膜编号设定)
            {
                #region 滤膜ID写入
                /**************
                 * 滤膜编号为19位的ASCII码字符串，该字符串只能包含阿拉伯数字0到9 ，且必须是19位，若不足19位，
                 * 则需要前面用‘0’补全。若收到的编号不是所规定的格式，可能导致操作失败
                 * ***********/
                if (this.CmdParameter1.Length == 0)
                {
                    this.AutoExcute = false;
                    this.MyDtoShow.SetMsg("请输入滤膜ID！");
                    return ;
                }
                if (this.CmdParameter1.Length > 19)
                {
                    this.AutoExcute = false;
                    this.MyDtoShow.SetMsg("滤膜ID不能超过19位！");
                    return ;
                }
                char[] arrID = this.CmdParameter1.PadLeft(19, '0').ToCharArray();
                data = new byte[19];//传入固定是19位的；
                byte bValue;
                for (int i = 0; i < arrID.Length; i++)
                {
                    bValue = (byte)arrID[i];
                    if (bValue != 0x00 && (bValue < 48 || bValue > 57))
                    {
                        this.AutoExcute = false;
                        this.MyDtoShow.SetMsg("滤膜ID每一位必须是0~9的数字！");
                        return ;
                    }
                    data[i] = bValue;
                }
                #endregion
            }
            else if (CmdCode_Selected == HDZCQCmdCode.采样次数)
            {
                #region 采样次数
                //例：十六进制值：00 00 00 06 代表6次     最大9次最小1次
                int iValue;
                if (!int.TryParse(this.CmdParameter1, out iValue) || iValue > 9 || iValue < 1)
                {
                    this.AutoExcute = false;
                    this.MyDtoShow.SetMsg("请正确采样次数，最小为1，最大为9！");
                    return ;
                }
                byte[] bsValue = StringHelper.IntToBit(iValue);
                //注意StringHelper.IntToBit获取的字节是高位在后，低位在前的，所以要转换一下
                data = new byte[4];
                data[0] = bsValue[3];
                data[1] = bsValue[2];
                data[2] = bsValue[1];
                data[3] = bsValue[0];
                #endregion
            }

            else if (CmdCode_Selected == HDZCQCmdCode.定时采样间隔)
            {
                #region 定时采样间隔
                //例：十六进制值：02 20 代表2和小时32分钟
                data = new byte[2];
                if (!byte.TryParse(this.CmdParameter1, out data[0]))
                {
                    this.AutoExcute = false;
                    this.MyDtoShow.SetMsg("请正确设置小时数！");
                    return ;
                }
                if (!byte.TryParse(this.CmdParameter2, out data[1]))
                {
                    this.AutoExcute = false;
                    this.MyDtoShow.SetMsg("请正确设置分钟数！");
                    return ;
                }
                #endregion
            }
            else if (CmdCode_Selected == HDZCQCmdCode.定时启动时间)
            {
                #region 定时启动时间
                //例：十六进制值：11 05 11  11 05  代表17年05月17日17时5分
                DateTime detValue;
                if (!DateTime.TryParse(this.CmdParameter1, out detValue))
                {
                    this.AutoExcute = false;
                    this.MyDtoShow.SetMsg("请正确输入时间！");
                    return ;
                }
                data = new byte[5];//这里只有5个字节，不用传入秒
                //把获取的时间的年月日时分转换成对应的byte，每个都是对应一个byte，所以我们取它低位就可以了
                byte[] bsTemp;
                //年份，2021的话，取21就可以了
                bsTemp = StringHelper.ShortToBytes((short)(detValue.Year - 2000));
                data[0] = bsTemp[1];
                //取月份
                bsTemp = StringHelper.ShortToBytes((short)detValue.Month);
                data[1] = bsTemp[1];//这里取低位就可以了；
                //取日期
                bsTemp = StringHelper.ShortToBytes((short)detValue.Day);
                data[2] = bsTemp[1];
                //取小时
                bsTemp = StringHelper.ShortToBytes((short)detValue.Hour);
                data[3] = bsTemp[1];
                //取分钟
                bsTemp = StringHelper.ShortToBytes((short)detValue.Minute);
                data[4] = bsTemp[1];
                #endregion
            }

            else if (CmdCode_Selected == HDZCQCmdCode.定时启动使能)
            {
                #region 定时启动时间
                //例：01代表定时启动使能 否则代表禁止定时启动
                data = new byte[1];
                if (!byte.TryParse(this.CmdParameter1, out data[0]) || (data[0] != 0x00 && data[0] != 0x01))
                {
                    this.AutoExcute = false;
                    this.MyDtoShow.SetMsg("请正确输入值，0或1！");
                    return ;
                }
                #endregion
            }
            else
            {
                data = new byte[] { };
            }
            #endregion
            var rs = await service.CmdExec(cmd, data);
            Logger.LogDebug($"结束[{EnumHelper.GetEnumDescription(this.CommunicationMode_Selected)}]通讯的[{EnumHelper.GetEnumDescription(cmd)}]命令：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")},共耗时{(DateTime.Now - detCmdStart).TotalMilliseconds.ToString("#########0.###")}毫秒");
            OnPropertyChanged("MacConnectedStatus");
            this.MyDtoShow.SetMsg("发送结束！");
        }


       
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
