using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.DAL.Context;
using Dist.Dme.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework.RuleSteps.AlgorithmInput
{
    /// <summary>
    /// 算法输入步骤元数据
    /// </summary>
    public class AlgorithmInputStepMeta : AbstractContext, IRuleStepMeta
    {
        public string RuleStepName
        {
            get
            {
                if (string.IsNullOrEmpty(RuleStepName))
                {
                    return this.RuleStepType.Name;
                }
                return RuleStepName;
            }
            set
            {
                RuleStepName = value;
            }
        }

        public IRuleStepType RuleStepType => new AlgorithmInputStepType();

        public void ReadAttributes(int stepId)
        {
            throw new NotImplementedException();
        }

        public Boolean SaveAttributes( int stepId, int modelId, int modelVersionId, IDictionary<string, object> attributes)
        {
            // 启用事务
            return base.Db.Ado.UseTran<Boolean>(() => 
            {
                // 先清除属性
                int deleteCount = base.Db.Deleteable<DmeRuleStepAttribute>().Where(rsa => rsa.RuleStepId == stepId).ExecuteCommand();
                DmeRuleStepAttribute[] attributesAdded = new DmeRuleStepAttribute[attributes.Count];
                int i = 0;
                foreach (var keyvalue in attributes)
                {
                    DmeRuleStepAttribute rsa = new DmeRuleStepAttribute
                    {
                        RuleStepId = stepId,
                        ModelId = modelId,
                        VersionId = modelVersionId,
                        AttributeCode = keyvalue.Key,
                        AttributeValue = keyvalue.Value
                    };
                    attributesAdded[i] = rsa;
                    i++;
                }
                return base.DmeRuleStepAttributeDb.InsertRange(attributesAdded);
            }).Data;
        }

        public Boolean SaveMeta(int stepId, int modelId, int modelVersionId, double guiLocationX, double guiLocationY, string stepName)
        {
            throw new NotImplementedException();
        }
    }
}
