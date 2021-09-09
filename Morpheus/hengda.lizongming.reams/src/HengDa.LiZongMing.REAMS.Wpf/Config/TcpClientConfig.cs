//using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Aming.DTU.Config
{
   public class TcpClientConfig : IDataEndPoint//<MQTTnet.TestApp.NetCore.MqttClientService>
    {
        public string Id { get; set; } = "TCP" + r.Next(111111, 999999);
        public string Charset { get; set; } = "gbk";
        public EndPointEnum EndPointType { get;  } = EndPointEnum.TCP;

        public string ServerIP { get; set; } = "127.0.0.1";
        public int ServerPort { get; set; } = 8881;
        /// <summary>
        /// 使用websocket时的url,包括协仪ip端口和路径. ws://127.0.0.1:8083/mqtt 
        /// </summary>
        //public string ServerUrl { get; set; } = "ws://127.0.0.1:8083/mqtt"; 



       private static Random r = new Random();

        public string RandomClientId()
        {
            return "TcpClient_" + r.Next(111111, 999999);
        }


    }


}
