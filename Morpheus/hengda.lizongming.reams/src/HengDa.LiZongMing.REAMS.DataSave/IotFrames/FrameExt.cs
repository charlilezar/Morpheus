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
    public static class FrameExt
    {
    

        /// <summary>
        /// 从完整内容创建能讯桢
        /// </summary>
        /// <param name="alldata"></param>
        /// <returns></returns>

        // public IFrame DecodeFrame(this FrameBase frame, byte[] alldata)
        //{
        //    return frame.DecodeFrame(alldata);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allHex"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static IFrame DecodeFrame(this FrameBase frame, string allHex,Encoding encode)
        {
            var alldata=allHex.HexStrToHexByte();
            return frame.DecodeFrame(alldata);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allHex"></param>
        /// <returns></returns>
        public static IFrame DecodeFrame(this FrameBase frame, string allHex )
        {
            return DecodeFrame(frame,allHex, gbk);
        }


        static Encoding gbk = Encoding.GetEncoding("gbk");
    }
}