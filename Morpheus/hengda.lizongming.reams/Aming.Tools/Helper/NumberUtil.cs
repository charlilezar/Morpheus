using System;

namespace Aming.Core
{
    /// <summary>
    ///     数值工具类
    /// </summary>
    internal static class NumberUtil
    {
        /// <summary>
        ///     转换成short
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static short ToShort(object obj, short defVal = 0)
        {
            try
            {
                if (obj == null) return defVal;
                short resShort;
                if (short.TryParse(obj.ToString(), out resShort)) return defVal;

                return resShort;
            }
            catch
            {
                return defVal;
            }
        }

        /// <summary>
        ///     转换成int
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static int ToInt(object obj, int defVal = 0)
        {
            try
            {
                if (obj == null) return defVal;
                int resInt;
                if (!int.TryParse(obj.ToString(), out resInt)) return defVal;
                return resInt;
            }
            catch
            {
                return defVal;
            }
        }

        /// <summary>
        ///     转换成long
        /// </summary>
        /// <param name="obj">オブジェクト</param>
        /// <param name="defVal">失敗した場合の処理</param>
        /// <returns></returns>
        public static long ToLong(object obj, long defVal = 0)
        {
            try
            {
                if (obj == null) return defVal;
                long resLong;
                if (!long.TryParse(obj.ToString(), out resLong)) return defVal;
                return resLong;
            }
            catch
            {
                return defVal;
            }
        }

        /// <summary>
        ///     转换成float
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static float ToFloat(object obj, float defVal = 0f)
        {
            try
            {
                if (obj == null) return defVal;
                float resInt;
                if (!float.TryParse(obj.ToString(), out resInt)) return defVal;
                return resInt;
            }
            catch
            {
                return defVal;
            }
        }

        /// <summary>
        ///     转换成double
        /// </summary>
        /// <param name="obj">オブジェクト</param>
        /// <param name="defVal">失敗した場合の処理</param>
        /// <returns></returns>
        public static double ToDouble(object obj, double defVal = 0.0)
        {
            try
            {
                if (obj == null) return defVal;
                var resDou = defVal;
                if (!double.TryParse(obj.ToString(), out resDou)) return defVal;
                return resDou;
            }
            catch
            {
                return defVal;
            }
        }

        /// <summary>
        ///     转换成decimal
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static decimal ToDecimal(object obj, decimal defVal = 0m)
        {
            try
            {
                if (obj == null) return defVal;
                decimal resDec;
                if (!decimal.TryParse(obj.ToString(), out resDec)) return defVal;
                return resDec;
            }
            catch
            {
                return defVal;
            }
        }

        /// <summary>
        ///     是否是有效的int
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsInt(object obj)
        {
            if (obj == null) return false;
            int intRes;
            return int.TryParse(obj.ToString(), out intRes);
        }

        /// <summary>
        ///     是否是有效的double
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsDouble(object obj)
        {
            if (obj == null) return false;
            double dRes;
            return double.TryParse(obj.ToString(), out dRes);
        }

        /// <summary>
        ///     数值每三位加逗号
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string PadComma(object obj)
        {
            if (obj == null) return "";
            var objStr = obj.ToString();
            if (!IsDouble(obj)) return objStr;
            if (!objStr.Contains(".")) return $"{ToInt(objStr):n0}";

            var pointPlace = objStr.IndexOf(".", StringComparison.Ordinal);
            var strIntPart = objStr.Substring(0, pointPlace);
            var strDecPart = objStr.Substring(pointPlace, objStr.Length - pointPlace);
            return $"{ToInt(strIntPart):n0}" + strDecPart;
        }

        /// <summary>
        ///     计算HashCode
        /// </summary>
        /// <param name="objAry"></param>
        /// <returns></returns>
        public static int ComputeHashCode(params object[] objAry)
        {
            unchecked
            {
                var hash = 17;
                foreach (var obj in objAry)
                {
                    var inc = obj?.GetHashCode() ?? 0;
                    hash = hash * 23 + inc;
                }

                return hash;
            }
        }

        /// <summary>
        ///     数值转换为String
        ///     例：ToIntString("00001") ⇒ "1"
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static string ToIntString(object obj, int defVal = 0)
        {
            return ToInt(obj, defVal).ToString();
        }

        /// <summary>
        ///     数值转换为String。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        public static string ToDoubleString(object obj, double defVal = 0.0)
        {
            return ToDouble(obj, defVal).ToString();
        }

        /// <summary>
        ///     是否是数值型。
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsNumericType(object o)
        {
            if (o == null) return false;

            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;

                default:
                    return false;
            }
        }
    }
}