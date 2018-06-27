using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.Entity
{
    [SugarTable("DME_DATASOURCE")]
    public class DmeDataSource
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID", OracleSequenceName = "SEQ_DME_DATASOURCE")]
        public int Id { get; set; }
        public String SysCode { get; set; }
        public String Name { get; set; }
        public String Type { get; set; }
        public String Connection { get; set; }
        public long CreateTime { get; set; }
        public String Remark { get; set; }
    }
}
