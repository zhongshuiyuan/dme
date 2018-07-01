using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 所有规则步骤信息
    /// </summary>
    public class ModelRuleStepInfoDTO
    {
        /// <summary>
        /// 模型版本唯一编码
        /// </summary>
        [Required]
        public string ModelVersionCode { get; set; }
        /// <summary>
        /// 规则步骤信息
        /// </summary>
        public IList<AtomicRuleStepInfoDTO> RuleSteps { get; set; }
        /// <summary>
        /// 步骤的向量
        /// </summary>
        public IList<RuleStepHopDTO> Vectors { get; set; }
    }
    /// <summary>
    /// 原子步骤信息
    /// </summary>
    public class AtomicRuleStepInfoDTO
    {
        /// <summary>
        /// 在一个模型内部不重复的key
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// gui界面坐标X值
        /// </summary>
        [Required]
        public double X { get; set; }
        /// <summary>
        /// gui界面坐标Y值
        /// </summary>
        [Required]
        public double Y { get; set; }
        /// <summary>
        /// 步骤类型编码
        /// </summary>
        [Required]
        public string TypeCode { get; set; }
        /// <summary>
        /// 步骤名称
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 步骤属性值清单，键值对
        /// </summary>
        public IDictionary<string, object> Attributes { get; set; }
    }
    /// <summary>
    /// 步骤的向量信息
    /// </summary>
    public class RuleStepHopDTO
    {
        /// <summary>
        /// 步骤的源，步骤的key
        /// </summary>
        public string FromKey { get; set; }
        /// <summary>
        /// 步骤的目标，步骤的key
        /// </summary>
        public string ToKey { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public int Enabled { get; set; }
    }
}
