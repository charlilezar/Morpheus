using Aming.Core;
using Aming.DTU.Config;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using HengDa.LiZongMing.REAMS;

namespace Aming.DTU
{
    /// <summary>
    /// 数据传输层接口
    /// </summary>
    public class ServiceFactory
    {
        public static ITransmissionService Create(ILoggerFactory loggerFactory, IDataEndPoint config)
        {
            ITransmissionService TranService = null;
            switch (config.EndPointType)
            {
                case EndPointEnum.UART:
                    {
                        var tsObj = new SerialPortService(loggerFactory, (UARTConfig)config);
                        TranService = tsObj;
                    }

                    break;
                case EndPointEnum.TCP:
                    {
                        var tsObj = new TcpClientService(loggerFactory, (TcpClientConfig)config);
                        TranService = tsObj;
                    }

                    break;
                case EndPointEnum.MQTT:
                    {
                        MqttClientService tsObj = new MqttClientService(loggerFactory, (MqttClientConfig)config);
                        //tsObj.funcCheckPackFormat = this.CheckPackFormatByModbus;
                        //定义Modbus下的不应该用粘包处理...先放着
                        TranService = tsObj;

                    }
                    break;
                default:
                    throw new NotImplementedException("不支持的方式"); //出错
                    //break;
            }
            return TranService;
        }
        public static ITransmissionService Create(ILoggerFactory loggerFactory, string jsonConfig)
        {
            IDataEndPoint config = DeviceConfigHelper.GetDeviceConfig(jsonConfig);

            return Create(loggerFactory, config);
        }
        public static IDataEndPoint CreateConfig(ILoggerFactory loggerFactory, string jsonConfig)
        {
            IDataEndPoint config = DeviceConfigHelper.GetDeviceConfig(jsonConfig);

            return config;
        }


    }
}