using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 模型版本新增简单DTO
    /// </summary>
    public class ModelVersionSimpleAddDTO
    {
        /// <summary>
        /// 模型id
        /// </summary>
        [Required]
        public int ModelId { get; set; }
        /// <summary>
        /// 版本名称
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
    }
}
