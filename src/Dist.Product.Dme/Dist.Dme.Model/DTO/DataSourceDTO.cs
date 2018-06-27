using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 数据源信息
    /// </summary>
    public class DataSourceDTO
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public string SysCode { get; set; }
        /// <summary>
        /// 是否本地
        /// </summary>
        //public int IsLocal { get; set; }
        /// <summary>
        /// 数据源类型，ENTERPRISE_GEODATABASE，PERSONAL_GEODATABASE
        /// </summary>
        public String Type { get; set; }
        /// <summary>
        /// 连接信息
        /// </summary>
        public string Connection { get; set; }
    }
}
