using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework.Interfaces
{
    /// <summary>
    /// 属性设置器
    /// </summary>
    public interface IPropertySetter
    {
        IDictionary<string, object> GetPropertyDictionary();
        object GetProperty(string name);
        void GetAllProperties(out IList<string> names, out IList<object> values);
        void SetProperty(string name, object value);
        void SetProperties(string[] names, object[] values);
        bool IsEqual(IPropertySetter propertySetter);
        bool IsExist(string name);
        Boolean RemoveProperty(string name);
        void RemoveAll();
    }
}
