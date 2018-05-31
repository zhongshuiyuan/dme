using Dist.Dme.Base.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework
{
    /// <summary>
    /// 规则步骤元数据基类
    /// </summary>
    public class BaseRuleStepMeta
    {
        /// <summary>
        /// 步骤需要的输入参数
        /// </summary>
        protected IDictionary<String, Property> InputParameters { get; set; } = new Dictionary<String, Property>();
    }
}
