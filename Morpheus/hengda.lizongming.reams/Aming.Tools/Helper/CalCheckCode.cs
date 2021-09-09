using System;
using System.Text;

namespace ddPCR.DriverPlatform.Ins.Mcu
{
    public class CalCheckCode
    {
        /// <summary>
        /// FCS验证函数公式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Fcs(String value)
        {
            int i, f;
            byte[] x;
            f = 0;
            for (i = 0; i < value.Length; i++)
            {
                x = ASCIIEncoding.ASCII.GetBytes(value.Substring(i, 1));
                f = f ^ x[0];
            }

            return f.ToString("X");
        }

        /// <summary>
        /// 计算和校验码
        /// </summary>
        /// <param name="memorySpage"></param>
        /// <returns></returns>
        public static string GetAddCheckCode(byte[] memorySpage)
        {
            if (memorySpage != null)
            {
                int num = 0;
                for (int i = 0; i < memorySpage.Length; i++)
                {
                    num = (num + memorySpage[i]) % 0xffff;
                }
                string result = String.Format("{0:X4}", num);
                return result;
            }
            return "";
        }

        public class Crc// 基于modbus CRC通信的crc校验循环冗长校验码
        {
            #region CRC16

            public static byte[] Crc16(byte[] data)
            {
                int len = data.Length;
                if (len > 0)
                {
                    ushort crc = 0xFFFF;

                    for (int i = 0; i < len; i++)
                    {
                        crc = (ushort)(crc ^ (data[i]));
                        for (int j = 0; j < 8; j++)
                        {
                            crc = (crc & 1) != 0 ? (ushort)((crc >> 1) ^ 0xA001) : (ushort)(crc >> 1);
                        }
                    }
                    byte hi = (byte)((crc & 0xFF00) >> 8);  //高位置
                    byte lo = (byte)(crc & 0x00FF);         //低位置

                    return new[] { hi, lo };
                }
                return new byte[] { 0, 0 };
            }

            /// <summary>
            /// 计算一个十六进制的字符串的CRC校验值，返回校验码的高字节和低字节
            /// </summary>
            /// <param name="hexString"></param>
            /// <returns></returns>
            public static byte[] Crc16(string hexString)
            {
                byte[] data = ConvertHelper.HexStrToHexByte(hexString);//将十六进制字符串转换为十六进制字节数组
                int len = data.Length;
                if (len > 0)
                {
                    ushort crc = 0xFFFF;

                    for (int i = 0; i < len; i++)
                    {
                        crc = (ushort)(crc ^ (data[i]));
                        for (int j = 0; j < 8; j++)
                        {
                            crc = (crc & 1) != 0 ? (ushort)((crc >> 1) ^ 0xA001) : (ushort)(crc >> 1);
                        }
                    }
                    byte hi = (byte)((crc & 0xFF00) >> 8);  //高位置
                    byte lo = (byte)(crc & 0x00FF);         //低位置

                    return new[] { hi, lo };
                }
                return new byte[] { 0, 0 };
            }

            #endregion CRC16

            #region ToCRC16

            public static string ToCrc16(string content)
            {
                return ToCrc16(content, Encoding.UTF8);
            }

            public static string ToCrc16(string content, bool isReverse)
            {
                return ToCrc16(content, Encoding.UTF8, isReverse);
            }

            public static string ToCrc16(string content, Encoding encoding)
            {
                return ByteToString(Crc16(encoding.GetBytes(content)), true);
            }

            public static string ToCrc16(string content, Encoding encoding, bool isReverse)
            {
                return ByteToString(Crc16(encoding.GetBytes(content)), isReverse);
            }

            public static string ToCrc16(byte[] data)
            {
                return ByteToString(Crc16(data), true);
            }

            public static string ToCrc16(byte[] data, bool isReverse)
            {
                return ByteToString(Crc16(data), isReverse);
            }

            #endregion ToCRC16

            #region ToModbusCRC16

            public static string ToModbusCrc16(string s)
            {
                return ToModbusCrc16(s, true);
            }

            public static string ToModbusCrc16(string s, bool isReverse)
            {
                return ByteToString(Crc16(StringToHexByte(s)), isReverse);
            }
            /// <summary>
            /// 计算校验码
            /// </summary>
            /// <param name="data">原始byte[]</param>
            /// <param name="isReverse">高位低位反转</param>
            /// <returns></returns>
            public static string ToModbusCrc16(byte[] data, bool isReverse)
            {
                return ByteToString(Crc16(data), isReverse);
            }

            #endregion ToModbusCRC16

            #region ByteToString

            public static string ByteToString(byte[] arr, bool isReverse)
            {
                try
                {
                    byte hi = arr[0], lo = arr[1];
                    return Convert.ToString(isReverse ? hi + lo * 0x100 : hi * 0x100 + lo, 16).ToUpper().PadLeft(4, '0');
                }
                catch (Exception ex) { throw (ex); }
            }

            public static string ByteToString(byte[] arr)
            {
                try
                {
                    return ByteToString(arr, true);
                }
                catch (Exception ex) { throw (ex); }
            }

            #endregion ByteToString

            #region StringToHexString

            public static string StringToHexString(string str)
            {
                StringBuilder s = new StringBuilder();
                foreach (short c in str.ToCharArray())
                {
                    s.Append(c.ToString("X4"));
                }
                return s.ToString();
            }

            #endregion StringToHexString

            #region StringToHexByte

            private static string ConvertChinese(string str)
            {
                StringBuilder s = new StringBuilder();
                foreach (short c in str.ToCharArray())
                {
                    if (c <= 0 || c >= 127)
                    {
                        s.Append(c.ToString("X4"));
                    }
                    else
                    {
                        s.Append((char)c);
                    }
                }
                return s.ToString();
            }

            private static string FilterChinese(string str)
            {
                StringBuilder s = new StringBuilder();
                foreach (short c in str.ToCharArray())
                {
                    if (c > 0 && c < 127)
                    {
                        s.Append((char)c);
                    }
                }
                return s.ToString();
            }

            /// <summary>
            /// 字符串转16进制字符数组
            /// </summary>
            /// <param name="hex"></param>
            /// <returns></returns>
            public static byte[] StringToHexByte(string str)
            {
                return StringToHexByte(str, false);
            }

            /// <summary>
            /// 字符串转16进制字符数组
            /// </summary>
            /// <param name="str"></param>
            /// <param name="isFilterChinese">是否过滤掉中文字符</param>
            /// <returns></returns>
            public static byte[] StringToHexByte(string str, bool isFilterChinese)
            {
                string hex = isFilterChinese ? FilterChinese(str) : ConvertChinese(str);

                //清除所有空格
                hex = hex.Replace(" ", "");
                //若字符个数为奇数，补一个0
                hex += hex.Length % 2 != 0 ? "0" : "";

                byte[] result = new byte[hex.Length / 2];
                for (int i = 0, c = result.Length; i < c; i++)
                {
                    result[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
                }
                return result;
            }

            #endregion StringToHexByte
        }

        /// <summary>
        /// 输入为字节格式
        /// </summary>
        /// <param name="auchMsg"></param>
        /// <returns></returns>返回值为byte十进制如果需要可以转换为十六进制
        public static string Lrc(byte[] auchMsg)
        {
            byte uchLrc = 0;
            foreach (byte item in auchMsg)
            {
                uchLrc += item;
            }
            return Convert.ToString((byte)((uchLrc ^ 0xFF) + 1), 16).ToUpper();
        }

        /// <summary>
        /// 输入为字符串
        /// </summary>
        /// <param name="auchMsg"></param>
        /// <returns></returns>返回值为byte十进制如果需要可以转换为十六进制
        public static string Lrc(string auchMsgStr)
        {
            return Lrc(ConvertHelper.HexStrToHexByte(auchMsgStr));
        }

        /// <summary>
        /// BCC异或校验
        /// </summary>
        /// <param name="cmdString">命令字符串</param>
        /// <returns></returns>
        public static string Bcc(string cmdString)
        {
            //BCC寄存器
            int bccCode = 0;
            //将字符串拆分成为16进制字节数据然后两位两位进行异或校验
            for (int i = 1; i < cmdString.Length / 2; i++)
            {
                string cmdHex = cmdString.Substring(i * 2, 2);
                if (i == 1)
                {
                    string cmdPrvHex = cmdString.Substring((i - 1) * 2, 2);
                    bccCode = (byte)Convert.ToInt32(cmdPrvHex, 16) ^ (byte)Convert.ToInt32(cmdHex, 16);
                }
                else
                {
                    bccCode = (byte)bccCode ^ (byte)Convert.ToInt32(cmdHex, 16);
                }
            }
            return Convert.ToString(bccCode, 16).ToUpper().PadLeft(2, '0');//返回16进制校验码
        }
    }
}