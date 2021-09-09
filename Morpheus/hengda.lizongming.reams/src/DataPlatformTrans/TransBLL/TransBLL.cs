using DataPlatformTrans.DataEntitys;
using DataPlatformTrans.DataEntitys.MessageEntity;
using DataPlatformTrans.DataFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataPlatformTrans.DataEntitys.AppConfig;
using DataPlatformTrans.DataEntitys.SpecialCmdEntitys;
using Microsoft.Extensions.Logging;

namespace DataPlatformTrans.TransBLL
{
    public class DataPlatformTransManager : AppRunningBase
    {
        /************
         * 数据上传逻辑处理对象，该对象赋值处理传输过程中的逻辑处理，前天传入对象负责实现功能，其他对象包含如下：
         * 1、_TransConnect：实现数据收发，直接与平台数据对接；
         * 2、_TaskManager：要发送的任务管理，包含超时重发，错误重发，超时时间等
         * 3、_CPDataFactory：数据获取对象：从数据库或其他地方获取相关数据，该对象和上面的而不同，仅实例化不行，需要定义Func，用以获取数据；
         * 4、ShowErr()：统一处理错误消息；
         * 5、ILoggerFactory 日志服务
         * 处理逻辑简单说明
         * 1、先进行身份验证，只有通过了才能发送任何命令（除请求身份验证的命令外）；
         * 2、大部分命令是接受的，就是通过ProReceiveData从平台获取的，所以业务处理都在这个函数中。其他的都是为了实现业务的功能函数
         * 3、一小部分命令是自动上传的，暂时应该就是心跳了，但也有可能其他的也有，目前文档上说的不是很清楚；
         * *************/
        /// <summary>
        /// 身份验证已经成功了
        /// </summary>
        public bool _TokenSuccessfully = false;
        public AppLogger _AppLogger = null;
        /// <summary>
        /// 通讯对象
        /// </summary>
        public TransConnect _TransConnect { get; set; }
        public TaskManager _TaskManager { get; set; }
        CPDataFactory _CPDataFactory { get; set; }
        public DataPlatformTransManager(TransConfig transConfig, CPDataFactory cPDataFactory, AppLogger appLogger)
        {
            //实例化连接对象
            _TransConnect = new TransConnect(transConfig, ProReceiveData);
            //数据提供给者
            this._CPDataFactory = cPDataFactory;
            //实例化任务管理对象
            this._TaskManager = new TaskManager(this._TransConnect, appLogger);
            this._AppLogger = appLogger;
        }
        public void InitTask()
        {
            //添加网络心跳包,这个不用区分站点；
            this._TaskManager.Special_AddHeartbeatMsg();
            //添加各个设备的自动上传命令，这里要上传所有站点的命令
            DeviceEntity device;
            foreach(DataEntitys.AppConfig.StationEntity station in DataEntitys.AppConfig.Stations)
            {
                if (!station.Enabled) continue;
                //超大气溶胶
                CmdReadData cmdAutoSendNBS = new CmdReadData(DeviceEntity.DeviceTypes.NBS);
                cmdAutoSendNBS.SNo = station.SNO;
                cmdAutoSendNBS.CN = (int)TransCNs.上传大流量采样实时数据;
                cmdAutoSendNBS.CreateNewQN();
                device = station.FindDeviceByType(DeviceEntity.DeviceTypes.NBS);
                this._TaskManager.AddTask(new TransCmdController(this._TaskManager, this._TransConnect, this._CPDataFactory, cmdAutoSendNBS, this._AppLogger)
                {
                    _CTime = device == null ? 0 : device.CTime,//循环上传的时间
                    TransCmdType = TransCmdController.TransCmdTypes.Cyclical,//需要一直往上传
                    TaskStatu = DataEntitys.TaskStatus.Activing//直接激活
                }) ;
                //气碘采样
            }
        }
        public async Task<bool> InitTransConnect()
        {
            if(this._TransConnect==null)
            {
                this.ShowErr("TransConnect位空，无法连接平台！");//正常情况是不可能会出现的，_TransConnect在该类实例化时已经声明
                return false;
            }
            RtnMessage msg = await this._TransConnect.ConnectAsync();//这里一定要等待，因为不连接上平台，后面的代码执行是没有意义的；
            if(msg==null)
            {
                this.ShowErr("连接平台时，返回的消息为空！");
                return false;
            }
            if (!msg.Sucessful)
            {
                string strErr = String.IsNullOrEmpty(msg.Msg) ? "连接失败，原因未知" : msg.Msg;
                this.ShowErr($"连接平台时出错：{strErr}");
                return false;
            }
            //连接成功
            return true;
        }
        public override async Task ProListening()
        {
            //读取站点信息
            if(AppConfig.Stations==null || AppConfig.Stations.Count==0)
            {
                Console.WriteLine("无任何站点信息，无法通信！");
                await Task.Delay(10000);//等待载入配置文件，这个环节可以外部程序控制
                return;
            }
            if(!this._TransConnect.Connected)
            {
                this.ShowLog("服务器未连接，尝试连接。");
                //每次只要网络断开了，都要进行身份验证，见协议《《辐射环境自动监测系统数据传输协议规范》.pdf》第7.4说明；
                this._TokenSuccessfully = false;
                if (!await this.InitTransConnect())
                {
                    //如果无法连接平台服务器，则不用执行任何操作了，但建议休眠一会，这种情况一般是断网了
                    await Task.Delay(2000);
                    return;
                }
                this.ShowLog("服务器连接成功，开始身份校验。");
                this._TaskManager.Special_AddTokenTask();
            }
            this._TaskManager.TaskTimeOutCollect();
            this._TaskManager.TaskCollect();//回收已经被删除的任务对象
            this._TaskManager.TaskRefreshCyclical();//刷新那些周期性循环发送的命令
            List<TransCmdController> taskDatas = this._TaskManager.GetActivingTask();
            if (taskDatas != null)
            {
                foreach (TransCmdController cmd in taskDatas)
                {
                    if (!this._TokenSuccessfully && cmd.CN != (int)TransCNs.身份验证)
                    {
                       // this.ShowLog($"因身份验证未通过，命令[{cmd.CN}]暂不发送");
                        //注意身份验证有肯能出现在通讯过程，因过程中可能会断网，恢复后我们就要重新验证，这个时候可能已经有任务等待发送；
                        //如果没有通过身份验证，所有挂起的任务都不要发送
                        continue;
                    }
                    //以异步形式上传
                    cmd.SendTaskAsync();
                    //this.ShowLog($"命令[{cmd.CN}]已发送");
                }
            }
        }
        public void ProReceiveData(string sData, TransConnect transConnect)
        {
            this.ShowLog("收到消息：" + sData);
            string strErr;
            CmdData cmd = AppHelper.TransCMDDecode.DecodeReceivedDta(sData, out strErr);
            if(cmd==null)
            {
                if (String.IsNullOrEmpty(strErr))
                    strErr = $"转换失败，原因未知，字符窜[{sData}]";
                this.ShowErr(strErr);
                return;
            }
            if (cmd.CPData != null && !String.IsNullOrEmpty(cmd.CPData.QN))
            {
                //当数据区域是QN的，则是回复的消息，则直接根据QN找到这个命令，并通知他已经收到回复了
                TransCmdController task = this._TaskManager.FindTaskByQn(cmd.CPData.QN);
                if (task != null)
                    task.Replay_SetReplayed(cmd);
            }
            //具体根据消息内容，处理数据
            if(cmd.ST==91)
            {
                this.ShowLog("收到的消息进行91处理");
                //此时是回复，则要查找它回复的是哪个命令
                this.ProCmdFromServer91(cmd);
            }
            else if(cmd.ST==38)
            {
                this.ShowLog("收到的消息进行38处理");
                //此时为数据平台让本地执行的命令
                this.ProCmdFromServer38(cmd);
            }
            else
            {
                this.ShowErr("收到的消息ST值既不是38也不是91，无法找到处理函数！");
            }
        }
        #region 处理中心发回来消息处理过程
        /// <summary>
        /// 处理数据中心发回的反馈消息，对应的ST=91；
        /// </summary>
        private async void ProCmdFromServer91(CmdData cmd)
        {
            string strQn = cmd.CPData == null ? string.Empty : cmd.CPData.QN;
            TransCmdController task = this._TaskManager.FindTaskByQn(strQn);
            if (task != null)
            {   
                //注意：下面的判断cmd.CPData不会空，如果是空的话task就找不到
                if (task.CN== (int)TransCNs.身份验证)
                {
                    #region 当前返回命令为之前发送的身份验证命令，则判断是否通过
                    if (cmd.CPData.ExeRtn == ExeRtnValues.执行成功)
                    {
                        this.ShowLog("身份验证通过！");
                        this._TokenSuccessfully = true;
                        //此时对方发送了验证成功的标识，则开始上传数据
                        this.InitTask();
                    }
                    else
                    {
                        //此时身份验证不通过
                        this.ShowErr(string.Format("平台返回的身份验证失败：{0}", cmd.CPData.ExeRtn));
                    }
                    #endregion
                }
                else if(task.CN== (int)TransCNs.上传大流量采样实时数据)
                {
                    #region 此时返回的是超大容量气溶胶的上传数据
                    //根据协议中标识，这个命令返回的数据中除了CP无其他了，没有QnRtn和ExpRtn，所以收到数据我们就认为是成功了，开始上传下一个命令了
                    //继续按照规定的频率自动上传
                    #endregion
                }
            }
        }
        private async void ProCmdFromServer38(CmdData cmd)
        {
            this.ShowLog($"收到命令[{cmd.CN},{(TransCNs)cmd.CN}]");
            /*************
             * 除了心跳包的反馈，ST38的都是要本地执行的命令，不是什么回复等信息；
             * *****************/
            if (cmd.CN==6031)
            {
                //网络心跳，身份验证属于整个交互的必须过程，所以单独写了功能函数来处理
                this._TaskManager.Special_HeartbeatMsgReceived();
            }
            else if (cmd.CN == (int)TransCNs.设置监测站时间)
            {
                #region 设置检测站时间
                //首先告诉平台站收到了
                this._TaskManager.Special_ReturnQnRtnCmd(cmd);
                //执行命令
                this._TaskManager.Special_ReturnExeRtnCmd(cmd, this.Exe_SetSystemTime(cmd));
                #endregion
            }
            else if (cmd.CN== (int)TransCNs.完全初始化命令)
            {
                #region 完全初始化命令
                //首先告诉平台站收到了
                this._TaskManager.Special_ReturnQnRtnCmd(cmd);
                //执行初始化命令
                this._TaskManager.Special_ReturnExeRtnCmd(cmd, this.Exe_InitApp(cmd));
                #endregion
            }
            else if(cmd.CN== (int)TransCNs.实时数据上传)
            {
                #region 上传实时数据
                //首先告诉平台站收到了
                this._TaskManager.Special_ReturnQnRtnCmd(cmd);
                //开始上传实时数据
                StartSendRealData();
                #endregion
            }
            else if (cmd.CN == (int)TransCNs.停止察看实时数据)
            {
                #region 终止上传实时数据
                this.StopSendRealData();//终止发送实时数据
                //首先告诉平台站收到了，注意这个命令是9013的，属于通知类回复
                this._TaskManager.Special_ReturnNoticeCmd(cmd);
                //开始上传实时数据
                #endregion
            }
            else if (cmd.CN == (int)TransCNs.设置采样周期)
            {
                #region 设置设备采样周期
                //首先告诉平台站收到了
                this._TaskManager.Special_ReturnQnRtnCmd(cmd);
                //执行初始化命令
                this._TaskManager.Special_ReturnExeRtnCmd(cmd, this.Exe_SetDeviceCTime(cmd));
                #endregion
            }
        }
        #endregion
        #region  执行过程函数
        /// <summary>
        /// 设置自动站系统时间(更新)
        /// </summary>
        /// <param name="cmd"></param>
        public ExeRtnValues Exe_SetSystemTime(CmdData cmd)
        {
            if(cmd.CPData==null)
            {
                this.ShowErr("执行1012命令时，数据包的CP数据为空！");
                return ExeRtnValues.数据包校验错误;
            }
            if (cmd.CPData.CPDataItems == null || cmd.CPData.CPDataItems.Count == 0)
            {
                this.ShowErr("执行1012命令时，数据包的CPDataItem数据为空！");
                return ExeRtnValues.数据包校验错误;
            }
            //该命令
            foreach (CmdCPDataItem item in cmd.CPData.CPDataItems)
            {
                string strValue = item.GetValue("SystemTime");
                if (strValue.Length != 14)
                {
                    this.ShowErr($"执行1012命令时，获取时间内容长度{strValue.Length}不是预期的14位，数据内容为：[{strValue}]！");
                    return ExeRtnValues.数据包校验错误;
                }
                strValue = string.Format("{0}-{1}-{2} {3}:{4}:{5}", strValue.Substring(0, 4), strValue.Substring(4, 2), strValue.Substring(6, 2)
                    , strValue.Substring(8, 2), strValue.Substring(10, 2), strValue.Substring(12, 2));
                DateTime det;
                if (!DateTime.TryParse(strValue, out det))
                {
                    this.ShowErr($"执行1012命令时，获取时间内容{strValue}不是预期的时间格式！");
                    return ExeRtnValues.数据包校验错误;
                }
                //设置指定站点或者所有站点的设备（如果传入的ENo为空则表示所有设备）时钟
                //???这里要发送MQTT来取认设备是否设置成了
            }
            //此时设置本地时间
            return ExeRtnValues.执行成功;
        }
        public ExeRtnValues Exe_InitApp(CmdData cmd)
        {
            if (cmd.CPData == null)
            {
                this.ShowErr("执行6021命令时，数据包的CP数据为空！");
                return ExeRtnValues.数据包校验错误;
            }
            if (cmd.CPData.CPDataItems == null || cmd.CPData.CPDataItems.Count == 0)
            {
                this.ShowErr("执行6021命令时，数据包的CPDataItem数据为空！");
                return ExeRtnValues.数据包校验错误;
            }
            //传入的内容可能只是其中1项或几项；
            int iOverTime, iRecount, iRtdHeartBeat,iWarnTime;
            iOverTime = -1;
            iRecount = -1;
            iRtdHeartBeat = -1;
            iWarnTime = -1;
            string strItems = string.Empty;//记录更细了哪些项目
            foreach (CmdCPDataItem item in cmd.CPData.CPDataItems)
            {
                string str = item.GetValue("OverTime");
                    if (str.Length > 0 && !int.TryParse(str, out iOverTime))
                    {
                        this.ShowErr($"执行6021命令时，OverTime值[{str}]无效！");
                        return ExeRtnValues.数据包校验错误;
                    }
                str =item.GetValue("ReCount");
                if (str.Length > 0 && !int.TryParse(str, out iRecount))
                {
                    this.ShowErr($"执行6021命令时，ReCount值[{str}]无效！");
                    return ExeRtnValues.数据包校验错误;
                }
                str =item.GetValue("RtdHeartbeat");
                if (str.Length > 0 && !int.TryParse(str, out iRtdHeartBeat))
                {
                    this.ShowErr($"执行6021命令时，RtdHeartbeat值[{str}]无效！");
                    return ExeRtnValues.数据包校验错误;
                }
                str =item.GetValue("WarnTime");
                if (str.Length > 0 && !int.TryParse(str, out iWarnTime))
                {
                    this.ShowErr($"执行6021命令时，WarnTime值[{str}]无效！");
                    return ExeRtnValues.数据包校验错误;
                }
                
                if (iOverTime > 0)
                {
                    strItems += "超时时间=" + iOverTime.ToString() + "、";
                    AppConfig.OverTime = iOverTime;
                }
                if (iRecount > 0)
                {
                    strItems += "重发此时=" + iRecount.ToString() + "、";
                    AppConfig.ReCount = iRecount;
                }
                if (iRtdHeartBeat>0)
                {
                    strItems += "心跳包间隔=" + iRtdHeartBeat.ToString() + "、";
                    AppConfig.RtdHeartbeat = iRtdHeartBeat;
                }
                if (iWarnTime > 0)
                {
                    strItems += "WarnTime=" + iWarnTime.ToString() + "、";
                    AppConfig.WarnTime = iWarnTime;
                }
                if (strItems.Length > 0) strItems.Substring(0, strItems.Length - 1);
                //直接赋值
                /*暂时将这些配置信息放到AppConfig中，不作为站点下的配置了
                 * foreach (DataEntitys.AppConfig.StationEntity station in DataEntitys.AppConfig.Stations)
                {
                    //注意，这里数据中心可能会在CP区域内传入一个站点号，虽然示例没这么写；
                    if (item.SNO.Length == 0 || item.SNO == station.SNO)
                    {
                        station.OverTime = iOverTime;
                        station.ReCount = iRecount;
                        station.RtdHeartbeat = iRtdHeartBeat;
                        station.WarnTime = iWarnTime;
                    }
                }*/
            }
            this.ShowLog($"完全初始化更新了项目：" + strItems);
            if (!this._TaskManager.Special_UpdateTaskAppConfig())
                return ExeRtnValues.执行失败但不知道原因;
            return ExeRtnValues.执行成功;
        }
        public ExeRtnValues Exe_SetDeviceCTime(CmdData cmd)
        {
            if (cmd.CPData == null)
            {
                this.ShowErr("执行3105命令时，数据包的CP数据为空！");
                return ExeRtnValues.数据包校验错误;
            }
            if (cmd.CPData.CPDataItems == null || cmd.CPData.CPDataItems.Count == 0)
            {
                this.ShowErr("执行3105命令时，数据包的CPDataItem数据为空！");
                return ExeRtnValues.数据包校验错误;
            }
            int iCTime;
            //注意返回的数据可能包含SNO和ENo一个或2个都有，也有可能没有，
            //没有的话，相当于所有；
            string strItem = string.Empty;
            DeviceEntity device;
            foreach (CmdCPDataItem item in cmd.CPData.CPDataItems)
            {
                string str = item.GetValue("CTime");
                if (!int.TryParse(str, out iCTime))
                {
                    this.ShowErr($"执行3105命令时，CTime值[{str}]无效！");
                    return ExeRtnValues.数据包校验错误;
                }
                //注意ITEM可能为空，如果为空的话表示所有站点的设备
                if(item.SNO.Length==0)
                {
                    //所有站点都设置
                    foreach(AppConfig.StationEntity station in AppConfig.Stations)
                    {
                        if (!station.Enabled) continue;//未启用的站点不用考虑
                        device = AppConfig.GetDeviceByENO(station.SNO, item.ENo);
                        if (device == null)
                        {
                            this.ShowErr($"设置站点[{item.SNO}]下设备[{item.ENo}]采样周期失败，未找到该设备。");
                            continue;
                        }
                        device.CTime = iCTime;
                        this.ShowLog($"设置了站点[{station.SNO}]的设备[{device.DeviceType},{device.ENO}]CTime={iCTime}");
                    }
                }
                else
                {
                    device = AppConfig.GetDeviceByENO(item.SNO, item.ENo);
                    if (device == null)
                    {
                        this.ShowErr($"设置站点[{item.SNO}]下设备[{item.ENo}]采样周期失败，未找到该设备。");
                        continue;
                    }
                    device.CTime = iCTime;
                    this.ShowLog($"设置了站点[{item.SNO}]的设备[{device.DeviceType},{device.ENO}]CTime={iCTime}");
                }
            }
            if (!this._TaskManager.Special_UpdateTaskAppConfig())
                return ExeRtnValues.执行失败但不知道原因;
            return ExeRtnValues.执行成功;
        }

        public void StartSendRealData()
        {
            //如果平台多次下发命令，这里移除之前创建的
            this.StopSendRealData();
            //开始上传实时数据
            foreach (DataEntitys.AppConfig.StationEntity station in DataEntitys.AppConfig.Stations)
            {
                if (!station.Enabled) continue;
                foreach(DeviceEntity device in station.Devices)
                {
                    //这里上传所有设备的实时数据
                    CmdReadData cmd = new CmdReadData(device.DeviceType);
                    cmd.SNo = station.SNO;
                    cmd.ST = 38;
                    cmd.CreateNewQN();
                    TransCmdController task = new TransCmdController(this._TaskManager, this._TransConnect, this._CPDataFactory, cmd, this._AppLogger);
                    task._CTime = device.CTime;
                    task.TransCmdType = TransCmdController.TransCmdTypes.Cyclical;
                    task.TaskStatu = DataEntitys.TaskStatus.Activing;
                    this._TaskManager.AddTask(task);
                }
            }
        }
        /// <summary>
        /// 执行终止上传操作
        /// </summary>
        public void StopSendRealData()
        {
            lock (TaskManager._ObjectTaskDataLock)
            {
                List<TransCmdController> tasks = this._TaskManager.FindTaskByCN(TransCNs.实时数据上传);
                if (tasks != null && tasks.Count > 0)
                {
                    //将之前的都移除，如果多次下发的话就会这样
                    foreach(TransCmdController t in tasks)
                    {
                        if (t.TaskStatu != DataEntitys.TaskStatus.Removed)
                            t.TaskStatu = DataEntitys.TaskStatus.Removed;
                    }
                }
            }
        }
        
        #endregion
        public void ShowErr(string sErr)
        {
            this._AppLogger.ShowErr(this, sErr);
        }
        public void ShowLog(string sLog)
        {
            this._AppLogger.ShowLog(this, sLog);
        }
    }
}
