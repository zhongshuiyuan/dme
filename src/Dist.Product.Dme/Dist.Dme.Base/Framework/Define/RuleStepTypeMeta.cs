using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework.Define
{
    /// <summary>
    /// 步骤类型元数据
    /// </summary>
    public class RuleStepTypeMeta
    {
        /// <summary>
        /// 唯一名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 分组，类别
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 显式名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
