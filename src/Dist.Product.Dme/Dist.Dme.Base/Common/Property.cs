using Dist.Dme.Base.Utils;
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
        /// 数据类型描述
        /// </summary>
        public string DataTypeDesc { get; set; }
        /// <summary>
        /// 数据类型编码
        /// </summary>
        public string DataTypeCode { get; set; }
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
        /// <summary>
        /// 属性值集，提供下拉选择
        /// </summary>
        public object[] ValueSet { get; set; }
        /// <summary>
        /// 是否必须，1：需要；0：可选
        /// </summary>
        public int Required { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="alias">别名</param>
        /// <param name="dataType">数据类型</param>
        /// <param name="value">值</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="remark">备注</param>
        /// <param name="valueSet">值的可选集</param>
        /// <param name="isVisible">是否可见</param>
        /// <param name="readOnly">是否只读</param>
        public Property(string name, string alias, EnumValueMetaType dataType, object value = null, object defaultValue = null, string remark = "", object[] valueSet = null, int isVisible = 1, int readOnly = 0, int required = 1)
        {
            this.Name = name;
            this.Alias = alias;
            this.DataType = (int)dataType;
            this.DataTypeDesc = EnumUtil.GetEnumDescription(dataType);
            this.DataTypeCode = EnumUtil.GetEnumName<EnumValueMetaType>(this.DataType);
            this.Value = value;
            this.DefaultValue = defaultValue;
            this.ValueSet = valueSet;
            this.IsVisible = isVisible;
            this.Remark = remark;
            this.ReadOnly = readOnly;
            this.Required = required;
        }
    }
}
