
using HengDa.LiZongMing.REAMS.CtrlServices;
using HengDa.LiZongMing.REAMS.Devices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Threading;
using HengDa.LiZongMing.REAMS;
using Aming.DTU.Config;
using HengDa.LiZongMing.REAMS.Wpf.Dto;
using Aming.Core;
using HengDa.LiZongMing.REAMS.Devices.Dtos;

namespace HengDa.LiZongMing.REAMS.DataSave.ZCQ
{
    public class ZCQDataSaveService : ISingletonDependency
    {
        private readonly IServiceProvider _serviceProvider;
        private ILoggerFactory _loggerFactory;
        public ILogger Logger { get; set; }
        CtrlHDZCQService _ZCQService = null;
        /// <summary>
        /// 当前设备信息对象，其他对象可能回来读取该值
        /// </summary>
        public DeviceDto _Device { get; set; }
        string _DeviceDesc
        {
            get
            {

                return _Device == null ? "device is empty" : $"{_Device.ProductName},{_Device.SN}";
            }
        }
        #region 公共属性
        /// <summary>
        /// 暂停线程执行（非退出），有时候因调试等原因可能希望采集功能暂停一下，这个时候可以通过控制属性实现
        /// </summary>
        public bool Pause { get; set; }
        #endregion
        public ZCQDataSaveService(
             IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory,
            CtrlHDZCQService zCQService)
        {
            this._ZCQService = zCQService;
            this._serviceProvider = serviceProvider;
            this._loggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger<ZCQDataSaveService>();
        }
        #region 数据采集过程
        HDZCQCmdCode _HDZCQCmdCode =HDZCQCmdCode.瞬时数据查询;
        public int DynamicTaskDelay = 2000;
        byte[] _Datas = null;
        public void SetNextCmd(HDZCQCmdCode cmd)
        {
            if (cmd == HDZCQCmdCode.读取文件)
            {
                this._FileNo++;//如果下次还要读的话，则直接+1
                //如果是读取文件的，则一直读取，直至设备上的文件记录都被存入数据库为主；
                this._HDZCQCmdCode = HDZCQCmdCode.读取文件;
                this._Datas = StringHelper.ShortToBytes(this._FileNo);
                this.DynamicTaskDelay = 2000;//500毫秒读取一次
            }
            else
            {
                this.DynamicTaskDelay = 3000;
                switch (cmd)
                {
                    case HDZCQCmdCode.瞬时数据查询:
                        this._HDZCQCmdCode = HDZCQCmdCode.故障查询;
                        break;
                    case HDZCQCmdCode.故障查询:
                        this._HDZCQCmdCode = HDZCQCmdCode.设置信息查询;
                        break;
                    case HDZCQCmdCode.设置信息查询:
                        this._HDZCQCmdCode = HDZCQCmdCode.工作状态查询;
                        break;
                    default:
                        this._HDZCQCmdCode = HDZCQCmdCode.瞬时数据查询;
                        break;
                }
            }
        }
        /// <summary>
        /// 开始气碘数据采集
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="device"></param>
        public void StartAsync(DeviceDto device)
        {
            /*************
             *  * 气碘设备数据采集逻辑
            * 1、该设备需要人驱动后或根据自定义时间后才会采集，通常是空闲状态的；
            * 2、读取实时状态，每隔1秒读取一次：如果工作状态从运行中变为停止了，则读取一次历史记录；
            * 2.1、实时状态3次读取：当前设置值、瞬时状态、报警状态、作业状态，分别读取；这些数据是实时更新远程数据库的表结构；
            * 2.2、如果设备是空闲状态的，则读取频率低一点，如果是作业中的则读取频率高一点。这个具体频率值可以在远程数据库中配置；
            * 3、历史数据采集：读取历史记录的，基本上只要软件不关闭，那就不会出现少读的情况，所以我们只要在软件启动时去判断一下数据库中的历史数据是否少了就可以了；
            * 3.1、判断历史数据是否少了的方式有3种；
            * 3.1.1第一种：每次程序启动时读取前20条数据（这个可以根据上次采集软件更新[更新工作状态查询]的时间来判断），读取后判断每个滤膜编号来判断是否应存入了历史数据库了，这个方案慢但数据可以
            * 更完整的记录。
            * 3.1.2第二种：查询数据库中总共有几条历史数据，再查询设备当前历史数据记录，差额就是我们要去补的数量。这个方法快一点，但是如果有人在设备中删除了历史记录那就
            * 会造成数据不完整。
            * 3.1.3第三种：每次软件开启循环从文件1开始读取，如果该文件已经存在了，则不用再继续读取了。如果不存在则继续往前读，直至数据库中已经存在了。
            * 3.1.4目前采取第三种方式；
            * 3.1.5由于前几种方式同步的话，设备采集最好用同步等待的处理，现在统一异步不等待，所以逻辑上要稍微调整，就是文件读取发送之后
             * *************************/
            this._Device = device;//存储设备唯一标识，后面上传数据会用到
            //初始化设备连接
            var config = device.GetConfig();
            //this._ZCQService = new CtrlHDZCQService(this._loggerFactory, config);
            this._ZCQService.Init(this._loggerFactory, config);
            this._ZCQService.OnUniMessageReceivedCallBack = this.OnUniMessageReceivedCallBack;
            this._FileNo = 0;//初始化文件的读取序号
            this.SetNextCmd(HDZCQCmdCode.读取文件);//每次开启先同步历史文件数据；
            AsyncHelper.RunSync(async () =>
            {
                while (true)
                {
                    Logger.LogWarning("ZCQDataSaveService working");
                    //这里去除了 using (var scope = _serviceProvider.CreateScope())的代码，不知道有什么用
                    if (this.Pause)
                    {
                        //添加日志
                        Logger.LogDebug($"设备[{this._DeviceDesc}]当前为暂停采集。");
                        //此时暂停了该设备的采集任务
                        await Task.Delay(1000);
                        continue;
                    }
                    //确保端口打开，如果已经打开，TryOpenAsync直接会返回，不会重复去打开
                    if (!await this._ZCQService.TranService.TryOpenAsync())
                    {
                        //添加日志，错误内容TryOpenAsync已经写入了
                        Logger.LogDebug($"设备[{this._DeviceDesc}]打开失败。");
                        await Task.Delay(3000);//如果设备连接失败，可能是配置错了，也可能是设备未连接，休眠一下再尝试打开
                        continue;
                    }
                    //开始采集
                    if(this._HDZCQCmdCode==HDZCQCmdCode.读取文件)
                    {
                        await this._ZCQService.CmdExecWaitReply(this._HDZCQCmdCode, this._Datas);
                    }
                    else
                    {
                        await this._ZCQService.CmdExec(this._HDZCQCmdCode, this._Datas);
                    }
                    this.SetNextCmd(this._HDZCQCmdCode);//执行下一个命令
                    //这里要判断如果设备状态从空闲转到工作中，则更新频率要增加；
                    await Task.Delay(this.DynamicTaskDelay);
                }
            });
        }
        #endregion
        #region 同步文件记录
        /// <summary>
        /// 读取文件的记录的文件号，该值如果是5，则表示要读取从1~5的文件记录
        /// </summary>
        short _FileNo = 0;
        public void StopFileRead()
        {
            //终止读取文件，继续循环读取
            this._HDZCQCmdCode = HDZCQCmdCode.瞬时数据查询;
        }
        #endregion
        #region 数据反馈相应
        bool _ZCQWorking = false;//设备是否正在工作
        public void OnUniMessageReceivedCallBack(MqttClientConfig config, UniMessageBase uniMessageBase, ZCQBaseDto zCQBaseDto)
        {
            Logger.LogWarning("TODO OnUniMessageReceivedCallBack");
            if(zCQBaseDto is ZCQWorkStatusDto dtoStatus)
            {
                if(this._ZCQWorking && dtoStatus.WorkStatus==0)
                {
                    //此时从工作状态转换为了空闲，则要读取文件

                }
            }
        }
        #endregion
    }
}
