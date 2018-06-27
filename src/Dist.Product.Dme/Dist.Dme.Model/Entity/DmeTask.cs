using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.Entity
{
    [SugarTable("DME_TASK")]
    public class DmeTask
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID", OracleSequenceName = "SEQ_DME_TASK")]
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
        public long CreateTime { get; set; }
        public long LastTime { get; set; }
    }
}
