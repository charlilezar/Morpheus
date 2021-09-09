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
        public static ushort ModBusCRC16(byte[] bytes)
        {
            ushort value;
            ushort newLoad = 0xffff, In_value;
            int count = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                value = (ushort)bytes[i];
                newLoad = (ushort)(Convert.ToInt32(value) ^ Convert.ToInt32(newLoad));
                In_value = 0xA001;
                while (count < 8)
                {
                    if (Convert.ToInt32(newLoad) % 2 == 1)//判断最低位是否为1
                    {
                        newLoad -= 0x00001;
                        newLoad = (ushort)(Convert.ToInt32(newLoad) / 2);//右移一位
                        count++;//计数器加一
                        newLoad = (ushort)(Convert.ToInt32(newLoad) ^ Convert.ToInt32(In_value));//异或操作
                    }
                    else
                    {
                        newLoad = (ushort)(Convert.ToInt32(newLoad) / 2);//右移一位
                        count++;//计数器加一
                    }
                }
                count = 0;
            }
            return newLoad;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] ModBusCRC16Bytes(byte[] bytes)
        {
            ushort value = ModBusCRC16(bytes);
            byte[] result = new byte[2];
            result[0] = (byte)(value & 0x00FF);
            result[1] = (byte)((value & 0xFF00) >> 8);
            return result;
        }
    }
}