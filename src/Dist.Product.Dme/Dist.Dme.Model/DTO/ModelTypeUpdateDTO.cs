using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 模型类型更新
    /// </summary>
    public class ModelTypeUpdateDTO
    {
        /// <summary>
        /// 唯一编码
        /// </summary>
        public string SysCode { get; set; }
        /// <summary>
        /// 新名称
        /// </summary>
        public string NewName { get; set; }
    }
}
