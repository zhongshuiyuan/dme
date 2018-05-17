using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 模型规则步骤添加DTO
    /// </summary>
    public class ModelRuleStepAddReqDTO
    {
        public string SysCode { get; set; }
        [Required]
        public int Modeld { get; set; }
        [Required]
        public int VersionId { get; set; }
        [Required]
        public int StepTypeId { get; set; }
        [Required]
        public string StepName { get; set; }
    }
}
