using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Aming.DTU.Config;
using HengDa.LiZongMing.REAMS.CtrlServices;
using HengDa.LiZongMing.REAMS.Devices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;

namespace HengDa.LiZongMing.REAMS.DataSave
{
    public class IotFactory : ISingletonDependency, IIotFactory
    {
        private ILoggerFactory loggerFactory;
        public ILogger Logger { get; set; }
        public IServiceScopeFactory _serviceScopeFactory;
        private readonly IServiceProvider _serviceProvider;
        //public IDeviceRepository devicRepository;
        //private readonly IUnitOfWorkManager _unitOfWorkManager;

        public IotFactory(ILoggerFactory loggerFactory, IServiceScopeFactory serviceScopeFactory, IServiceProvider serviceProvider)
        {
            this.loggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger<IotFactory>();
            _serviceScopeFactory = serviceScopeFactory;
            _serviceProvider = serviceProvider;
            //this.devicRepository = devicRepository;
            //_unitOfWorkManager = _serviceProvider.GetRequiredService<IUnitOfWorkManager>();

        }
        /// <summary>
        /// 创
        /// </summary>
        public async Task CreateIotDeviceConnect()
        {
           
            await DefaultConfig();

            await LoadIotDeviceList();
                //A1读取设备列表();
                //A2创建通讯连接
                //B启动轮询
                //C处理消息
           
        }
        public async Task LoadIotDeviceList()
        {
            //using (var scope = _serviceProvider.CreateScope())
            //{
            //    var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

            //    using (var uow = uowManager.Begin())
            //    {

                    IDeviceRepository devicRepository = _serviceProvider.GetRequiredService<IDeviceRepository>();
                    var devices = await devicRepository.GetPagedListAsync(0, 20, "Name");
                    for (int i = 0; i < devices.Count; i++)
                    {
                        //TODO：其它的请补全
                        if (devices[i].ProductName.Contains("气碘"))
                        { //暂时从名字区分，以后再改进
                            CtrlHDZCQService zCQService = new CtrlHDZCQService(loggerFactory, devices[i].HWConnectionJson);
                        }

                    }
                    //uow.SaveChangesAsync();
            //    }
            //}

            Logger.LogDebug("初始化通讯方式：");
            //A2创建通讯连接
            //B启动轮询
            //C处理消息
        }

        public async Task DefaultConfig()
        {
            Device device = new Device()
            {
                SN = "3c6105156c3c",
                IsEnable = true,
                Name = "气碘",
                ProductName = "大气碘采样器"
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
            File.WriteAllText("ZcqConfig.json", json);
            device.HWConnectionJson = json;

            //using (var scope = _serviceProvider.CreateScope())
            //{
            //    var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

            //    using (var uow = uowManager.Begin())
            //    {
                    IDeviceRepository devicRepository = _serviceProvider.GetRequiredService<IDeviceRepository>();
                    if (!devicRepository.AsNoTracking().Where(m => m.SN == device.SN).Any())
                    {
                        await devicRepository.InsertAsync(device);
                    }
                    else
                    {
                        //await devicRepository.UpdateAsync(device);
                    }
            //        uow.SaveChangesAsync();
            //    }
            //}

            //A2创建通讯连接
            //B启动轮询
            //C处理消息
        }
    }
}