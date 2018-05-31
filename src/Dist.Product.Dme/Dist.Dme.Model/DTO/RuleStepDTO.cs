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
        public int Id { get; set; }
        public String SysCode { get; set; }
        public int ModelId { get; set; }
        public int VersionId { get; set; }
        public double GuiLocationX { get; set; }
        public double GuiLocationY { get; set; }
        public DmeRuleStepType StepType { get; set; }
        // public int StepTypeId { get; set; }

        /// <summary>
        /// 步骤关联的属性值
        /// </summary>
        public IList<DmeRuleStepAttribute> RuleStepAttributes { get; set; } = new List<DmeRuleStepAttribute>();
    }
}
