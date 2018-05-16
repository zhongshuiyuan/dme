using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Dist.Dme.Base.Utils
{
    /// <summary>
    /// 类属性/字段的值复制工具
    /// </summary>
    public class ClassValueCopier<T>
    {
        /// <summary>
        /// 拷贝对象的属性值
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public static T Copy(object source)
        {
            // T destination = (T)Activator.CreateInstance(typeof(T));// CreationHelper<T>.New();
            return Copy(source, null);
        }
        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="source">来源</param>
        /// <param name="excludeName">排除下列名称的属性不要复制</param>
        /// <returns>成功复制的值个数</returns>
        public static T Copy(object source, IEnumerable<string> excludeName)
        {
            if (source == null)
            {
                return default(T);
            }
            if (excludeName == null)
            {
                excludeName = new List<string>();
            }
            Type desType = typeof(T);
            Type sourceType = source.GetType();
            T destination = (T)Activator.CreateInstance(typeof(T));
            foreach (FieldInfo mi in desType.GetFields())
            {
                if (excludeName.Contains(mi.Name))
                {
                    continue;
                }
                try
                {
                    FieldInfo des = sourceType.GetField(mi.Name);
                    if (des != null && des.FieldType == mi.FieldType)
                    {
                        des.SetValue(destination, mi.GetValue(source));
                    }
                }
                catch
                {
                }
            }
            foreach (PropertyInfo pi in sourceType.GetProperties())
            {
                if (excludeName.Contains(pi.Name))
                {
                    continue;
                }
                try
                {
                    PropertyInfo des = desType.GetProperty(pi.Name);
                    if (des != null && des.PropertyType == pi.PropertyType && des.CanWrite && pi.CanRead)
                    {
                        des.SetValue(destination, pi.GetValue(source, null), null);
                    }
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
            return destination;
        }
    }
}
