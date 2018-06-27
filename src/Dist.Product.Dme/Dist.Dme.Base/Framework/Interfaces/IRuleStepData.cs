using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework.Interfaces
{
    /// <summary>
    /// 规则步骤数据
    /// </summary>
    public interface IRuleStepData
    {
        /// <summary>
        /// 步骤运行
        /// </summary>
        /// <returns></returns>
        Result Run();
    }
}
