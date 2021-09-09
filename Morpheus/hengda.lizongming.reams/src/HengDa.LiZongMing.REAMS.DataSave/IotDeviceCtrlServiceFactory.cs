using Aming.DTU.Config;
using HengDa.LiZongMing.REAMS.CtrlServices;
using HengDa.LiZongMing.REAMS;
using HengDa.LiZongMing.REAMS.Devices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using Volo.Abp.Threading;
using Volo.Abp.Uow;
using RSNetDevice;
using RSNetDevice.Data;
using RSNetDevice.Model;
using Aming.DTU;
using HengDa.LiZongMing.REAMS.Devices.Dtos;
using AutoMapper;
using Volo.Abp.AutoMapper;
using HengDa.LiZongMing.REAMS.Wpf.IotFrames;

namespace HengDa.LiZongMing.REAMS.DataSave
{

    /// <summary>
    /// 调起硬件通讯的后台服务的总入口
    /// </summary>
    public class IotDeviceCtrlServiceFactory : ISingletonDependency
    {
        //1、声明应用层服务
        public readonly IDeviceRepository _deviceAppService;
        private readonly ITenantRepository _tenantRepository;
        private readonly ICurrentTenant _currentTenant;
        private readonly IServiceProvider _serviceProvider;
        private ILoggerFactory loggerFactory;
        public ILogger Logger { get; set; }
        public IMapper Mapper { get; set; }

        /// <summary>
        /// 转发专用的mqtt服务器连接
        /// </summary>
        public static ICtrlService ForwardMqttServer = null;
        /// <summary>
        /// 所有的设备连接
        /// </summary>
        public static System.Collections.Concurrent.ConcurrentDictionary<string, ICtrlService> ListServices = new System.Collections.Concurrent.ConcurrentDictionary<string, ICtrlService>();


        //2、注入应用层服务
        public IotDeviceCtrlServiceFactory(IDeviceRepository deviceRepository,
            ITenantRepository tenantRepository,
            ICurrentTenant currentTenant,
             IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory)
        {
            this._deviceAppService = deviceRepository;
            _tenantRepository = tenantRepository;
            _currentTenant = currentTenant;
            _serviceProvider = serviceProvider;
            this.loggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger<IotDeviceCtrlServiceFactory>();
            Mapper = serviceProvider.GetService<IMapperAccessor>().Mapper;

        }

        /// <summary>
        /// 入口
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            try
            {
                await CreateIotDeviceConnect();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "DataSave Host terminated unexpectedly!");
                //return 1;
            }
            finally
            {
                //Log.CloseAndFlush();
            }
        }


        public async System.Threading.Tasks.Task SayHelloAsync_SS()
        {
            try
            {
                await CreateIotDeviceConnect();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "DataSave Host terminated unexpectedly!");
                //return 1;
            }
            finally
            {
                //Log.CloseAndFlush();
            }
        }
        /// <summary>
        /// 创
        /// </summary>
        public async Task CreateIotDeviceConnect()
        {
            #region 从数据库载入，注意数据库里直接改数据可能会不更新因为有缓存
            //using (var scope = _serviceProvider.CreateScope())
            //{
            //    var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
            //    using (var uow = uowManager.Begin())
            //    {
            //        //TODO:这里写死一个租户id，以后再改进
            //        using (_currentTenant.Change(Guid.Parse("39f99bf5-c4eb-33d2-a623-591eec290001")))
            //        {

            //            //await DefaultConfig();


            //            await LoadIotDeviceList();

            //            //TODO: 加这里代码
            //            //A1读取设备列表();
            //            //A2创建通讯连接
            //            //B启动轮询
            //            //C处理消息

            //        }
            //    }
            //} 
            #endregion

            #region 从配置文件载入



            await LoadIotDeviceListByJson();




            #endregion


        }
        private async Task LoadIotDeviceListByJson()
        {

            //A读取设备列表();
            //B创建通讯连接
            //C启动轮询
            //D处理消息
            var devicesFile = Directory.GetFiles("devices_config", "*.json");
            List<DeviceDto> devices = new List<DeviceDto>();
            ListServices.Clear();
            for (int i = 0; i < devicesFile.Length; i++)
            {
                var json = File.ReadAllText(devicesFile[i]);
                var device = JsonConvert.DeserializeObject<DeviceDto>(json);
                if (!device.IsEnable)
                    continue; //跳过禁用的设置 //TODO:同事调试时请将不调的设备json里这个值暂时关掉，然后不要提交这个json就行
                ////test
                //if (device.Name == "高压电离室")
                //{
                //    //临时测试内容
                //    device.SN = "123456";
                //    TcpClientConfig c = new TcpClientConfig()
                //    {
                //        //EndPointType= EndPointEnum.TCP,
                //        ServerIP = "192.168.1.33",
                //        ServerPort = 3010,
                //        EnableForward = true,
                //        ForwardToId = "ForwardMqttServer",
                //        ForwardFromTopic = $"/mingdtu/{device.SN}/0/json/down",
                //        ForwardToTopic = $"/mingdtu/{device.SN}/0/json/up"
                //    };
                //    device.HWConnectionJson = JsonConvert.SerializeObject(c);
                //}
                //else if (device.Name == "mqtt转发服务器")
                //{
                //    #region mqtt转发服务器 单独处理
                //    //临时测试内容
                //    device.SN = "1234567";
                //    //MqttClientConfig c = new MqttClientConfig()
                //    //{
                //    //    Id = "ForwardMqttServer",
                //    //    //EndPointType= EndPointEnum.TCP,
                //    //    ServerIP = "192.168.1.250",
                //    //    ServerPort = 1883,
                //    //    ClientId = "to-MQTTserver",

                //    //    UserName = "88888888",
                //    //    Password = "88888888",
                //    //    //Raw2Hex=false, //不转Hex
                //    //    SubTopic = $"/mingdtu/{device.SN}/0/hex/up",
                //    //    //PubTopicUp = $"/mingdtu/{device.SN}/0/hex/up",
                //    //    //PubTopicDown = $"/mingdtu/{device.SN}/0/hex/down",
                //    //    EnableForward = false,
                //    //    //ForwardToId = "MQTTserver",
                //    //    //ForwardFromTopic = $"/mingdtu/{device.SN}/hex/down",
                //    //    //ForwardToTopic = $"/mingdtu/{device.SN}/hex/up"
                //    //};
                //    //device.HWConnectionJson = JsonConvert.SerializeObject(c);

                //    //暂时从名字区分，以后再改进
                //    var config = device.GetConfig();
                //    ForwardMqttServer = new CtrlForwardMqttServer(loggerFactory, config);
                //    ForwardMqttServer.Device = device;
                //    ListServices.AddOrUpdate("ForwardMqttServer", ForwardMqttServer, (k, o1) => { return ForwardMqttServer; });
                //    #endregion

                //    continue; //跳过
                //}
                devices.Add(device);
            }
            // 建立连接
            await Task.Run(new Action(delegate
            {

                for (int i = 0; i < devices.Count; i++)
                {
                    var device = devices[i];
                    if (device.ProductName.Contains("气碘"))
                    {
                        Task.Run(new Action(async () =>
                        {
                            //var zcqService = this._serviceProvider.GetService<ZCQ.ZCQDataSaveService>();
                            //ListServices.AddOrUpdate(device.Id.ToString(), zcqService, (k, o1) => { return zcqService; });
                            //zcqService.StartAsync(device);
                            var zcqService = this._serviceProvider.GetService<CtrlHDZCQService>();
                            zcqService.Init(this.loggerFactory, device.GetConfig());
                            ListServices.AddOrUpdate(device.Id.ToString(), zcqService, (k, o1) => { return zcqService; });
                            zcqService.StartAcquireAsync(device);
                            Logger.LogWarning($"设备[{device.ProductName},{device.SN}]已经启动。");
                        }));
                    }
                    else if (device.ProductName.Contains("动环"))
                    {
                        AsyncHelper.RunSync(async () =>
                        {
                            var ctrlsSService = this._serviceProvider.GetService<CtrlHDSSService>();
                            ctrlsSService.Init(loggerFactory, device.GetConfig());
                            ctrlsSService.Device = device;
                            ListServices.AddOrUpdate(device.Id.ToString(), ctrlsSService, (k, o1) => { return ctrlsSService; });
                            Logger.LogWarning($"设备[{device.ProductName},{device.SN}]已经启动。");
                            //var t = Task.Run(() =>
                            //{
                            //    var ssService = this._serviceProvider.GetService<SS.SSDataSaveService>();
                            //    ssService.StartAsync(device);
                            //});


                        });
                    }
                    else if (device.ProductName.Contains("转发服务器"))
                    {
                        AsyncHelper.RunSync(async () =>
                        {
                            var ctrlmqttServerService = this._serviceProvider.GetService<CtrlForwardMqttServer>();
                            ctrlmqttServerService.Init(loggerFactory, device.GetConfig());
                            ctrlmqttServerService.Device = device;
                            ListServices.AddOrUpdate(device.Id.ToString(), ctrlmqttServerService, (k, o1) => { return ctrlmqttServerService; });
                            Logger.LogWarning($"设备[{device.ProductName},{device.SN}]已经启动。");
                            //var t = Task.Run(() =>
                            //{
                            //    var ssService = this._serviceProvider.GetService<SS.SSDataSaveService>();
                            //    ssService.StartAsync(device);
                            //});


                        });
                    }
                    else if (device.ProductName.Contains("碘化钠谱仪"))
                    {
                        //CtrlHDNSService
                         AsyncHelper.RunSync(async () =>
                          {
                            var ctrlnsService = this._serviceProvider.GetService<CtrlHDNaISService>();
                            ctrlnsService.Init(loggerFactory, device.GetConfig());
                            ctrlnsService.Device = device;
                            ListServices.AddOrUpdate(device.Id.ToString(), ctrlnsService, (k, ol) => { return ctrlnsService; });
                            Logger.LogWarning($"设备[{device.ProductName},{device.SN}]已经启动。");
                        });

                    }
                    else if (device.ProductName.Contains("高压电离室"))
                    { //暂时从名字区分，以后再改进
                        var t = Task.Run(async () =>
                        {
                            var config = device.GetConfig();
                            // CtrlHVEService hveService = new CtrlHVEService(loggerFactory, config);
                            var hveService = this._serviceProvider.GetService<CtrlServices.CtrlHVEService>();
                            hveService.Init(loggerFactory, config);
                            // listServices.Add(config.Id, hveService.TranService);

                            while (true)
                            {
                                using (var scope = _serviceProvider.CreateScope())
                                {
                                    //反复查询状态
                                    //if (hveService.TranService.IsConnected)
                                    //{
                                    //    await hveService.;
                                    //}
                                }
                                await Task.Delay(1000 * 10);
                            }


                            //   await zCQService.StartAcquireAsync(device);
                        });

                    }
                    else if (device.ProductName.Contains("气象"))
                    { //暂时从名字区分，以后再改进
                        var t = Task.Run(async () =>
                        {
                            var config = device.GetConfig();
                            // CtrlHVEService hveService = new CtrlHVEService(loggerFactory, config);
                            var xphService = this._serviceProvider.GetService<CtrlServices.CtrlXPHService>();
                            xphService.Init(loggerFactory, config);
                            xphService.Device = device;
                            // listServices.Add(config.Id, hveService.TranService);

                            while (true)
                            {
                                try
                                {
                                    using (var scope = _serviceProvider.CreateScope())
                                    {
                                        //反复查询状态
                                        if (xphService.TranService.IsConnected)
                                        {
                                            await xphService.CmdExec(XPHCmdCode.实时环境参数数据);
                                            //await xphService.CmdExec(XPHCmdCode.历史数据重读);
                                        }
                                        else
                                        {
                                            await xphService.TranService.TryOpenAsync();
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.LogError(ex,"eeeeee");
                                }
                                await Task.Delay(1000 * 10);
                            }


                            //   await zCQService.StartAcquireAsync(device);
                        });

                    }
                    //else if (device.ProductName.Contains("高压电离室"))
                    //{ //暂时从名字区分，以后再改进
                    //    var config = device.GetConfig();
                    //    CtrlHVEService hveService = new CtrlHVEService(loggerFactory, config, device);
                    //    ListServices.AddOrUpdate(device.Id.ToString(), hveService, (k, o1) => { return hveService; });

                    //    });

                    //    Task.WaitAll(new Task[] { t }, 2500);
                    //}


                    // }


                    //var json = JsonConvert.SerializeObject(devices[i]);
                    //File.WriteAllText($"devices_{devices[i].SN}.json", json);

                }

                foreach (var k in ListServices.Keys)
                {
                    try

                    {

                        var A = ListServices[k];
                        if (A.TranService.Config.EnableForward)
                        {

                            if (ListServices.Keys.Contains(A.TranService.Config.ForwardToId))

                            {
                                var B = ListServices[A.TranService.Config.ForwardToId];
                                string topic = A.TranService.Config.GetForwardFromTopic("", "", ""); //down
                                string topicTo = A.TranService.Config.GetForwardToTopic("", "", ""); //up
                                //TODO:参数要增强
                                //转发
                                A.OnDataForwardTo += ((config, msg) =>
                                {
                                    msg.Topic = topicTo;
                                    var convertmsg = System.Text.Encoding.UTF8.GetString(msg.Payload);
                                    Logger.LogWarning($"convertmsg{convertmsg}");
                                    B.SendUniMessage(msg);
                                });
                                if (!string.IsNullOrEmpty(topic) && B.TranService is MqttClientService bs)
                                {
                                    AsyncHelper.RunSync(async () =>
                                    {
                                        await bs.SubscribeAsync(topic); //订阅需要的主题

                                    });
                                }
                                //回发
                                B.OnUniMessageDataReceived += ((config, msg) =>
                                {
                                    //回发可能要过滤消息主题
                                    if (MatchTopic(A.TranService, msg))
                                    {
                                        A.SendUniMessage(msg);
                                    }
                                });
                            }

                            else
                            {
                                Logger.LogWarning($"消息转发配置不正确，找不到对应的{A.TranService.Config.ForwardToId}项配置");
                            }

                        }


                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, $"出错了");
                    }
                }


            }));
            return;
        }

        private void B_OnDataForwardTo(IDataEndPoint config, UniMessageBase msg)
        {
            //forward to server oper
        }

        private bool MatchTopic(ITransmissionService A, UniMessageBase msg)
        {
            string topic = A.Config.GetForwardFromTopic("", "", ""); //TODO:参数要增强
            //string topic = A.Config.GetForwardToTopic("", "", ""); //TODO:参数要增强
            return topic.IsNullOrWhiteSpace() || topic == msg.Topic;
        }
        public async Task DefaultConfig()
        {
            try
            {
                var device = new Device()
                {
                    Name = "大气碘采样器B",
                    GeoLocation_Altitude = 0,
                    GeoLocation_Lat = 30,
                    GeoLocation_Lng = 120,
                    HWConnectionJson = "{}",
                    ManufactureDate = DateTime.Today,
                    MqttUserName = "88888888",
                    MqttPassword = "88888888",
                    ProductName = "大气碘采样器B",
                    ProductModel = "大气碘采样器型号",
                    SN = "3c6105156c3c",
                    TestAddress = "在测地址BBBB",
                    //CustomerService="售后工程师1",
                    IsEnable = true,
                    TenantId = _currentTenant?.Id // Guid.Parse("39f99bf5-c4eb-33d2-a623-591eec290001")

                };

                var ZcqConfig = new Aming.DTU.Config.MqttClientConfig()
                {
                    ServerIP = "172.16.1.250",
                    ServerPort = 1883,
                    UserName = "88888888",
                    Password = "88888888",
                    //SubTopic = "/mingdtu/dtu/3c6105156c3c/hex/up",
                    //PubTopicUp = "/mingdtu/dtu/3c6105156c3c/hex/up",
                    //PubTopicDown = "/mingdtu/dtu/3c6105156c3c/hex/down"
                    SubTopic = $"/mingdtu/dtu/{device.SN}/hex/up",
                    PubTopicUp = $"/mingdtu/dtu/{device.SN}/hex/up",
                    PubTopicDown = $"/mingdtu/dtu/{device.SN}/hex/down"
                };
                var json = JsonConvert.SerializeObject(ZcqConfig);
                //var json = "{\"name\": \"1234escx\"}";
                //File.WriteAllText("ZcqConfig.json", json);
                device.HWConnectionJson = json;


                IDeviceRepository deviceRepository = _serviceProvider.GetRequiredService<IDeviceRepository>();
                var devices = await deviceRepository.GetListAsync();
                if (!devices.Where(m => m.SN == device.SN).Any())
                {
                    await deviceRepository.InsertAsync(device, true);
                }
                else
                {
                    //await devicRepository.UpdateAsync(device);
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "初始化加载设备通讯时出错!");
                //return 1;
            }
            //A2创建通讯连接
            //B启动轮询
            //C处理消息
        }

    }
}
