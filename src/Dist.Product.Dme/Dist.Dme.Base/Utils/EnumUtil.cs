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
