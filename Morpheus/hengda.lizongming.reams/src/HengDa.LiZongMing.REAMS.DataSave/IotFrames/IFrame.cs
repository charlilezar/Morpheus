using System;
using System.IO;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using Aming.Core;

namespace HengDa.LiZongMing.REAMS
{

    /// <summary>
    /// 控制设备通讯接口
    /// </summary>
    public interface IFrame
    {
        /// <summary>
        /// 原始Hex
        /// </summary>
        string RAW { get;  }


        /// <summary>
        /// 计算Bytes
        /// </summary>
        /// <returns></returns>
        //byte[] GetBytes();
        string ToString()
        {
            //if (string.IsNullOrEmpty(RAW))
            //    GetBytes();
            return RAW??"没有正确设置数据";
        }

        /// <summary>
        /// 用关键参数组装一个完整的桢，在具体类中实现，也可以扩展不同的重载。
        /// </summary>
        /// <param name="datas">部份关键数据</param>
        /// <returns></returns>
          IFrame BuildFram(byte[] cmd,params byte[] datas);

        /// <summary>
        /// 从完整内容创建能讯桢
        /// </summary>
        /// <param name="alldata"></param>
        /// <returns></returns>

        IFrame DecodeFrame(byte[] alldata);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allHex"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        IFrame DecodeFrame(string allHex, Encoding encode)
        {
            var alldata = allHex.HexStrToHexByte();
            return DecodeFrame(alldata);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allHex"></param>
        /// <returns></returns>
        IFrame DecodeFrame(string allHex)
        {
            return DecodeFrame(allHex, gbk);
        }

        /// <summary>
        /// 验证桢头和校验码，保证是有效数据
        /// </summary>
        /// <returns></returns>
        bool Validate(byte[] alldata)
        {
            return true;
        }
        static Encoding gbk = Encoding.GetEncoding("gbk");
    }
}