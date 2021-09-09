using Aming.Core;
using Aming.DTU;
using Aming.DTU.Config;
using HengDa.LiZongMing.REAMS.NaI;
using HengDa.LiZongMing.REAMS.NaI.Dtos;
using HengDa.LiZongMing.REAMS.Wpf.Dto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace HengDa.LiZongMing.REAMS.CtrlServices
{
    /// <summary>
    /// 控制具体硬件的类，方法是一些组合步骤
    /// </summary>
    public class HDNaISDataService : ISingletonDependency
    {
        private ILoggerFactory loggerFactory;
        private ILogger Logger;
        /// <summary>
        /// 将每次接受的数据存储在该实例中，这样设备反馈数据后只需更新当前反馈的内容即可，其他保持不变，这样提交至数据库时确保都能有值；
        /// </summary>
        private readonly IObjectMapper ObjectMapper;
        private readonly INaiRecordRepository _naIRecordRepository;
        private readonly NaiRecordDto _naiRecordDto;

        /// <summary>
        /// 事件回调时，收到数据触发解析
        /// </summary>
        public Action<MqttClientConfig, UniMessageBase, ZCQBaseDto> OnUniMessageReceivedCallBack { get; set; } = null;
        public HDNaISDataService(ILoggerFactory loggerFactory,
            IObjectMapper ObjectMapper,
            NaiRecordDto  naiRecordDto,
            INaiRecordRepository  naiRecords)
        {
            this._naIRecordRepository = naiRecords;
            this._naiRecordDto = naiRecordDto;
            this.ObjectMapper = ObjectMapper;
            this.loggerFactory = loggerFactory;
            this.Logger = loggerFactory.CreateLogger<HDNaISDataService>();
        }

        /// <summary>
        /// 处理数据的保存
        /// </summary>
        /// <param name="mqttClientConfig"></param>
        /// <param name="msg"></param>
        /// <param name="dtoData"></param>
        public async Task DataSave(CtrlHDNaISService service, UniMessageBase msg, BaseDto dtoData)
        {
       
            if (service == null)
            {
                Logger.LogError($"通知停止读取文件时，发现NaIService为空！");
                return;
            }
            //if (!string.IsNullOrEmpty(dtoData.ErrMsg))
            //{
            //    Logger.LogError($"{dtoData.ErrMsg}");
            //}
            if (service.Device ==null)
            {
                return;
            }
            var id = service.Device.Id;//读取设备唯一标识
            Guid? sTenantId = service.Device.TenantId;//读取租户信息
            Logger.LogWarning($"NaIS record id:{this._naiRecordDto.Id.ToString()}");
            this._naiRecordDto.TenantId = sTenantId;
            this._naiRecordDto.CreationTime = DateTime.Now;
            this._naiRecordDto.DoseRate = (dtoData as NaIRealDataDto).DoseRate ;
            this._naiRecordDto.xml = (dtoData as NaIRealDataDto).DoseRateXML;

            //保存到数据库
            var _serviceProvider = Aming.Tools.IocHelper.ServiceProvider;
            using (var scope = _serviceProvider.CreateScope())
            {
                var uowManager = scope.ServiceProvider.GetRequiredService<Volo.Abp.Uow.IUnitOfWorkManager>();

                using (var uow = uowManager.Begin())
                {
                    try
                    {
                        var naIRecordSave = this.ObjectMapper.Map<NaiRecordDto, NaiRecord> (this._naiRecordDto);
                        naIRecordSave = await this._naIRecordRepository.InsertAsync(naIRecordSave, true);
                    }
                    catch (Exception ex)
                    {
                        this.Logger.LogError(ex, $"实时状态插入历史记录时出错。");
                    }
                }
            }

        }
  

    }
}
