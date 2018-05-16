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
        /// 键
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }

        public AlgorithmMetaDefine()
        {
        }
        public AlgorithmMetaDefine(string key, string value, string desc)
        {
            this.Key = key;
            this.Value = value;
            this.Desc = desc;
        }
    }
}
