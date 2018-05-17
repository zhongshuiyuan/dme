using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework.Interfaces
{
    public interface IRuleStep
    {
        /// <summary>
        /// 执行步骤
        /// </summary>
        /// <param name="stepMeta">步骤配置元数据信息</param>
        /// <param name="stepData">步骤数据</param>
        /// <returns></returns>
        Boolean Process(IRuleStepMeta stepMeta, IRuleStepData stepData);
    }
}
