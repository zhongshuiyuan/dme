using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.DAL.Context;
using Dist.Dme.Model.Entity;
using Dist.Dme.RuleSteps;
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
      
        public override object InParams
        {
            get
            {
                // new Property(nameof(this.FeatureClass_Source_First), "总规用地的图层信息", ValueTypeMeta.TYPE_MDB_FEATURECLASS, "", "", 1, "总规用地的图层信息，格式：mdb路径"+ SEPARATOR_FEATURE_PATH + "要素类"))
                return base.InputParameters[nameof(AlgorithmCode)] = 
                    new Property(nameof(AlgorithmCode), "算法唯一标识符", ValueTypeEnum.TYPE_STRING, "","", "算法唯一标识符，需要选择一个算法。", null);
            }
        }

    }
}
