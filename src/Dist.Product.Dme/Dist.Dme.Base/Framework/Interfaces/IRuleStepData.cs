using Dist.Dme.Base.Common;
using System.Collections.Generic;

namespace Dist.Dme.Base.Framework.Interfaces
{
    /// <summary>
    /// 规则步骤数据
    /// </summary>
    public interface IRuleStepData
    {
        /// <summary>
        /// 获取步骤元数据
        /// </summary>
        IRuleStepMeta RuleStepMeta { get; }
        /// <summary>
        /// 步骤异步运行
        /// </summary>
        /// <returns></returns>
        Result Run();
        /// <summary>
        /// 保存步骤元数据属性
        /// </summary>
        /// <param name="attributes">属性key-value</param>
        /// <returns></returns>
        bool SaveAttributes(IDictionary<string, Property> attributes);
    }
}
