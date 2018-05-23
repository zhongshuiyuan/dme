using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework.Interfaces
{
    public interface IRuleStepType
    {
        /// <summary>
        /// 步骤类型唯一代码
        /// </summary>
        string Code { get; }
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 备注信息
        /// </summary>
        string Remark { get; }
        object 
    }
}
