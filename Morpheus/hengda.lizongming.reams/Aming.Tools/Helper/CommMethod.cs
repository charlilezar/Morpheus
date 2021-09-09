using System;
using System.Net.NetworkInformation;
using System.Text;

namespace ddPCR.DriverPlatform.Ins.Mcu
{
    /// <summary>
    /// 公共方法类
    /// </summary>
    public static class CommMethod
    {
        #region 获取字节数组中的一部分元素

        /// <summary>
        /// 从指定索引开始获取字节数组中指定长度的那一部分
        /// </summary>
        /// <param name="buffer">总数组</param>
        /// <param name="startIndex">起始元素索引</param>
        /// <param name="length">截取的长度</param>
        /// <returns></returns>
        public static byte[] GetSomeElem(byte[] buffer, int startIndex, int length)
        {
            if (buffer != null && buffer.Length > 0 && startIndex >= 0 && length > 0 && length <= buffer.Length - startIndex)
            {
                byte[] temp = new byte[length];
                Array.Copy(buffer, startIndex, temp, 0, length);
                return temp;
            }
            return null;
        }

        #endregion 获取字节数组中的一部分元素

        #region ASCII码与字符串之间的相互转换

        /// <summary>
        /// 将字符串中的每个字母转换成ASCII码中对应的值，然后将每个值转换成十六进制字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StringToAscHexStr(string str)
        {
            byte[] array = Encoding.ASCII.GetBytes(str);
            string asciIstr2 = "";
            for (int i = 0; i < array.Length; i++)
            {
                asciIstr2 += ConvertHelper.ByteToHexStr(array[i]);
            }
            return asciIstr2;
        }

        /// <summary>
        /// 将ASCII码形式的十六进制数据转换为实际的十进制数据
        /// </summary>
        /// <param name="ascHexStr"></param>
        /// <returns></returns>
        public static string AscHexStrToString(string ascHexStr)
        {
            StringBuilder sb = new StringBuilder();
            //将十六进制数字转为十进制数字
            for (int i = 0; i < ascHexStr.Length; i += 2)
            {
                sb.Append(ConvertHelper.HexStringToString(ascHexStr.Substring(i, 2), Encoding.ASCII));
            }
            return sb.ToString();
        }

        #endregion ASCII码与字符串之间的相互转换

        #region 检查网络连通性

        /// <summary>
        /// 用于检查IP地址或域名是否可以使用TCP/IP协议访问(使用Ping命令),true表示Ping成功,false表示Ping失败
        /// </summary>
        /// <param name="strIpOrDName">输入参数,表示IP地址或域名</param>
        /// <returns>是否联通</returns>
        public static bool PingIpOrDomainName(string strIpOrDName)
        {
            try
            {
                //允许应用程序检查网络
                Ping objPingSender = new Ping();
                PingOptions objPinOptions = new PingOptions();
                objPinOptions.DontFragment = true;
                string data = "";
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                int intTimeout = 120;
                PingReply objPinReply = objPingSender.Send(strIpOrDName, intTimeout, buffer, objPinOptions);
                string strInfo = objPinReply.Status.ToString();
                if (strInfo == "Success")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion 检查网络连通性
    }
}