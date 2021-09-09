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
using System.IO;
using Model.DataService;
using System.Net.Sockets;

using System.Collections.Generic;
using CommonLibrary;
using Model.Commands;
using System.Text;
using Model.Measurements;
using Model.Events;
using Model.Configurations;

using HengDa.LiZongMing.REAMS.Devices.Dtos;
using HengDa.LiZongMing.REAMS.HVE.Dtos;
using Volo.Abp.AutoMapper;
using Aming.Tools;
using HengDa.LiZongMing.REAMS.HVE;
using Volo.Abp.Threading;
using HengDa.LiZongMing.REAMS.Devices;
using HengDa.LiZongMing.REAMS.DeviceAlarm;
//using Model.MvvmMessages;

namespace HengDa.LiZongMing.REAMS.CtrlServices
{
    /// <summary>
    /// 控制高压电离室的类，方法是一些组合步骤
    /// </summary>
    public class CtrlHVEService : ICtrlService, ISingletonDependency, IDisposable
    {
        private ILoggerFactory loggerFactory;
        private ILogger Logger;
        private ILogger HveLogger;
        /// <summary>
        /// 当前通讯方式对象
        /// </summary>
        public IDataEndPoint MyDataEndPoint { get; set; } = null;
        /// <summary>
        /// 超时时间
        /// </summary>
        public int WaitTimeout { get; set; } = 30000;

        /// <summary>
        /// 配置和事件状态
        /// </summary>
        public HveRunStatusDto HveRunStatus;

        /// <summary>
        /// 更多测量值，10分钟更新
        /// </summary>
        public HveExtRecordDto HveExtRecord;

        /// <summary>
        /// 伽马测量值，每秒更新，放慢到每30秒更新
        /// </summary>
        public HveRecordDto HveRecord;

        /// <summary>
        /// 设备信息
        /// </summary>
        public DeviceDto Device { get; set; }

        /// <summary>
        /// 将本端数据转发到别的端口的处理程序
        /// </summary>
        public event ReceivedUniMessageCallBack OnDataForwardTo = null;

        /// <summary>
        /// 将消息接收的事件传递到更多的地方处理，主要是
        /// </summary>
        public event ReceivedUniMessageCallBack OnUniMessageDataReceived;

        ///<summary>
        /// 伽马剂量率单位
        ///</summary>
        public static string DoseRateUnit { get; set; }

        /// <summary>
        /// 事件回调时，收到数据触发解析，返回到wpf用
        /// </summary>
        public Action<MqttClientConfig, UniMessageBase, ZCQBaseDto> OnUniMessageReceivedCallBack { get; set; } = null;

        public CtrlHVEService(ILoggerFactory loggerFactory, string jsonConfig, DeviceDto Device)
        {
            this.Device = Device;
            IDataEndPoint config = null;
            if (jsonConfig.Contains("MqttType"))
                config = JsonConvert.DeserializeObject<MqttClientConfig>(jsonConfig);
            else
                config = JsonConvert.DeserializeObject<UARTConfig>(jsonConfig);
            ////TODO：其它的请补全
            Init(loggerFactory, config);
        }
        public CtrlHVEService(ILoggerFactory loggerFactory, IDataEndPoint config, DeviceDto Device)
        {
            this.Device = Device;
            Init(loggerFactory, config);
        }



        public CtrlHVEService Init(ILoggerFactory loggerFactory, IDataEndPoint config)
        {
            //DebugXml = true; // 开启后日志很多

            this.loggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger<CtrlHVEService>();
            this.HveLogger = loggerFactory.CreateLogger("HVE.XML");


            this.MyDataEndPoint = config;


            ////高压电离室目前确定只用一种TCP+UDP的网络连接方式。
            var tsObj = ServiceFactory.Create(loggerFactory, config);
            tsObj.funcCheckPackFormat = this.CheckPackFormat;//定义粘包处理检查类
            tsObj.OnUniMessageReceived += OnUniMessageReceived;
            TranService = tsObj;
            var t = Task.Run(async () =>
            {
                bool ok = await tsObj.TryOpenAsync();
                //if (!isSerial)
                {
                    //UdpNotifyInput.OpenUdpClient(0, mvvmMessenger);
                    //hiaming将本地UDP端口从随机改为固定 13005,从UdpNotifyInput.NotifyPortNumber设定的
                    Model.Notifications.UdpNotifyInput.NotifyPortNumber = 3006;
                    Model.Notifications.UdpNotifyInput.OnUdpScalarMeasurementReceived = OnUdpScalarMeasurementReceived;
                    Model.Notifications.UdpNotifyInput.OnUdpCompositeMeasurementReceived = OnUdpCompositeMeasurementReceived;
                    Model.Notifications.UdpNotifyInput.OnUdpSentinelStateReceived = OnUdpSentinelStateReceived;
                    Model.Notifications.UdpNotifyInput.Logger = HveLogger; // loggerFactory.CreateLogger("HVE.UdpNotifyInput");
                    Model.SentryDebug.Logger = loggerFactory.CreateLogger("HVE.SentryDebug");


                    Model.Notifications.UdpNotifyInput.OpenUdpClient(Model.Notifications.UdpNotifyInput.NotifyPortNumber/*, mvvmMessenger*/);
                }
            });
            Task.WaitAll(new Task[] { t }, 25000);//等他结束


            if (tsObj.IsConnected)
            {
                var t2 = Task.Run(async () =>
                {
                    //启动后的任务，第一次要查询出常用配置等
                    await Task.Delay(5000);
                    GetAllConfigurations();
                    await Task.Delay(5000);
                    //GetAllEvents();
                    await Task.Delay(5000);
                    GetAllMeasurements(false);
                    await Task.Delay(5000);
                    SetNotifyEnable(true);
                });
            }

            return this;

        }
        /// <summary>
        /// 并闭并重新连接设备（只能断开tcp，设备重启不了）
        /// </summary>
        /// <returns></returns>
        public bool Restart()
        {
            var Config = this.TranService.Config;
            Close();
            try
            {
                Init(this.loggerFactory, Config);
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        #region 添加粘包处理

        /// <summary>
        /// 高压电离室的xml数据包，前面有5个头字节，每一个固定，后4字节是长度，再后面是xml的标准字符内容
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        public int CheckPackFormat(List<byte> buff)
        {
            if (buff.Count < 10) return 0;//小于3个肯定没接收完；
            if (240 != buff[0])
            {
                //Logger.LogDebug("收到的高压电离室的数据包没有前面的5个头字节");
                return 0;
            }
            int length = Aming.Core.StringHelper.BitToInt(buff.ToArray(), 1, 4);
            if (buff.Count >= length + 5)
            {
                //有完整内容
                return buff.Count;
            }
            //其他方式是非预期的，则直接都返回接受完了；
            return 0;
        }
        #endregion



        /// <summary>
        /// SerialPortService 对象
        /// </summary>
        public ITransmissionService TranService { get; set; } //在这个设备里没用

        /// <summary>
        /// 串口收到消息时触发
        /// </summary>
        /// <param name="config"></param>
        /// <param name="data"></param>
        public void OnUdpCompositeMeasurementReceived(CompositeMeasurement obj)
        {
            //Logger.LogError("请在这里 CtrlHVEService.OnUdpMessageReceived 添加处理代码"+(obj.GetType()));

            //不太重要，暂时不存
        }

        /// <summary>
        /// 事件回发用的
        /// </summary>
        /// <param name="obj"></param>
        public async void OnUdpSentinelStateReceived(SentinelState obj)
        {
            if (obj.Name == "ElectrometerUpdate")
                return; //跳过些无用的
            ///转换数据
            bool IsAlarm = HVEHelper.SetSentinelStateResponse(ref this.HveRunStatus, obj);

            if (IsAlarm)
            {
                //应该上传

                //保存
                HveExtRecord.CreationTime = obj.Time;
                var service = IocHelper.ServiceProvider.GetService<HVEDataService>();
                await service.UpdateHveRunStatus(this, this.HveRunStatus);
                //报警
                var alarmManager = IocHelper.ServiceProvider.GetService<DeviceAlarmDataService>();
                IotDeviceAlarmDto deviceAlarm = new IotDeviceAlarmDto()
                {
                    CreationTime = obj.Time,
                    Prop = obj.Name,
                    Data = obj.CurrentState,
                    Text = $"高压电离室报警{obj.Name}从{obj.PriorState}变成了{obj.CurrentState}",
                    TenantId = this.Device.TenantId,
                    DeviceId = this.Device.Id,
                    LogType = "报警"
                };

                await alarmManager.CreateAsync(deviceAlarm, this.Device.TenantId);

                if (OnDataForwardTo != null && !TranService.Config.ForwardToTopic.IsNullOrWhiteSpace())
                {
                    //单独线程中去转发，不阻塞
                    var t = Task.Run(() =>
                     {
                         string json = JsonConvert.SerializeObject(this.HveRunStatus);
                         Logger.LogDebug("转发高压电离室警报:" + json);
                         byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(json);
                         var msg = new UniMessageBase { Payload = data, Topic = TranService.Config.ForwardToTopic };
                         OnDataForwardTo(TranService.Config, msg);
                     });
                }
            }
        }
        /// <summary>
        /// UDP收到几个重要的测量值
        /// </summary>
        /// <param name="obj"></param>
        public void OnUdpScalarMeasurementReceived(ScalarMeasurement obj)
        {
            //Logger.LogError("请在这里 CtrlHVEService.OnUdpMessageReceived 添加处理代码"+(obj.GetType()));
            ///转换数据
            HVEHelper.SetScalarMeasurementsResponse(ref this.HveRecord, ref this.HveExtRecord, obj);
            if (obj.Name == "DoseRate")
            {
                if (obj.Time.Second == 0 || obj.Time.Second == 30) //从这里看30秒值
                {
                    var t = Task.Run(async () =>
                   {
                       //保存
                       HveRecord.CreationTime = obj.Time;
                       var service = IocHelper.ServiceProvider.GetService<HVEDataService>();
                       await service.SaveRecord(this, this.HveRecord);

                   });
                    if (OnDataForwardTo != null && !TranService.Config.ForwardToTopic.IsNullOrWhiteSpace())
                    {
                        //单独线程中去转发，不阻塞
                        t = Task.Run(() =>
                      {

                          string json = JsonConvert.SerializeObject(this.HveRecord);
                          Logger.LogDebug("转发高压电离室关键数据:" + json);
                          byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(json);
                          var msg = new UniMessageBase { Payload = data, Topic = TranService.Config.ForwardToTopic };
                          OnDataForwardTo(TranService.Config, msg);
                      });
                    }
                }
                //if (obj.Time.Second == 0 && obj.Time.Minute == 0 && obj.Time.Hour % 4 == 0) //每4小时一次
                if (obj.Time.Second == 0 && obj.Time.Minute % 10 == 0) //每4小时一次
                {
                    var t = Task.Run(async () =>
                    {
                        //保存
                        HveExtRecord.CreationTime = obj.Time;
                        var service = IocHelper.ServiceProvider.GetService<HVEDataService>();
                        await service.SaveExtRecord(this, this.HveExtRecord);

                        await service.UpdateHveRunStatus(this, this.HveRunStatus);

                        if (OnDataForwardTo != null && !TranService.Config.ForwardToTopic.IsNullOrWhiteSpace())
                        {
                            //单独线程中去转发，不阻塞
                            var t2 = Task.Run(() =>
                           {
                               {

                                   string json = JsonConvert.SerializeObject(this.HveExtRecord);
                                   Logger.LogDebug("转发高压电离室扩展数据:" + json);
                                   byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(json);
                                   var msg = new UniMessageBase { Payload = data, Topic = TranService.Config.ForwardToTopic };
                                   OnDataForwardTo(TranService.Config, msg);
                               }
                               {
                                   string json = JsonConvert.SerializeObject(this.HveRunStatus);
                                   Logger.LogDebug("转发高压电离室状态:" + json);
                                   byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(json);
                                   var msg = new UniMessageBase { Payload = data, Topic = TranService.Config.ForwardToTopic };
                                   OnDataForwardTo(TranService.Config, msg);
                               }
                           });
                        }

                    });
                }
            }

        }

        /// <summary>
        /// 高压电离室特别处理，转发从mqtt来的数据
        /// </summary>
        /// <param name="msg"></param>
        public void SendUniMessage(UniMessageBase msg)
        {
            if (msg.Topic == this.TranService.Config.ForwardFromTopic)
            {
                string xml = System.Text.Encoding.ASCII.GetString(msg.Payload);
                Logger.LogDebug("向高压电离室发送：" + xml);
                this.SendXml(xml); //加上长度头发出去
            }
        }

        /// <summary>
        /// 串口收到消息时触发
        /// </summary>
        /// <param name="config"></param>
        /// <param name="data"></param>
        public void OnUniMessageReceived(IDataEndPoint config, UniMessageBase msg)
        {


            //此时正常，则开始解析数据
            var obj = ProcessResponse(msg.Payload, msg.Payload.Length);




            //TODO: 请在这里添加处理代码
            if (OnUniMessageReceivedCallBack != null) //传统的事件触发传出去
            {
                //var msg1 = new UniMessageBase { Payload = data, Topic = $"/DTU/{Config.Port}/from" };
                OnUniMessageReceivedCallBack(config as MqttClientConfig, msg, null);
            }
            else
            {
                //Logger.LogError("请在这里 添加处理代码");
            }
        }



        #region 命令

        //public async Task<IotResult<UniMessageBase>> SendXml(string xml)
        //{
        //    IotResult<UniMessageBase> _cmdResult = null;
        //    try
        //    {
        //        bool ok = SentryDataService.SendXml(xml);
        //        _cmdResult = new IotResult<UniMessageBase>(true, $"发送己送出！");
        //    }
        //    catch (Exception ex)
        //    {
        //        _cmdResult = new IotResult<UniMessageBase>(false, $"通讯出错：{ex.Message}({ex.Source})");
        //    }
        //    return _cmdResult;
        //} 
        #region 常用命令
        public void SetState(string stateName, string stateValue)
        {
            SetState obj = new SetState(stateName, stateValue);
            this.SendXml(Serializers.Serializer<SetState>(obj));
        }

        // Token: 0x060002EA RID: 746 RVA: 0x00007900 File Offset: 0x00005B00
        public void SetNotifyEnable(bool enable)
        {
            NotifyEnable obj = new NotifyEnable(enable, Model.Notifications.UdpNotifyInput.NotifyPortNumber);
            this.SendXml(Serializers.Serializer<NotifyEnable>(obj));
        }

        // Token: 0x060002EB RID: 747 RVA: 0x00007928 File Offset: 0x00005B28
        public void SetElectrometerAcquisitionEnable(bool enable)
        {
            ElectrometerAcquisitionEnable obj = new ElectrometerAcquisitionEnable(enable);
            this.SendXml(Serializers.Serializer<ElectrometerAcquisitionEnable>(obj));
        }

        // Token: 0x060002EC RID: 748 RVA: 0x0000794C File Offset: 0x00005B4C
        public void GetAllConfigurations()
        {
            GetAllConfigurations obj = new GetAllConfigurations();
            this.SendXml(Serializers.Serializer<GetAllConfigurations>(obj));
        }

        // Token: 0x060002ED RID: 749 RVA: 0x0000796C File Offset: 0x00005B6C
        public void GetAllEvents()
        {
            GetAllEvents obj = new GetAllEvents();
            this.SendXml(Serializers.Serializer<GetAllEvents>(obj));
        }

        // Token: 0x060002EE RID: 750 RVA: 0x0000798C File Offset: 0x00005B8C
        public void GetEvent(string eventName)
        {
            GetEvent obj = new GetEvent(eventName);
            this.SendXml(Serializers.Serializer<GetEvent>(obj));
        }

        // Token: 0x060002EF RID: 751 RVA: 0x000079B0 File Offset: 0x00005BB0
        public void SetTime(DateTime time, bool ingnoreTimeZone)
        {
            SetTime obj = new SetTime(time, ingnoreTimeZone);
            this.SendXml(Serializers.Serializer<SetTime>(obj));
        }

        // Token: 0x060002F0 RID: 752 RVA: 0x000079D4 File Offset: 0x00005BD4
        public void GetTime()
        {
            GetTime obj = new GetTime();
            this.SendXml(Serializers.Serializer<GetTime>(obj));
        }

        // Token: 0x060002F1 RID: 753 RVA: 0x000079F4 File Offset: 0x00005BF4
        public void GetAllMeasurements(bool updatesOnly)
        {
            GetAllScalarMeasurements obj = new GetAllScalarMeasurements();
            this.SendXml(Serializers.Serializer<GetAllScalarMeasurements>(obj));
        }

        // Token: 0x060002F2 RID: 754 RVA: 0x00007A14 File Offset: 0x00005C14
        public void GetScalarMeasurement(string measName)
        {
            GetScalarMeasurement obj = new GetScalarMeasurement(measName);
            this.SendXml(Serializers.Serializer<GetScalarMeasurement>(obj));
        }

        // Token: 0x060002F3 RID: 755 RVA: 0x00007A38 File Offset: 0x00005C38
        public bool SetConfiguration(Configuration config)
        {
            SetConfiguration obj = new SetConfiguration(config);
            return this.SendXml(Serializers.Serializer<SetConfiguration>(obj));
        }

        // Token: 0x060002F4 RID: 756 RVA: 0x00007A60 File Offset: 0x00005C60
        public bool GetConfiguration(string name)
        {
            GetConfiguration obj = new GetConfiguration(name);
            return this.SendXml(Serializers.Serializer<GetConfiguration>(obj));
        }

        // Token: 0x060002F5 RID: 757 RVA: 0x00007A88 File Offset: 0x00005C88
        public void SendWaveformTrigger(Model.WaveformTriggerMode mode)
        {
            WaveformTrigger obj = new WaveformTrigger(mode);
            this.SendXml(Serializers.Serializer<WaveformTrigger>(obj));
        }

        // Token: 0x060002F6 RID: 758 RVA: 0x00007AAC File Offset: 0x00005CAC
        public void SendTestRequest(string testName, string testParameter)
        {
            TestRequest obj = new TestRequest(testName, testParameter);
            this.SendXml(Serializers.Serializer<TestRequest>(obj));
        }

        // Token: 0x060002F7 RID: 759 RVA: 0x00007AD0 File Offset: 0x00005CD0
        public void SetHighVoltageControl(HighVoltageControlMethod method, double setpoint, double slope, double sampleInterval)
        {
            SetHighVoltageControl obj = new SetHighVoltageControl(method, (double)((float)setpoint), (double)((float)sampleInterval), (double)((float)slope));
            this.SendXml(Serializers.Serializer<SetHighVoltageControl>(obj));
        }

        // Token: 0x060002F8 RID: 760 RVA: 0x00007AFC File Offset: 0x00005CFC
        public void SendSqlQuery(string query)
        {
            SqlQuery obj = new SqlQuery(query);
            this.SendXml(Serializers.Serializer<SqlQuery>(obj));
        }

        // Token: 0x060002F9 RID: 761 RVA: 0x00007B20 File Offset: 0x00005D20
        public void SendMeasurementQuery(string name, DateTime start, DateTime end)
        {
            SqlMeasurementQuery obj = new SqlMeasurementQuery(name, start, end);
            this.SendXml(Serializers.Serializer<SqlMeasurementQuery>(obj));
        }

        // Token: 0x060002FA RID: 762 RVA: 0x00007B44 File Offset: 0x00005D44
        public void CancelMeasurementQuery()
        {
            SqlMeasurementQuery obj = new SqlMeasurementQuery(true);
            this.SendXml(Serializers.Serializer<SqlMeasurementQuery>(obj));
        }

        // Token: 0x060002FB RID: 763 RVA: 0x00007B68 File Offset: 0x00005D68
        public void SendEventQuery(string name, DateTime start, DateTime end)
        {
            SqlEventQuery obj = new SqlEventQuery(name, start, end);
            this.SendXml(Serializers.Serializer<SqlEventQuery>(obj));
        }

        // Token: 0x060002FC RID: 764 RVA: 0x00007B8C File Offset: 0x00005D8C
        public void SendAllEventQuery(DateTime start, DateTime end)
        {
            SqlEventQuery obj = new SqlEventQuery("All", start, end);
            this.SendXml(Serializers.Serializer<SqlEventQuery>(obj));
        }

        // Token: 0x060002FD RID: 765 RVA: 0x00007BB4 File Offset: 0x00005DB4
        public void GetDatabaseSummary(bool includeRecordCount)
        {
            GetDatabaseSummary obj = new GetDatabaseSummary(includeRecordCount);
            this.SendXml(Serializers.Serializer<GetDatabaseSummary>(obj));
        }

        // Token: 0x060002FE RID: 766 RVA: 0x00007BD8 File Offset: 0x00005DD8
        public void GetDatabaseMeasurementSummary(string measName, DateTime start, DateTime end)
        {
            GetDatabaseMeasurementSummary obj = new GetDatabaseMeasurementSummary(measName, start, end);
            this.SendXml(Serializers.Serializer<GetDatabaseMeasurementSummary>(obj));
        }

        // Token: 0x060002FF RID: 767 RVA: 0x00007BFC File Offset: 0x00005DFC
        public void DeleteDatabaseRecordsOlderThan(string table, DateTime time, bool compact)
        {
            DeleteDatabaseRecords obj = new DeleteDatabaseRecords(table, time, compact);
            this.SendXml(Serializers.Serializer<DeleteDatabaseRecords>(obj));
        }

        // Token: 0x06000300 RID: 768 RVA: 0x00007C20 File Offset: 0x00005E20
        public void SetElectrometerId(string serialNumber, string revision)
        {
            SetElectrometerId obj = new SetElectrometerId(serialNumber, revision);
            this.SendXml(Serializers.Serializer<SetElectrometerId>(obj));
        }

        // Token: 0x06000301 RID: 769 RVA: 0x00007C44 File Offset: 0x00005E44
        public void GetElectrometerId()
        {
            GetElectrometerId obj = new GetElectrometerId();
            this.SendXml(Serializers.Serializer<GetElectrometerId>(obj));
        }

        // Token: 0x06000302 RID: 770 RVA: 0x00007C64 File Offset: 0x00005E64
        public void SetUnitId(UnitInfo unitInfo)
        {
            SetUnitId obj = new SetUnitId(unitInfo);
            this.SendXml(Serializers.Serializer<SetUnitId>(obj));
        }

        // Token: 0x06000303 RID: 771 RVA: 0x00007C88 File Offset: 0x00005E88
        public void GetUnitId()
        {
            GetUnitId obj = new GetUnitId();
            this.SendXml(Serializers.Serializer<GetUnitId>(obj));
        }

        // Token: 0x06000304 RID: 772 RVA: 0x00007CA8 File Offset: 0x00005EA8
        public void GetHpicConfiguration()
        {
            GetHpicConfiguration obj = new GetHpicConfiguration();
            this.SendXml(Serializers.Serializer<GetHpicConfiguration>(obj));
        }

        // Token: 0x06000305 RID: 773 RVA: 0x00007CC8 File Offset: 0x00005EC8
        public void SetHpicConfiguration(Model.Electrometer.HpicConfiguration config)
        {
            SetHpicConfiguration obj = new SetHpicConfiguration(config);
            this.SendXml(Serializers.Serializer<SetHpicConfiguration>(obj));
        }

        // Token: 0x06000306 RID: 774 RVA: 0x00007CEC File Offset: 0x00005EEC
        public void SetElectrometerConfiguration(Model.Electrometer.ElectrometerConfiguration config, bool writeToEeprom)
        {
            SetElectrometerConfiguration obj = new SetElectrometerConfiguration(config, writeToEeprom);
            this.SendXml(Serializers.Serializer<SetElectrometerConfiguration>(obj));
        }

        // Token: 0x06000307 RID: 775 RVA: 0x00007D10 File Offset: 0x00005F10
        public void GetElectrometerConfiguration()
        {
            GetElectrometerConfiguration obj = new GetElectrometerConfiguration();
            this.SendXml(Serializers.Serializer<GetElectrometerConfiguration>(obj));
        }

        // Token: 0x06000308 RID: 776 RVA: 0x00007D30 File Offset: 0x00005F30
        public void SetElectrometerConfigurationToDefault()
        {
            SetElectrometerConfigurationToDefault obj = new SetElectrometerConfigurationToDefault();
            this.SendXml(Serializers.Serializer<SetElectrometerConfigurationToDefault>(obj));
        }

        // Token: 0x06000309 RID: 777 RVA: 0x00007D50 File Offset: 0x00005F50
        public void GetPerformanceMeasurement(string name)
        {
            GetPerformanceMeasurement obj = new GetPerformanceMeasurement(name);
            this.SendXml(Serializers.Serializer<GetPerformanceMeasurement>(obj));
        }

        // Token: 0x0600030A RID: 778 RVA: 0x00007D74 File Offset: 0x00005F74
        public void ClearPerformanceMeasurement(string name)
        {
            ClearPerformanceHistogram obj = new ClearPerformanceHistogram(name);
            this.SendXml(Serializers.Serializer<ClearPerformanceHistogram>(obj));
        }

        // Token: 0x0600030B RID: 779 RVA: 0x00007D98 File Offset: 0x00005F98
        public void SetChargerParameter(double voltage, double current)
        {
            SetChargerParameters obj = new SetChargerParameters(voltage, current);
            this.SendXml(Serializers.Serializer<SetChargerParameters>(obj));
        }

        // Token: 0x0600030C RID: 780 RVA: 0x00007DBC File Offset: 0x00005FBC
        public bool SendDiscovery(string commChannelName)
        {
            bool result;
            try
            {
                Logger.LogDebug("SendDiscovery" + commChannelName);
                Discovery obj = new Discovery(commChannelName);
                string ouputText = Serializers.Serializer<Discovery>(obj);
                this.SendXml(ouputText);
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        // Token: 0x0600030D RID: 781 RVA: 0x00007E0C File Offset: 0x0000600C
        public void LogTestData(LogTestMeasurements testMeasurements)
        {
            this.SendXml(Serializers.Serializer<LogTestMeasurements>(testMeasurements));
        }

        // Token: 0x0600030E RID: 782 RVA: 0x00007E1B File Offset: 0x0000601B
        public void StartFirmwareUpdate()
        {
            this.SendXml(Serializers.Serializer<InitiateFirmwareUpdate>(new InitiateFirmwareUpdate()));
        }

        // Token: 0x0600030F RID: 783 RVA: 0x00007E2E File Offset: 0x0000602E
        public void ClearDebugLogFile()
        {
            this.SendXml(Serializers.Serializer<ClearDebugLog>(new ClearDebugLog()));
        }

        // Token: 0x06000310 RID: 784 RVA: 0x00007E41 File Offset: 0x00006041
        public void CopyDatabaseFile()
        {
            this.SendXml(Serializers.Serializer<CopyDatabaseFile>(new CopyDatabaseFile()));
        }

        // Token: 0x06000311 RID: 785 RVA: 0x00007E54 File Offset: 0x00006054
        public void CompactDatabaseFile()
        {
            this.SendXml(Serializers.Serializer<SqlDatabaseCompact>(new SqlDatabaseCompact()));
        }

        // Token: 0x06000312 RID: 786 RVA: 0x00007E67 File Offset: 0x00006067
        public void CleanDatabaseFile()
        {
            this.SendXml(Serializers.Serializer<SqlDatabaseClean>(new SqlDatabaseClean()));
        }


        #endregion

        // Token: 0x06000313 RID: 787 RVA: 0x00007E7C File Offset: 0x0000607C
        public bool SendXml(string ouputText)
        {
            bool result = false;
            try
            {
                int length = ouputText.Length;
                byte[] array = new byte[length + 5];
                array[0] = 240;
                Array.Copy(BitConverter.GetBytes(length), 0, array, 1, 4);
                Array.Copy(Encoding.ASCII.GetBytes(ouputText), 0, array, 5, length);
                if (DebugXml)
                {
                    HveLogger.LogDebug($" ###发送 SendXml request lenght:{length} :\r\n" + "```\r\n" + ouputText + "\r\n```\r\n");
                }

                var t = Task.Run(async () =>
                {
                    var _cmdResult = await TranService.SendAsync(array, "xml");
                    result = _cmdResult.Ok;
                });
                Task.WaitAll(new Task[] { t }, 25000);//等他结束


            }
            catch (Exception ex)
            {
                //SentryDebug.WriteLine(ouputText);
                //SentryDebug.WriteException(ex);
                Logger.LogError(ex, "发送出错");
                result = false;
            }
            return result;
        }

        #region MyRegion
        // Token: 0x06000314 RID: 788 RVA: 0x00007EFC File Offset: 0x000060FC
        public void SendCloseConnectionRequest()
        {
            CloseConnection obj = new CloseConnection();
            this.SendXml(Serializers.Serializer<CloseConnection>(obj));
        }

        // Token: 0x06000315 RID: 789 RVA: 0x00007F1C File Offset: 0x0000611C
        public void SendRestartUnitRequest()
        {
            RestartUnit obj = new RestartUnit();
            this.SendXml(Serializers.Serializer<RestartUnit>(obj));
        }
        #endregion


        #endregion

        #region 消息回调处理

        /// <summary>
        /// TCP或串口直接返回的数据处理
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public Tuple<object, Type> ProcessResponse(byte[] buffer, int length)
        {
            /**====因为考虑到还会有UDP的回发，所以这里读取的配置和状态只缓存不直接插入。=====**/
            string @string = Encoding.ASCII.GetString(buffer, 5, length - 5); //注意我这里要除掉前辍5个字节
            ///SentryDebug.WriteLine("response received " + @string);
            if (DebugXml)
            {
                HveLogger.LogDebug($" === 接收到 Response : lenght:{length} :\r\n```\r\n" + @string + "\r\n```\r\n");
            }
            Tuple<object, Type> tuple = DeserializeUnknownCommand(@string);
            var _serviceProvider = Aming.Tools.IocHelper.ServiceProvider;

            try
            {


                //if (tuple.Item2 == typeof(CommandStatusResponse) && tuple.Item1 != null)
                //{
                //    if (((CommandStatusResponse)tuple.Item1).Command == "CloseConnection")
                //    {
                //        this._dataServiceClosing = true;
                //    }
                //    //this._mvvmMessenger.Send<mvvmCommandStatusResponse>(new mvvmCommandStatusResponse(this, (CommandStatusResponse)tuple.Item1));
                //    return tuple;
                //}
                //if (tuple.Item2 == typeof(CommandStatusResponse) && tuple.Item1 != null)
                //{
                //    if (((CommandStatusResponse)tuple.Item1).Command == "RestartUnit")
                //    {
                //        this._dataServiceClosing = true;
                //    }
                //    //this._mvvmMessenger.Send<mvvmCommandStatusResponse>(new mvvmCommandStatusResponse(this, (CommandStatusResponse)tuple.Item1));
                //    return tuple;
                //}
                //if (tuple.Item2 == typeof(ScalarMeasurementResponse))
                //{
                //    Logger.LogDebug($"未处理{tuple.Item1.GetType()}");
                //    //this._mvvmMessenger.Send<mvvmScalarMeasurement>(new mvvmScalarMeasurement(this, (ScalarMeasurementResponse)tuple.Item1));
                //    return tuple;
                //}
                //if (tuple.Item2 == typeof(EventResponse))
                //{
                //    Logger.LogDebug($"未处理{tuple.Item1.GetType()}");
                //    //this._mvvmMessenger.Send<mvvmSentinelState>(new mvvmSentinelState(this, (EventResponse)tuple.Item1));
                //    return tuple;
                //}
                //if (tuple.Item2 == typeof(PerformanceMeasurementResponse))
                //{
                //    Logger.LogDebug($"未处理{tuple.Item1.GetType()}");
                //    //this._mvvmMessenger.Send<mvvmPerformanceMeasurementResponse>(new mvvmPerformanceMeasurementResponse(this, (PerformanceMeasurementResponse)tuple.Item1));
                //    return tuple;
                //}
                //if (tuple.Item2 == typeof(GetAllEventsResponse))
                //{
                //    Logger.LogDebug($"未处理{tuple.Item1.GetType()}");
                //    //this._mvvmMessenger.Send<mvvmGetAllEventsResponse>(new mvvmGetAllEventsResponse(this, (GetAllEventsResponse)tuple.Item1));
                //    return tuple;
                //}
                if (tuple.Item2 == typeof(GetAllConfigurationsResponse))
                {

                    //this._mvvmMessenger.Send<mvvmGetAllConfigurationsResponse>(new mvvmGetAllConfigurationsResponse(this, (GetAllConfigurationsResponse)tuple.Item1));
                    var list = ((GetAllConfigurationsResponse)tuple.Item1).ConfigurationList;
                    HVEHelper.SetAllConfigurationsResponse(ref this.HveRunStatus, list);
                    return tuple;
                }
                //if (tuple.Item2 == typeof(GetConfigurationResponse))
                //{
                //    Logger.LogDebug($"未处理{tuple.Item1.GetType()}");
                //    //this._mvvmMessenger.Send<mvvmGetConfigurationResponse>(new mvvmGetConfigurationResponse(this, (GetConfigurationResponse)tuple.Item1));
                //    return tuple;
                //}
                if (tuple.Item2 == typeof(GetAllScalarMeasurementsResponse))
                {
                    //Logger.LogDebug($"未处理{tuple.Item1.GetType()}");
                    //this._mvvmMessenger.Send<mvvmGetAllScalarMeasurementsResponse>(new mvvmGetAllScalarMeasurementsResponse(this, (GetAllScalarMeasurementsResponse)tuple.Item1));
                    var list = ((GetAllScalarMeasurementsResponse)tuple.Item1).MeasurementList;
                    HVEHelper.SetScalarMeasurementsResponse(ref this.HveRecord, ref this.HveExtRecord, list);
                    return tuple;
                }
                //if (tuple.Item2 == typeof(GetElectrometerConfigurationResponse))
                //{
                //    Logger.LogDebug($"未处理{tuple.Item1.GetType()}");
                //    //this._mvvmMessenger.Send<mvvmGetElectrometerConfigurationResponse>(new mvvmGetElectrometerConfigurationResponse(this, (GetElectrometerConfigurationResponse)tuple.Item1));
                //    return tuple;
                //}
                //if (tuple.Item2 == typeof(GetElectrometerIdResponse))
                //{
                //    Logger.LogDebug($"未处理{tuple.Item1.GetType()}");
                //    //this._mvvmMessenger.Send<mvvmGetElectrometerIdResponse>(new mvvmGetElectrometerIdResponse(this, (GetElectrometerIdResponse)tuple.Item1));
                //    return tuple;
                //}
                //if (tuple.Item2 == typeof(GetUnitIdResponse))
                //{
                //    Logger.LogDebug($"未处理{tuple.Item1.GetType()}");
                //    //this._mvvmMessenger.Send<mvvmGetUnitIdResponse>(new mvvmGetUnitIdResponse(this, (GetUnitIdResponse)tuple.Item1));
                //    return tuple;
                //}
                //if (tuple.Item2 == typeof(GetHpicConfigurationResponse))
                //{
                //    Logger.LogDebug($"未处理{tuple.Item1.GetType()}");
                //    //this._mvvmMessenger.Send<mvvmGetHpicConfigurationResponse>(new mvvmGetHpicConfigurationResponse(this, (GetHpicConfigurationResponse)tuple.Item1));
                //    return tuple;
                //}
                if (tuple.Item2 == typeof(SqlQueryResponse))
                {
                    Logger.LogDebug($"未处理{tuple.Item1.GetType()}");
                    //this._mvvmMessenger.Send<mvvmDatasetResponse>(new mvvmDatasetResponse(this, (SqlQueryResponse)tuple.Item1));
                    return tuple;
                }
                if (tuple.Item2 == typeof(SqlEventQueryResponse))
                {
                    Logger.LogDebug($"未处理{tuple.Item1.GetType()}");
                    //this._mvvmMessenger.Send<mvvmEventQueryResponse>(new mvvmEventQueryResponse(this, (SqlEventQueryResponse)tuple.Item1));
                    return tuple;
                }
                if (tuple.Item2 == typeof(SqlMeasurementQueryResponse))
                {
                    Logger.LogDebug($"未处理{tuple.Item1.GetType()}");
                    //this._mvvmMessenger.Send<mvvmMeasurementQueryResponse>(new mvvmMeasurementQueryResponse(this, (SqlMeasurementQueryResponse)tuple.Item1));
                    return tuple;
                }
                //if (tuple.Item2 == typeof(TestResponse))
                //{
                //    Logger.LogDebug($"未处理{tuple.Item1.GetType()}");
                //    //this._mvvmMessenger.Send<mvvmTestResponse>(new mvvmTestResponse(this, (TestResponse)tuple.Item1));
                //    return tuple;
                //}
                //if (tuple.Item2 == typeof(GetDatabaseSummaryResponse))
                //{
                //    Logger.LogDebug($"未处理{tuple.Item1.GetType()}");
                //    //this._mvvmMessenger.Send<mvvmGetDatabaseSummaryResponse>(new mvvmGetDatabaseSummaryResponse(this, (GetDatabaseSummaryResponse)tuple.Item1));
                //    return tuple;
                //}
                //if (tuple.Item2 == typeof(DiscoveryResponse))
                //{
                //    Logger.LogDebug($"未处理{tuple.Item1.GetType()}");
                //    //this._mvvmMessenger.Send<DiscoveryResponse>((DiscoveryResponse)tuple.Item1);
                //    return tuple;
                //}
                //if (tuple.Item2 == typeof(GetTimeResponse))
                //{
                //    Logger.LogDebug($"未处理{tuple.Item1.GetType()}");
                //    //this._mvvmMessenger.Send<mvvmGetTimeResponse>(new mvvmGetTimeResponse(this, (GetTimeResponse)tuple.Item1));
                //    return tuple;
                //}
                //if (tuple.Item2 == typeof(ScalarMeasurement))
                //{
                //    Logger.LogDebug($"未处理{tuple.Item1.GetType()}");
                //    //this._mvvmMessenger.Send<mvvmScalarMeasurement>(new mvvmScalarMeasurement(this, (ScalarMeasurement)tuple.Item1));
                //    return tuple;
                //}
                //if (tuple.Item2 == typeof(SentinelState))
                //{
                //    Logger.LogDebug($"未处理{tuple.Item1.GetType()}");
                //    //this._mvvmMessenger.Send<mvvmSentinelState>(new mvvmSentinelState(this, (SentinelState)tuple.Item1));
                //    return tuple;
                //}
                //if (tuple.Item2 == typeof(BatteryMeasurement))
                //{
                //    Logger.LogDebug($"未处理{tuple.Item1.GetType()}");
                //    //this._mvvmMessenger.Send<mvvmBatteryMeasurement>(new mvvmBatteryMeasurement(this, (BatteryMeasurement)tuple.Item1));
                //    return tuple;
                //}
            }
            catch (Exception ex)
            {
                Logger.LogDebug("处理数据出错！");
            }
            return tuple;
            //SentryDebug.WriteLine("Unrecognized response received " + @string);
        }

        // Token: 0x0600031F RID: 799 RVA: 0x00008944 File Offset: 0x00006B44
        private static Tuple<object, Type> DeserializeUnknownCommand(string xmlCommand)
        {
            Tuple<object, Type> result;
            try
            {
                if (xmlCommand.Contains(typeof(CommandStatusResponse).Name))
                {
                    result = new Tuple<object, Type>(Serializers.Deserialize<CommandStatusResponse>(xmlCommand), typeof(CommandStatusResponse));
                }
                else if (xmlCommand.Contains(typeof(GetAllConfigurationsResponse).Name))
                {
                    var o = Serializers.Deserialize<GetAllConfigurationsResponse>(xmlCommand);
                    result = new Tuple<object, Type>(o, typeof(GetAllConfigurationsResponse));
                }
                else if (xmlCommand.Contains(typeof(GetConfigurationResponse).Name))
                {
                    result = new Tuple<object, Type>(Serializers.Deserialize<GetConfigurationResponse>(xmlCommand), typeof(GetConfigurationResponse));
                }
                else if (xmlCommand.Contains(typeof(GetAllScalarMeasurementsResponse).Name))
                {
                    result = new Tuple<object, Type>(Serializers.Deserialize<GetAllScalarMeasurementsResponse>(xmlCommand), typeof(GetAllScalarMeasurementsResponse));
                }
                else if (xmlCommand.Contains(typeof(GetAllEventsResponse).Name))
                {
                    result = new Tuple<object, Type>(Serializers.Deserialize<GetAllEventsResponse>(xmlCommand), typeof(GetAllEventsResponse));
                }
                else if (xmlCommand.Contains(typeof(EventResponse).Name))
                {
                    result = new Tuple<object, Type>(Serializers.Deserialize<EventResponse>(xmlCommand), typeof(EventResponse));
                }
                else if (xmlCommand.Contains(typeof(PerformanceMeasurementResponse).Name))
                {
                    result = new Tuple<object, Type>(Serializers.Deserialize<PerformanceMeasurementResponse>(xmlCommand), typeof(PerformanceMeasurementResponse));
                }
                else if (xmlCommand.Contains(typeof(TestResponse).Name))
                {
                    result = new Tuple<object, Type>(Serializers.Deserialize<TestResponse>(xmlCommand), typeof(TestResponse));
                }
                else if (xmlCommand.Contains(typeof(SqlQueryResponse).Name))
                {
                    result = new Tuple<object, Type>(Serializers.Deserialize<SqlQueryResponse>(xmlCommand), typeof(SqlQueryResponse));
                }
                else if (xmlCommand.Contains(typeof(SqlEventQueryResponse).Name))
                {
                    result = new Tuple<object, Type>(Serializers.Deserialize<SqlEventQueryResponse>(xmlCommand), typeof(SqlEventQueryResponse));
                }
                else if (xmlCommand.Contains(typeof(SqlMeasurementQueryResponse).Name))
                {
                    SqlMeasurementQueryResponse item = Serializers.Deserialize<SqlMeasurementQueryResponse>(xmlCommand);
                    result = new Tuple<object, Type>(item, typeof(SqlMeasurementQueryResponse));
                }
                else if (xmlCommand.Contains(typeof(GetElectrometerIdResponse).Name))
                {
                    result = new Tuple<object, Type>(Serializers.Deserialize<GetElectrometerIdResponse>(xmlCommand), typeof(GetElectrometerIdResponse));
                }
                else if (xmlCommand.Contains(typeof(GetUnitIdResponse).Name))
                {
                    result = new Tuple<object, Type>(Serializers.Deserialize<GetUnitIdResponse>(xmlCommand), typeof(GetUnitIdResponse));
                }
                else if (xmlCommand.Contains(typeof(GetHpicConfigurationResponse).Name))
                {
                    result = new Tuple<object, Type>(Serializers.Deserialize<GetHpicConfigurationResponse>(xmlCommand), typeof(GetHpicConfigurationResponse));
                }
                else if (xmlCommand.Contains(typeof(GetElectrometerConfigurationResponse).Name))
                {
                    result = new Tuple<object, Type>(Serializers.Deserialize<GetElectrometerConfigurationResponse>(xmlCommand), typeof(GetElectrometerConfigurationResponse));
                }
                else if (xmlCommand.Contains(typeof(ScalarMeasurementResponse).Name))
                {
                    result = new Tuple<object, Type>(Serializers.Deserialize<ScalarMeasurementResponse>(xmlCommand), typeof(ScalarMeasurementResponse));
                }
                else if (xmlCommand.Contains(typeof(GetDatabaseSummaryResponse).Name))
                {
                    result = new Tuple<object, Type>(Serializers.Deserialize<GetDatabaseSummaryResponse>(xmlCommand), typeof(GetDatabaseSummaryResponse));
                }

                //hiamin 注：原来没有Discovery这一样，也不能加，否则就发现不了设备了

                else if (xmlCommand.Contains(typeof(DiscoveryResponse).Name))
                {
                    result = new Tuple<object, Type>(Serializers.Deserialize<DiscoveryResponse>(xmlCommand), typeof(DiscoveryResponse));
                }
                else if (xmlCommand.Contains(typeof(GetTimeResponse).Name))
                {
                    result = new Tuple<object, Type>(Serializers.Deserialize<GetTimeResponse>(xmlCommand), typeof(GetTimeResponse));
                }
                else if (xmlCommand.Contains(typeof(ScalarMeasurement).Name))
                {
                    result = new Tuple<object, Type>(Serializers.Deserialize<ScalarMeasurement>(xmlCommand), typeof(ScalarMeasurement));
                }
                else if (xmlCommand.Contains(typeof(SentinelState).Name))
                {
                    result = new Tuple<object, Type>(Serializers.Deserialize<SentinelState>(xmlCommand), typeof(SentinelState));
                }
                else if (xmlCommand.Contains(typeof(BatteryMeasurement).Name))
                {
                    result = new Tuple<object, Type>(Serializers.Deserialize<BatteryMeasurement>(xmlCommand), typeof(BatteryMeasurement));
                }
                else
                {
                    result = null;
                }

            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            if (this.TranService != null && this.TranService.IsConnected)
            {
                var t = Task.Run(() =>
                {
                    //要停止
                    this.SetNotifyEnable(false);

                });
                Task.WaitAll(new Task[] { t }, 25000);//等他结束
                                                      //                Model.Notifications.UdpNotifyInput.
            }
        }


        #endregion

        //hiaming 打印xml通讯细节，日志会很多，启动时命令行添加   /debugxml可开启
        public static bool DebugXml = false;
        private bool _dataServiceClosing;
        //internal static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    }
}