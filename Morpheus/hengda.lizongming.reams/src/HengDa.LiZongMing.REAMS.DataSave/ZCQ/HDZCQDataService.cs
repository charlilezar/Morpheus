using Aming.Core;
using Aming.DTU;
using Aming.DTU.Config;
using HengDa.LiZongMing.REAMS.DataSave.ZCQ;
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

namespace HengDa.LiZongMing.REAMS.CtrlServices
{
    /// <summary>
    /// 保存数据用的类
    /// </summary>
    public class HDZCQDataService : ISingletonDependency
    {
        private IObjectMapper ObjectMapper;
        private ILoggerFactory loggerFactory;
        private ILogger Logger;
        /// <summary>
        /// 将每次接受的数据存储在该实例中，这样设备反馈数据后只需更新当前反馈的内容即可，其他保持不变，这样提交至数据库时确保都能有值；
        /// </summary>
        ZcqRunStatusDto _ZcqRunStatusDto { get; set; }

        IZcqRunStatusRepository _ZcqRunStatusRepository = null;
        IZcqRecordRepository _ZcqRecordRepository = null;
        List<ZcqRecord> _ReceivedZcqRecord = null;
        /// <summary>
        /// 事件回调时，收到数据触发解析
        /// </summary>
        public Action<MqttClientConfig, UniMessageBase, ZCQBaseDto> OnUniMessageReceivedCallBack { get; set; } = null;
        public HDZCQDataService(ILoggerFactory loggerFactory,
            IObjectMapper objectMapper)
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
        public async void DataSave(CtrlHDZCQService zcqSevice, UniMessageBase msg, ZCQBaseDto dtoData)
        {
            var _serviceProvider = Aming.Tools.IocHelper.ServiceProvider;
            if (!string.IsNullOrEmpty(dtoData.ErrMsg))
            {
                Logger.LogError($"{dtoData.ErrMsg}");
            }
            if (zcqSevice.Device == null)
            {
                return;
            }
            if (zcqSevice == null)
            {
                Logger.LogError($"CtrlHDZCQService对象获取失败！");
                return;//false;
            }
            long deviceId = zcqSevice.Device.Id;//读取设备唯一标识
            Guid? gTenantId = zcqSevice.Device.TenantId;//读取租户信息
            if (zcqSevice == null)
            {
                Logger.LogError($"CtrlHDZCQService对象获取失败！");
                return;//false;
            }
            using (var scope = _serviceProvider.CreateScope())
            {
                ICurrentTenant currentTenant = scope.ServiceProvider.GetService<ICurrentTenant>();
                using (currentTenant.Change(gTenantId)) //切换租户
                {
                    var uowManager = scope.ServiceProvider.GetRequiredService<Volo.Abp.Uow.IUnitOfWorkManager>();
                    using (var uow = uowManager.Begin())
                    {
                        //按不同的类型保存数据
                        if (dtoData is ZCQFileInfoDto dto)
                        {
                            if (!dto.Sucessfully)
                            {
                                this.Logger.LogWarning("传入的文件对象解析不成功，无法保存至数据库");
                                return;//false;
                            }
                            if ((!dto.FileExist))
                            {
                                //zcqSevice.StopFileRead();//此时终止查找文件，超过最大了
                                this.Logger.LogWarning("传入的文件对象解析成功，但文件不存在，无需提交至数据库");
                                return;//true;
                            }
                            if (!zcqSevice.IsFileSave2Db(dto)) return;
                            ZcqRecord zcqRecord = new ZcqRecord()
                            {
                                #region 文件信息赋值
                                TenantId = gTenantId,
                                DeviceId = deviceId,
                                FileNo = dto.FileNo,
                                BatchNumber = dto.BatchNumber,
                                StartTime = dto.StartTime,
                                EndTime = dto.EndTime,
                                InstantaneousFlow = dto.InstantaneousFlow,
                                StandardVolume = dto.StandardVolume,
                                WorkingVolume = dto.WorkingVolume,
                                Temperature = dto.Temperature,
                                Humidity = dto.Humidity,
                                Atmosphere = dto.Atmosphere,
                                CreationTime = DateTime.Now,
                                #endregion
                            };
                            ////文件这里处理要先判断是否已经存在了
                            this._ZcqRecordRepository = scope.ServiceProvider.GetService<IZcqRecordRepository>();
                            var q = this._ZcqRecordRepository.Where(m => m.DeviceId == deviceId)
                                .Where(m => m.BatchNumber == dto.BatchNumber)
                                .Where(m => m.StartTime == dto.StartTime)
                                .Where(m => m.EndTime == dto.EndTime);
                            if (q.Any())
                            {
                                this.Logger.LogDebug($"文件[{zcqRecord.FileNo},{zcqRecord.BatchNumber},{zcqRecord.StartTime}]已经存在了，不做更新。");
                            }
                            else
                            {
                                //await this._ZcqRecordRepository.InsertAsync(zcqRecord, true);
                                if (_ReceivedZcqRecord == null)
                                    _ReceivedZcqRecord = new List<ZcqRecord>();
                                _ReceivedZcqRecord.Insert(0, zcqRecord);//最新查询到的要在最上面
                                this.Logger.LogDebug($"文件批次[{zcqRecord.FileNo},{zcqRecord.BatchNumber},{zcqRecord.StartTime}]不存在，暂存入本地缓存，待接收完全后再全部存入数据库。");
                            }
                            //通知数据执行完毕，这个在函数NextFileRead中有详细说明
                            if (zcqSevice.NextFileInfoSaveCompeleted(zcqRecord, scope))
                            {
                                if (this._ReceivedZcqRecord != null && this._ReceivedZcqRecord.Count > 0)
                                {
                                    try
                                    {
                                        await this._ZcqRecordRepository.InsertManyAsync(this._ReceivedZcqRecord, true);
                                    }
                                    catch (Exception ex)
                                    {
                                        this.Logger.LogError(ex, $"文件对象提交至数据库时出错");
                                        return;
                                    }
                                    this.Logger.LogDebug($"成功存储了{this._ReceivedZcqRecord.Count}个文件记录数据");
                                    _ReceivedZcqRecord.Clear();
                                }
                            }
                        }
                        else if (dtoData is ZCQMacAlermDto dto2)
                        {
                            if (!await this.CreateDeviceRunStatusEntity(zcqSevice, scope, dtoData.Cmd)) return;//false;
                            #region 设备故障赋值
                            this._ZcqRunStatusDto.IsDoorOpened = dto2.IsDoorOpened;
                            this._ZcqRunStatusDto.LCDStatus = dto2.LCDStatus;
                            this._ZcqRunStatusDto.IsPressureAOverrun = dto2.IsPressureAOverrun;
                            this._ZcqRunStatusDto.IsPressureBOverrun = dto2.IsPressureBOverrun;
                            this._ZcqRunStatusDto.IsIodineOverrun = dto2.IsIodineOverrun;
                            this._ZcqRunStatusDto.FS4003AError = dto2.FS4003AError;
                            this._ZcqRunStatusDto.FS4003BError = dto2.FS4003BError;
                            this._ZcqRunStatusDto.DpSensorError = dto2.DpSensorError;
                            this._ZcqRunStatusDto.AtmosphereModuleError = dto2.AtmosphereModuleError;
                            this._ZcqRunStatusDto.TemModuleError = dto2.TemModuleError;
                            this._ZcqRunStatusDto.AlarmUpdateTime = DateTime.Now;//报警时间更新
                            #endregion
                            //更新数据库
                            await this.ZcqRunStatusRepositoryUpdate(zcqSevice, dto2.Cmd, scope);
                        }
                        else if (dtoData is ZCQSettedInfoDto dto3)
                        {
                            if (!await this.CreateDeviceRunStatusEntity(zcqSevice, scope, dtoData.Cmd)) return;//false;
                            //设备设置信息
                            this._ZcqRunStatusDto.SettingFlow = dto3.SettingFlow;
                            this._ZcqRunStatusDto.SettingHour = dto3.SettingHour;
                            this._ZcqRunStatusDto.SettingMin = dto3.SettingMin;
                            this._ZcqRunStatusDto.SettingTotalFlow = dto3.SettingTotalFlow;
                            //更新数据库
                            await this.ZcqRunStatusRepositoryUpdate(zcqSevice, dto3.Cmd, scope);
                        }
                        else if (dtoData is ZCQWorkStatusDto dto4)
                        {
                            if (!await this.CreateDeviceRunStatusEntity(zcqSevice, scope, dtoData.Cmd)) return;//false;
                            //设备工作状态
                            this._ZcqRunStatusDto.WorkStatus = dto4.WorkStatus;
                            this._ZcqRunStatusDto.WorkStatusName = dto4.WorkStatusName;
                            //更新数据库
                            await this.ZcqRunStatusRepositoryUpdate(zcqSevice, dto4.Cmd, scope);
                        }
                        else if (dtoData is ZCQRealDataDto dto5)
                        {
                            if (!await this.CreateDeviceRunStatusEntity(zcqSevice, scope, dtoData.Cmd)) return;//false;
                                                                                                            //设备瞬时数据
                            this._ZcqRunStatusDto.StandardVolume = dto5.StandardVolume;
                            this._ZcqRunStatusDto.WorkingVolume = dto5.WorkingVolume;
                            //更新数据库
                            await this.ZcqRunStatusRepositoryUpdate(zcqSevice, dto5.Cmd, scope);
                        }
                    }
                }
                return;//true;
            }
        }
        /// <summary>
        /// 系统初始化时如果数据库不存在任何记录创建一条新的实时数据
        /// </summary>
        /// <param name="zcqSevice"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        private async Task<bool> CreateDeviceRunStatusEntity(CtrlHDZCQService zcqSevice, IServiceScope scope,HDZCQCmdCode cmdCode)
        {
            if (this._ZcqRunStatusDto != null) return true;
            this._ZcqRunStatusRepository = scope.ServiceProvider.GetService<IZcqRunStatusRepository>();
            if (this._ZcqRunStatusRepository == null)
            {
                this.Logger.LogError($"设备实时状态的Repository获取为空！数据无法保存。");
                return false;
            }
            ZcqRunStatus zcqRunStatus;
            try
            {
                var query = _ZcqRunStatusRepository
                    .Where(m => m.DeviceId == zcqSevice.Device.Id)
                    .WhereIf(zcqSevice.Device.TenantId != null, m => m.TenantId == zcqSevice.Device.TenantId)
                    .OrderByDescending(m => m.CreationTime);
                zcqRunStatus = query.FirstOrDefault();//TODO:如果为空的话Fisrt()会报错，用这个不会报错，如果没数据返回null，与我们想要的结果一致；
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"读取设备[{zcqSevice.Device.Id}]对应的实时状态ID值时出错");
                return false;
            }
            if (zcqRunStatus == null || (DateTime.Now - zcqRunStatus.CreationTime).TotalSeconds >= zcqSevice.RunStatusSaveHisInteralSeconds)
            {
                //此时没有或者数据库中的最新记录已经超过了我们规定的时间，则要新增
                zcqRunStatus = new ZcqRunStatus()
                {
                    TenantId = zcqSevice.Device.TenantId,//补上租户，否则存入为NULL
                    DeviceId = zcqSevice.Device.Id,
                    CreationTime = this.FormatCurrentDateTime(zcqSevice.RunStatusSaveHisInteralSeconds)
                };
                try
                {
                    //这里zcqRunStatus对象已经被INSERT过后就会被tracking，无法再被update了，所以后面直接要在拷贝一次（用GetNewZcqRunStatusEntity）
                    zcqRunStatus = await this._ZcqRunStatusRepository.InsertAsync(zcqRunStatus, true);
                    this.Logger.LogDebug($"已经新增实时状态数据，返回数据ID[{zcqRunStatus.Id}]。");
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, $"创建实时状态的空数据时出错");
                    return false;
                }
            }
            //这里要直接复制数据库中原有的数据
            this._ZcqRunStatusDto = this.ObjectMapper.Map<ZcqRunStatus,ZcqRunStatusDto>(zcqRunStatus); 
            return true;
        }
        /// <summary>
        /// 更新实时数据，并在一定时间间隔内将实时数据存储为历史数据
        /// </summary>
        /// <param name="zcqSevice"></param>
        /// <param name="cmd"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public async Task<bool> ZcqRunStatusRepositoryUpdate(CtrlHDZCQService zcqSevice, HDZCQCmdCode cmd, IServiceScope scope)
        {
            /*************
             * 更新实时数据逻辑：
             * 1、实时数据每隔1个小时存储一条，这个时间间隔暂时写死在这里，待李工设计完这些参数传入后再调整；
             * 2、间隔没到指定时间的，则更新数据数据库
             * ********************/
            if (this._ZcqRunStatusDto == null) return false;
            //设置更新时间
            this._ZcqRunStatusDto.RunUpdateTime = DateTime.Now;
            //将实时状态数据更新至数据库
            this._ZcqRunStatusRepository = scope.ServiceProvider.GetService<IZcqRunStatusRepository>();
            if (this._ZcqRunStatusRepository == null)
            {
                this.Logger.LogError($"设备实时状态的Repository获取为空！数据无法更新。");
                return false;
            }
            TimeSpan ts = DateTime.Now - this._ZcqRunStatusDto.CreationTime;
            ZcqRunStatus zcqRunStatusSave;
            long iSecd = (long)ts.TotalSeconds;
            if (iSecd >= zcqSevice.RunStatusSaveHisInteralSeconds)//TODO:前期测试短一点，后期直接从配置文件中读取
            {
                //此时要插入新的数据了
                this._ZcqRunStatusDto.Id = Guid.Empty;
                this._ZcqRunStatusDto.RunUpdateTime = DateTime.Now;//存储一下更新时间，可以知道具体是什么时间存入的
                this._ZcqRunStatusDto.CreationTime = this.FormatCurrentDateTime(zcqSevice.RunStatusSaveHisInteralSeconds);//将时间重新修饰一下，不要显示分钟和秒                                                                                               //zcqRunStatusSave = this.MapToGetRecordByDto<ZcqRunStatusDto, ZcqRunStatus>(this._ZcqRunStatusDto);//MapToGetOutputZcqRunStatus(this._ZcqRunStatusDto);
                try
                {
                    zcqRunStatusSave= this.ObjectMapper.Map<ZcqRunStatusDto, ZcqRunStatus>(this._ZcqRunStatusDto);
                    zcqRunStatusSave = await this._ZcqRunStatusRepository.InsertAsync(zcqRunStatusSave, true);
                    this.Logger.LogDebug($"实时状态已达{iSecd}秒，存入一行历史记录,新数据ID[{zcqRunStatusSave.Id}]");
                    this._ZcqRunStatusDto.Id = zcqRunStatusSave.Id;
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
                    zcqRunStatusSave = await this._ZcqRunStatusRepository.GetAsync(this._ZcqRunStatusDto.Id);
                    ObjectMapper.Map<ZcqRunStatusDto, ZcqRunStatus>(this._ZcqRunStatusDto, zcqRunStatusSave);
                    await this._ZcqRunStatusRepository.UpdateAsync(zcqRunStatusSave, true);
                    this.Logger.LogDebug($"气碘采样实时状态[{EnumHelper.GetEnumDescription<HDZCQCmdCode>(cmd)}]更新至数据库成功，距上次存储历史记录{iSecd}秒。");
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, $"气碘采样实时状态[{EnumHelper.GetEnumDescription<HDZCQCmdCode>(cmd)}]更新至数据库时出错");
                    return false;
                }
            }
            return true;
        }
        #region 功能函数
        /// <summary>
        /// 重新定义创建时间，根据传入的存储频率来确定精确到时分秒；
        /// </summary>
        /// <param name="iIntervalSecds">存储的频率</param>
        /// <returns>重新定义的时间</returns>
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
