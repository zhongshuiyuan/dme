using Dist.Dme.Base.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    public class AttributeReqDTO
    {
        public string Name { get; set; }
        public object Value { get; set; }
        /// <summary>
        /// 数据类型编码
        /// </summary>
        public string DataTypeCode { get; set; }
        public int IsNeedPrecursor { get; set; }
        public string DataSourceCode { get; set; }
        /// <summary>
        /// 类别，0：一般属性；1：运行时属性，对应EnumAttributeType的值
        /// </summary>
        public EnumAttributeType Type { get; set; } = EnumAttributeType.NORMAL;
    }
}
