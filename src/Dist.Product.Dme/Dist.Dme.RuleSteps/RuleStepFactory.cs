using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.Model.Entity;
using Dist.Dme.RuleSteps.AlgorithmInput;
using Dist.Dme.RuleSteps.DataSourceInput;
using Dist.Dme.RuleSteps.MongoDBOutput;
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
        /// 获取规则步骤数据操作类
        /// @TODO 这一块需要独立到配置文件去，否则不便于二次开发
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
                    ruleStepData = new DataSourceInputStepData(repository, taskId, step);
                    break;
                case EnumRuleStepTypes.MongodbOutput:
                    ruleStepData = new MongoDBOutputStepData(repository, taskId, step);
                    break;
                default:
                    break;
            }
            return ruleStepData;
        }
    }
}
