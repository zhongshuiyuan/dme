using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Service.Interfaces
{
    /// <summary>
    /// 模型服务
    /// </summary>
    public interface IModelService
    {
        /// <summary>
        /// 获取用地冲突分析输入参数
        /// </summary>
        /// <returns></returns>
        object GetLandConflictInputParameters();
        /// <summary>
        /// 获取用地冲突分析输出参数
        /// </summary>
        /// <returns></returns>
        object GetLandConflictOutputParameters();
        /// <summary>
        /// 用地冲突分析计算
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        object LandConflictExecute(IDictionary<String, Object> parameters);
    }
}
