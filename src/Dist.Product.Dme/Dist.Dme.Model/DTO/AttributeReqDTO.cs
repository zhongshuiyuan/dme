using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    public class AttributeReqDTO
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public string DataTypeCode { get; set; }
        public int IsNeedPrecursor { get; set; }
        public string DataSourceCode { get; set; }
    }
}
