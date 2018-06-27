using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework.AlgorithmTypes
{
    /// <summary>
    /// 算法元数据定义对象
    /// </summary>
    public class AlgorithmMetaDefine
    {
        /// <summary>
        /// 程序集，DLL或JAR
        /// </summary>
        public string Assembly { get; set; }
        /// <summary>
        /// 主类
        /// </summary>
        public string MainClass { get; set; }
        /// <summary>
        /// 主方法
        /// </summary>
        public string MainMethod { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }
      
    }
}
