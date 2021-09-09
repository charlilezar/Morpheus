//using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Aming.DTU.Config
{
    public class ForwardsConfig
    {
        public UARTConfig Left { get; set; }//IDataEndPoint
        public MqttConfig Right { get; set; }

        /// <summary>
        /// 从左到右时是否将二进制转成Hex的16进制的字符,返回的数据也会再反转换
        /// </summary>
        public bool Raw2Hex { get; set; } = false;
        /// <summary>
        /// 从左到右时是否将二进制转成base64编码,比hex短,返回的数据也会再反转换
        /// </summary>
        public bool Raw2Base64 { get; set; } = false;
        /// <summary>
        /// yyy
        /// </summary>
        //public List<ForwardItem> Items { get; set; } = new List<ForwardItem>();

        //public ForwardItem Item { get; set; } = new ForwardItem();
        //public Dictionary<string, IDataEndPoint> EndPoints { get; set; } = new Dictionary<string, IDataEndPoint>();

        internal static ForwardsConfig Default()
        {
            ForwardsConfig config = new ForwardsConfig();

            UARTConfig u = new UARTConfig();
            MqttConfig mq = new MqttConfig();

            config.Left = u;
            config.Right = mq;

            return config;
        }

        /// <summary>
        /// 界面上显示的编码,只支持utf-8和 gbk两种
        /// </summary>
        public string UIEncoding { get; set; } = "utf-8"; //


        public Encoding GetUIEncoding()
        {
            return GetEncoding(UIEncoding);
        }
        public Encoding GetEncoding(string encoding)
        {
            if (encoding == null || encoding.ToUpper() == "UTF8" || encoding.ToUpper() == "UTF-8")
                return Encoding.UTF8;
            return Encoding.GetEncoding("gbk");
        }
    }
    //public class ForwardItem
    //{
    //    //public string Left { get; set; } = "COM1";
    //    //public string Right { get; set; } = "MQTT1";
    //    public IDataEndPoint Left { get; set; } 
    //    public IDataEndPoint Right { get; set; } 

    //    /// <summary>
    //    /// 从左到右时是否将二进制转成Hex的16进制的字符,返回的数据也会再反转换
    //    /// </summary>
    //    public bool Raw2Hex { get; set; } = false;
    //    /// <summary>
    //    /// 从左到右时是否将二进制转成base64编码,比hex短,返回的数据也会再反转换
    //    /// </summary>
    //    public bool Raw2Base64 { get; set; } = false;

    //    ///// <summary>
    //    ///// 左边的字符编码,只支持utf-8和 gbk两种
    //    ///// </summary>
    //    //public string LeftEncoding { get; set; } = "gbk"; //utf-8
    //    ///// <summary>
    //    ///// 右边的字符编码,只支持utf-8和 gbk两种
    //    ///// </summary>
    //    //public string RightEncoding { get; set; } = "gbk"; //utf-8


    //}
}
