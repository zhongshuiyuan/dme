using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.Entity
{
    [SugarTable("DME_MODEL_TYPE")]
    public class DmeModelType
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID", OracleSequenceName = "SEQ_DME_MODEL_TYPE")]
        public int Id { get; set; }
        public String SysCode { get; set; }
        public String Name { get; set; }
        public long CreateTime { get; set; }
        public long LastTime { get; set; }
    }
}
