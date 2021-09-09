using Aming.Core;
using Aming.DTU;
using Aming.DTU.Config;
using HengDa.LiZongMing.REAMS.Room;
using HengDa.LiZongMing.REAMS.Room.Dtos;
using HengDa.LiZongMing.REAMS.Wpf.Dto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace HengDa.LiZongMing.REAMS.CtrlServices
{
    /// <summary>
    /// 控制具体硬件的类，方法是一些组合步骤
    /// </summary>
    public class HDSSDataService : ISingletonDependency
    {
        private ILoggerFactory loggerFactory;
        private ILogger Logger;
        /// <summary>
        /// 将每次接受的数据存储在该实例中，这样设备反馈数据后只需更新当前反馈的内容即可，其他保持不变，这样提交至数据库时确保都能有值；
        /// </summary>
        private readonly IObjectMapper ObjectMapper;
        private RoomRunStatusDto _RoomRunStatusDto;
        private readonly IRoomRunStatusRepository _RoomRunStatusRepository;


        /// <summary>
        /// 事件回调时，收到数据触发解析
        /// </summary>
        public Action<MqttClientConfig, UniMessageBase, ZCQBaseDto> OnUniMessageReceivedCallBack { get; set; } = null;
        public HDSSDataService(ILoggerFactory loggerFactory,
              IObjectMapper ObjectMapper,
              RoomRunStatusDto roomRunStatusDto,
              IRoomRunStatusRepository roomRunStatusRepository)
        {
            this._RoomRunStatusRepository = roomRunStatusRepository;
            this._RoomRunStatusDto = roomRunStatusDto;
            this.ObjectMapper = ObjectMapper;
            this.loggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger<HDSSDataService>();
        }
        //public bool InitRepositoryService(IServiceProvider serviceProvider)
        //{
        //    if (serviceProvider == null) return false;
        //    if (this._ZcqRunStatusRepository == null)
        //    {
        //        this._ZcqRunStatusRepository = serviceProvider.GetService<IZcqRunStatusRepository>();
        //    }
        //    if (this._ZcqRecordRepository == null)
        //        this._ZcqRecordRepository = Aming.Tools.IocHelper.ServiceProvider.GetService<IZcqRecordRepository>();
        //    return true;
        //}
        /// <summary>
        /// 处理数据的保存
        /// </summary>
        /// <param name="mqttClientConfig"></param>
        /// <param name="msg"></param>
        /// <param name="dtoData"></param>
        public async Task DataSave(CtrlHDSSService service, UniMessageBase msg, IEnumerable<SSBaseDto> dtoData)
        {
            await Task.Run(async () =>
            {
                var _serviceProvider = Aming.Tools.IocHelper.ServiceProvider;
                if (service == null)
                {
                    Logger.LogError($"通知停止读取文件时，发现SSService为空！");
                    return;
                }
                var id = service.Device.Id;//读取设备唯一标识
                Guid? sTenantId = service.Device.TenantId;//读取租户信息
                Logger.LogWarning($"ss roonRunStatus id:{this._RoomRunStatusDto.Id.ToString()}");
                foreach (SSRealDataDto roomRunStatusItem in dtoData)
                {
                    this._RoomRunStatusDto = new RoomRunStatusDto();
                    this._RoomRunStatusDto.CreationTime = DateTime.Now;
                    this._RoomRunStatusDto.DeviceId = roomRunStatusItem.DeviceID;
                    this._RoomRunStatusDto.ExternalPower = false;
                    this._RoomRunStatusDto.TenantId = sTenantId;
                    this._RoomRunStatusDto.GeoLocation_Altitude = roomRunStatusItem.CoordinateType;
                    this._RoomRunStatusDto.GeoLocation_Lat = (decimal)roomRunStatusItem.Lat;
                    this._RoomRunStatusDto.GeoLocation_Lng = (decimal)roomRunStatusItem.Lng;
                    this._RoomRunStatusDto.Humidity = (decimal)roomRunStatusItem.Hum;
                    this._RoomRunStatusDto.NodeID = roomRunStatusItem.NodeID.ToString();
                    this._RoomRunStatusDto.Temperature = (decimal)roomRunStatusItem.Tem;
                    this._RoomRunStatusDto.Relay1 = false;

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var uowManager = scope.ServiceProvider.GetRequiredService<Volo.Abp.Uow.IUnitOfWorkManager>();

                        using (var uow = uowManager.Begin())
                        {
                            try
                            {
                                var dtoSave = this.ObjectMapper.Map<RoomRunStatusDto, RoomRunStatus>(this._RoomRunStatusDto);
                                dtoSave = await this._RoomRunStatusRepository.InsertAsync(dtoSave, true);
                            }
                            catch (Exception ex)
                            {
                                this.Logger.LogError(ex, $"实时状态插入历史记录时出错。");
                            }
                        }
                    }

                }

            });

            return;

        }



    }
}
