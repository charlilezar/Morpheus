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
    public class UcHDRNSViewModel : Screen
    {
        const string EXCUTINGVIEW = "执行中...";
        const string EXCUTEVIEW = "执行";
        private ILoggerFactory loggerFactory;
        private ILogger Logger;
        private readonly IServiceProvider ServiceProvider;
        private IWindowManager windowManager;
        #region 发送命令所需参数
        /// <summary>
        /// 下雨加热温度阀值
        /// </summary>
        public decimal RainingTmpMaxSet { get; set; }
        /// <summary>
        /// 不下雨加热温度阀值
        /// </summary>
        public decimal UnRainTmpMaxSet { get; set; }
        public bool AutoExcute { get; set; } = false;//是否自动执行，如果该变量为真的话，会一直执行命令
        public string ExcuteStateView { get; set; } = EXCUTEVIEW;
        public List<NameValue<HDRNSCmdCode>> CmdCodeList { get; set; }
        /// <summary>
        /// 当前要执行的命令
        /// </summary>
        public HDRNSCmdCode CmdCode_Selected { get; set; }
        
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
        #region DTO数据
        /// <summary>
        /// 获取的感雨器实时数据
        /// </summary>
        public DataGridResult<Dto.RNSRealDataDto> _MyRNSRealDataDtos { get; set; } = new DataGridResult<RNSRealDataDto>();
        #endregion
        public Dto.DtoShowEntity MyDtoShow { get; set; }//用于获取的DTO信息转换成List<MyViewEntity>形式

        public UcHDRNSViewModel(IServiceProvider serviceProvider, IWindowManager windowManager, ILoggerFactory loggerFactory)
        {
            this.ServiceProvider = serviceProvider;
            this.windowManager = windowManager;
            this.loggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger<UcHDRNSViewModel>();
            CmdCodeList = EnumHelper.GetEnumListNameValue<HDRNSCmdCode>();
            CommunicationMode = EnumHelper.GetEnumListNameValue<EndPointEnum>();
            MyDtoShow = new DtoShowEntity(35);
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
                    ServerPort = 504
                };
                Logger.LogDebug("初始化通讯方式：" + EnumHelper.GetEnumDescription(EndPointEnum.TCP));
            }
            else if (this.CommunicationMode_Selected == EndPointEnum.UART)
            {
                config = new Aming.DTU.Config.UARTConfig()
                {
                    Port = "COM4"
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
            service = new CtrlHDRNSService(loggerFactory, config);

            //var config = new Aming.DTU.Config.TcpClientConfig()
            //{
            //    ServerPort= 8881,
            //    ServerIP = "192.168.1.82"
            //    //ServerIP = "192.168.1.82"
            //};

            //service = new CtrlHDZCQService(loggerFactory, config);
            return true;

        }
        public async Task StartCmdExec()
        {
            //该函数目的是用于页面调用，测试通讯效果
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
            Logger.LogDebug("命令执行开始");
            LoadConfigAndConn();
            IotResult<RNSBaseDto> rs = null;
            if (this.CmdCode_Selected == HDRNSCmdCode.设置下雨及不下雨加热温度阀值)
            {
                #region 组合设置_同时设置下雨和不下雨的加温上限
                rs = await CmdExec(HDRNSCmdCode.设置下雨加热温度阀值);
                if (rs.Ok && rs.Result != null && rs.Result.Sucessfully)
                    rs = await CmdExec(HDRNSCmdCode.设置不下雨加热温度阀值);
                #endregion
            }
            else
            {
                //此时为普通信息，直接显示结果
                rs = await CmdExec(this.CmdCode_Selected);
            }
            if (rs != null && rs.Result != null)
            {
                this.MyDtoShow.SetDotValuesOrder(rs.Result);
            }
            Logger.LogDebug("命令执行结束");
            OnPropertyChanged("MacConnectedStatus");
        }
        public async Task<IotResult<RNSBaseDto>> CmdExec(HDRNSCmdCode cmd)
        {
            byte[] data;
            if (cmd == 0x00)
            {
                this.AutoExcute = false;
                return new IotResult<RNSBaseDto>(false, "请选择命令");
            }
            if (cmd == HDRNSCmdCode.设置下雨加热温度阀值)
            {
                #region 设置下雨加热温度阀值
                /**********
                 * 写入单个地址000eH，类型为ushort；
                 * 该值为实际温度*10；也就是说实际温度精确到小数点后1位；
                 * 注意：按照modbus协议高位在前，低位在后
                 * *************/
                //高位在前，地位在后，StringHelper.UShortToBytes转换后也是高位在前低位在后的
                data = StringHelper.UShortToBytes((ushort)(this.RainingTmpMaxSet * 10));
                #endregion
            }
            else if (cmd == HDRNSCmdCode.设置不下雨加热温度阀值)
            {
                #region 设置不下雨加热温度阀值
                /**********
                * 写入单个地址0010H，类型为ushort；
                * 该值为实际温度*10；也就是说实际温度精确到小数点后1位；
                * 注意：按照modbus协议高位在前，低位在后
                * *************/
                //高位在前，地位在后，StringHelper.UShortToBytes转换后也是高位在前低位在后的
                data = StringHelper.UShortToBytes((ushort)(this.UnRainTmpMaxSet * 10));
                #endregion
            }
            else
            {
                data = new byte[] { };
            }
            return await service.CmdExec(this.MacNo, cmd, data);
        }
        CtrlHDRNSService service { get; set; } = null;
        
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
