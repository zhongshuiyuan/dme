using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 数据源连接信息
    /// </summary>
    public class DataSourceConnDTO
    {
        /// <summary>
        /// 类型编码
        /// </summary>
        [Required]
        public string TypeCode { get; set; }
        /// <summary>
        /// 连接串
        /// </summary>
        [Required]
        public string Connection { get; set; }
    }
}
