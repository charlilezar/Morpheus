﻿//using MQTTnet.Client;
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
        public string Id { get; set; } = "TCP" + r.Next(1111, 9999);
        //public string Charset { get; set; } = "gbk";
        //[JsonIgnore]
        public EndPointEnum EndPointType { get;  } = EndPointEnum.TCP;

        public string ServerIP { get; set; } = "127.0.0.1";
        public int ServerPort { get; set; } = 8881;
        /// <summary>
        /// 使用websocket时的url,包括协仪ip端口和路径. ws://127.0.0.1:8083/mqtt 
        /// </summary>
        //public string ServerUrl { get; set; } = "ws://127.0.0.1:8083/mqtt"; 

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

        private static Random r = new Random();

        public string RandomClientId()
        {
            return "TcpClient_" + r.Next(111111, 999999);
        }


    }


}
