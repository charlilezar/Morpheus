using Aming.Core;
using Aming.DTU;
using Aming.DTU.Config;
using HengDa.LiZongMing.REAMS.Wpf.Dto;
using HengDa.LiZongMing.REAMS.ZJC;
using HengDa.LiZongMing.REAMS.ZJC.Dtos;
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
using Volo.Abp.Threading;
using Volo.Abp.MultiTenancy;

namespace HengDa.LiZongMing.REAMS.CtrlServices
{
    /// <summary>
    /// 保存数据用的类
    /// </summary>
    public class HDZJCDataService : ISingletonDependency
    {
        private IObjectMapper ObjectMapper;
        private ILoggerFactory loggerFactory;
        private ILogger Logger;
        /// <summary>
        /// 事件回调时，收到数据触发解析
        /// </summary>
        public Action<MqttClientConfig, UniMessageBase, ZJCBaseDto> OnUniMessageReceivedCallBack { get; set; } = null;
        public HDZJCDataService(ILoggerFactory loggerFactory, IObjectMapper objectMapper)
        {
            this.ObjectMapper = objectMapper;
            this.loggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger<CtrlHDZJCService>();
        }
        /// <summary>
        /// 处理数据的保存
        /// </summary>
        /// <param name="mqttClientConfig"></param>
        /// <param name="msg"></param>
        /// <param name="dtoData"></param>
        public async Task<bool> DataSave(CtrlHDZJCService zjcSevice, UniMessageBase msg, ZJCBaseDto dtoData)
        {
            var _serviceProvider = Aming.Tools.IocHelper.ServiceProvider;
            if (zjcSevice == null)
            {
                Logger.LogError($"CtrlHDZJCService对象获取失败！");
                return false;
            }
            long deviceId = zjcSevice.Device.Id;//读取设备唯一标识
            Guid? gTenantId = zjcSevice.Device.TenantId;//读取租户信息
            using (var scope = _serviceProvider.CreateScope())
            {
                var uowManager = scope.ServiceProvider.GetRequiredService<Volo.Abp.Uow.IUnitOfWorkManager>();
                ICurrentTenant currentTenant = scope.ServiceProvider.GetService<ICurrentTenant>();
                using (currentTenant.Change(gTenantId)) //切换租户
                {
                    using (var uow = uowManager.Begin())
                    {
                        //TODO:按不同的类型保存数据
                        if (dtoData is ZJCRealDataDto dto2)
                        {
                            if (!await this.CreateDeviceRunStatusEntity(zjcSevice, scope)) return false;
                            #region 设备实时状态赋值
                            if (this._ZjcRunStatusDto == null)
                            {
                                this.Logger.LogError($"RunStatusDto为空，无法更新！");
                                return false;
                            }
                            this._ZjcRunStatusDto.DryDepositionCount = dto2.DryDepositionCount;
                            this._ZjcRunStatusDto.Status = dto2.Status;
                            this._ZjcRunStatusDto.InstrumentTime = dto2.InstrumentTime;
                            this._ZjcRunStatusDto.RainfallYesterday = dto2.RainfallYesterday;
                            this._ZjcRunStatusDto.RainfallCurrent = dto2.RainfallCurrent;
                            this._ZjcRunStatusDto.Temperature = dto2.Temperature;
                            this._ZjcRunStatusDto.TempOfBox = dto2.TempOfBox;
                            this._ZjcRunStatusDto.TempOfBucket = dto2.TempOfBucket;
                            this._ZjcRunStatusDto.TempOfRainSensor = dto2.TempOfRainSensor;
                            this._ZjcRunStatusDto.Raintimes = dto2.Raintimes;
                            this._ZjcRunStatusDto.FilledWater = dto2.FilledWater;
                            this._ZjcRunStatusDto.Humidity = dto2.Humidity;
                            this._ZjcRunStatusDto.AlarmLidOpenedOver = dto2.AlarmLidOpenedOver;
                            this._ZjcRunStatusDto.AlarmLidClosedOver = dto2.AlarmLidClosedOver;
                            this._ZjcRunStatusDto.AlarmBoxTemp = dto2.AlarmBoxTemp;
                            this._ZjcRunStatusDto.AlarmBucketTemp = dto2.AlarmBucketTemp;
                            this._ZjcRunStatusDto.AlarmRainSensor = dto2.AlarmRainSensor;
                            this._ZjcRunStatusDto.AlarmDryBucketFilled = dto2.AlarmDryBucketFilled;
                            this._ZjcRunStatusDto.AlarmDryBucketWaterLess = dto2.AlarmDryBucketWaterLess;
                            this._ZjcRunStatusDto.AlarmJyqFilled = dto2.AlarmJyqFilled;
                            this._ZjcRunStatusDto.AlarmTemperature = dto2.AlarmTemperature;
                            this._ZjcRunStatusDto.RunUpdateTime = DateTime.Now;
                            this._ZjcRunStatusDto.AlarmUpdateTime = DateTime.Now;
                            #endregion
                            //更新数据库
                            await this.ZjcRunStatusRepositoryUpdate(zjcSevice, dto2.Cmd, scope);
                        }
                    }
                }
                return true;
            }
        }
        #region 仓储对象
        IZjcRunStatusRepository _ZjcRunStatusRepository = null;
        IZjcRecordRepository _ZjcRecordRepository = null;
        IZjcRainRecordRepository _ZjcRainRecordRepository = null;
        #endregion
        #region 干沉降数据处理
        /// <summary>
        /// 判断干沉降数据是否已经存在了
        /// </summary>
        /// <param name="zjcSevice">逻辑处理层对象</param>
        /// <param name="dto">干沉降的DTO数据</param>
        /// <param name="blExist">是否存在</param>
        /// <returns>是否判断成功</returns>
        public bool DryRecordExist(CtrlHDZJCService zjcSevice, ZJCDryDepositionDataDto dto,out bool blExist)
        {
            var _serviceProvider = Aming.Tools.IocHelper.ServiceProvider;
            long deviceId = zjcSevice.Device.Id;//读取设备唯一标识
            Guid? gTenantId = zjcSevice.Device.TenantId;//读取租户信息
            using (var scope = _serviceProvider.CreateScope())
            {
                var uowManager = scope.ServiceProvider.GetRequiredService<Volo.Abp.Uow.IUnitOfWorkManager>();
                ICurrentTenant currentTenant = scope.ServiceProvider.GetService<ICurrentTenant>();
                using (currentTenant.Change(gTenantId)) //切换租户
                {
                    using (var uow = uowManager.Begin())
                    {

                        this._ZjcRecordRepository = scope.ServiceProvider.GetService<IZjcRecordRepository>();
                        var q = this._ZjcRecordRepository.Where(m => m.DeviceId == deviceId)
                            .Where(m => m.StartTime == dto.StartTime)
                            .Where(m => m.EndTime == dto.EndTime);
                        try
                        {
                            blExist = q.Any();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, "查询干沉降记录出错");
                            blExist = false;
                            return false;
                        }
                    }
                }
            }
        }
        public bool DryRecordSave(CtrlHDZJCService zjcSevice, List<ZJCDryDepositionDataDto> dtos)
        {
            var _serviceProvider = Aming.Tools.IocHelper.ServiceProvider;
            if (zjcSevice == null)
            {
                Logger.LogError($"CtrlHDZJCService对象为空，干沉降记录无法保存！");
                return false;
            }
            long deviceId = zjcSevice.Device.Id;//读取设备唯一标识
            Guid? gTenantId = zjcSevice.Device.TenantId;//读取租户信息
            List<ZjcRecord> zjcRecords = new List<ZjcRecord>();//用于实际存入数据，数据来源为从传入的List<ZjcRainRecordDto>复制；
            using (var scope = _serviceProvider.CreateScope())
            {
                ICurrentTenant currentTenant = scope.ServiceProvider.GetService<ICurrentTenant>();
                using (currentTenant.Change(gTenantId)) //切换租户
                {
                    var uowManager = scope.ServiceProvider.GetRequiredService<Volo.Abp.Uow.IUnitOfWorkManager>();
                    using (var uow = uowManager.Begin())
                    {

                        this._ZjcRecordRepository = scope.ServiceProvider.GetService<IZjcRecordRepository>();
                        foreach (ZJCDryDepositionDataDto dto in dtos)
                        {
                            ZjcRecord zjcRecord = new ZjcRecord()
                            {
                                //TenantId = zjcSevice.Device.TenantId,
                                DeviceId = zjcSevice.Device.Id,
                                StartTime = dto.StartTime,
                                EndTime = dto.EndTime,
                            };
                            zjcRecords.Add(zjcRecord);
                        }
                        try
                        {
                            this._ZjcRecordRepository.InsertManyAsync(zjcRecords, true);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, $"干湿沉降降记录({zjcRecords.Count}行)提交至数据库失败。");
                            return false;
                        }
                        return true;
                    }
                }
            }
        }
        #endregion
        #region 降雨记录数据处理
        public async Task<bool> SaveRainDataDetail(CtrlHDZJCService zjcSevice, List<ZJCRainDataDto> dtos)
        {
            var _serviceProvider = Aming.Tools.IocHelper.ServiceProvider;
            if (zjcSevice == null)
            {
                Logger.LogError($"CtrlHDZJCService对象为空，降雨记录无法保存！");
                return false;
            }
            long deviceId = zjcSevice.Device.Id;//读取设备唯一标识
            Guid? gTenantId = zjcSevice.Device.TenantId;//读取租户信息
            List<ZjcRainRecord> zjcRecords = new List<ZjcRainRecord>();//用于实际存入数据，数据来源为从传入的List<ZjcRainRecordDto>复制；
            using (var scope = _serviceProvider.CreateScope())
            {
                var uowManager = scope.ServiceProvider.GetRequiredService<Volo.Abp.Uow.IUnitOfWorkManager>();
                using (var uow = uowManager.Begin())
                {
                    ICurrentTenant currentTenant = _serviceProvider.GetService<ICurrentTenant>();
                    using (currentTenant.Change(gTenantId)) //切换租户
                    {
                        this._ZjcRainRecordRepository = scope.ServiceProvider.GetService<IZjcRainRecordRepository>();
                        foreach (ZJCRainDataDto dto in dtos)
                        {
                            #region 判断降雨记录是否存在，是的话无需存储
                            //判断是否数据库已经存在该时间段的降雨记录了，判断规则：起始时间和结束时间
                            //这个是很有可能存在的，比如今天的记录设备还未全部生成，但我们已经将数据存入数据了，
                            //待明天打开软件来查询的时候还是会遍历以便的，这个时候只要存储设备最新生成的就可以了。
                            try
                            {
                                var q = this._ZjcRainRecordRepository.Where(m => m.DeviceId == deviceId)
                                    .Where(m => m.StartTime == dto.StartTime)
                                    .Where(m => m.EndTime == dto.EndTime);
                                if (!q.Any())
                                {
                                    ZjcRainRecordDto recordDto = new ZjcRainRecordDto()
                                    {
                                        //TenantId = gTenantId,
                                        DeviceId = deviceId,
                                        StartTime = dto.StartTime,
                                        EndTime = dto.EndTime,
                                        Rainfall = dto.Rainfall,
                                        Intensity = dto.Intensity,
                                    };
                                    zjcRecords.Add(this.MapToGetRecordByDto<ZjcRainRecordDto, ZjcRainRecord>(recordDto));
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError(ex, "查询干湿沉降降雨记录是否存在时出错。");
                                return false;
                            }
                            #endregion
                        }
                        #region 保存降雨记录
                        //统一一起保存
                        if (zjcRecords.Count > 0)
                        {
                            try
                            {
                                await this._ZjcRainRecordRepository.InsertManyAsync(zjcRecords, true);
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError(ex, $"干湿沉降降雨记录({zjcRecords.Count}行)提交至数据库失败。");
                                return false;
                            }
                        }
                        #endregion
                    }
                }
            }
            return true;
        }
        #endregion
        #region 实时数据保存
        /// <summary>
        /// 将每次接受的数据存储在该实例中，这样设备反馈数据后只需更新当前反馈的内容即可，其他保持不变，这样提交至数据库时确保都能有值；
        /// 该对象在系统初始化时从数据库中获得
        /// </summary>
        ZjcRunStatusDto _ZjcRunStatusDto { get; set; }
        private async Task<bool> CreateDeviceRunStatusEntity(CtrlHDZJCService zjcSevice, IServiceScope scope)
        {
            if (this._ZjcRunStatusDto != null) return true;
            this._ZjcRunStatusRepository = scope.ServiceProvider.GetService<IZjcRunStatusRepository>();
            if (this._ZjcRunStatusRepository == null)
            {
                this.Logger.LogError($"设备实时状态的Repository获取为空！数据无法保存。");
                return false;
            }
            ZjcRunStatus zcqRunStatus;
            try
            {
                var query = _ZjcRunStatusRepository
                    .Where(m => m.DeviceId == zjcSevice.Device.Id);
                zcqRunStatus = query.FirstOrDefault();//TODO:如果为空的话Fisrt()会报错，用这个不会报错，如果没数据返回null，与我们想要的结果一致；
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"读取设备[{zjcSevice.Device.Id}]对应的实时状态ID值时出错");
                return false;
            }
            if (zcqRunStatus == null)
            {
                //此时没有，则要新增
                zcqRunStatus = new ZjcRunStatus()
                {
                    DeviceId = zjcSevice.Device.Id,
                    TenantId = zjcSevice.Device.TenantId,//租户，可能为空
                    CreationTime = this.FormatCurrentDateTime(zjcSevice.RunStatusSaveHisIntervalSeconds)
                };
                try
                {
                    //这里zcqRunStatus对象已经被INSERT过后就会被tracking，无法再被update了，所以后面直接要在拷贝一次（用GetNewZjcRunStatusEntity）
                    zcqRunStatus = await this._ZjcRunStatusRepository.InsertAsync(zcqRunStatus, true);
                    this.Logger.LogDebug($"已经新增实时状态数据，返回数据ID[{zcqRunStatus.Id}]。");
                    //这里要直接复制数据库中原有的数据
                    this._ZjcRunStatusDto = this.MapToGetRecordByDto<ZjcRunStatus, ZjcRunStatusDto>(zcqRunStatus);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, $"创建实时状态的空数据时出错");
                    return false;
                }
            }
            else
            {
                this._ZjcRunStatusDto = this.MapToGetRecordByDto<ZjcRunStatus, ZjcRunStatusDto>(zcqRunStatus);
            }
            return true;
        }
        public async Task<bool> ZjcRunStatusRepositoryUpdate(CtrlHDZJCService zjcSevice, HDZJCCmdCode cmd, IServiceScope scope)
        {
            /*************
             * 更新实时数据逻辑：
             * 1、实时数据每隔一定时间存储一条，这个时间间隔暂时写死在这里，待李工设计完这些参数传入后再调整；
             * 2、间隔没到指定时间的，则更新数据数据库
             * ********************/
            if (this._ZjcRunStatusDto == null) return false;
            //设置更新时间
            this._ZjcRunStatusDto.RunUpdateTime = DateTime.Now;
            //将实时状态数据更新至数据库
            this._ZjcRunStatusRepository = scope.ServiceProvider.GetService<IZjcRunStatusRepository>();
            if (this._ZjcRunStatusRepository == null)
            {
                this.Logger.LogError($"设备实时状态的Repository获取为空！数据无法更新。");
                return false;
            }
            TimeSpan ts = DateTime.Now - this._ZjcRunStatusDto.CreationTime;
            ZjcRunStatus zcqRunStatusSave;
            int iSecd = (int)ts.TotalSeconds;
            if (iSecd >= zjcSevice.RunStatusSaveHisIntervalSeconds)//TODO:前期测试可以短一点，后期直接从配置文件中读取
            {
                //此时要插入新的数据了
                this._ZjcRunStatusDto.Id = Guid.Empty;
                this._ZjcRunStatusDto.RunUpdateTime = DateTime.Now;//存储一下更新时间，可以知道具体是什么时间存入的
                this._ZjcRunStatusDto.CreationTime = this.FormatCurrentDateTime(zjcSevice.RunStatusSaveHisIntervalSeconds);//将时间重新修饰一下，不要显示分钟和秒
                zcqRunStatusSave = this.MapToGetRecordByDto<ZjcRunStatusDto, ZjcRunStatus>(this._ZjcRunStatusDto);//MapToGetOutputZjcRunStatus(this._ZjcRunStatusDto);
                try
                {
                    zcqRunStatusSave = await this._ZjcRunStatusRepository.InsertAsync(zcqRunStatusSave, true);
                    this.Logger.LogDebug($"实时状态已达{iSecd}秒，存入一行历史记录,新数据ID[{zcqRunStatusSave.Id}]");
                    this._ZjcRunStatusDto.Id = zcqRunStatusSave.Id;
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, $"实时状态插入历史记录时出错。");
                    return false;
                }
            }
            else
            {
                //注意，更新也需要重新创建实例，否则UpdateAsync会报.....cannot be tracked because another instance with the same key value for {'Id'} is already being tracked.....
                try
                {
                    zcqRunStatusSave = await this._ZjcRunStatusRepository.GetAsync(this._ZjcRunStatusDto.Id);
                    ObjectMapper.Map<ZjcRunStatusDto, ZjcRunStatus>(this._ZjcRunStatusDto, zcqRunStatusSave);
                    await this._ZjcRunStatusRepository.UpdateAsync(zcqRunStatusSave, true);
                    this.Logger.LogDebug($"干湿沉降实时状态[{EnumHelper.GetEnumDescription<HDZJCCmdCode>(cmd)}]更新至数据库成功，距上次存储历史记录{iSecd}秒。");
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, $"干湿沉降实时状态[{EnumHelper.GetEnumDescription<HDZJCCmdCode>(cmd)}]更新至数据库时出错");
                    return false;
                }
            }
            return true;
        }
        #endregion
        #region 功能函数
        public TDestination MapToGetRecordByDto<TSource, TDestination>(TSource source)
        {
            return this.ObjectMapper.Map<TSource, TDestination>(source);
        }
        private DateTime FormatCurrentDateTime(int iIntervalSecds)
        {
            if (iIntervalSecds >= 3600)
            {
                return DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00:01"));
            }
            else if (iIntervalSecds >= 60)
            {
                return DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:01"));
            }
            else
            {
                return DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }
        #endregion
    }
}
