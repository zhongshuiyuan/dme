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
        /// 类型
        /// </summary>
        public int Type { get; set; }
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

        public Property(string name, int type, object value, object defaultValue, int isVisible, string remark)
        {
            this.Name = name;
            this.Type = type;
            this.Value = value;
            this.DefaultValue = defaultValue;
            this.IsVisible = isVisible;
            this.Remark = remark;
        }
    }
}
