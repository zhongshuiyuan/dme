using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework
{
    /// <summary>
    /// 算法接口，标识算法
    /// </summary>
    public interface IAlgorithm
    {
        /// <summary>
        /// 获取公式唯一标识
        /// </summary>
        /// <returns></returns>
        String SysCode { get; }
        /// <summary>
        /// 获取输入参数元数据
        /// </summary>
        /// <returns></returns>
        Object GetInParametersMetadata();
        /// <summary>
        /// 获取输出参数元数据
        /// </summary>
        /// <returns></returns>
        Object GetOutParametersMetadata();
        /// <summary>
        /// 执行
        /// </summary>
        void Execute();
    }
}
