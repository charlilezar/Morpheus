using System;
using System.Text;

namespace Aming.Core
{
    public static partial class CrcTools
    {
        /// <summary>
        /// 从指定索引开始，将指定长度的byte[]转为16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte CrcAdd8Low(byte[] bytes, int startIndex, int length)
        {
            int sum = 0;
            //        sum += memorySpage[i];
            //    }
            //    sum = sum & 0xff;
            //    var str = sum.ToString("X");
            if (bytes != null)
            {
                for (int i = startIndex; i < startIndex + length; i++)
                {
                    sum += bytes[i];

                }
            }
            //byte bt = (byte)((cks & 0xff00) >> 8);//取校验和高8位
            byte bt = (byte)(sum & 0xff);//取校验和低8位
            return bt;//.ToString("X2");
        }
        public static byte CrcAdd8Low(byte[] bytes)
        {
            return CrcAdd8Low(bytes, 0, bytes.Length);
        }
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