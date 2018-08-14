using Dist.Dme.Base.Common;
using Dist.Dme.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 规则步骤添加
    /// </summary>
    public class RuleStepAddDTO
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public RuleStepTypeDTO StepType { get; set; }

        /// <summary>
        /// 步骤关联的属性列表
        /// </summary>
        public IList<AttributeReqDTO> Attributes { get; set; } = new List<AttributeReqDTO>();
    }
    public class RuleStepTypeDTO
    {
        public String Code { get; set; }
    }
}
