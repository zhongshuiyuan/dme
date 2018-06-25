using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 要素类输入信息
    /// </summary>
    public class InputFeatureClassDTO
    {
        /// <summary>
        /// 要素类名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 一定是来源于哪个数据源的
        /// </summary>
     public DataSourceDTO Source { get; set; }
    }
}
