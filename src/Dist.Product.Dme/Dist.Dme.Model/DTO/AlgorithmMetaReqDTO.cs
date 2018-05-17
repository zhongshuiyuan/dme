using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 算法元数据请求DTO
    /// </summary>
    public class AlgorithmMetaReqDTO
    {
        [Required]
        public String Name { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        [Required]
        public int DataType { get; set; }
        /// <summary>
        /// 输入和输出值，值：in、out和
        /// </summary>
        [Required]
        public String Inout { get; set; }
        /// <summary>
        /// 是否可见，1：可见；0：不可见
        /// </summary>
        public int IsVisible { get; set; } = 1; // C# 6.0默认值用法
        /// <summary>
        /// 备注信息
        /// </summary>
        public String Remark { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        public String Alias { get; set; }
        /// <summary>
        /// 是否只读。1：只读；0：可编辑
        /// </summary>
        public int ReadOnly { get; set; } = 0;
    }
}
