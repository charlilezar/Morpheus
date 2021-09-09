

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
//using Volo.Abp.ObjectMapping;



namespace HengDa.LiZongMing.REAMS.Devices
{

    /// <summary>
    /// 设备报警服务
    /// </summary>

    public partial class DeviceAlarmManager : DomainService
    {

        #region 构造方法

        private readonly IIotDeviceAlarmRepository deviceAlarmRepository;


        public DeviceAlarmManager(
                            IIotDeviceAlarmRepository deviceAlarmRepository
                            ) : base()
        {


            this.deviceAlarmRepository = deviceAlarmRepository;
        }

        #endregion


        public async Task CreateAsync(IotDeviceAlarm input)
        {

            await deviceAlarmRepository.InsertAsync(input,true);

            //根据不同的警告级别，调用后台通知服务，发送邮件、短信、微信等通知


        }


    }
}