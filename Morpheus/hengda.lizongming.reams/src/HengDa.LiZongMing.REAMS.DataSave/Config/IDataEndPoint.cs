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
    /// 一个通讯终结点
    /// </summary>
    public interface IDataEndPoint //<T> where T:class
    {
        string Id { get; set; }

        public EndPointEnum EndPointType { get; }

        #region 转发配置
        /// <summary>
        /// 启用转发
        /// </summary>
        bool EnableForward { get; set; }

        /// <summary>
        /// 转发到接口的配置Id
        /// </summary>
        string ForwardToId { get; set; }
        /// <summary>
        /// 数据转发到的目标主题
        /// </summary>
        string ForwardToTopic { get; set; }
        /// <summary>
        /// 数据转发到的目标主题
        /// </summary>
        string ForwardFromTopic { get; set; }


        string GetForwardToTopic(string UserName, string ClientId, string SN);
        string GetForwardFromTopic(string UserName, string ClientId, string SN);
        #endregion
    }

    public enum EndPointEnum:int
    {
        UART=0,
        UDP = 1,
        UDPSERVER= 2,
        TCP = 3,
        TCPSERVER = 4,
        HTTP = 5,
        MQTT=6
    }
}
