using Aming.Core;
using Aming.DTU;
using Aming.DTU.Config;
using HengDa.LiZongMing.REAMS.Wpf.Dto;
using HengDa.LiZongMing.REAMS.ZCQ;
using HengDa.LiZongMing.REAMS.ZCQ.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using Volo.Abp.ObjectMapping;
using Volo.Abp.MultiTenancy;
using HengDa.LiZongMing.REAMS.Atm;

namespace HengDa.LiZongMing.REAMS.CtrlServices
{
    /// <summary>
    /// 保存数据用的类
    /// </summary>
    public class XPHDataService : ISingletonDependency
    {
        private IObjectMapper ObjectMapper;
        private ILoggerFactory loggerFactory;
        private ILogger Logger;

        IAtmosphereRecordRepository atmosphereRecordRepository = null;
        /// <summary>
        /// 事件回调时，收到数据触发解析
        /// </summary>
        public Action<MqttClientConfig, UniMessageBase, ZCQBaseDto> OnUniMessageReceivedCallBack { get; set; } = null;
        public XPHDataService(ILoggerFactory loggerFactory, IObjectMapper objectMapper)
        {
            this.ObjectMapper = objectMapper;
            this.loggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger<CtrlHDZCQService>();
        }
        /// <summary>
        /// 处理数据的保存
        /// </summary>
        /// <param name="mqttClientConfig"></param>
        /// <param name="msg"></param>
        /// <param name="dtoData"></param>
        public async void DataSave(CtrlXPHService zcqSevice, UniMessageBase msg, List<XPHBaseDto> dtoDatas)
        {
            var _serviceProvider = Aming.Tools.IocHelper.ServiceProvider;

            if (zcqSevice == null)
            {
                Logger.LogError($"CtrlHDZCQService对象获取失败！");
                return;//false;
            }
            long deviceId = zcqSevice.Device.Id;//读取设备唯一标识
            Guid? gTenantId = zcqSevice.Device.TenantId;//读取租户信息
            using (var scope = _serviceProvider.CreateScope())
            {
                ICurrentTenant currentTenant = scope.ServiceProvider.GetService<ICurrentTenant>();
                using (currentTenant.Change(gTenantId)) //切换租户
                {
                    var uowManager = scope.ServiceProvider.GetRequiredService<Volo.Abp.Uow.IUnitOfWorkManager>();
                    using (var uow = uowManager.Begin())
                    {
                        //按不同的类型保存数据
                        if(dtoDatas==null || dtoDatas.Count == 0)   return;
                        foreach (var dto in dtoDatas)
                        {

                            AtmosphereRecord  record = new AtmosphereRecord()
                            {
                                #region 文件信息赋值
                                TenantId = gTenantId,
                                DeviceId = deviceId,
                                WindSpeed  = dto.风速,
                                WindDirection = dto.风向,
                                Noise = dto.雨量,
                                
                                Temperature = dto.温度,
                                Humidity = dto.湿度,
                                Atmosphere = dto.气压,
                                CreationTime = DateTime.Now,
                                #endregion
                            };
                            ////文件这里处理要先判断是否已经存在了
                            this.atmosphereRecordRepository = scope.ServiceProvider.GetService<IAtmosphereRecordRepository>();
                            var q = this.atmosphereRecordRepository.Where(m => m.DeviceId == deviceId)
                                .Where(m => m.CreationTime == dto.RecordTime);
                            if (q.Any())
                            {
                                this.Logger.LogDebug($"文件[{ dto.RecordTime}]已经存在了，不做更新。");
                            }
                            else
                            {
                                await this.atmosphereRecordRepository.InsertAsync(record, true);
                                this.Logger.LogDebug($"文件批次[{ dto.RecordTime}]不存在，存入数据库。");
                            }
                          
                        }
                    }
                }
                return;//true;
            }
        }
      
     

    }
}
