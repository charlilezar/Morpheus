//using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Aming.DTU.Config
{
    /// <summary>
    /// 通用的Message基类，方便不同通道的数据统一传输，主要兼容MqttApplicationMessage
    /// </summary>
    public class UniMessageBase
    {
        public UniMessageBase()
        {

        }

        public string ClientId { get; set; }
        public string UserName { get; set; }
        public string Topic { get; set; }
        public byte[] Payload { get; set; }

    }


}
