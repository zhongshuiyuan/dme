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
    }
}
