using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.Entity
{
    [SugarTable("DME_LOG")]
    public class DmeLog
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID", OracleSequenceName = "SEQ_DME_LOG")]
        public int Id { get; set; }
        public string LogType { get; set; }
        public string LogLevel { get; set; }
        public string UserCode { get; set; }
        public long CreateTime { get; set; }
        public string Address { get; set; }
        public string Apps { get; set; }
        public string ObjectType { get; set; }
        public string ObjectId { get; set; }
        public string Remark { get; set; }
    }
}
