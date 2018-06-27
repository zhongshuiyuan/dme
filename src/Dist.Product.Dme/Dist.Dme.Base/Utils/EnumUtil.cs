using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Dist.Dme.Base.Utils
{
    public sealed class EnumUtil
    {
        /// <summary>
        /// 获取一个枚举值的中文描述
        /// </summary>
        /// <param name="enum">枚举值</param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum @enum)
        {
            FieldInfo fi = @enum.GetType().GetField(@enum.ToString());
            DescriptionAttribute[] arrDesc = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return arrDesc[0].Description;
        }
        /// <summary>
        /// 获取枚举显示名称
        /// </summary>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static string GetEnumDisplayName(Enum @enum)
        {
            FieldInfo fi = @enum.GetType().GetField(@enum.ToString());
            DisplayNameAttribute[] arr = (DisplayNameAttribute[])fi.GetCustomAttributes(typeof(DisplayNameAttribute), false);
            return arr[0].DisplayName;
        }
        /// <summary>
        /// 根据值，获取枚举的名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumName<T>(int value)
        {
            return Enum.GetName(typeof(T), value);
        }
        /// <summary>
        /// 根据名称获取枚举对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetEnumObjByName<T>(string name)
        {
            return (T)Enum.Parse(typeof(T), name);
        }
        /// <summary>
        /// 根据值获取枚举对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T GetEnumObjByValue<T>(int value)
        {
            return (T)Enum.Parse(typeof(T), value.ToString());
        }
    }
}
