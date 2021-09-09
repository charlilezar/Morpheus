using DataPlatformTrans.DataEntitys.MessageEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPlatformTrans.DataFactory
{
    public class DeviceInfo
    {
        /// <summary>
        /// 数据请求所在的租户
        /// </summary>
        public string TenantId { get; set; }
        /// <summary>
        /// 网络节点
        /// </summary>
        public string MN { get; set; }
        /// <summary>
        /// 设备所属站点
        /// </summary>
        public string SNo { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>
        public string ENO { get; set; }
        /// <summary>
        /// 设备在数采系统中的唯一识别号
        /// </summary>
        public long ID{ get; set; }
    }
    public class CPDataFactory
    {
        /// <summary>
        /// 存储接受的数据
        /// </summary>
        public Action<string> Common_SaveReceivedData = null;
        /// <summary>
        /// 存储发送的数据
        /// </summary>
        public Action<string> Common_SaveSendData = null;
        /// <summary>
        /// 获取指定设备的超大容量数据；
        /// </summary>
        public Func<DeviceInfo,int, Task<RtnMessage<RecordRealNBS>>> NBSRecord_GetData = null;
        /// <summary>
        /// 提交到平台后，本地数据库存储一份上传记录。以便与本地管理人员核实上传数据
        /// 但保存失败的话，不再进行存储，只能以日志形式存储在本地
        /// </summary>
        public Action<DeviceInfo, RecordRealNBS> NBSRecord_SaveRecord = null;
        /// <summary>
        /// 获取实时数据；
        /// </summary>
        public Func<DeviceInfo,int, Task<RtnMessage<RecordRealZCQ>>> ZCQRealRecord_GetData = null;
        /// <summary>
        /// 实时数据提交到平台后，本地数据库存储一份上传记录。以便与本地管理人员核实上传数据
        /// 但保存失败的话，不再进行存储，只能以日志形式存储在本地
        /// </summary>
        public Action<DeviceInfo, RecordRealZCQ> ZCQRealRecord_SaveRecord = null;
        /// <summary>
        /// 高压电离室实时数据读取
        /// </summary>
        public Func<DeviceInfo, int, Task<RtnMessage<RecordRealHve>>> HveRealRecord_GetData = null;
        /// <summary>
        /// 高压电离室实时数据存储（本地记录）
        /// </summary>
        public Action<DeviceInfo, RecordRealHve> HveRealRecord_SaveRecord = null;
        /// <summary>
        /// 气象参数实时数据获取
        /// </summary>
        public Func<DeviceInfo, int, Task<RtnMessage<RecordRealAtmosphere>>> AtmosphereRealRecord_GetData = null;
        /// <summary>
        /// 气象参数实时数据存储后保存（本地记录）
        /// </summary>
        public Action<DeviceInfo, RecordRealAtmosphere> AtmosphereRealRecord_SaveRecord = null;
        /// <summary>
        /// 碘化钠溥仪实时数据获取
        /// </summary>
        public Func<DeviceInfo, int, Task<RtnMessage<RecordNaI>>> NaIRealRecord_GetData = null;
        /// <summary>
        /// 碘化钠溥仪实时数据保存成功的保存（本地记录）
        /// </summary>
        public Action<DeviceInfo, RecordNaI> RecordNaIRealRecord_SaveRecord = null;

    }
    #region 数据存储对象
    public class SendPlatFormDataBase
    {
    }
    /// <summary>
    /// 高压电离实时数据
    /// </summary>
    public class RecordRealHve : SendPlatFormDataBase
    {
        ///<summary>
        /// 伽马剂量率，单位：，从高压电离数据HveRecord对应的表读取
        ///</summary>
        public decimal SpCol_0102060301 { get; set; }
        /// <summary>
        /// 电池电压，单位：，从高压电离数据HveRecord对应的表读取
        /// </summary>
        public decimal SpCol_0102069901 { get; set; }
        ///<summary>
        /// 高电压，协议中说的是garma计温度，从厂家工程师那（陈文胜）得知就是高电压，单位：V，从高压电离数据HveRecord的字段HighVoltage读取对应的表读取
        ///</summary>
        public decimal SpCol_0102069902 { get; set; }
        /// <summary>
        /// 目前不清楚是什么，厂家工程师那（陈文胜）说可以用DAQTemperature替代，但这个温度没存储
        /// </summary>
        public decimal SpCol_0102069903 { get; set; }
    }
    /// <summary>
    /// 超大容量气溶胶实时数据
    /// </summary>
    public class RecordRealNBS : SendPlatFormDataBase
    {
        public string WorkID { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime DataTime { get; set; }
        /// <summary>
        /// 瞬时采样流量，单位：m3/h
        /// </summary>
        public decimal SpCol_0102069904 { get; set; }
        /// <summary>
        /// 累积采样体积，单位：m3
        /// </summary>
        public decimal SpCol_0102069905 { get; set; }
    }

    /// <summary>
    /// 自动气象站实时数据
    /// </summary>
    public class RecordRealAtmosphere : SendPlatFormDataBase
    {
        ///<summary>
        /// 大气温度，单位：℃，从气象数据读取
        ///</summary>
        public decimal SpCol_0901040101 { get; set; }
        /// <summary>
        /// 大气湿度，单位：%，从气象数据读取
        /// </summary>
        public decimal SpCol_0901040102 { get; set; }
        ///<summary>
        /// 大气气压(hpa)，从气象数据读取
        ///</summary>
        public decimal SpCol_0901040103 { get; set; }
        /// <summary>
        /// 风速，单位：(m/s)，从气象数据读取
        /// </summary>
        public decimal SpCol_0901040106 { get; set; }

        ///<summary>
        /// 风向，单位：°，从气象数据读取
        ///</summary>
        public decimal SpCol_0901040105 { get; set; }
        /// <summary>
        /// 感雨功能，true：下雨，false为不下雨，从动环数据中读取
        /// </summary>
        public bool SpCol_0901040107 { get; set; }
        /// <summary>
        /// 降雨强度，单位：mm/min，目前没有地方去读，毛工他们的意思让软件部门自己算
        /// </summary>
        public decimal SpCol_0901040104 { get; set; }
    }
    /// <summary>
    /// 气碘采样实时数据
    /// </summary>
    public class RecordRealZCQ: SendPlatFormDataBase
    {
        public string WorkID { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime DataTime { get; set; }
        /// <summary>
        /// 瞬时采样流量，单位：L/min
        /// </summary>
        public decimal SpCol_0102069904 { get; set; }
        /// <summary>
        /// 累积采样体积，单位：L
        /// </summary>
        public decimal SpCol_0102069905 { get; set; }
    }
    public class RecordRealZJC : SendPlatFormDataBase
    {
        /// <summary>
        /// 降雨起始时间
        /// </summary>
        public DateTime BeginTime { get; set; }
        /// <summary>
        /// 降雨结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
    }
    public class RecordNaI : SendPlatFormDataBase
    {
        /// <summary>
        /// 碘化钠原始数据，XML格式的
        /// </summary>
        public string SpCol_0102060332 { get; set; }
    }
    #endregion

}
