using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 运行时属性响应DTO
    /// </summary>
    public class AttributeRuntimeRespDTO
    {
        public int ModelId { get; set; }
        public int VersionId { get; set; }
        public int RuleStepId { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string DataTypeCode { get; set; }
        public string DataTypeDesc { get; set; }
    }
}
