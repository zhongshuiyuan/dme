using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model
{
    [SugarTable("DME_MODEL")]
    public class DmeModel
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID", OracleSequenceName = "SEQ_DME_MODEL")]
        public int Id { get; set; }
        public String SysCode { get; set; }
        public String Name { get; set; }
        public String Remark { get; set; }
        public String UserCode { get; set; }
    }
}
