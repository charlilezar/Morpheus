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
    public abstract class FrameBase
    {


        /// <summary>
        /// 原始Hex
        /// </summary>
        public string RAW { get; set; }

        public string Btyes { get;private set; }

        /// <summary>
        /// 组合参数并计算Bytes,
        /// </summary>
        /// <returns></returns>
        public virtual byte[] GetBytes()
        {
            throw new NotImplementedException();
        }


        public override string ToString()
        {
            if (string.IsNullOrEmpty(RAW))
                GetBytes();
            return RAW;
        }

        ///// <summary>
        ///// 用关键参数组装一个完整的桢，在具体类中实现，也可以扩展不同的重载。
        ///// </summary>
        ///// <param name="datas">部份关键数据</param>
        ///// <returns></returns>
        //public static IFrame BuildFram(params byte[] datas){

        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// 从完整内容创建能讯桢
        ///// </summary>
        ///// <param name="alldata"></param>
        ///// <returns></returns>

        //public static IFrame DecodeFrame(byte[] alldata)
        //{
        //    throw new NotImplementedException();
        //}

       

        /// <summary>
        /// 验证桢头和校验码，保证是有效数据
        /// </summary>
        /// <returns></returns>
         public virtual bool Validate(byte[] alldata)
        {
            return true;
        }
        public static Encoding gbk = Encoding.GetEncoding("gbk");
    }


}