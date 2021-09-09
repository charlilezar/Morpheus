using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataPlatformTrans.DataEntitys
{
    public class AppConfig
    {
        public static string TenantId = "";
        /// <summary>
        /// 访问密码
        /// </summary>
        public static string PWD = "123456";
        /// <summary>
        /// 网络心跳包发送间隔，单位：秒，从协议上看，这个不分站点，见命令6031的示例；
        /// </summary>
        public static int RtdHeartbeat = 30;
        /// <summary>
        /// 超时时间(单位：秒)
        /// </summary>
        public static int OverTime = 15;
        /// <summary>
        /// 超时后的重发次数，如果该值是1，则表示发送失败后，可以再重新发3次，加上第一次总共尝试4次；该值由数据平台定义是多少
        /// </summary>
        public static int ReCount = 3;
        /// <summary>
        /// 发送失败后的发送次数，该值不受数据中心控制
        /// </summary>
        public static int SendFaildReCount = 2;
        /// <summary>
        /// 超限报警时间（延时）（什么意思？），单位:秒
        /// </summary>
        public static int WarnTime = 300;
        /// <summary>
        /// 站点集合，一套程序可能应对多个站点
        /// </summary>
        public static List<StationEntity> Stations;
        /// <summary>
        /// MN编码，每条程序对应一个网络节点标识
        /// </summary>
        public static string MN { get; set; } = "0101A010000000";
        public class StationEntity
        {
            /// <summary>
            /// 站点编号
            /// </summary>
            public string SNO { get; set; }
            /// <summary>
            /// 是否启用，如果启用的话，不会上传该数据
            /// </summary>
            public bool Enabled { get; set; }
            ///// <summary>
            ///// 超时时间（什么的超时时间？），单位：秒
            ///// </summary>
            //public int OverTime = 5;
            ///// <summary>
            ///// 重发次数，如果该值是1，则表示发送失败后，可以再重新发3次，加上第一次总共尝试4次；
            ///// </summary>
            //public int ReCount = 3;
            ///// <summary>
            ///// 超限报警时间（延时）（什么意思？），单位:秒
            ///// </summary>
            //public int WarnTime = 300;
            
            public List<DeviceEntity> Devices { get; set; }
            public DeviceEntity FindDeviceByENo(string sENo)
            {
                if (Devices == null || Devices.Count == 0) return null;
                return Devices.Find(m => m.ENO == sENo);
            }
            public DeviceEntity FindDeviceByType(DeviceEntity.DeviceTypes type)
            {
                if (Devices == null || Devices.Count == 0) return null;
                return Devices.Find(m => m.DeviceType == type);
            }
        }
        public class DeviceEntity
        {
            #region 设备类型枚举
            public enum DeviceTypes
            {
                /// <summary>
                /// 气碘采样器
                /// </summary>
                ZCQ=1,
                /// <summary>
                /// 干湿沉降
                /// </summary>
                ZJC=2,
                /// <summary>
                /// 超大容量气溶胶
                /// </summary>
                NBS=3,
                /// <summary>
                /// 高压电离室
                /// </summary>
                HVE=4,
                /// <summary>
                /// 碘化钠溥仪
                /// </summary>
                NaIs=6,
                /// <summary>
                /// 自动气象站
                /// </summary>
                Atmosphere=7
            }
            #endregion
            public DeviceTypes DeviceType { get; set; }

            public string ENO { get; set; }
            /// <summary>
            /// 设备唯一识别号，这个在本系统中不需要用到，这个信息在获取数据是会使用到
            /// </summary>
            public string SN { get; set; }
            /// <summary>
            /// 数采系统中设备的唯一识别号，该值由应用层配置，上传平台系统用不上，只是传给DataFactory实现数据读取时用到
            /// </summary>
            public long ID { get; set; }
            /// <summary>
            /// 设备站点，目前来说没什么用
            /// </summary>
            public string SNO { get; set; }
            /// <summary>
            /// 设备上传数据的循环周期，单位：秒
            /// </summary>
            public int CTime { get; set; }
        }
        #region 功能函数
        public static bool ReadConfigFromJsonFile(string sJsonFile, out string sErr)
        {
            if (!File.Exists(sJsonFile))
            {
                sErr = $"文件[{sJsonFile}]不存在！";
                return false;
            }
            var sjson = File.ReadAllText(sJsonFile);
            if(String.IsNullOrWhiteSpace(sjson))
            {
                sErr = $"文件[{sJsonFile}]内容为空，无法读取站点信息！";
                return false;
            }
            //读取Json文件加载站点等信息
            return ReadConfigFromJsonText(sjson, out sErr);
        }
        public static bool ReadConfigFromJsonText(string sJsText, out string sErr)
        {
            //读取Json文件加载站点等信息
            try
            {
                Stations = JsonConvert.DeserializeObject<List<StationEntity>>(sJsText);
            }
            catch (Exception ex)
            {
                sErr = $"读取站点配置文件出错：{ex.Message}({ex.Source})";
                return false;
            }
            sErr = string.Empty;
            return true;
        }
        /// <summary>
        /// 获取SNo对应的站点
        /// </summary>
        /// <param name="sSNO"></param>
        /// <returns></returns>
        public static StationEntity GetStationBySNO(string sSNO)
        {
            if (AppConfig.Stations == null || AppConfig.Stations.Count == 0) return null;
            return AppConfig.Stations.Find(m => string.Compare(m.SNO, sSNO, true) == 0);
        }
        /// <summary>
        /// 根据传入的设备ENO编码，获取指定站点中的设备对象
        /// </summary>
        /// <param name="sSNO">指定站点</param>
        /// <param name="sENO">指定设备的ENO编号</param>
        /// <returns></returns>
        public static DeviceEntity GetDeviceByENO(string sSNO,string sENO)
        {
            StationEntity station = GetStationBySNO(sSNO);
            if (station == null) return null;
            DeviceEntity device = station.FindDeviceByENo(sENO);
            return device;
        }
        /// <summary>
        /// 根据传入的设备类型获取，获取指定站点中的设备对象
        /// </summary>
        /// <param name="sSNO">指定站点</param>
        /// <param name="type">设备类型</param>
        /// <returns></returns>
        public static DeviceEntity GetDeviceByDeviceTtype(string sSNO,DeviceEntity.DeviceTypes type)
        {
            StationEntity station = GetStationBySNO(sSNO);
            if (station == null) return null;
            DeviceEntity device = station.FindDeviceByType(type);
            return device;
        }
        #endregion
    }
}
