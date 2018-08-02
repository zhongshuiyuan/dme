using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 模型添加请求DTO
    /// </summary>
    public class ModelAddReqDTO
    {
        public String SysCode { get; set; }
        [Required(AllowEmptyStrings = false)]
        public String Name { get; set; }
        public String Remark { get; set; }
        /// <summary>
        /// 模型类型编码
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public String TypeCode { get; set; }
        /// <summary>
        /// 模型版本集合
        /// </summary>
        public IList<ModelVersionAddDTO> Versions { get; set; } = new List<ModelVersionAddDTO>();
    }
}
