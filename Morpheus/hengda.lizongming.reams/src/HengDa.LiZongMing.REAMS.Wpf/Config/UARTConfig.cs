//using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Aming.DTU.Config
{
   public class UARTConfig : IDataEndPoint
    {
        public string Id { get; set; } = "UART" + r.Next(111111, 999999);
        public string Charset { get; set; } = "gbk";

        [JsonIgnore]
        public EndPointEnum EndPointType { get;  } = EndPointEnum.UART;
        public string Port { get; set; } = "COM8"; //WS,WSS,SSL
        public int BaudRate { get; set; } = 9600;

        private static Random r = new Random();

    }


}
