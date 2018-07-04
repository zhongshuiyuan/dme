using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Dist.Dme.RuleSteps
{
    /// <summary>
    /// 步骤类型
    /// </summary>
    public enum EnumRuleStepTypes
    {
        [Description("选择已注册的算法，配置算法参数")]
        [DisplayName("算法输入")]
        AlgorithmInput,
        [Description("数据源输入，选择数据源")]
        [DisplayName("数据源输入")]
        DataSourceInput
    }
}
