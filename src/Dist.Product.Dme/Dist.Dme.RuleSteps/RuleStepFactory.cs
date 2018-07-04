using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.Model.Entity;
using Dist.Dme.RuleSteps.AlgorithmInput;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.RuleSteps
{
    /// <summary>
    /// 简单工厂模式，步骤工厂类
    /// </summary>
    public static class RuleStepFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stepTypeCode">步骤类型唯一编码</param>
        /// <param name="repository"></param>
        /// <param name="taskId"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IRuleStepData GetRuleStepData(string stepTypeCode, IRepository repository, int taskId, DmeRuleStep step)
        {
            IRuleStepData ruleStepData = null;
            EnumRuleStepTypes @enum = EnumUtil.GetEnumObjByName<EnumRuleStepTypes>(stepTypeCode);
            switch (@enum)
            {
                case EnumRuleStepTypes.AlgorithmInput:
                    ruleStepData = new AlgorithmInputStepData(repository, taskId, step);
                    break;
                case EnumRuleStepTypes.DataSourceInput:
                    break;
                default:
                    break;
            }
            return ruleStepData;
        }
    }
}
