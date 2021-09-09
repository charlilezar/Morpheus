using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.ComponentModel;
using System.Reflection;


namespace Aming.Core
{


    public static class EnumHelper
    {
        /// <summary>
        /// 寻找一个Enum值是否包括在enumValues中,(可以是多值)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumItem"></param>
        /// <param name="enumValues"></param>
        /// <returns></returns>
        public static bool InFlag(this Enum enumItem, int enumValues)
        {
            Type enumType = enumItem.GetType();
            var value = Enum.ToObject(enumType, enumValues) as Enum;
            return value.HasFlag(enumItem);
        }

        /// <summary>
        /// 返回枚举项的描述信息。
        /// </summary>
        /// <param name="value">要获取描述信息的枚举项。</param>
        /// <param name="isTop">获取整个枚举类的描述。</param>
        /// <returns>枚举想的描述信息。</returns>
        public static string GetDescription(this Enum value, bool isTop = false)
        {
            Type enumType = value.GetType();
            DescriptionAttribute attr = null;
            if (isTop)
            {
                attr = (DescriptionAttribute)Attribute.GetCustomAttribute(enumType, typeof(DescriptionAttribute));
            }
            else
            {
                // 获取枚举常数名称。
                string name = Enum.GetName(enumType, value);
                if (name != null)
                {
                    // 获取枚举字段。
                    FieldInfo fieldInfo = enumType.GetField(name);
                    if (fieldInfo != null)
                    {
                        // 获取描述的属性。
                        attr = Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute), false) as DescriptionAttribute;
                    }
                }
            }

            if (attr != null && !string.IsNullOrEmpty(attr.Description))
                return attr.Description;
            else
            {
                //return string.Empty;
                return Enum.GetName(enumType, value).Replace('_', ' ');
            }
        }

        #region 枚举类转换名值对用来做UI绑定
        /// <summary>
        /// 把一个字符串数组对应成一个从1开始编号的 List<EnumName<Int32>>,好作为绑定数据源
        /// </summary>
        /// <param name="Strings"></param>
        /// <returns></returns>
        public static List<NameValue<Int32>> ParseArray(IEnumerable<string> Strings)
        {
            List<NameValue<Int32>> list = new List<NameValue<Int32>>();
            int i = 0;
            foreach (string o in Strings)
            {
                i++;
                list.Add(new NameValue<Int32>
                {
                    Value = i,
                    Name = o
                });
            }
            return list;
        }

        /// <summary>
        /// 把一个Enum转换成对应值和名称的对象，好作为绑定数据源
        /// </summary>
        /// <returns></returns>
        public static List<NameValue<T>> ParseEnum<T>() where T : Enum
        {
            List<NameValue<T>> list = new List<NameValue<T>>();

            foreach (object o in Enum.GetValues(typeof(T)))
            {
                list.Add(new NameValue<T>
                {
                    Value = (T)o,
                    Name = GetEnumDescription((T)o)
                });
            }

            return list;
        }
        public static List<NameValue<T>> GetEnumListNameValue<T>() where T : Enum
        {
            return ParseEnum<T>();
        }

        public static string GetEnumDescription<T>(T value) where T : Enum
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);
            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                //return value.ToString();
                return Enum.GetName(typeof(T), value).Replace('_', ' ');

        } 
        #endregion

    }
}