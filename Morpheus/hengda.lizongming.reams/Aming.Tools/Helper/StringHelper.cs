using System;
using System.Text;
//using log4net;

namespace Aming.Core
{
    public static partial class StringHelper
    {
        #region Fields

        //private static readonly ILog _objLogger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region MyRegion

        /// <summary>
        /// Converts an ASCII string to it's string representation as hex bytes
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToHexString(this string s)
        {
            return ToHexString(Encoding.ASCII.GetBytes(s));
        }

        public static string ToHexString(this byte[] arrBytes)
        {
            return ToHexString(arrBytes, 0, arrBytes.Length);
        }

        /// <summary>
        /// Converts an array of bytes to a string of hex bytes
        /// </summary>
        /// <param name="arrBytes"></param>
        /// <param name="iOffset"></param>
        /// <param name="iLength"></param>
        /// <returns></returns>
        public static string ToHexString(this byte[] arrBytes, int startIndex, int length, string spanString="")
        {
            if (arrBytes.Length == 0)
                return "";

            var sb = new StringBuilder("");


            for (int i = startIndex; i < length; i++)
            {
                sb.AppendFormat("{0:X2}{spanString}", arrBytes[i]);
            }

            return sb.ToString();

        }

        /// <summary>
        /// 将字符串转为16进制字符，允许中文
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string StringToHexString(this string s, Encoding encode, string spanString)
        {
            byte[] b = encode.GetBytes(s);//按照指定编码将string编程字节数组
            string result = string.Empty;
            for (int i = 0; i < b.Length; i++)//逐字节变为16进制字符
            {
                result += Convert.ToString(b[i], 16) + spanString;
            }
            return result;
        }

        /// <summary>
        /// 将16进制字符串按照指定的编码格式转为普通字符串
        /// </summary>
        /// <param name="hs"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string HexStringToString(this string hs, Encoding encode)
        {
            string strTemp = "";
            byte[] b = new byte[hs.Length / 2];
            for (int i = 0; i < hs.Length / 2; i++)
            {
                strTemp = hs.Substring(i * 2, 2);
                b[i] = Convert.ToByte(strTemp, 16);
            }
            //按照指定编码将字节数组变为字符串
            return encode.GetString(b);
        }

        /// <summary>
        /// 将整个byte数组转为16进制字符串，注意：如果不需要全部转换请使用重载
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ByteToHexStr(this byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }

        ///// <summary>
        ///// 将整个byte数组转为16进制字符串，注意：如果不需要全部转换请使用重载
        ///// </summary>
        ///// <param name="bytes"></param>
        ///// <returns></returns>
        //public static string PrintByteToHexStr(byte[] bytes)
        //{
        //    string returnStr = "";
        //    if (bytes != null)
        //    {
        //        for (int i = 0; i < bytes.Length; i++)
        //        {
        //            returnStr += bytes[i].ToString("X2") + " ";
        //            if (i == 7 || i == bytes.Length - 1 - 3) //从长度来加空格分开MCU和下层设备的内容
        //                returnStr += " ";
        //        }
        //    }
        //    return returnStr;
        //}
        /// <summary>
        /// 将byte数组中指定的前length个字节数组转为16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ByteToHexStr(this byte[] bytes, int length)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < length && i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }


        /// <summary>
        /// 从指定索引开始，将指定长度的byte[]转为16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ByteToHexStr(this byte[] bytes, int startIndex, int length)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = startIndex; i < startIndex + length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }

        /// <summary>
        /// 将16进制的字符串转为byte[]
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] HexStrToHexByte(this string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if (hexString.Length % 2 != 0)
                throw new ArgumentException("你提供的十六进制字符串长度不对，必须是2的倍数。" + hexString);
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                var hexStr = hexString.Substring(i * 2, 2);
                //Console.WriteLine("找到数据：>>" + hexStr+"<<");
                returnBytes[i] = Convert.ToByte(hexStr, 16);
            }
            return returnBytes;
        }

        public static string ToHexString(this byte data)
        {
            return string.Format("{0:X2}", data);
        }


        #endregion


        #region 获取字节数组中的一部分元素

        /// <summary>
        /// 从指定索引开始获取字节数组中指定长度的那一部分
        /// </summary>
        /// <param name="buffer">总数组</param>
        /// <param name="startIndex">起始元素索引</param>
        /// <param name="length">截取的长度</param>
        /// <returns></returns>
        public static byte[] GetSub(this byte[] buffer, int startIndex, int length)
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


        #region 编码转换
        static Encoding gbk = Encoding.GetEncoding("gbk");
        public static string GBKtoUTF8(this string text)//GBK 转 UTF8
        {

            var buffer = gbk.GetBytes(text);//读取内容
            buffer = Encoding.Convert(gbk, Encoding.UTF8, buffer);//转码
            return Encoding.UTF8.GetString(buffer);
        }

        public static string UTF8toGBK(this string text)//UTF8 转 GBK
        {
            var buffer = Encoding.UTF8.GetBytes(text);//读取内容
            buffer = Encoding.Convert(Encoding.UTF8, gbk, buffer);//转码
            return gbk.GetString(buffer);
        }

        #endregion

        #region 数字类转换

        /// <summary>
        /// 数字和字节之间互转
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static byte[] IntToBit(int num)
        {
            byte[] bytes = BitConverter.GetBytes(num);//将int32转换为字节数组
            return bytes;
        }
        ///// <summary>
        ///// 将字节数组转换成Int32,并格式化为 {D2}的两位数字符串
        ///// </summary>
        ///// <param name="bytes"></param>
        ///// <returns></returns>
        //public static string BitToInt32StringD2(byte[] bytes)
        //{
        //    int temp;
        //    temp = BitToInt(bytes);//将字节数组内容再转成int32类型
        //    return temp.ToString("D2");
        //}
        /// <summary>
        /// 将字节数组转换成Int32，字节顺序：高位在前，低位在后
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static int BitToInt(this byte[] bytes)
        {
            int temp;
            temp = BitConverter.ToInt32(bytes, 0);//将字节数组内容再转成int32类型
            return temp;
        }

        public static int BitToInt(this byte[] bytes, int startIndex, int length)
        {
            int temp;
            byte[] buffer = new byte[length];
            Array.Copy(bytes, startIndex, buffer, 0, length);
            temp = BitConverter.ToInt32(buffer, 0);//将字节数组内容再转成int32类型
            return temp;
        }
        /// <summary>
        /// 将传入的value转换成2个byte字节，bytes[0]为高位，bytes[1]为低位，
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ShortToBytes(short value)
        {
            byte[] result = new byte[2];
            result[0] = (byte)((value & 0xFF00) >> 8);
            result[1] = (byte)(value & 0x00FF);
            return result;
        }
        /// <summary>
        /// 将ushort转换成2个字节，例如5转换成bs[0]=0,bs[5]=5
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] UShortToBytes(ushort value)
        {
            byte[] result = new byte[2];
            result[0] = (byte)((value & 0xFF00) >> 8);
            result[1] = (byte)(value & 0x00FF);
            return result;
        }

        public static short BytesToShort(byte[] bytes, int start)
        {
            return (short)((bytes[start] << 8) + bytes[start + 1]);
        }
        public static ushort BytesToUShort(byte[] bytes, int start)
        {
            return (ushort)((bytes[start] << 8) + bytes[start + 1]);
        }
        /// <summary>
        /// 将bytes[]数组转成float，转入的数组长度必须是4
        /// </summary>
        /// <param name="bytes">要转换数组，长度必须是4</param>
        /// <returns></returns>
        public static float BytesToFloat(byte[] bytes)
        {
            return BitConverter.ToSingle(bytes, 0);//直接转换成单精度
        }
        /// <summary>
        /// 将bytes[]数组转成float，转入的数组长度必须是4
        /// </summary>
        /// <param name="bytes">要转换的数据</param>
        /// <param name="blReverse">是否字节顺序去反，true为取反，原先bytes[0]变成bytes[3]</param>
        /// <returns></returns>
        public static float BytesToFloat(byte[] bytes, bool blReverse)
        {
            if (blReverse)
            {
                byte[] bsNew = new byte[bytes.Length];
                for (int i = 0; i < bytes.Length; i++)
                {
                    bsNew[i] = bytes[bytes.Length - i - 1];
                }
                return BytesToFloat(bsNew);
            }
            else return BytesToFloat(bytes);//直接转换成单精度
        }
        /// <summary>
        /// 将bytes[]数组转成double，转入的数组长度必须是8
        /// </summary>
        /// <param name="bytes">要转换数组，长度必须是8</param>
        /// <returns></returns>
        public static double BytesToDouble(byte[] bytes)
        {
            return BitConverter.ToDouble(bytes, 0);//直接转换成单精度
        }
        public static double BytesToDouble(byte[] bytes, bool blReverse)
        {
            if (blReverse)
            {
                byte[] bsNew = new byte[bytes.Length];
                for (int i = 0; i < bytes.Length; i++)
                {
                    bsNew[i] = bytes[bytes.Length - i - 1];
                }
                return BytesToDouble(bsNew);
            }
            else return BytesToDouble(bytes);//直接转换成双精度
        }
        #endregion
        /// <summary>
        /// byte转换成二进制字符串
        /// </summary>
        /// <param name="data">要转换的字节</param>
        /// <returns>传入参数对应的8位二进制字符串，高位在前</returns>
        public static string ByteToBinString8(this byte data)
        {
            return Convert.ToString(data, 2).PadLeft(8, '0');
        }
        /// <summary>
        /// 将二进制字符串转换成byte
        /// </summary>
        /// <param name="sData">二进制字符串，长度最多8位</param>
        /// <returns>传入参数对应的字节值</returns>
        public static byte BinStringToByte(string sData)
        {
            return Convert.ToByte(sData, 2);
        }

        /// <summary>
        /// 获取字符中用/分割后的第N个字符，
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="idx">序号，小于0时是从后面数</param>
        /// <returns></returns>
        public static string GetTopicItem(string topic, int idx)
        {
            if (string.IsNullOrWhiteSpace(topic))
                return "";
            var arr = topic.Split('/');
            if (idx >= 0 && arr.Length > idx)
                return arr[idx];
            else if(idx < 0 && arr.Length + idx>=0)
                return arr[arr.Length + idx];
            return "";
        }

    }
}
