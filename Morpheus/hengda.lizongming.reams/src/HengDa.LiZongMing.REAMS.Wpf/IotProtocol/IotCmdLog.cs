using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommCore.CommEntity
{
    /// <summary>
    /// 通讯命令实体类;
    /// </summary>
    public class IotCmdLog
    {

        /// <summary>
        /// 设备ID;
        /// </summary>
        public Int64 DeviceID { get; set; }

        /// <summary>
        /// 设备名称;
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 通讯IP;
        /// </summary>
        public string CommIP { get; set; }

        /// <summary>
        /// 通讯端口;
        /// </summary>
        public int CommPort { get; set; }

        /// <summary>
        /// 设备型号;
        /// </summary>
       // public EnumADeviceModel DeviceModel { get; set; }

        /// <summary>
        /// 当前设备对应的设备序列号;
        /// </summary>
        public string SerialNO { get; set; }

        /// <summary>
        /// 命令名称,用于判断回复时使用;
        /// </summary>
        public string CmdName { get; set; }

        /// <summary>
        /// 命令内容字符串;
        /// </summary>
        public string SendString { get; set; }   //字符串型命令;

        /// <summary>
        /// 命令内容BYTES;
        /// </summary>
        public byte[] SendBytes { get; set; }    //Byte数组型命令;

        /// <summary>
        /// 设备是否已回复本命令;
        /// </summary>
        public bool IsDeviceAnswered { get; set; }  //设备是否已回答;

        /// <summary>
        /// 等待次数，用于描述命令发送后的等待时间;
        /// </summary>
        public int WaitCount { get; set; }  //命令发送后等待次数;



    }
}
