using Aming.Core;
using Aming.DTU;
using Aming.DTU.Config;
using Volo.Abp.ObjectMapping;
using HengDa.LiZongMing.REAMS.HVE;
using HengDa.LiZongMing.REAMS.HVE.Dtos;
using HengDa.LiZongMing.REAMS.Wpf.Dto;
using HengDa.LiZongMing.REAMS.ZCQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Volo.Abp.AutoMapper;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;

namespace HengDa.LiZongMing.REAMS.CtrlServices
{
    /// <summary>
    /// 保存数据用的类
    /// </summary>
    public class HVEDataService : ISingletonDependency
    {
        private ILoggerFactory loggerFactory;
        private ILogger Logger;
        private readonly IHveRecordRepository hveRecordRepository;
        private readonly IHveExtRecordRepository hveExtRecordRepository;
        private readonly IHveRunStatusRepository hveRunStatusRepository;
        private readonly IObjectMapper ObjectMapper;

        /// <summary>
        /// 事件回调时，收到数据触发解析
        /// </summary>
        public Action<MqttClientConfig, UniMessageBase, ZCQBaseDto> OnUniMessageReceivedCallBack { get; set; } = null;

        public HVEDataService(ILoggerFactory loggerFactory,
                            IHveRecordRepository hveRecordRepository,
                            IHveExtRecordRepository hveExtRecordRepository,
                            IHveRunStatusRepository hveRunStatusRepository,
                            IObjectMapper ObjectMapper)
        {

            this.loggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger<CtrlHDZCQService>();


            this.hveRecordRepository = hveRecordRepository;
            this.hveExtRecordRepository = hveExtRecordRepository;
            this.hveRunStatusRepository = hveRunStatusRepository;
            this.ObjectMapper = ObjectMapper;
        }


        /// <summary>
        /// 处理数据的保存
        /// </summary>
        /// <param name="mqttClientConfig"></param>
        /// <param name="msg"></param>
        /// <param name="dtoData"></param>
        public async Task SaveRecord(CtrlHVEService service,  HveRecordDto dtoData)
        {
            var _serviceProvider = Aming.Tools.IocHelper.ServiceProvider;
            using (var scope = _serviceProvider.CreateScope())
            {
                var uowManager = scope.ServiceProvider.GetRequiredService<Volo.Abp.Uow.IUnitOfWorkManager>();

                using (var uow = uowManager.Begin())
                {
                    try
                    {
                        dtoData.TenantId = service.Device.TenantId;
                        dtoData.DeviceId = service.Device.Id;

                        var hveRecord = this.ObjectMapper.Map<HveRecordDto,HveRecord>(dtoData);

                        await hveRecordRepository.InsertAsync(hveRecord,true);
                    }catch(Exception ex)
                    {
                        Logger.LogDebug("保存时出错了！");
                    }
                }
            }
        }

        public async Task SaveExtRecord(CtrlHVEService service, HveExtRecordDto dtoData)
        {
            var _serviceProvider = Aming.Tools.IocHelper.ServiceProvider;
            using (var scope = _serviceProvider.CreateScope())
            {
                var uowManager = scope.ServiceProvider.GetRequiredService<Volo.Abp.Uow.IUnitOfWorkManager>();

                using (var uow = uowManager.Begin())
                {
                    try
                    {
                        dtoData.TenantId = service.Device.TenantId;
                        dtoData.DeviceId = service.Device.Id;

                        var hveExtRecord = this.ObjectMapper.Map<HveExtRecordDto, HveExtRecord>(dtoData);

                        await hveExtRecordRepository.InsertAsync(hveExtRecord, true);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogDebug("保存时出错了！");
                    }
                }
            }
        }
        public async Task UpdateHveRunStatus(CtrlHVEService service, HveRunStatusDto dtoData)
        {
            var _serviceProvider = Aming.Tools.IocHelper.ServiceProvider;
            using (var scope = _serviceProvider.CreateScope())
            {
                var uowManager = scope.ServiceProvider.GetRequiredService<Volo.Abp.Uow.IUnitOfWorkManager>();

                using (var uow = uowManager.Begin())
                {
                    try
                    {
                        dtoData.TenantId = service.Device.TenantId;
                        dtoData.DeviceId = service.Device.Id;

                        var hveRunStatus = await hveRunStatusRepository.FindAsync(m=>m.DeviceId== service.Device.Id
                                                            && m.TenantId== service.Device.TenantId
                                                            //&& m.CreationTime == dtoData.CreationTime  //如果不存多条就不要这个条件了
                                                            );
                        if (hveRunStatus == null)
                        {
                            hveRunStatus=this.ObjectMapper.Map<HveRunStatusDto, HveRunStatus>(dtoData);
                            await hveRunStatusRepository.InsertAsync(hveRunStatus, true);
                        }
                        else
                        {
                            this.ObjectMapper.Map<HveRunStatusDto, HveRunStatus>(dtoData, hveRunStatus);
                            await hveRunStatusRepository.UpdateAsync(hveRunStatus, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogDebug("保存时出错了！");
                    }
                }
            }
        }
    }
}
