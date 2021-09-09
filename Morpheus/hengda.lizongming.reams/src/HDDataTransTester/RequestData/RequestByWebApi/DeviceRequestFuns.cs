using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataPlatformTrans.DataEntitys.MessageEntity;
using DataPlatformTrans.DataFactory;
using HDDataTransTester.RequestData.RequestDataEntitys;
using Newtonsoft.Json;

namespace HDDataTransTester.RequestData.RequestByWebApi.DeviceRequestFuns
{
    /// <summary>
    ///  各设备读取数据
    /// </summary>
    public class ZCQRequestFuns
    {
        public static async Task<RtnMessage<RecordRealZCQ>> GetRealAsync(RequestDataFilter filter)
        {
            string strUrl = HttpUitls.APIURL_ZCQREAL;
            RtnMessage<string> responseMsg = null;
            try
            {
                responseMsg = await HttpUitls.GetAsync(filter, strUrl);
            }
            catch (Exception ex)
            {
                return new RtnMessage<RecordRealZCQ>($"请求ZCQ实时数据出错：{ex.Message}({ex.Source})");
            }
            if (responseMsg == null)
            {
                return new RtnMessage<RecordRealZCQ>($"请求ZCQ实时返回空对象！");
            }
            if (!responseMsg.Sucessful)
            {
                return new RtnMessage<RecordRealZCQ>($"请求ZCQ实时数据失败：{responseMsg.Msg}");
            }
            if (String.IsNullOrWhiteSpace(responseMsg.Result))
            {
                return new RtnMessage<RecordRealZCQ>($"请求ZCQ实时数据失败：Response内容为空！请检查接口是否正确！");
            }
            //此时数据获取成功，则要开始解析了，由于我们请求接口时固定反馈是json格式。这里解析json代码即可；
            ReponseEntity<ZCQRunStatusResponse> zcq = null;
            try
            {
                zcq = JsonConvert.DeserializeObject<ReponseEntity<ZCQRunStatusResponse>>(responseMsg.Result);
            }
            catch (Exception ex)
            {
                return new RtnMessage<RecordRealZCQ>($"解析ZCQ实时数据出错：{ex.Message}({ex.Source})，获取的数据为：{responseMsg.Result}");
            }
            if (zcq == null)
            {
                //这种情况一般不会发生
                return new RtnMessage<RecordRealZCQ>($"解析ZCQ实时数据为空，获取的数据为：{responseMsg.Result}");
            }

            if (zcq.items == null || zcq.items.Count == 0)
            {
                //数据有可能为空，这是正常的，数据还未采集进来，或者设备关闭了
                return new RtnMessage<RecordRealZCQ>($"解析ZCQ实时数据为空，数据总数为:{zcq.totalCount}，获取的数据为：{responseMsg.Result}");
            }
            RecordRealZCQ rtnZcq = new RecordRealZCQ()
            {
                WorkID = "",//暂时缺少这个字段
                BeginTime = zcq.items[0].TaskStartTime,//该字段目前值为空的
                EndTime = null,//缺少该字段读取
                DataTime = zcq.items[0].CreationTime,
                SpCol_0102069904 = zcq.items[0].SettingFlow,//瞬时采样流量没有采集，只能用设定值了
                SpCol_0102069905 = zcq.items[0].WorkingVolume//协议没说明是标况体积还是工况体积，因为是否停止采样是以工况体积来定的，例如设置定量值为5m³时，当工况体积达到5m³时就会停止采样
            };
            return new RtnMessage<RecordRealZCQ>(rtnZcq);
        }
        #region 相关类
        /// <summary>
        /// 气碘实时数据的接收
        /// </summary>
        public class ZCQRunStatusResponse
        {
            #region 基本属性
            ///<summary>
            /// 设备Id
            ///</summary>
            [Display(Name = "设备Id", Order = 1)]
            public long DeviceId { get; set; }


            ///<summary>
            /// 工作模式
            ///</summary>
            [MaxLength(256)]
            [Display(Name = "工作模式", Order = 2)]
            public string WorkMode { get; set; }


            ///<summary>
            /// 设备状态
            ///</summary>
            [Display(Name = "设备状态", Order = 3)]
            public ushort WorkStatus { get; set; }


            ///<summary>
            /// 设备状态描述
            ///</summary>
            [MaxLength(256)]
            [Display(Name = "设备状态描述", Order = 4)]
            public string WorkStatusName { get; set; }


            ///<summary>
            /// 标况体积
            ///</summary>
            [Display(Name = "标况体积", Order = 5)]
            public decimal StandardVolume { get; set; }


            ///<summary>
            /// 工况体积
            ///</summary>
            [Display(Name = "工况体积", Order = 6)]
            public decimal WorkingVolume { get; set; }


            ///<summary>
            /// 舱门是否打开
            ///</summary>
            [Display(Name = "舱门是否打开", Order = 7)]
            public bool IsDoorOpened { get; set; }


            ///<summary>
            /// LCD连接状态
            ///</summary>
            [Display(Name = "LCD连接状态", Order = 8)]
            public bool LCDStatus { get; set; }


            ///<summary>
            /// 大气A是否超限
            ///</summary>
            [Display(Name = "大气A是否超限", Order = 9)]
            public bool IsPressureAOverrun { get; set; }


            ///<summary>
            /// 大气B是否超限
            ///</summary>
            [Display(Name = "大气B是否超限", Order = 10)]
            public bool IsPressureBOverrun { get; set; }


            ///<summary>
            /// 气碘是否超限
            ///</summary>
            [Display(Name = "气碘是否超限", Order = 11)]
            public bool IsIodineOverrun { get; set; }


            ///<summary>
            /// A电子流量计FS4003故障
            ///</summary>
            [Display(Name = "A电子流量计FS4003故障", Order = 12)]
            public bool FS4003AError { get; set; }


            ///<summary>
            /// B电子流量计FS4003故障
            ///</summary>
            [Display(Name = "B电子流量计FS4003故障", Order = 13)]
            public bool FS4003BError { get; set; }


            ///<summary>
            /// 差压传感器故障
            ///</summary>
            [Display(Name = "差压传感器故障", Order = 14)]
            public bool DpSensorError { get; set; }


            ///<summary>
            /// 大气压模块故障
            ///</summary>
            [Display(Name = "大气压模块故障", Order = 15)]
            public bool AtmosphereModuleError { get; set; }


            ///<summary>
            /// 温度模块故障
            ///</summary>
            [Display(Name = "温度模块故障", Order = 16)]
            public bool TemModuleError { get; set; }


            ///<summary>
            /// 瞬时采样流量设定值
            ///</summary>
            [Display(Name = "瞬时采样流量设定值", Order = 17)]
            public int SettingFlow { get; set; }


            ///<summary>
            /// 已设定的采样时间的小时部分值
            ///</summary>
            [Display(Name = "已设定的采样时间的小时部分值", Order = 18)]
            public int SettingHour { get; set; }


            ///<summary>
            /// 已设定的采样时间的分钟部分值
            ///</summary>
            [Display(Name = "已设定的采样时间的分钟部分值", Order = 19)]
            public short SettingMin { get; set; }


            ///<summary>
            /// 已设定的采样体积值
            ///</summary>
            [Display(Name = "已设定的采样体积值", Order = 20)]
            public decimal SettingTotalFlow { get; set; }


            ///<summary>
            /// 任务启动时间
            ///</summary>
            [Display(Name = "任务启动时间", Order = 21)]
            public DateTime? TaskStartTime { get; set; }


            ///<summary>
            /// 创建时间
            ///</summary>
            [Display(Name = "创建时间", Order = 22)]
            public DateTime CreationTime { get; set; }


            ///<summary>
            /// 状态更新时间
            ///</summary>
            [Display(Name = "状态更新时间", Order = 23)]
            public DateTime? RunUpdateTime { get; set; }


            ///<summary>
            /// 警报更新时间
            ///</summary>
            [Display(Name = "警报更新时间", Order = 24)]
            public DateTime? AlarmUpdateTime { get; set; }


            ///<summary>
            /// 多租户
            ///</summary>
            [Display(Name = "多租户", Order = 25)]
            public Guid? TenantId { get; set; }


            #endregion
        }
        #endregion
    }
    public class HveRequestFuns
    {
        public static async Task<RtnMessage<RecordRealHve>> GetRealAsync(RequestDataFilter filter)
        {
            string strUrl = HttpUitls.APIURL_HveREAL;
            RtnMessage<string> responseMsg = null;
            try
            {
                responseMsg = await HttpUitls.GetAsync(filter, strUrl);
            }
            catch (Exception ex)
            {
                return new RtnMessage<RecordRealHve>($"请求Hve实时数据出错：{ex.Message}({ex.Source})");
            }
            if (responseMsg == null)
            {
                return new RtnMessage<RecordRealHve>($"请求Hve实时返回空对象！");
            }
            if (!responseMsg.Sucessful)
            {
                return new RtnMessage<RecordRealHve>($"请求Hve实时数据失败：{responseMsg.Msg}");
            }
            if (String.IsNullOrWhiteSpace(responseMsg.Result))
            {
                return new RtnMessage<RecordRealHve>($"请求Hve实时数据失败：Response内容为空！请检查接口是否正确！");
            }
            //此时数据获取成功，则要开始解析了，由于我们请求接口时固定反馈是json格式。这里解析json代码即可；
            ReponseEntity<HveRecordResponse> hve = null;
            try
            {
                hve = JsonConvert.DeserializeObject<ReponseEntity<HveRecordResponse>>(responseMsg.Result);
            }
            catch (Exception ex)
            {
                return new RtnMessage<RecordRealHve>($"解析Hve实时数据出错：{ex.Message}({ex.Source})，获取的数据为：{responseMsg.Result}");
            }
            if (hve == null)
            {
                //这种情况一般不会发生
                return new RtnMessage<RecordRealHve>($"解析Hve实时数据为空，获取的数据为：{responseMsg.Result}");
            }

            if (hve.items == null || hve.items.Count == 0)
            {
                //数据有可能为空，这是正常的，数据还未采集进来，或者设备关闭了
                return new RtnMessage<RecordRealHve>($"解析Hve实时数据为空，数据总数为:{hve.totalCount}，获取的数据为：{responseMsg.Result}");
            }
            RecordRealHve rtnHve = new RecordRealHve()
            {
                SpCol_0102060301 = hve.items[0].DoseRate,
                SpCol_0102069901 = hve.items[0].BatteryVoltage,
                SpCol_0102069902 = hve.items[0].HighVoltage,
                SpCol_0102069903 = 0M//TODO:目前没有该值的存储
            };
            return new RtnMessage<RecordRealHve>(rtnHve);
        }
        #region 相关类
        /// <summary>
        /// 气碘实时数据的接收
        /// </summary>
        public class HveRecordResponse
        {
            ///<summary>
            /// 创建时间
            ///</summary>
            [Display(Name = "创建时间", Order = 2)]
            public DateTime CreationTime { get; set; }


            ///<summary>
            /// 设备Id
            ///</summary>
            [Display(Name = "设备Id", Order = 3)]
            public long DeviceId { get; set; }


            ///<summary>
            /// 伽马剂量率
            ///</summary>
            [Display(Name = "伽马剂量率", Order = 4)]
            public decimal DoseRate { get; set; }


            ///<summary>
            /// 高电压
            ///</summary>
            [Display(Name = "高电压", Order = 5)]
            public decimal HighVoltage { get; set; }


            ///<summary>
            /// 静电计温度
            ///</summary>
            [Display(Name = "静电计温度", Order = 6)]
            public decimal ElectrometerTemperature { get; set; }


            ///<summary>
            /// 电池电压
            ///</summary>
            [Display(Name = "电池电压", Order = 7)]
            public decimal BatteryVoltage { get; set; }


            ///<summary>
            /// 多租户
            ///</summary>
            [Display(Name = "多租户", Order = 8)]
            public Guid? TenantId { get; set; }
        }
        #endregion
    }
    public class NBSRequestFuns
    {
        public static async Task<RtnMessage<RecordRealNBS>> GetRealAsync(RequestDataFilter filter)
        {
            string strUrl = HttpUitls.APIURL_NBSREAL;
            RtnMessage<string> responseMsg = null;
            try
            {
                responseMsg = await HttpUitls.GetAsync(filter, strUrl);
            }
            catch (Exception ex)
            {
                return new RtnMessage<RecordRealNBS>($"请求NBS实时数据出错：{ex.Message}({ex.Source})");
            }
            if (responseMsg == null)
            {
                return new RtnMessage<RecordRealNBS>($"请求NBS实时返回空对象！");
            }
            if (!responseMsg.Sucessful)
            {
                return new RtnMessage<RecordRealNBS>($"请求NBS实时数据失败：{responseMsg.Msg}");
            }
            if (String.IsNullOrWhiteSpace(responseMsg.Result))
            {
                return new RtnMessage<RecordRealNBS>($"请求NBS实时数据失败：Response内容为空！请检查接口是否正确！");
            }
            //此时数据获取成功，则要开始解析了，由于我们请求接口时固定反馈是json格式。这里解析json代码即可；
            ReponseEntity<NBSRecordResponse> NBS = null;
            try
            {
                NBS = JsonConvert.DeserializeObject<ReponseEntity<NBSRecordResponse>>(responseMsg.Result);
            }
            catch (Exception ex)
            {
                return new RtnMessage<RecordRealNBS>($"解析NBS实时数据出错：{ex.Message}({ex.Source})，获取的数据为：{responseMsg.Result}");
            }
            if (NBS == null)
            {
                //这种情况一般不会发生
                return new RtnMessage<RecordRealNBS>($"解析NBS实时数据为空，获取的数据为：{responseMsg.Result}");
            }

            if (NBS.items == null || NBS.items.Count == 0)
            {
                //数据有可能为空，这是正常的，数据还未采集进来，或者设备关闭了
                return new RtnMessage<RecordRealNBS>($"解析NBS实时数据为空，数据总数为:{NBS.totalCount}，获取的数据为：{responseMsg.Result}");
            }
            RecordRealNBS rtnNBS = new RecordRealNBS()
            {
                WorkID = "",//暂时缺少这个字段
                BeginTime = NBS.items[0].TaskStartTime,//TODO:该字段目前值为空的
                EndTime = null,////TODO:缺少该字段读取
                DataTime = NBS.items[0].CreationTime,
                SpCol_0102069904 = NBS.items[0].SettingFlow,////TODO:瞬时采样流量没有采集，只能用设定值了
                SpCol_0102069905 = NBS.items[0].WorkingVolume//协议没说明是标况体积还是工况体积，因为是否停止采样是以工况体积来定的，例如设置定量值为5m³时，当工况体积达到5m³时就会停止采样
            };
            return new RtnMessage<RecordRealNBS>(rtnNBS);
        }
        #region 相关类
        public class NBSRecordResponse
        {
            ///<summary>
            /// 设备Id
            ///</summary>
            [Display(Name = "设备Id", Order = 1)]
            public virtual long DeviceId { get; set; }


            ///<summary>
            /// 工作模式
            ///</summary>
            [MaxLength(256)]
            [Display(Name = "工作模式", Order = 2)]
            public virtual string WorkMode { get; set; }


            ///<summary>
            /// 设备状态
            ///</summary>
            [Display(Name = "设备状态", Order = 3)]
            public virtual ushort WorkStatus { get; set; }


            ///<summary>
            /// 设备状态描述
            ///</summary>
            [MaxLength(256)]
            [Display(Name = "设备状态描述", Order = 4)]
            public virtual string WorkStatusName { get; set; }


            ///<summary>
            /// 标况体积
            ///</summary>
            [Display(Name = "标况体积", Order = 5)]
            public virtual decimal StandardVolume { get; set; }


            ///<summary>
            /// 工况体积
            ///</summary>
            [Display(Name = "工况体积", Order = 6)]
            public virtual decimal WorkingVolume { get; set; }


            ///<summary>
            /// 舱门是否打开
            ///</summary>
            [Display(Name = "舱门是否打开", Order = 7)]
            public virtual bool IsDoorOpened { get; set; }


            ///<summary>
            /// LCD连接状态
            ///</summary>
            [Display(Name = "LCD连接状态", Order = 8)]
            public virtual bool LCDStatus { get; set; }


            ///<summary>
            /// 大气A是否超限
            ///</summary>
            [Display(Name = "大气A是否超限", Order = 9)]
            public virtual bool IsPressureAOverrun { get; set; }


            ///<summary>
            /// 大气B是否超限
            ///</summary>
            [Display(Name = "大气B是否超限", Order = 10)]
            public virtual bool IsPressureBOverrun { get; set; }


            ///<summary>
            /// 气碘是否超限
            ///</summary>
            [Display(Name = "气碘是否超限", Order = 11)]
            public virtual bool IsIodineOverrun { get; set; }


            ///<summary>
            /// A电子流量计FS4003故障
            ///</summary>
            [Display(Name = "A电子流量计FS4003故障", Order = 12)]
            public virtual bool FS4003AError { get; set; }


            ///<summary>
            /// B电子流量计FS4003故障
            ///</summary>
            [Display(Name = "B电子流量计FS4003故障", Order = 13)]
            public virtual bool FS4003BError { get; set; }


            ///<summary>
            /// 差压传感器故障
            ///</summary>
            [Display(Name = "差压传感器故障", Order = 14)]
            public virtual bool DpSensorError { get; set; }


            ///<summary>
            /// 大气压模块故障
            ///</summary>
            [Display(Name = "大气压模块故障", Order = 15)]
            public virtual bool AtmosphereModuleError { get; set; }


            ///<summary>
            /// 温度模块故障
            ///</summary>
            [Display(Name = "温度模块故障", Order = 16)]
            public virtual bool TemModuleError { get; set; }


            ///<summary>
            /// 瞬时采样流量设定值
            ///</summary>
            [Display(Name = "瞬时采样流量设定值", Order = 17)]
            public virtual int SettingFlow { get; set; }


            ///<summary>
            /// 已设定的采样时间的小时部分值
            ///</summary>
            [Display(Name = "已设定的采样时间的小时部分值", Order = 18)]
            public virtual int SettingHour { get; set; }


            ///<summary>
            /// 已设定的采样时间的分钟部分值
            ///</summary>
            [Display(Name = "已设定的采样时间的分钟部分值", Order = 19)]
            public virtual short SettingMin { get; set; }


            ///<summary>
            /// 已设定的采样体积值
            ///</summary>
            [Display(Name = "已设定的采样体积值", Order = 20)]
            public virtual decimal SettingTotalFlow { get; set; }


            ///<summary>
            /// 任务启动时间
            ///</summary>
            [Display(Name = "任务启动时间", Order = 21)]
            public virtual DateTime? TaskStartTime { get; set; }


            ///<summary>
            /// 箱体温度
            ///</summary>
            [Display(Name = "箱体温度", Order = 22)]
            public virtual decimal BoxTemperature { get; set; }


            ///<summary>
            /// 环境温度
            ///</summary>
            [Display(Name = "环境温度", Order = 23)]
            public virtual decimal Temperature { get; set; }


            ///<summary>
            /// 计前温度
            ///</summary>
            [Display(Name = "计前温度", Order = 24)]
            public virtual decimal InitTemperature { get; set; }


            ///<summary>
            /// 剩余时间
            ///</summary>
            [Display(Name = "剩余时间", Order = 25)]
            public virtual int RemainWorkTimes { get; set; }


            ///<summary>
            /// 剩余体积
            ///</summary>
            [Display(Name = "剩余体积", Order = 26)]
            public virtual decimal RemainV { get; set; }


            ///<summary>
            /// 故障代码
            ///</summary>
            [Display(Name = "故障代码", Order = 27)]
            public virtual short AlarmCode { get; set; }


            ///<summary>
            /// 故障名称
            ///</summary>
            [MaxLength(256)]
            [Display(Name = "故障名称", Order = 28)]
            public virtual string AlarmDesc { get; set; }


            ///<summary>
            /// 创建时间
            ///</summary>
            [Display(Name = "创建时间", Order = 29)]
            public virtual DateTime CreationTime { get; set; }


            ///<summary>
            /// 状态更新时间
            ///</summary>
            [Display(Name = "状态更新时间", Order = 30)]
            public virtual DateTime? RunUpdateTime { get; set; }


            ///<summary>
            /// 警报更新时间
            ///</summary>
            [Display(Name = "警报更新时间", Order = 31)]
            public virtual DateTime? AlarmUpdateTime { get; set; }


            ///<summary>
            /// 多租户
            ///</summary>
            [Display(Name = "多租户", Order = 32)]
            public virtual Guid? TenantId { get; set; }
        }
        #endregion
    }
    public class AtmosphereRequestFuns
    {
        public static async Task<RtnMessage<RecordRealAtmosphere>> GetRealAsync(RequestDataFilter filter)
        {
            string strUrl = HttpUitls.APIURL_AtmosphereREAL;
            RtnMessage<string> responseMsg = null;
            try
            {
                responseMsg = await HttpUitls.GetAsync(filter, strUrl);
            }
            catch (Exception ex)
            {
                return new RtnMessage<RecordRealAtmosphere>($"请求Atmosphere实时数据出错：{ex.Message}({ex.Source})");
            }
            if (responseMsg == null)
            {
                return new RtnMessage<RecordRealAtmosphere>($"请求Atmosphere实时返回空对象！");
            }
            if (!responseMsg.Sucessful)
            {
                return new RtnMessage<RecordRealAtmosphere>($"请求Atmosphere实时数据失败：{responseMsg.Msg}");
            }
            if (String.IsNullOrWhiteSpace(responseMsg.Result))
            {
                return new RtnMessage<RecordRealAtmosphere>($"请求Atmosphere实时数据失败：Response内容为空！请检查接口是否正确！");
            }
            //此时数据获取成功，则要开始解析了，由于我们请求接口时固定反馈是json格式。这里解析json代码即可；
            ReponseEntity<AtmosphereRecordResponse> Atmosphere = null;
            try
            {
                Atmosphere = JsonConvert.DeserializeObject<ReponseEntity<AtmosphereRecordResponse>>(responseMsg.Result);
            }
            catch (Exception ex)
            {
                return new RtnMessage<RecordRealAtmosphere>($"解析Atmosphere实时数据出错：{ex.Message}({ex.Source})，获取的数据为：{responseMsg.Result}");
            }
            if (Atmosphere == null)
            {
                //这种情况一般不会发生
                return new RtnMessage<RecordRealAtmosphere>($"解析Atmosphere实时数据为空，获取的数据为：{responseMsg.Result}");
            }
            if (Atmosphere.items == null || Atmosphere.items.Count == 0)
            {
                //数据有可能为空，这是正常的，数据还未采集进来，或者设备关闭了
                return new RtnMessage<RecordRealAtmosphere>($"解析Atmosphere实时数据为空，数据总数为:{Atmosphere.totalCount}，获取的数据为：{responseMsg.Result}");
            }
            //注意该对象中的感雨器另外从动环实时数据中去读取
            RecordRealAtmosphere rtnAtmosphere = new RecordRealAtmosphere()
            {
                SpCol_0901040103 = Atmosphere.items[0].Atmosphere,
                SpCol_0901040102 = Atmosphere.items[0].Humidity,
                SpCol_0901040104 = 0M,//TODO:降雨强度目前没有
                SpCol_0901040101 = Atmosphere.items[0].Temperature,
                SpCol_0901040105 = Atmosphere.items[0].WindDirection,
                SpCol_0901040106 = Atmosphere.items[0].WindSpeed,
            };
            return new RtnMessage<RecordRealAtmosphere>(rtnAtmosphere);
        }
        #region 相关类
        public class AtmosphereRecordResponse
        {
            #region 基本属性

            // ///<summary>
            // /// 编号
            // ///</summary>
            // [Key]
            // [Column(Order = 1)]
            // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            // [Required()]
            // [Display(Name = "编号", Order = 1)]
            // [UIHint("NotEdit")]
            // public override Guid Id { get; set; }

            ///<summary>
            /// 创建时间
            ///</summary>
            [Display(Name = "创建时间", Order = 2)]
            public virtual DateTime CreationTime { get; set; }


            ///<summary>
            /// 设备Id
            ///</summary>
            [Display(Name = "设备Id", Order = 3)]
            public virtual long DeviceId { get; set; }


            ///<summary>
            /// 地理位置-经度
            ///</summary>
            [Display(Name = "地理位置-经度", Order = 4)]
            public virtual decimal GeoLocation_Lng { get; set; }


            ///<summary>
            /// 地理位置-伟度
            ///</summary>
            [Display(Name = "地理位置-伟度", Order = 5)]
            public virtual decimal GeoLocation_Lat { get; set; }


            ///<summary>
            /// 大气温度(℃)
            ///</summary>
            [Display(Name = "大气温度(℃)", Order = 6)]
            public virtual decimal Temperature { get; set; }


            ///<summary>
            /// 大气湿度(%RH)
            ///</summary>
            [Display(Name = "大气湿度(%RH)", Order = 7)]
            public virtual decimal Humidity { get; set; }


            ///<summary>
            /// 大气气压(hpa)
            ///</summary>
            [Display(Name = "大气气压(hpa)", Order = 8)]
            public virtual decimal Atmosphere { get; set; }


            ///<summary>
            /// 风速(m/s)
            ///</summary>
            [Display(Name = "风速(m/s)", Order = 9)]
            public virtual decimal WindSpeed { get; set; }


            ///<summary>
            /// 风向(°)
            ///</summary>
            [Display(Name = "风向(°)", Order = 10)]
            public virtual decimal WindDirection { get; set; }


            ///<summary>
            /// 噪声(dB)
            ///</summary>
            [Display(Name = "噪声(dB)", Order = 11)]
            public virtual decimal Noise { get; set; }


            ///<summary>
            /// CO (μg/m3)
            ///</summary>
            [Display(Name = "CO (μg/m3)", Order = 12)]
            public virtual decimal CO { get; set; }


            ///<summary>
            /// SO2(μg/m3)
            ///</summary>
            [Display(Name = "SO2(μg/m3)", Order = 13)]
            public virtual decimal SO2 { get; set; }


            ///<summary>
            /// O3(μg/m3)
            ///</summary>
            [Display(Name = "O3(μg/m3)", Order = 14)]
            public virtual decimal O3 { get; set; }


            ///<summary>
            /// O3_滚动8小平均(μg/m3)
            ///</summary>
            [Display(Name = "O3_滚动8小平均(μg/m3)", Order = 15)]
            public virtual decimal? O3_Avg8H { get; set; }


            ///<summary>
            /// NO2(μg/m3)
            ///</summary>
            [Display(Name = "NO2(μg/m3)", Order = 16)]
            public virtual decimal NO2 { get; set; }


            ///<summary>
            /// TVOC(μg/m3)
            ///</summary>
            [Display(Name = "TVOC(μg/m3)", Order = 17)]
            public virtual decimal VOC { get; set; }


            ///<summary>
            /// PM10(μg/m3)
            ///</summary>
            [Display(Name = "PM10(μg/m3)", Order = 18)]
            public virtual decimal PM10 { get; set; }


            ///<summary>
            /// PM2.5(μg/m3)
            ///</summary>
            [Display(Name = "PM2.5(μg/m3)", Order = 19)]
            public virtual decimal PM2_5 { get; set; }


            ///<summary>
            /// NH3(μg/m3)
            ///</summary>
            [Display(Name = "NH3(μg/m3)", Order = 20)]
            public virtual decimal NH3 { get; set; }


            ///<summary>
            /// HCL (μg/m3)
            ///</summary>
            [Display(Name = "HCL (μg/m3)", Order = 21)]
            public virtual decimal HCL { get; set; }


            ///<summary>
            /// CL2(μg/m3)
            ///</summary>
            [Display(Name = "CL2(μg/m3)", Order = 22)]
            public virtual decimal CL2 { get; set; }


            ///<summary>
            /// C2H4O2(μg/m3)
            ///</summary>
            [Display(Name = "C2H4O2(μg/m3)", Order = 23)]
            public virtual decimal C2H4O2 { get; set; }


            ///<summary>
            /// C6H6(μg/m3)
            ///</summary>
            [Display(Name = "C6H6(μg/m3)", Order = 24)]
            public virtual decimal C6H6 { get; set; }


            ///<summary>
            /// 多租户
            ///</summary>
            [Display(Name = "多租户", Order = 25)]
            public virtual Guid? TenantId { get; set; }


            #endregion
        }
        #endregion
    }
    public class ReponseEntity<T>
    {
        /// <summary>
        /// 总行数，无论请求多少条数据该值都反映的是数据库中总共有多少行数据，该值对于当前交互没什么用，仅作为解析数据用
        /// </summary>
        public long totalCount { get; set; }
        /// <summary>
        /// 实际反馈对象，基本上我们只请求一行数据，该对象最多1行
        /// </summary>
        public List<T> items { get; set; }
    }
}
