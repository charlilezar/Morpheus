using System;
using System.Text;

namespace ddPCR.DriverPlatform.Ins.Mcu
{
    /// <summary>
    /// 这个类封装了类型转换的一些常用方法
    /// </summary>
    public static class ConvertHelper
    {
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
        /// 将字节数组转换成Int32
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static int BitToInt(byte[] bytes)
        {
            int temp;
            temp = BitConverter.ToInt32(bytes, 0);//将字节数组内容再转成int32类型
            return temp;
        }

        public static int BitToInt(byte[] bytes, int startIndex, int length)
        {
            int temp;
            byte[] buffer = new byte[length];
            Array.Copy(bytes, startIndex, buffer, 0, length);
            temp = BitConverter.ToInt32(buffer, 0);//将字节数组内容再转成int32类型
            return temp;
        }

        /// <summary>
        /// 将字符串转为16进制字符，允许中文
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string StringToHexString(string s, Encoding encode, string spanString)
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
        public static string HexStringToString(string hs, Encoding encode)
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
        public static string ByteToHexStr(byte[] bytes)
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

        /// <summary>
        /// 将整个byte数组转为16进制字符串，注意：如果不需要全部转换请使用重载
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string PrintByteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2") + " ";
                    if (i == 7 || i == bytes.Length - 1 - 3) //从长度来加空格分开MCU和下层设备的内容
                        returnStr += " ";
                }
            }
            return returnStr;
        }
        /// <summary>
        /// 将byte数组中指定的前length个字节数组转为16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ByteToHexStr(byte[] bytes, int length)
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

        public static string ByteToHexStr(byte b)
        {
            string returnStr = "";
            returnStr += b.ToString("X2");
            return returnStr;
        }

        /// <summary>
        /// 从指定索引开始，将指定长度的byte[]转为16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ByteToHexStr(byte[] bytes, int startIndex, int length)
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
        public static byte[] HexStrToHexByte(string hexString)
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

        public static byte[] Short2Bytes(short value)
        {
            byte[] result = new byte[2];
            result[0] = (byte)((value & 0xFF00) >> 8);
            result[1] = (byte)(value & 0x00FF);
            return result;
        }

        public static short Bytes2Short(byte[] bytes, int start)
        {
            return (short)((bytes[start] << 8) + bytes[start + 1]);
        }
    }
}