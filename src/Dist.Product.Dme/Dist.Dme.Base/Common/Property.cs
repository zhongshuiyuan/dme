using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Common
{
    /// <summary>
    /// 属性
    /// </summary>
    public class Property
    {
        /// <summary>
        /// 名称
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        public String Alias { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public int DataType { get; set; }
        /// <summary>
        /// 属性值
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public object DefaultValue { get; set; }
        /// <summary>
        /// 是否可见，0：不可见；1：可见。可用于在模型级别上显示，供最终用户编辑
        /// </summary>
        public int IsVisible { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>
        public String Remark { get; set; }
        /// <summary>
        /// 是否只读，1：只读；0：可编辑
        /// </summary>
        public int ReadOnly { get; set; }

        public Property(string name, string alias, int dataType, object value, object defaultValue, int isVisible, string remark, int readOnly = 0)
        {
            this.Name = name;
            this.Alias = alias;
            this.DataType = dataType;
            this.Value = value;
            this.DefaultValue = defaultValue;
            this.IsVisible = isVisible;
            this.Remark = remark;
            this.ReadOnly = readOnly;
        }
    }
}
