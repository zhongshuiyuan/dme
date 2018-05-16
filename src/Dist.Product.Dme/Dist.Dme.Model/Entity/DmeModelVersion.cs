using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dist.Dme.Model.Entity
{
    [SugarTable("DME_MODEL_VERSION")]
    public class DmeModelVersion
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID", OracleSequenceName = "SEQ_DME_MODEL_VERSION")]
        public int Id { get; set; }
        [Required]
        public String SysCode { get; set; }
        /// <summary>
        /// 版本名称
        /// </summary>
        public String Name { get; set; }
        [Required]
        [SugarColumn(ColumnName = "MODEL_ID")]
        public int ModelId { get; set; }
        public long CreateTime { get; set; }
        public String UserCode { get; set; }
    }
}
