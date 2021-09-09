//using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Aming.DTU.Config
{
   public class MqttConfig: IDataEndPoint//<MQTTnet.TestApp.NetCore.MqttClientService>
    {
        public string Id { get; set; } = "MQTT" + r.Next(111111, 999999);
        public string Charset { get; set; } = "gbk";
        public EndPointEnum EndPointType { get;  } = EndPointEnum.MQTT;
        public string MqttType { get; set; } = "TCP"; //WS,WSS,SSL
        public string ServerIP { get; set; } = "api.hengda.show";
        public int ServerPort { get; set; } = 1883;
        /// <summary>
        /// 使用websocket时的url,包括协仪ip端口和路径. ws://127.0.0.1:8083/mqtt 
        /// </summary>
        public string ServerUrl { get; set; } = "ws://127.0.0.1:8083/mqtt"; 

        //[JsonIgnore]
        public string ClientId { get; set; } = "Client_" + r.Next(111111,999999);
        public string UserName { get; set; } = "88888888";
        public string Password { get; set; } = "88888888";

        public string SubTopic { get; set; } = "/mingdtu/dtu/3c6105156c3c/#";
        /// <summary>
        /// 硬件上传数据的主题
        /// </summary>
        public string PubTopicUp { get; set; } = "/mingdtu/dtu/3c6105156c3c/hex/up";
        /// <summary>
        /// 下载到硬件的主题 
        /// </summary>
        public string PubTopicDown { get; set; } = "/mingdtu/dtu/3c6105156c3c/hex/down";

        /// <summary>
        /// 掉线时的遗言发布主题,暂不支持
        /// </summary>
        public string WillTopic { get; set; } = "DTU/{UserName}/offline"; // 在线状态发布

       private static Random r = new Random();

        public string RandomClientId()
        {
            return "Client_" + r.Next(111111, 999999);
        }

        public string GetPubTopicUp()
        {
            return PubTopicUp;//PubTopicUp.Replace("{UserName}", UserName).Replace("{ClientId}", ClientId);
        }
        public string GetPubTopicDown()
        {
            return PubTopicDown;//PubTopicDown.Replace("{UserName}", UserName).Replace("{ClientId}", ClientId);
        }
    }


}
