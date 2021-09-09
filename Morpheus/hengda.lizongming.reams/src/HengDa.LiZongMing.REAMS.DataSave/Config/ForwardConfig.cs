//using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Aming.DTU.Config
{
    public class ForwardConfig
    {
        //public string Id { get; set; } = "UART" + MqttClientConfig.r.Next(111111, 999999);

        //[JsonIgnore]
        //public EndPointEnum EndPointType { get; } = EndPointEnum.MQTT;

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

    }

}
