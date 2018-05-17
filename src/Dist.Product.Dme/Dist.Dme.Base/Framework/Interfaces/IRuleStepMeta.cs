using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework.Interfaces
{
    public interface IRuleStepMeta
    {
        /// <summary>
        /// 规则步骤类型
        /// </summary>
        IRuleStepType RuleStepType { get; }
        void SetDefault();
        void SaveMeta(int stepId, int modelId, int versionId);
    }
}
