using Dist.Dme.Base.Common;
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
    public class AlgorithmInputStepMeta : BaseRuleStepMeta, IRuleStepMeta
    {
        /// <summary>
        /// 算法唯一编码
        /// </summary>
        private string AlgorithmCode { get; set; }

        public IRuleStepType RuleStepType => new AlgorithmInputStepType();

        public string RuleStepName { get; set; } = "算法输入";
      
        public object InParams
        {
            get
            {
                // new Property(nameof(this.FeatureClass_Source_First), "总规用地的图层信息", ValueTypeMeta.TYPE_MDB_FEATURECLASS, "", "", 1, "总规用地的图层信息，格式：mdb路径"+ SEPARATOR_FEATURE_PATH + "要素类"))
                return base.InputParameters[nameof(AlgorithmCode)] = new Property(nameof(AlgorithmCode), "算法唯一标识符", ValueTypeMeta.TYPE_STRING, "","", 1, "算法唯一标识符，需要选择一个算法。");
            }
        }

        public void ReadAttributes(IRepository repository, int stepId)
        {
            throw new NotImplementedException();
        }

        public Boolean SaveAttributes(IRepository repository, int stepId, int modelId, int modelVersionId, IDictionary<string, object> attributes)
        {
            // 启用事务
            return repository.GetDbContext().Ado.UseTran<Boolean>(() => 
            {
                // 先清除属性
                int deleteCount = repository.GetDbContext().Deleteable<DmeRuleStepAttribute>().Where(rsa => rsa.RuleStepId == stepId).ExecuteCommand();
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
                return repository.GetDbContext().Insertable<DmeRuleStepAttribute>(attributesAdded).ExecuteCommand() >0 ;
            }).Data;
        }

        public bool SaveMeta(IRepository repository, int stepId, int modelId, int modelVersionId, double guiLocationX, double guiLocationY, string stepName)
        {
            throw new NotImplementedException();
        }
    }
}
