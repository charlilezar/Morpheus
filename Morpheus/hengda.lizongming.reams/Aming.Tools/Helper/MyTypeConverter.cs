using System;
using System.ComponentModel;

namespace Aming.Core
{
    /// <summary>
    ///     类型转换器,在泛型类时有用
    /// </summary>
    public static class MyTypeConverter
    {
        /// <summary>
        ///     是否可以转换？
        /// </summary>
        /// <param name="fromTp"></param>
        /// <param name="toTp"></param>
        /// <returns>True:可以转换</returns>
        public static bool CanConvert(Type fromTp, Type toTp)
        {
            var tc = TypeDescriptor.GetConverter(fromTp);
            return tc.CanConvertFrom(toTp);
        }

        /// <summary>
        ///     类型转换。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="tp"></param>
        /// <returns></returns>
        public static object ChangeType(object obj, Type tp)
        {
            var tc = TypeDescriptor.GetConverter(tp);
            if (obj == null || obj == DBNull.Value) return null;
            object innerConvertVal;
            if (TryConvertKnowType(obj, tp, out innerConvertVal)) return innerConvertVal;
            if (tc.CanConvertFrom(obj.GetType()))
                return tc.ConvertFrom(obj);
            return Convert.ChangeType(obj, tp);
        }

        /// <summary>
        ///     转换。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ChangeType<T>(object value)
        {
            return (T) ChangeType(value, typeof(T));
        }

        /// <summary>
        ///     注册一个转换类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TC"></typeparam>
        public static void RegisterTypeConverter<T, TC>() where TC : TypeConverter
        {
            TypeDescriptor.AddAttributes(typeof(T), new TypeConverterAttribute(typeof(TC)));
        }

        /// <summary>
        ///     尝试转换基本类型
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="tp"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        private static bool TryConvertKnowType(object obj, Type tp, out object val)
        {
            var realTp = Nullable.GetUnderlyingType(tp) ?? tp;
            val = null;
            if (realTp == typeof(short))
            {
                val = NumberUtil.ToShort(obj);
                return true;
            }

            if (realTp == typeof(int))
            {
                val = NumberUtil.ToInt(obj);
                return true;
            }

            if (realTp == typeof(float))
            {
                val = NumberUtil.ToFloat(obj);
                return true;
            }

            if (realTp == typeof(double))
            {
                val = NumberUtil.ToDouble(obj);
                return true;
            }

            if (realTp == typeof(decimal))
            {
                val = NumberUtil.ToDecimal(obj);
                return true;
            }

            return false;
        }
    }
}