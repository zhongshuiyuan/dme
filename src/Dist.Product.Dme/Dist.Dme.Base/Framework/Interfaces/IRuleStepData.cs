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
        /// <summary>
        /// 获取相同模型下其他步骤的属性值信息
        /// </summary>
        /// <param name="otherStepName">其它步骤名称</param>
        /// <param name="attributeCode">属性编码</param>
        /// <returns></returns>
        Property GetStepAttributeValue(string otherStepName, string attributeCode);
    }
}
