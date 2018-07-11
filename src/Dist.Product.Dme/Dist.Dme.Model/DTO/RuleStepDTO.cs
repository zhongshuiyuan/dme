using Dist.Dme.Base.Common;
using Dist.Dme.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 规则步骤
    /// </summary>
    public class RuleStepDTO
    {
        // public int Id { get; set; }
        public String SysCode { get; set; }
        // public int ModelId { get; set; }
        // public int VersionId { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public DmeRuleStepType StepType { get; set; }
   
        /// <summary>
        /// 步骤关联的属性键值对
        /// </summary>
        public IList<Property> Attributes { get; set; } = new List<Property>();
    }
}
