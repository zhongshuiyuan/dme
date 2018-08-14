using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 运行时属性设置
    /// </summary>
    public class AttributeRuntimeDTO
    {
        public int RuleStepId { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public string DataSourceCode { get; set; }
    }
}
