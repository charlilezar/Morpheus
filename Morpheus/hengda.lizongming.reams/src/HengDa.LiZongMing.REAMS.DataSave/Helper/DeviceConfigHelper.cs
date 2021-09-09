using Aming.DTU.Config;
using HengDa.LiZongMing.REAMS.Devices;
using HengDa.LiZongMing.REAMS.Devices.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HengDa.LiZongMing.REAMS
{
    public static class DeviceConfigHelper
    {
        /// <summary>
        /// 正则
        /// </summary>
       static Regex rgx =new Regex("\"EndPointType\":(\\d+),");
        /// <summary>
        /// 解析硬件连接配置
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static IDataEndPoint GetDeviceConfig(string jsonConfig)
        {
            // String tcpPattern = "\\"+"\"Id\""+"\\"+":"+"\"\\s"+"TCP"+"\\S*\"";
            // IDataEndPoint config = null;
            //var m= rgx.Match(jsonConfig);
            // if (jsonConfig.Contains("MqttType"))
            //     config = JsonConvert.DeserializeObject<MqttClientConfig>(jsonConfig);
            // else if (System.Text.RegularExpressions.Regex.IsMatch(jsonConfig, tcpPattern))
            //     config = JsonConvert.DeserializeObject<TcpClientConfig>(jsonConfig);
            // else
            //     config = JsonConvert.DeserializeObject<UARTConfig>(jsonConfig);
            // return config;
            IDataEndPoint config = null;
            var m = rgx.Match(jsonConfig);
            if (jsonConfig.Contains("MqttType"))
                config = JsonConvert.DeserializeObject<MqttClientConfig>(jsonConfig);
            else if (m.Success && m.Groups[1].Value == ((int)EndPointEnum.TCP).ToString())
                config = JsonConvert.DeserializeObject<TcpClientConfig>(jsonConfig);
            else
                config = JsonConvert.DeserializeObject<UARTConfig>(jsonConfig);
            return config;
        }

        /// <summary>
        /// 硬件连接配置反序列化为json
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string SetDeviceConfig(IDataEndPoint config)
        {
            string jsonConfig = JsonConvert.SerializeObject(config);
            return jsonConfig;
        }
        /// <summary>
        /// 解析硬件连接配置
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static IDataEndPoint GetConfig(this Device device)
        {
            return GetDeviceConfig(device.HWConnectionJson);
        }

        /// <summary>
        /// 硬件连接配置反序列化为json
        /// </summary>
        /// <param name="device"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Device SetConfig(this Device device, IDataEndPoint config)
        {
            string jsonConfig = SetDeviceConfig(config);
            device.HWConnectionJson = jsonConfig;
            return device;
        }

        public static IDataEndPoint GetConfig(this DeviceDto device)
        {
            return GetDeviceConfig(device.HWConnectionJson);
        }
        public static DeviceDto SetConfig(this DeviceDto device, IDataEndPoint config)
        {
            string jsonConfig = SetDeviceConfig(config);
            device.HWConnectionJson = jsonConfig;
            return device;
        }

        public static EndPointEnum GetEndPointType(this Device device)
        {
            return GetEndPointType(device.HWConnectionJson);
        }
        /// <summary>
        /// 获取配置中的连接类型
        /// </summary>
        /// <param name="jsonConfig"></param>
        /// <returns></returns>
        public static EndPointEnum GetEndPointType(string jsonConfig)
        {
            
            var m = rgx.Match(jsonConfig);
            if (m.Success && !string.IsNullOrEmpty(m.Groups[1].Value))
            {
                return (EndPointEnum) Convert.ToInt32(m.Groups[1].Value);
            }
            return EndPointEnum.UART;

        }
    }
}
