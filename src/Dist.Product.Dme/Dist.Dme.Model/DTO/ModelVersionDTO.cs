using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 模型版本
    /// </summary>
    public class ModelVersionDTO
    {
        public string SysCode { get; set; }
        public string Name { get; set; }
        public long CreateTime { get; set; }
        /// <summary>
        /// 模型版本关联的步骤
        /// </summary>
        public IList<RuleStepDTO> Steps { get; set; } = new List<RuleStepDTO>();
    }
}
