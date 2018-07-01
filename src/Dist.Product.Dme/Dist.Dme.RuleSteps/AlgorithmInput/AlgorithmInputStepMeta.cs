using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework.AlgorithmTypes;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.DAL.Context;
using Dist.Dme.Model.Entity;
using Dist.Dme.RuleSteps;
using Dist.Dme.RuleSteps.AlgorithmInput.DTO;
using Newtonsoft.Json;
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

        public override IRuleStepType RuleStepType => new AlgorithmInputStepType();

        public override string RuleStepName { get; set; } = "算法输入";

        public AlgorithmInputStepMeta(IRepository repository, DmeRuleStep step) 
            : base(repository, step)
        {
        }
        public override object InParams
        {
            get
            {
                // new Property(nameof(this.FeatureClass_Source_First), "总规用地的图层信息", ValueTypeMeta.TYPE_MDB_FEATURECLASS, "", "", 1, "总规用地的图层信息，格式：mdb路径"+ SEPARATOR_FEATURE_PATH + "要素类"))
                return base.InputParameters[nameof(AlgorithmInputStepMeta.AlgorithmCode)] = 
                    new Property(nameof(AlgorithmInputStepMeta.AlgorithmCode), "算法唯一标识符", Common.EnumValueMetaType.TYPE_STRING, "","", "算法唯一标识符，需要选择一个算法。");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public AlgorithmDTO GetAlgorithm()
        {
            DmeRuleStepAttribute dmeRuleStepAttribute = base.repository.GetDbContext().Queryable<DmeRuleStepAttribute>().Single(rsa => rsa.RuleStepId == this.step.Id && rsa.AttributeCode == nameof(this.AlgorithmCode));
            if (null == dmeRuleStepAttribute)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, "没有找到步骤关联的算法");
            }
            
            AlgorithmDTO algorithmDTO = new AlgorithmDTO();
            string algorithmCode = dmeRuleStepAttribute.AttributeValue.ToString();
            algorithmDTO.Algorithm = base.repository.GetDbContext().Queryable<DmeAlgorithm>().Single(alg => alg.SysCode == algorithmCode);
            algorithmDTO.MetaDefine = JsonConvert.DeserializeObject<AlgorithmMetaDefine>(algorithmDTO.Algorithm.Extension.ToString());
            return algorithmDTO;
        }
    }
}
