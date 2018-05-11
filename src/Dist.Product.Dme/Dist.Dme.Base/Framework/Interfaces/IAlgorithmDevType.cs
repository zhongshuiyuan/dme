using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework.Interfaces
{
    /// <summary>
    /// 算法开发类型接口
    /// </summary>
    public interface IAlgorithmDevType
    {
        /// <summary>
        ///  编码
        /// </summary>
        String Code { get;}
        /// <summary>
        ///  名称/描述
        /// </summary>
        String Name { get;}
        /// <summary>
        /// 定义信息
        /// 表示这个算法类型，应该包括什么元数据信息
        /// 格式：key-value，键值-描述
        /// 如DLL，则Metadata= 
        /// { 
        ///   "assembly":"DLL库名称"，     
        ///   "mainClass":"主类名称",      
        ///   "mainMethod":"主方法", 
        ///   "path":"DLL路径"               
        ///    .......
        /// }
        /// </summary>
        object Metadata { get;}
    }
}
