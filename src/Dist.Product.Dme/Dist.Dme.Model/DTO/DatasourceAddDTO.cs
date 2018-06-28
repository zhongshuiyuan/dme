using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 数据源添加DTO
    /// </summary>
    public class DatasourceAddDTO
    {
        [Required]
        public String Name { get; set; }
        // public int IsLocal { get; set; }
        [Required]
        public String Type { get; set; }
        [Required]
        public String Connection { get; set; }
        public long CreateTime { get; set; }
        public String Remark { get; set; }
    }
}
