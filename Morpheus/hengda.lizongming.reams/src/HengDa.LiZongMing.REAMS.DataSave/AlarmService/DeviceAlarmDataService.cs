using HengDa.LiZongMing.REAMS.Devices;
using HengDa.LiZongMing.REAMS.Devices.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using Volo.Abp.ObjectMapping;

namespace HengDa.LiZongMing.REAMS.DeviceAlarm
{
    /// <summary>
    /// ����������,������������֪ͨ����
    /// </summary>
    public abstract class DeviceAlarmDataService :  ISingletonDependency
    {
        private ILoggerFactory loggerFactory;
        private ILogger Logger;
        private readonly IObjectMapper ObjectMapper;
        protected IBackgroundJobManager BackgroundJobManager { get; }

        private readonly IIotDeviceAlarmRepository deviceAlarmRepository;
        /// <summary>
        /// Constructor.
        /// </summary>
        protected DeviceAlarmDataService(ILoggerFactory loggerFactory,
            IIotDeviceAlarmRepository deviceAlarmRepository,
            IObjectMapper ObjectMapper,
            IBackgroundJobManager backgroundJobManager)
        {
            this.loggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger<DeviceAlarmDataService>();


            this.deviceAlarmRepository = deviceAlarmRepository;
            this.ObjectMapper = ObjectMapper;
            BackgroundJobManager = backgroundJobManager;
        }

        public async Task CreateAsync(IotDeviceAlarmDto input,Guid? TenantId)
        {
            var _serviceProvider = Aming.Tools.IocHelper.ServiceProvider;
            var deviceAlarm = this.ObjectMapper.Map<IotDeviceAlarmDto, IotDeviceAlarm>(input);
            using (var scope = _serviceProvider.CreateScope())
            {
                var uowManager = scope.ServiceProvider.GetRequiredService<Volo.Abp.Uow.IUnitOfWorkManager>();

                using (var uow = uowManager.Begin())
                {
                    try
                    {
                        deviceAlarm.TenantId = TenantId;
                        //deviceAlarm.DeviceId = service.Device.Id;

                        await deviceAlarmRepository.InsertAsync(deviceAlarm, true);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogDebug("����ʱ�����ˣ�");
                    }
                }
            }



            //TODO:�����Ӹ��ݲ�ͬ�ľ��漶�𣬵��ú�̨֪ͨ���񣬷����ʼ������š�΢�ŵ�֪ͨ


        }



    }
}
