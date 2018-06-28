using Dist.Dme.Base.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework
{
    public class PropertySetter : IPropertySetter
    {
        private IDictionary<string, object> _dictionary = new Dictionary<string, object>();

        public IDictionary<string, object> GetPropertyDictionary()
        {
            return _dictionary;
        }
        public void GetAllProperties(out IList<string> names, out IList<object> values)
        {
            names = new List<string>();
            values = new List<object>();

            foreach (var item in _dictionary)
            {
                names.Add(item.Key);
                values.Add(item.Value);
            }
        }
        public object GetProperty(string name)
        {
            return this._dictionary[name];
        }

        public bool IsEqual(IPropertySetter propertySetter)
        {
            throw new NotImplementedException();
        }

        public bool IsExist(string name)
        {
            return _dictionary.ContainsKey(name);
        }

        public void RemoveAll()
        {
            this._dictionary.Clear();
        }

        public Boolean RemoveProperty(string name)
        {
            return this._dictionary.Remove(name);
        }

        public void SetProperties(string[] names, object[] values)
        {
            if (null == names || null == values || names.Length != values.Length)
            {
                return;
            }
            for (int i = 0; i < names.Length; i++)
            {
                this._dictionary[names[i]] = values[i];
            }
        }

        public void SetProperty(string name, object value)
        {
            this._dictionary[name] = value;
        }
    }
}
