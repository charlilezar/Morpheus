using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataPlatformTrans.DataEntitys.MessageEntity;
using DataPlatformTrans.DataFactory;
using Microsoft.Extensions.Logging;

namespace DataPlatformTrans.DataEntitys
{
    public enum TaskStatus
    {
        /// <summary>
        /// 空闲无任何状态
        /// </summary>
        Free=0,
        /// <summary>
        /// 激活状态，该状态是马上要发送出去的
        /// </summary>
        Activing=1,
        /// <summary>
        /// 正在发送中
        /// </summary>
        Sending=2,
        /// <summary>
        /// 已被移除，等待Manager来删除
        /// </summary>
        Removed=3,
        /// <summary>
        /// 消息等待回复
        /// </summary>
        WaittingReplay=4,
    }
    public class TaskManager
    {
        #region 需外部传入的对象实例
        public AppLogger _AppLogger = null;
        public TransConnect TransConnect { get; set; }
        #endregion
        ILogger _Logger = null;
        public TaskManager(TransConnect transConnect,AppLogger appLogger)
        {
            this.TransConnect = transConnect;
            this._AppLogger = appLogger;
        }
        public Dictionary<string, TransCmdController> TaskDatas = null;
        public void AddTask(TransCmdController task)
        {
            if (this.TaskDatas == null)
                this.TaskDatas = new Dictionary<string, TransCmdController>();
            this.TaskDatas.Add(task.CmdData.QN, task);
        }

        public TransCmdController FindTaskByQn(string sQN)
        {
            //QN是唯一的，所以该查询最多只可能返回1个
            if (this.TaskDatas == null || this.TaskDatas.Count == 0) return null;
            TransCmdController task;
            if(!this.TaskDatas.TryGetValue(sQN,out task))
            {
                return null;
            }
            return task;
        }
        /// <summary>
        /// 根据CN值查找任务
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<TransCmdController> FindTaskByCN(TransCNs cn)
        {
            if (this.TaskDatas == null || this.TaskDatas.Count == 0) return null;
            return TaskDatas.Values.ToList<TransCmdController>().FindAll(m => m.CN == (int)cn);
        }
        /// <summary>
        /// 获取激活的任务
        /// </summary>
        /// <returns></returns>
        public List<TransCmdController> GetActivingTask()
        {
            return GetTask(TaskStatus.Activing);
        }
        public static object _ObjectTaskDataLock = new object();
        public void TaskCollect()
        {
            lock (_ObjectTaskDataLock)
            {
                List<TransCmdController> tasks = this.GetRemovedTask();
                if (tasks != null)
                {
                    for (int i = tasks.Count; i > 0; i--)
                    {
                        this.TaskDatas.Remove(tasks[i - 1].QN);
                        tasks[i - 1] = null;
                    }
                    GC.Collect();
                }
            }
        }
        public void TaskRefreshCyclical()
        {
            lock (_ObjectTaskDataLock)
            {
                List<TransCmdController> tasks = this.GetTask(TaskStatus.Free);
                if (tasks != null)
                {
                    for (int i = tasks.Count; i > 0; i--)
                    {
                        //如果是循环利用的话，看看是否时间到了
                        if (tasks[i - 1].TransCmdType == TransCmdController.TransCmdTypes.Cyclical)
                            tasks[i - 1].RefreshCyclicalTaskStatus();
                    }
                }
            }
        }
        public void TaskTimeOutCollect()
        {
            lock (_ObjectTaskDataLock)
            {
                //Replay_SetReActiving
                List<TransCmdController> tasks = this.GetTask(TaskStatus.WaittingReplay);
                if (tasks != null && tasks.Count > 0)
                {
                    foreach (TransCmdController task in tasks)
                    {
                        task.Replay_CheckTimeOut();
                    }
                }
            }
        }
        public List<TransCmdController> GetRemovedTask()
        {
            return GetTask(TaskStatus.Removed);
        }
        public List<TransCmdController> GetTask(TaskStatus status)
        {
            if (this.TaskDatas == null || this.TaskDatas.Count == 0) return null;
            return TaskDatas.Values.ToList<TransCmdController>().FindAll(delegate (TransCmdController data) { return data.TaskStatu == status; });
        }
        #region 命令功能函数
        /// <summary>
        /// 添加身份校验命令
        /// </summary>
        public void Special_AddTokenTask()
        {
            List<TransCmdController> tasks = this.FindTaskByCN(TransCNs.身份验证);
            if (tasks != null && tasks.Count > 0)
            {
                if (tasks[0].TaskStatu != TaskStatus.Sending || tasks[0].TaskStatu != TaskStatus.Activing)
                    tasks[0].TaskStatu = TaskStatus.Activing;
                //此时如果是未激活的则直接激活它
                return;
            }
            CmdData cmd = new CmdData();
            cmd.CN = (int)TransCNs.身份验证;
            cmd.CPData = null;
            cmd.ST = 38;
            cmd.Flag = 1;
            cmd.CreateNewQN();
            TransCmdController task = new TransCmdController(this,this.TransConnect, null, cmd,this._AppLogger)
            {
                TaskStatu = TaskStatus.Activing,
            };
            task.Replay_MaxResendCnt = int.MaxValue;//身份验证超时的话，不要考虑次数了；
            this.AddTask(task);
        }
        /// <summary>
        /// 网络心跳包收到信号后，快捷处理
        /// </summary>
        public void Special_HeartbeatMsgReceived()
        {
            //收到了网络心跳包
            //在心跳包上更新一下，平台反馈时间
        }
        public void Special_AddHeartbeatMsg()
        {
            List<TransCmdController> tasks = this.FindTaskByCN(TransCNs.网络心跳包);
            if(tasks==null || tasks.Count==0)
            {
                CmdData cmd = new CmdData();
                cmd.CN = (int)TransCNs.网络心跳包;
                cmd.Flag = 0;
                cmd.MN = AppConfig.MN;
                cmd.ST = 38;
                cmd.CPData = null;//心跳包不需要数据
                cmd.CreateNewQN();
                TransCmdController task = new TransCmdController(this, this.TransConnect, null, cmd, this._AppLogger);
                task._CTime = AppConfig.RtdHeartbeat;
                task.TransCmdType = TransCmdController.TransCmdTypes.Cyclical;
                task.TaskStatu = TaskStatus.Activing;
                this.AddTask(task);
            }
        }
        /// <summary>
        /// 回复平台消息
        /// </summary>
        /// <param name="sQn">要回复的命令QN值</param>
        /// <param name="blQnRtnValue">返回的消息是否正确</param>
        public void Special_ReturnQnRtnCmd(CmdData cmdSource, QnRtnValues value = QnRtnValues.准备执行请求)
        {
            CmdData cmd = new CmdData();
            cmd.CreateNewQN();
            cmd.CN = 9011;
            cmd.ST = 91;
            cmd.Flag = 0;
            cmd.SNo = cmdSource.SNo;//如果上一层发来的是空的，则我们也返回空；
            cmd.CPData = new CmdCPData();
            cmd.CPData.QN = cmdSource.QN;
            cmd.CPData.QnRtn = value;
            TransCmdController task = new TransCmdController(this, this.TransConnect, null, cmd, this._AppLogger);
            task.TaskStatu = TaskStatus.Activing;
            this.AddTask(task);//统一发送
            //task.SendTaskAsync();//直接发送给平台
        }
        /// <summary>
        /// 反馈通知应答9013
        /// </summary>
        /// <param name="cmdSource"></param>
        public void Special_ReturnNoticeCmd(CmdData cmdSource)
        {
            //根据协议中的示例，该返回内容不需要QnRtn或其他标识，只要一个QN就可以了
            CmdData cmd = new CmdData();
            cmd.CreateNewQN();
            cmd.CN = 9013;
            cmd.ST = 91;
            cmd.Flag = 0;
            cmd.SNo = cmdSource.SNo;//如果上一层发来的是空的，则我们也返回空；
            cmd.CPData = new CmdCPData();
            cmd.CPData.QN = cmdSource.QN;
            TransCmdController task = new TransCmdController(this, this.TransConnect, null, cmd, this._AppLogger);
            task.TaskStatu = TaskStatus.Activing;
            this.AddTask(task);//统一发送
        }
        public void Special_ReturnExeRtnCmd(CmdData cmdSource, ExeRtnValues value = ExeRtnValues.执行成功)
        {
            CmdData cmd = new CmdData();
            cmd.CreateNewQN();
            cmd.CN = 9012;
            cmd.ST = 91;
            cmd.Flag = 0;
            cmd.SNo = cmdSource.SNo;//如果上一层发来的是空的，则我们也返回空；
            cmd.CPData = new CmdCPData();
            cmd.CPData.QN = cmdSource.QN;
            cmd.CPData.ExeRtn = value;
            TransCmdController task = new TransCmdController(this, this.TransConnect, null, cmd, this._AppLogger);
            task.TaskStatu = TaskStatus.Activing;
            this.AddTask(task);
            // task.SendTaskAsync();//直接发送给平台
        }
        /// <summary>
        /// 系统配置更改后，已创建的任务刷新一下配置
        /// </summary>
        public bool Special_UpdateTaskAppConfig()
        {
            if(this.TaskDatas==null || this.TaskDatas.Count==0)
            {
                return true;
            }
            foreach(TransCmdController task in this.TaskDatas.Values)
            {
                if (task.TaskStatu == TaskStatus.Removed) continue;
                if (task.CN == (int)TransCNs.网络心跳包)
                    task._CTime = AppConfig.RtdHeartbeat;
                //重发次数设定
                if(task.Replay_MaxResendCnt!=AppConfig.ReCount)
                {
                    //身份验证的不能改
                    if (task.CN != (int)TransCNs.身份验证)
                        task.Replay_MaxResendCnt = AppConfig.ReCount;
                }
            }
            return true;
        }

        #endregion
    }
    public class TransCmdController
    {
        #region 外部插入的实例化对象
        TaskManager TaskManager { get; set; }
        public TransConnect TransConnect { get; set; }
        public CPDataFactory CPDataFactory { get; set; }
        AppLogger _AppLogger { get; set; }
        #endregion
        public TransCmdController(TaskManager taskManager,TransConnect transConnect, CPDataFactory cPDataFactory, CmdData cmdData, AppLogger appLogger)
        {
            this.TaskManager = taskManager;
            this.TransConnect = transConnect;
            this.CmdData = cmdData;
            this.CPDataFactory = cPDataFactory;
            _AppLogger = appLogger;
            //直接读取系统配置， AppConfig.ReCount是由数据中心定义的值
            this.Replay_MaxResendCnt = AppConfig.ReCount;
            this.CreatTime = DateTime.Now;
        }
        public TransCmdTypes TransCmdType { get; set; }
        /// <summary>
        /// 任务创建时间，目前没什么用，仅作为调试时看
        /// </summary>
        public DateTime CreatTime { get; set; }
        #region 命令循环使用
        /// <summary>
        /// 记录当前时间
        /// </summary>
        public DateTime? _LastSendTime = null;
        /// <summary>
        /// 循环执行的时间，如果为0的话，则表示不再执行，否则不停的刷新
        /// </summary>
        public int _CTime =0;
        public void RefreshCyclicalTaskStatus()
        {
            /********
             * 该函数仅处理循环执行的命令，是否到达激活状态了
             * **************/
            if (this.TaskStatu != TaskStatus.Free) return;
            if (_CTime == 0) return;
            if(this._LastSendTime==null || (DateTime.Now-(DateTime)this._LastSendTime).TotalSeconds>=this._CTime)
            {
                this.TaskStatu = TaskStatus.Activing;
            }
        }
        #endregion
        /// <summary>
        /// 命令的详细数据包，包含了要发送的内容
        /// </summary>
        public CmdData CmdData { get; set; }
        public string QN
        {
            get
            {
                if (this.CmdData == null) return string.Empty;
                return this.CmdData.QN;
            }
        }
        public int CN
        {
            get
            {
                if (this.CmdData == null) return 0;
                return this.CmdData.CN;
            }
        }
        /// <summary>
        /// 获取发送命令时的的数据，通过解析当前类和CP数据生成
        /// </summary>
        /// <returns>用于通过TransConnect发送给中心的</returns>
        public string GetTransText()
        {

            return string.Empty;
        }
        /// <summary>
        /// 当前因网络等其他本地故障一起的发送失败后的重发次数记录
        /// </summary>
        public int SendFaild_ReCounter = 0;
        /// <summary>
        /// 命令是否被激活，如果被激活的话，则需要发送，默认为空闲
        /// </summary>
        public TaskStatus TaskStatu { get; set; } = TaskStatus.Free;
        
        /// <summary>
        /// 发送当前命令
        /// </summary>
        /// <returns></returns>
        public async void SendTaskAsync()
        {
            /***********
             * 发送任务：
             * 1、将该任务取消激活状态；
             * 2、获取数据，获取失败的话，执行重发（根据中心设定的重发次数）
             * 3、发送完成后，调用完成函数；
             * **************/
            //初始化失败计数
            if (this.SendFaild_ReCounter != 0)
                this.SendFaild_ReCounter = 0;
            this.TaskStatu = TaskStatus.Sending;
            bool blSucessful = false;
            RtnMessage rtn = null;
            int iRetCntSetted = -1;//设定值，没发送失败的话不用去查找了，这个要根据每个站点的配置来。因为平台设置该值时传入了站点号
            while (!blSucessful)
            {
                rtn = await TaskSending();
                blSucessful = rtn.Sucessful;
                if (!blSucessful)
                {
                    this.SendFaild_ReCounter++;//每次失败都要加一下
                    if (iRetCntSetted < 0)
                    {
                        iRetCntSetted = AppConfig.SendFaildReCount;
                    }
                    if (this.SendFaild_ReCounter > iRetCntSetted)
                    {
                        this._AppLogger.ShowErr(this, $"命令[{this.CN}]发送失败，当前第{SendFaild_ReCounter}次失败，不再发送，错误内容：{rtn.Msg}。");
                        break;//此时超过了设定的此时则退出执行了
                    }
                    else
                    {
                        this._AppLogger.ShowErr(this, $"命令[{this.CN}]发送失败，当前第{SendFaild_ReCounter}次失败，还不到上限的{iRetCntSetted + 1}次，继续发送，错误内容：{rtn.Msg}。");
                    }
                }
            }
            this._LastSendTime = DateTime.Now;//记录下命令发送时间，后续判断超时，循环发送等有用到
            //注意如果超时时间设置为0或负数的表示无超时判断，那就无法定义重发，这种情况也被看做是无需重发了，
            //这种情况直接将任务定义为只发1次就可以了
            if (AppConfig.OverTime>0 && this.Replay_MaxResendCnt>0 && this.Replay_IsNeed)
            {
                //此时表示该任务需要回复，则挂起，等待是否回复了，应为根据协议固定，如果没有收到系统回复消息的话要按照指定次数重发的
                this.TaskStatu = TaskStatus.WaittingReplay;
            }
            else
            {
                //此时无需等待回复，无论是否成功，都要处理掉该任务
                this.TaskCompeleted();
            }
        }
        /// <summary>
        /// 发送任务，该函数仅供SendTaskAsync调用
        /// </summary>
        /// <returns></returns>
        private async Task<RtnMessage> TaskSending()
        {
            if (TransConnect == null)
            {
                return new RtnMessage("通讯对象为空！");
            }
            if (CmdData == null)
            {
                return new RtnMessage("数据对象对象为空，发送失败！");
            }
            //获取CP区域数据，该过程会查询数据库；
            RtnMessage rtn = await this.CmdData.SetCPDataAsync(this.CPDataFactory);
            if (!rtn.Sucessful) return rtn;
            //将数据发送至平台
            string strData;
            string strErr;
            if (!this.CmdData.GetTransText(out strData, out strErr))
            {
                return new RtnMessage("发送内容获取失败，原因：" + strErr);
            }
           // this._AppLogger.ShowLog(this, $"任务[{this.CN}]，发送数据[{strData}]");
            //开始发送数据
            rtn = await this.TransConnect.SendDataAsync(strData);
            if (!rtn.Sucessful)
                return rtn;
            this._AppLogger.ShowLog(this, $"任务[{this.CN}]，发送成功！数据[{strData}]");
            //此时该任务成功执行了
            return new RtnMessage();
        }
        private void TaskCompeleted()
        {
            if (this.TransCmdType == TransCmdTypes.Cyclical)
            {
                //此时为循环利用的命令，则设置为空闲等待
                this.TaskStatu = TaskStatus.Free;
                //这里要复位重发次数
                this.Replay_ReSendCnt = 0;
                this.RefreshQN();
                this._AppLogger.ShowLog(this, $"任务[{this.CN}]，因循环发送，当前设置为[{this.TaskStatu }]，并重新创建QN值[{(this.CmdData == null ? "NULL" : this.CmdData.QN)}]。");
            }
            else
            {
                //将任务标号为已删除，系统会移除这些任务
                this.TaskStatu = TaskStatus.Removed;
                this._AppLogger.ShowLog(this, $"任务[{this.CN}]执行完成，非循环类型，当前设置为[{this.TaskStatu }]");
            }
        }
        /// <summary>
        /// 刷新当前任务的QN值
        /// </summary>
        public void RefreshQN()
        {
            if (this.CmdData == null) return;
            /***********
             * 由于QN是任务集合索引，所以不能直接更改Cmd中的QN就完事了，这样会找无法从任务集合中找到任务的，
             * 所以我们更新cmd的qn的同时，更新任务集合的索引，目前没有找到直接修改Dictionary的key值；
             * 通过移除原Key，添加新Key来解决
             * **************/
            string sOrignalQN = this.QN;
            if (String.IsNullOrEmpty(sOrignalQN)) return;
            lock (TaskManager._ObjectTaskDataLock)
            {
                this.TaskManager.TaskDatas.Remove(sOrignalQN);
                this.CmdData.CreateNewQN();
                this.TaskManager.TaskDatas.Add(this.QN, this);
            }
        }
        #region  消息回复管理
        /// <summary>
        /// 允许最大超时重发次数，该值有平台配置，个别命令可能特殊，例如身份校验，应该没有期限
        /// </summary>
        public int Replay_MaxResendCnt = 3;
        /// <summary>
        /// 因未收到回复而重发的次数
        /// </summary>
        private short Replay_ReSendCnt = 0;
        /// <summary>
        /// 该命令是否需要平台答复，如果指定时间内没有答复的话，认为超时，需要重新发送指定次数的
        /// </summary>
        private bool Replay_IsNeed
        {
            get
            {
                //注意Flag的说明，见协议《辐射环境自动监测系统数据传输协议规范.pdf》，第10页：字节的最后一个bit位如果为1就是要回复的
                //协议中指定Flag的范围是0-255；则一个字节就可以满足了
                if (this.CmdData == null) return false;
                return this.CmdData.IsNeedReplay;
            }
        }
        /// <summary>
        ///  设置消息已经收到回复了，去向可能是删除或者循环使用
        /// </summary>
        /// <param name="replayData">收到的消息内容</param>
        public void Replay_SetReplayed(CmdData replayData)
        {
            //这里有个问题？如果回复失败，那我们是重发还是不重发，
            //因为回复失败的话，重发数据还是会失败，因为我们没法重新定义数据；
            //目前规定，不重发，但将信息已错误信息显示出来；
            this.TaskCompeleted();
        }
        /// <summary>
        /// 检查是否命令超时了并开启重发机制
        /// </summary>
        public void Replay_CheckTimeOut()
        {
            if (this.TaskStatu != TaskStatus.WaittingReplay)
            {
                //不是等待状态的话，可能已经重新发送了，不用管了
                return;
            }
            int iOverTime = AppConfig.OverTime;
            if(iOverTime<=0)
            {
                //超时不能为空；
                //如果程序进入这里，表示程序在运行过程中，远程数据平台下发了设置超时和重发次数的命令，
                //且将他们设置为了0，也有可能是本地程序调试模式下人为将这些字段设置了0了；
                this._AppLogger.ShowLog(this, "当前AppConfig中关于超时重发的定义为空，当前强制设置为3秒。");
                iOverTime = 3;
            }
            //判断时间是否到达了
            if(this._LastSendTime==null)
            {
                this._LastSendTime = DateTime.Now;
                return;//下次再判断，但基本上程序不会进入这里，除非有bug引起
            }
            //这里引入Replay_MaxResendCnt变量，而不是读取AppConfig.ReCont的原因是，个别命令的超时重发次数会不同，毕竟每个命令的重要性和性质不同，例如身份验证；
            //为了避免将业务功能引入功能对象中，引入该字段由Bll层去处理；
            if ((DateTime.Now - (DateTime)this._LastSendTime).TotalSeconds >= AppConfig.OverTime)
            {
                //此时超时了，则判断超时次数是否已达上限
                this.Replay_ReSendCnt++;
                if (Replay_ReSendCnt >= this.Replay_MaxResendCnt)
                {
                    this._AppLogger.ShowLog(this, $"命令[{this.CN}]超时{this.Replay_ReSendCnt}次,达到设定{this.Replay_MaxResendCnt}次，不再发送！");
                    //此时已经是达到数量了，则标识该任务完成，并发送错误消息
                    this.TaskCompeleted();
                    return;
                }
                else
                {
                    this._AppLogger.ShowLog(this, $"命令[{this.CN}]超时{this.Replay_ReSendCnt}次,还未达到设定{this.Replay_MaxResendCnt}次，继续发送！");
                    //重新激活该任务
                    this.TaskStatu = TaskStatus.Activing;
                }
            }
        }
        #endregion
        #region  相关枚举
        public enum TransCmdTypes
        {
            /// <summary>
            /// 发送完后直接删除
            /// </summary>
            RemovedAfterCompeleted = 0,
            /// <summary>
            /// 循环利用，例如心跳、上传实时参数
            /// </summary>
            Cyclical = 1
        }
        #endregion
    }
    public class CmdData
    {
        public CmdData()
        {
            //MN为固定节点
            this.MN = AppConfig.MN;
        }
        public bool IsNeedReplay
        {
            get
            {
                //注意Flag的说明，见协议《辐射环境自动监测系统数据传输协议规范.pdf》，第10页：字节的最后一个bit位如果为1就是要回复的
                //协议中指定Flag的范围是0-255；则一个字节就可以满足了
                byte b = (byte)this.Flag;
                string strData = Convert.ToString(b, 2);
                return strData.EndsWith("1");
            }
        }
        /// <summary>
        /// 创建全新的QN
        /// </summary>
        public void CreateNewQN()
        {
            //无论如何都要生产一个，本地命令识别也是要靠这个的
            this.QN = AppHelper.TimeStamper.GetNewTimeStampe();
        }
        #region 任务数据关键标识
        /// <summary>
        /// 命令唯一识别号，时间戳
        /// </summary>
        public string QN { get; set; }
        /// <summary>
        /// 关键数据：命令代码
        /// </summary>
        public int CN { get; set; }
        /// <summary>
        /// 关键数据：38或91，详细参考协议说明
        /// </summary>
        public int ST { get; set; }
        /// <summary>
        /// 标识出是否要拆分CP数据以及是否要回复
        /// </summary>
        public int Flag { get; set; }
        /// <summary>
        /// 关键数据：站点号
        /// </summary>
        public string SNo { get; set; }
        /// <summary>
        /// 关键数据：网络节点
        /// </summary>
        public string MN { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string PW { get; set; }
        #endregion
        /// <summary>
        /// 命令的详细数据内容
        /// </summary>
        public CmdCPData CPData { get; set; }
        public SendPlatFormDataBase CPDataSource { get; set; }
        public bool GetTransText(out string sData,out string sErr)
        {
            sData = string.Empty;
            //添加必填项目
            if (this.IsNeedReplay && String.IsNullOrEmpty(this.QN))
            {
                sErr = "发送命令生成失败，需要回复的命令，其QN不能为空！";
                return false;
            }
            if (this.ST!=38 && this.ST!=91)
            {
                sErr = "发送命令生成失败，ST不是预期的38或91！";
                return false;
            }
            if (this.CN <= 0)
            {
                sErr = "发送命令生成失败，CN不能小于等于0！";
                return false;
            }
            if (String.IsNullOrEmpty(AppConfig.MN))
            {
                sErr = "发送命令生成失败，MN为空！";
                return false;
            }
            if (this.IsNeedReplay)
                sData = $"QN={this.QN};";
            sData += string.Format("ST={0};CN={1};PWD={2};MN={3};Flag={4};", this.ST, this.CN, AppConfig.PWD, AppConfig.MN, this.Flag);
            //添加数据区域
            string sCp = string.Empty;
            if(this.CPData!=null)
            {
                //QN，可能有，也可能没有
                if (!String.IsNullOrEmpty(this.CPData.QN))
                    sCp += string.Format($"QN={this.CPData.QN};");
                if (this.CPData.QnRtn > 0)
                    sCp += string.Format($"QnRtn={(int)this.CPData.QnRtn};");
                if (this.CPData.ExeRtn > 0)
                    sCp += string.Format($"ExeRtn={(int)this.CPData.ExeRtn};");
                //开始记录其他明细项目
                string strTemp;
                if(this.CPData.CPDataItems!=null && this.CPData.CPDataItems.Count>0)
                {
                    foreach(CmdCPDataItem item in this.CPData.CPDataItems)
                    {
                        //SNo和ENo比较特殊，一个CP内可能包含多个SNO，同一个SNO可能包含多个ENo，但同一个ENO下面的项目是有唯一性的
                        //所以这里在添加SNo和ENo时要判断是否已经存在了，在了的话不用再加上去了，当然他们也有可能为空的，如果是空的那就不用加了
                        if (!String.IsNullOrEmpty(item.SNO))
                        {
                            strTemp = $"SNO={item.SNO};";
                            if (AppHelper.AppFuns.FindTextFirstIndexByRegex(sCp, @"SNO=\S*?;") < 0)
                            {
                                sCp += strTemp;
                            }
                        }
                        if (!String.IsNullOrEmpty(item.ENo))
                        {
                            strTemp = $"ENO={item.ENo};";
                            if (AppHelper.AppFuns.FindTextFirstIndexByRegex(sCp, @"ENO=\S*?;") < 0)
                            {
                                sCp += strTemp;
                            }
                        }
                        //其他项目依次添加
                        if (item.Items != null && item.Items.Count > 0)
                        {
                            foreach (var v in item.Items)
                            {
                                sCp += string.Format("{0}={1};", v.Key, v.Value);
                            }
                        }
                    }
                }
                //删除最后一个分号
                if (sCp.EndsWith(";"))
                    sCp = sCp.Substring(0, sCp.Length - 1);
            }
            //确保最后是；结尾的
            if (!sData.EndsWith(";"))
                sData += ";";
            sData = string.Format("{0}CP=&&{1}&&", sData, sCp);
            if (sData.Length > 1024)
            {
                //根据协议第9页，7.3.1 通信包结构组成，数据段有长度约束
                sErr = "数据段内容超过了1024个字符，这是不允许的。";
                return false;
            }
            string sCRC = AppHelper.AppFuns.GetCrc16(sData);//根据协议第9页，7.3.1 通信包结构组成，内容说明，crc只对数据段进行校验，所以这里不用管包头和长度内容了；
            sData += sCRC + "\r\n";
            sData = "##" + sData.Length.ToString().PadLeft(4, '0') + sData;
            sErr = string.Empty;
            return true;
        }
        public virtual async Task<RtnMessage> SetCPDataAsync(CPDataFactory factory)
        {
            if(factory!=null)
            {
                //可能为空，有些命令是不需要查询数据的
            }
            return new RtnMessage<string>();
        }
    }
    public class CmdCPData
    {
        /// <summary>
        /// CP常用数据：发送的数据时针对哪个命令的，可以为空，当命令为9011或9012时有这个值
        /// </summary>
        public string QN { get; set; }
        /// <summary>
        /// 反馈消息值
        /// </summary>
        public QnRtnValues QnRtn { get; set; }
        /// <summary>
        /// 执行结果值
        /// </summary>
        public ExeRtnValues ExeRtn { get; set; }
        /// <summary>
        /// CP的其他数据
        /// </summary>
        public List<CmdCPDataItem> CPDataItems { get; set; }
    }
    public class CmdCPDataItem
    {
        /// <summary>
        /// 数据所属的站点号
        /// </summary>
        public string SNO { get; set; }
        /// <summary>
        /// 数据所属的设备识别号
        /// </summary>
        public string ENo { get; set; }
        public Dictionary<string, string> Items = null;
        public CmdCPDataItem()
        {

        }
        public CmdCPDataItem(Dictionary<string,string> valuePairs)
        {
            this.Items = valuePairs;
        }
        public void AddItem(string sKey,string sValue)
        {
            if (Items == null)
                this.Items = new Dictionary<string, string>();
            this.Items.Add(sKey, sValue);
        }
        /// <summary>
        /// 获取CP内容值
        /// </summary>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public string GetValue(string sKey)
        {
            if (this.Items == null) return string.Empty;
            string strValue;
            if (!this.Items.TryGetValue(sKey, out strValue))
                return string.Empty;
            return strValue;
        }
    }
    
}
