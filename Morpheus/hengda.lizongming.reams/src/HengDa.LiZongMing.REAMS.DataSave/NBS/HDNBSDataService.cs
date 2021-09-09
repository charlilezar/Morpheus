using Aming.Core;
using Aming.DTU;
using Aming.DTU.Config;
using HengDa.LiZongMing.REAMS.Wpf.Dto;
using HengDa.LiZongMing.REAMS.NBS;
using HengDa.LiZongMing.REAMS.NBS.Dtos;
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
    public class HDNBSDataService : ISingletonDependency
    {
        private IObjectMapper ObjectMapper;
        private ILoggerFactory loggerFactory;
        private ILogger Logger;
        /// <summary>
        /// 将每次接受的数据存储在该实例中，这样设备反馈数据后只需更新当前反馈的内容即可，其他保持不变，这样提交至数据库时确保都能有值；
        /// </summary>

        private NbsRunStatus _NbsRunStatus;
        NbsRunStatusDto _NbsRunStatusDto { get; set; }
        INbsRunStatusRepository _NbsRunStatusRepository = null;
        INbsRecordRepository _NbsRecordRepository = null;
        List<NbsRecord> _ReceivedNbsRecord = null;
        /// <summary>
        /// 事件回调时，收到数据触发解析
        /// </summary>
        public Action<MqttClientConfig, UniMessageBase, NBSBaseDto> OnUniMessageReceivedCallBack { get; set; } = null;
        public HDNBSDataService(ILoggerFactory loggerFactory,
            NbsRunStatus nbsRunStatus,
            IObjectMapper objectMapper)
        {
            this._NbsRunStatus = nbsRunStatus;
            this.ObjectMapper = objectMapper;
            this.loggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger<CtrlHDNBSService>();
        }
        /// <summary>
        /// 处理数据的保存
        /// </summary>
        /// <param name="mqttClientConfig"></param>
        /// <param name="msg"></param>
        /// <param name="dtoData"></param>
        public async void DataSave(CtrlHDNBSService nbsSevice, UniMessageBase msg, NBSBaseDto dtoData)
        {
            var _serviceProvider = Aming.Tools.IocHelper.ServiceProvider;
            if (!string.IsNullOrEmpty(dtoData.ErrMsg))
            {
                Logger.LogError($"{dtoData.ErrMsg}");
            }
            if (nbsSevice.Device == null)
            {
                return;
            }
            if (nbsSevice == null)
            {
                Logger.LogError($"CtrlHDNBSService对象获取失败！");
                return;//false;
            }
            long deviceId = nbsSevice.Device.Id;//读取设备唯一标识
            Guid? gTenantId = nbsSevice.Device.TenantId;//读取租户信息
            if (nbsSevice == null)
            {
                Logger.LogError($"CtrlHDNBSService对象获取失败！");
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
                        if (dtoData is NBSFileInfoDto dto)
                        {
                            if (!dto.Sucessfully)
                            {
                                this.Logger.LogWarning("传入的文件对象解析不成功，无法保存至数据库");
                                return;//false;
                            }
                            if ((!dto.FileExist))
                            {
                                //nbsSevice.StopFileRead();//此时终止查找文件，超过最大了
                                this.Logger.LogWarning("传入的文件对象解析成功，但文件不存在，无需提交至数据库");
                                return;//true;
                            }
                            if (!nbsSevice.IsFileSave2Db(dto)) return;
                            NbsRecord nbsRecord = new NbsRecord()
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
                            this._NbsRecordRepository = scope.ServiceProvider.GetService<INbsRecordRepository>();
                            var q = this._NbsRecordRepository.Where(m => m.DeviceId == deviceId)
                                .Where(m => m.BatchNumber == dto.BatchNumber)
                                .Where(m => m.StartTime == dto.StartTime)
                                .Where(m => m.EndTime == dto.EndTime);
                            if (q.Any())
                            {
                                this.Logger.LogDebug($"文件[{nbsRecord.FileNo},{nbsRecord.BatchNumber},{nbsRecord.StartTime}]已经存在了，不做更新。");
                            }
                            else
                            {
                                //await this._NbsRecordRepository.InsertAsync(nbsRecord, true);
                                if (_ReceivedNbsRecord == null)
                                    _ReceivedNbsRecord = new List<NbsRecord>();
                                _ReceivedNbsRecord.Insert(0, nbsRecord);//最新查询到的要在最上面
                                this.Logger.LogDebug($"文件批次[{nbsRecord.FileNo},{nbsRecord.BatchNumber},{nbsRecord.StartTime}]不存在，暂存入本地缓存，待接收完全后再全部存入数据库。");
                            }
                            //通知数据执行完毕，这个在函数中有详细说明
                            if (nbsSevice.NextFileInfoSaveCompeleted(nbsRecord, scope))
                            {
                                if (this._ReceivedNbsRecord != null && this._ReceivedNbsRecord.Count > 0)
                                {
                                    try
                                    {
                                        await this._NbsRecordRepository.InsertManyAsync(this._ReceivedNbsRecord, true);
                                    }
                                    catch (Exception ex)
                                    {
                                        this.Logger.LogError(ex, $"文件对象提交至数据库时出错。");
                                        return;
                                    }
                                    this.Logger.LogDebug($"成功存储了{this._ReceivedNbsRecord.Count}个文件记录数据");
                                    _ReceivedNbsRecord.Clear();
                                }
                            }
                        }
                        else if (dtoData is NBSMacAlermDto dto2)
                        {
                            //设备故障
                            if (!await this.CreateDeviceRunStatusEntity(nbsSevice, scope)) return;//false;
                            this._NbsRunStatusDto.AlarmCode = dto2.AlarmValue;
                            this._NbsRunStatusDto.AlarmDesc = dto2.AlarmDesc;
                            this._NbsRunStatusDto.AlarmUpdateTime = DateTime.Now;//报警时间更新
                            //更新数据库
                            await this.NbsRunStatusRepositoryUpdate(nbsSevice, dto2.Cmd, scope);
                        }
                        else if (dtoData is NBSSettedInfotDto dto3)
                        {
                            if (!await this.CreateDeviceRunStatusEntity(nbsSevice, scope)) return;//false;
                            //设备设置信息
                            this._NbsRunStatusDto.SettingFlow = dto3.SettingFlow;
                            this._NbsRunStatusDto.SettingHour = dto3.SettingHour;
                            this._NbsRunStatusDto.SettingMin = dto3.SettingMin;
                            this._NbsRunStatusDto.SettingTotalFlow = dto3.SettingTotalFlow;
                            //更新数据库
                            await this.NbsRunStatusRepositoryUpdate(nbsSevice, dto3.Cmd, scope);
                        }
                        else if (dtoData is NBSWorkStatusDto dto4)
                        {
                            if (!await this.CreateDeviceRunStatusEntity(nbsSevice, scope)) return;//false;
                            //设备工作状态
                            this._NbsRunStatusDto.WorkStatus = (ushort)dto4.WorkStatus;
                            this._NbsRunStatusDto.WorkStatusName = dto4.WorkStatusName;
                            //更新数据库
                            await this.NbsRunStatusRepositoryUpdate(nbsSevice, dto4.Cmd, scope);
                        }
                        else if (dtoData is NBSRealDataDto dto5)
                        {
                            //瞬时数据
                            if (!await this.CreateDeviceRunStatusEntity(nbsSevice, scope)) return;//false;                                                                      //设备瞬时数据
                            this._NbsRunStatusDto.StandardVolume = dto5.StandardVolume;
                            this._NbsRunStatusDto.WorkingVolume = dto5.WorkingVolume;
                            this._NbsRunStatusDto.BoxTemperature = dto5.BoxTemperature;
                            this._NbsRunStatusDto.Temperature = dto5.Temperature;
                            this._NbsRunStatusDto.InitTemperature = dto5.BoxTemperature;
                            //更新数据库
                            await this.NbsRunStatusRepositoryUpdate(nbsSevice, dto5.Cmd, scope);
                        }
                        else if (dtoData is NBSRemainWorkTimesDto dto6)
                        {
                            //剩余工作时间
                            if (!await this.CreateDeviceRunStatusEntity(nbsSevice, scope)) return;//false;
                            this._NbsRunStatusDto.RemainWorkTimes = dto6.TotalSeconds;
                            //更新数据库
                            await this.NbsRunStatusRepositoryUpdate(nbsSevice, dto6.Cmd, scope);
                        }
                        else if (dtoData is NBSRemainWorkVDto dto7)
                        {
                            //剩余体积
                            if (!await this.CreateDeviceRunStatusEntity(nbsSevice, scope)) return;//false;
                            this._NbsRunStatusDto.RemainV = dto7.RemainV;
                            //更新数据库
                            await this.NbsRunStatusRepositoryUpdate(nbsSevice, dto7.Cmd, scope);
                        }
                        else if (dtoData is NBSWorkModeSetDto dto8)
                        {
                            //运行模式
                            if (!await this.CreateDeviceRunStatusEntity(nbsSevice, scope)) return;//false;
                            if (dto8.WorkMode == 0x00)
                                this._NbsRunStatusDto.WorkMode = "半自动定量";
                            else if (dto8.WorkMode == 0x01)
                                this._NbsRunStatusDto.WorkMode = "半自动定时";
                            else
                                this._NbsRunStatusDto.WorkMode = "未知模式";
                            //更新数据库
                            await this.NbsRunStatusRepositoryUpdate(nbsSevice, dto8.Cmd, scope);
                        }
                    }
                }
                return;//true;
            }
        }
        /// <summary>
        /// 系统初始化时如果数据库不存在任何记录创建一条新的实时数据
        /// </summary>
        /// <param name="nbsSevice"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        private async Task<bool> CreateDeviceRunStatusEntity(CtrlHDNBSService nbsSevice, IServiceScope scope)
        {
            if (this._NbsRunStatusDto != null) return true;
            this._NbsRunStatusRepository = scope.ServiceProvider.GetService<INbsRunStatusRepository>();
            if (this._NbsRunStatusRepository == null)
            {
                this.Logger.LogError($"设备实时状态的Repository获取为空！数据无法保存。");
                return false;
            }
            NbsRunStatus nbsRunStatus;
            try
            {
                var query = _NbsRunStatusRepository
                    .Where(m => m.DeviceId == nbsSevice.Device.Id);
                nbsRunStatus = query.FirstOrDefault();//TODO:如果为空的话Fisrt()会报错，用这个不会报错，如果没数据返回null，与我们想要的结果一致；
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"读取设备[{nbsSevice.Device.Id}]对应的实时状态ID值时出错");
                return false;
            }
            if (nbsRunStatus == null)
            {
                //此时没有，则要新增
                nbsRunStatus = new NbsRunStatus()
                {
                    DeviceId = nbsSevice.Device.Id,
                    CreationTime = this.FormatCurrentDateTime(nbsSevice.RunStatusSaveHisInteralSeconds)
                };
                try
                {
                    //这里nbsRunStatus对象已经被INSERT过后就会被tracking，无法再被update了，所以后面直接要在拷贝一次（用GetNewNbsRunStatusEntity）
                    nbsRunStatus=await this._NbsRunStatusRepository.InsertAsync(nbsRunStatus, true);
                    this.Logger.LogDebug($"已经新增实时状态数据，返回数据ID[{nbsRunStatus.Id}]。");
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, $"创建实时状态的空数据时出错");
                    return false;
                }
            }
            //这里要直接复制数据库中原有的数据
            this._NbsRunStatusDto = this.ObjectMapper.Map<NbsRunStatus,NbsRunStatusDto>(nbsRunStatus); 
            return true;
        }
        /// <summary>
        /// 更新实时数据，并在一定时间间隔内将实时数据存储为历史数据
        /// </summary>
        /// <param name="nbsSevice"></param>
        /// <param name="cmd"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public async Task<bool> NbsRunStatusRepositoryUpdate(CtrlHDNBSService nbsSevice, HDNBSCmdCode cmd, IServiceScope scope)
        {
            /*************
             * 更新实时数据逻辑：
             * 1、实时数据每隔1个小时存储一条，这个时间间隔暂时写死在这里，待李工设计完这些参数传入后再调整；
             * 2、间隔没到指定时间的，则更新数据数据库
             * ********************/
            if (this._NbsRunStatusDto == null) return false;
            //设置更新时间
            this._NbsRunStatusDto.RunUpdateTime = DateTime.Now;
            //将实时状态数据更新至数据库
            this._NbsRunStatusRepository = scope.ServiceProvider.GetService<INbsRunStatusRepository>();
            if (this._NbsRunStatusRepository == null)
            {
                this.Logger.LogError($"设备实时状态的Repository获取为空！数据无法更新。");
                return false;
            }
            TimeSpan ts = DateTime.Now - this._NbsRunStatusDto.CreationTime;
            NbsRunStatus nbsRunStatusSave;
            long iSecd = (long)ts.TotalSeconds;
            if (iSecd >= nbsSevice.RunStatusSaveHisInteralSeconds)//TODO:前期测试短一点，后期直接从配置文件中读取
            {
                //此时要插入新的数据了
                this._NbsRunStatusDto.Id = Guid.Empty;
                this._NbsRunStatusDto.RunUpdateTime = DateTime.Now;//存储一下更新时间，可以知道具体是什么时间存入的
                this._NbsRunStatusDto.CreationTime = this.FormatCurrentDateTime(nbsSevice.RunStatusSaveHisInteralSeconds);//将时间重新修饰一下，不要显示分钟和秒                                                                                               //nbsRunStatusSave = this.MapToGetRecordByDto<NbsRunStatusDto, NbsRunStatus>(this._NbsRunStatusDto);//MapToGetOutputNbsRunStatus(this._NbsRunStatusDto);
                try
                {
                    nbsRunStatusSave= this.ObjectMapper.Map<NbsRunStatusDto, NbsRunStatus>(this._NbsRunStatusDto);
                    nbsRunStatusSave = await this._NbsRunStatusRepository.InsertAsync(nbsRunStatusSave, true);
                    this.Logger.LogDebug($"实时状态已达{iSecd}秒，存入一行历史记录,新数据ID[{nbsRunStatusSave.Id}]");
                    this._NbsRunStatusDto.Id = nbsRunStatusSave.Id;
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
                    nbsRunStatusSave = await this._NbsRunStatusRepository.GetAsync(this._NbsRunStatusDto.Id);
                    ObjectMapper.Map<NbsRunStatusDto, NbsRunStatus>(this._NbsRunStatusDto, nbsRunStatusSave);
                    await this._NbsRunStatusRepository.UpdateAsync(nbsRunStatusSave, true);
                    this.Logger.LogDebug($"气碘采样实时状态[{EnumHelper.GetEnumDescription<HDNBSCmdCode>(cmd)}]更新至数据库成功，距上次存储历史记录{iSecd}秒。");
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, $"气碘采样实时状态[{EnumHelper.GetEnumDescription<HDNBSCmdCode>(cmd)}]更新至数据库时出错");
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
