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
        /// 名称
        /// </summary>
        String Name { get; }
        /// <summary>
        /// 别名
        /// </summary>
        String Alias { get; }
        /// <summary>
        /// 版本
        /// </summary>
        String Version { get; }
        /// <summary>
        /// 算法备注信息
        /// </summary>
        String Remark { get; }
        /// <summary>
        /// 获取输入参数元数据
        /// </summary>
        /// <returns></returns>
        Object InParams { get; }
        /// <summary>
        /// 获取输出参数元数据
        /// </summary>
        /// <returns></returns>
        Object OutParams { get; }
        /// <summary>
        /// 特征参数，例如容差值、权重等等
        /// </summary>
        Object FeatureParams { get; }
        /// <summary>
        /// 初始化工作，设置参数
        /// </summary>
        /// <returns></returns>
        void Init(IDictionary<string, object> parameters);
        /// <summary>
        /// 获取算法json元数据信息
        /// </summary>
        /// <returns></returns>
        object MetadataJSON { get; }
        /// <summary>
        /// 执行
        /// </summary>
        Result Execute();
    }
}
