using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    public class AlgorithmMetaDTO
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public int DataType { get; set; }
        public string DataTypeDesc { get; set; }
        public string DataTypeCode { get; set; }
        public String Inout { get; set; }
        public int AlgorithmId { get; set; }
        public int IsVisible { get; set; }
        public String Remark { get; set; }
        public String Alias { get; set; }
        /// <summary>
        /// 是否只读。1：只读；0：可编辑
        /// </summary>
        public int ReadOnly { get; set; }
        /// <summary>
        /// 是否必须。1：必需；0：可选
        /// </summary>
        public int Required { get; set; }
    }
}
