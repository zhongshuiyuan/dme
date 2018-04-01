using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model
{
    [SugarTable("DME_DATABASETYPE")]
    public class DmeDatabaseType
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID", OracleSequenceName = "SEQ_DME_DATABASETYPE")]
        public int Id { get; set; }
        public String SysCode { get; set; }
        public String Code { get; set; }
        public String Remark { get; set; }
    }
}
