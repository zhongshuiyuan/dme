using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model
{
    [SugarTable("DME_JOB")]
    public class DmeJob
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID", OracleSequenceName = "SEQ_DME_JOB")]
        public int Id { get; set; }
        public String SysCode { get; set; }
        public String Name { get; set; }
        /// <summary>
        /// 状态，运行中：running，停止：stop，成功：success，失败：fail
        /// </summary>
        public String Status { get; set; }
        [SugarColumn(ColumnName = "MODEL_ID")]
        public int ModelId { get; set; }
        [SugarColumn(ColumnName = "VERSION_ID")]
        public int VersionId { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastTime { get; set; }
        public String UserCode { get; set; }
    }
}
