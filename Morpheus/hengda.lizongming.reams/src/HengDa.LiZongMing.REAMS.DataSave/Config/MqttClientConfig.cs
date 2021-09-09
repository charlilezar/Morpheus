//using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Aming.DTU.Config
{
   public class MqttClientConfig : IDataEndPoint//<MQTTnet.TestApp.NetCore.MqttClientService>
    {
        public string Id { get; set; } = "MQTT" + r.Next(1111, 9999);
        //public string Charset { get; set; } = "gbk";
        public string SN { get; set; } = "sn";
        //[JsonIgnore]
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

        /// <summary>
        /// 订阅主题，看需要是否有通配符，
        /// </summary>
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
        public string WillTopic { get; set; } = "/mingdtu/dtu/{UserName}/offline"; // 在线状态发布




        ///// <summary>
        ///// 是否将二进制转成Hex的16进制的字符,返回的数据也会再反转换
        ///// </summary>
        //public bool Raw2Hex { get; set; } = true;
        ///// <summary>
        ///// 是否将二进制转成base64编码,比hex短,返回的数据也会再反转换
        ///// </summary>
        //public bool Raw2Base64 { get; set; } = false;

        #region 转发配置
        /// <summary>
        /// 启用转发
        /// </summary>
        public bool EnableForward { get; set; } = true;

        /// <summary>
        /// 转发到接口的配置Id
        /// </summary>
        public string ForwardToId { get; set; } = ""; // 在线状态发布
        /// <summary>
        /// 数据转发到的目标主题
        /// </summary>
        public string ForwardToTopic { get; set; } = ""; // 在线状态发布
        /// <summary>
        /// 数据转发到的目标主题
        /// </summary>
        public string ForwardFromTopic { get; set; } = ""; // 在线状态发布 


        public string GetForwardToTopic(string UserName, string ClientId, string SN)
        {
            return ForwardToTopic.Replace("{UserName}", UserName).Replace("{ClientId}", ClientId).Replace("{SN}", SN);
        }
        public string GetForwardFromTopic(string UserName, string ClientId, string SN)
        {
            return ForwardFromTopic.Replace("{UserName}", UserName).Replace("{ClientId}", ClientId).Replace("{SN}", SN);
        }
        #endregion


        internal static Random r = new Random();

        public string RandomClientId()
        {
            return "Client_" + r.Next(111111, 999999);
        }

        public string GetSubTopicUp()
        {
            return PubTopicUp.Replace("{UserName}", UserName).Replace("{ClientId}", ClientId).Replace("{SN}", SN);
        }
        public string GetPubTopicDown()
        {
            return PubTopicDown.Replace("{UserName}", UserName).Replace("{ClientId}", ClientId).Replace("{SN}", SN);
        }

  
    }


}
