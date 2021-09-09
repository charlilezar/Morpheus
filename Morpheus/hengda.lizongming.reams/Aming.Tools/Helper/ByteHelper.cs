using System;
using System.Runtime.InteropServices;
using System.Text;
//using log4net;

namespace Aming.Core
{
    public static partial class ByteHelper
    {
      
        #region 编码转换
        static Encoding gbk = Encoding.GetEncoding("gbk");
        /// <summary>
        /// struct转换为byte[]
        /// </summary>
        /// <param name="structObj"></param>
        /// <returns></returns>
        public static byte[] StructToBytes(object structObj)
        {
            int size = Marshal.SizeOf(structObj);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structObj, buffer, false);
                byte[] bytes = new byte[size];
                Marshal.Copy(buffer, bytes, 0, size);
                return bytes;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        /// <summary>
        /// byte[]转换为struct
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object BytesToStruct(Type type, byte[] bytes, int startIndex, int length)
        {
            int size = Marshal.SizeOf(type);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, startIndex, buffer, Math.Min(length,size));
                return Marshal.PtrToStructure(buffer, type);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
        public static object BytesToStruct(Type type, byte[] bytes)
        {
            return BytesToStruct(type, bytes, 0, bytes.Length);
        }
        #endregion



        }
}
