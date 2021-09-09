using Aming.DTU.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommCore.CommEntity
{
    /// <summary>
    /// 通讯命令日志
    /// </summary>
    public class IotCmdLog
    {

        /// <summary>
        /// 设备ID;
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 设备名称;
        /// </summary>
        public string DeviceName { get; set; }


        /// <summary>
        /// 当前设备对应的设备序列号;
        /// </summary>
        public string SN { get; set; }

        ///// <summary>
        ///// 通讯协议类型
        ///// </summary>
        //public EndPointEnum EndPointType { get; } = EndPointEnum.MQTT;

        ///// <summary>
        ///// 通讯IP;
        ///// </summary>
        //public string CommIP { get; set; }

        ///// <summary>
        ///// 通讯端口;
        ///// </summary>
        //public int CommPort { get; set; }


        /// <summary>
        /// 命令名称,用于判断回复时使用;
        /// </summary>
        public string CmdName { get; set; }

        /// <summary>
        /// 命令内容全文（转hex字符串);
        /// </summary>
        public string Send { get; set; }   //字符串型命令;

        ///// <summary>
        ///// 命令内容BYTES;
        ///// </summary>
        //public byte[] SendBytes { get; set; }    //Byte数组型命令;

        /// <summary>
        /// 命令内容字符串;
        /// </summary>
        public DateTime SendDateTime { get; set; }   //字符串型命令;

        /// <summary>
        /// 命令响应内容全文（转hex字符串);
        /// </summary>
        public string Response { get; set; }   //字符串型命令;

        ///// <summary>
        ///// 命令内容BYTES;
        ///// </summary>
        //public byte[] ResponseBytes { get; set; }    //Byte数组型命令;

        /// <summary>
        /// 命令内容字符串;
        /// </summary>
        public DateTime ResponseDateTime { get; set; }   //字符串型命令;


        


    }
}
