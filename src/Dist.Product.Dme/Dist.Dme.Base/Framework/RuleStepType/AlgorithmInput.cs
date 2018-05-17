using Dist.Dme.Base.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework.RuleStepType
{
    /// <summary>
    /// 算法输入类型
    /// </summary>
    public class AlgorithmInput : IRuleStepType
    {
        public string Code => "AlgorithmInput";

        public string Name => "算法输入";

        public string Remark => "选择已注册的算法，配置算法参数";
    }
}
