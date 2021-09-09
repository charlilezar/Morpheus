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
        public EndPointEnum EndPointType { get; }
        public string Charset { get; set; }

        ///// <summary>
        ///// 是否在打开或连接状态
        ///// </summary>
        //public bool IsOpen { get; }

        ///// <summary>
        ///// 打开连接,并返回创建的对象
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <returns></returns>
        //public T Open();

        //public bool Close();


    }

    public enum EndPointEnum
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
