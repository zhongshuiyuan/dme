using Dist.Dme.Model.Entity;
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
        /// <summary>
        /// 关联的数据源唯一编码
        /// </summary>
        public IList<string> DataSources { get; set; } = new List<string>();
        /// <summary>
        /// 节点与节点连接信息
        /// </summary>
        public IList<DmeRuleStepHop> Hops { get; set; } = new List<DmeRuleStepHop>();
    }
}
