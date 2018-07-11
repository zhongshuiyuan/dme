using Dist.Dme.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 模型版本添加
    /// </summary>
    public class ModelVersionAddDTO
    {
        public string Name { get; set; }
        /// <summary>
        /// 模型版本关联的步骤
        /// </summary>
        public IList<RuleStepAddDTO> Steps { get; set; } = new List<RuleStepAddDTO>();
        /// <summary>
        /// 节点与节点连接信息
        /// </summary>
        public IList<RuleStepHopDTO> Hops { get; set; } = new List<RuleStepHopDTO>();
    }
}
