using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.Entity
{
    [SugarTable("DME_ALGORITHM")]
    public class DmeAlgorithm
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID", OracleSequenceName = "SEQ_DME_ALGORITHM")]
        public int Id { get; set; }
        public String SysCode { get; set; }
        public String Name { get; set; }
        public String Alias { get; set; }
        public String Version { get; set; }
        public int RegisterTime { get; set; }
        public String Remark { get; set; }
        public String UserCode { get; set; }
        public String Path { get; set; }
    }
}
