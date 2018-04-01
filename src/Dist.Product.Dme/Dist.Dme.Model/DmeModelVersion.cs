using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model
{
    [SugarTable("DME_MODEL_VERSION")]
    public class DmeModelVersion
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID", OracleSequenceName = "SEQ_DME_MODEL_VERSION")]
        public int Id { get; set; }
        public String SysCode { get; set; }
        /// <summary>
        /// 版本名称
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 状态，运行中：running，停止：stop，成功：success，失败：fail
        /// </summary>
        public String Status { get; set; }
        [SugarColumn(ColumnName = "MODEL_ID")]
        public int ModelId { get; set; }
        public DateTime CreateTime { get; set; }
        public String UserCode { get; set; }
    }
}
