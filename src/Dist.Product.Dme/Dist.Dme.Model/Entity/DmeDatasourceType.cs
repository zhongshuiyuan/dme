using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.Entity
{
    [SugarTable("DME_DATASOURCE_TYPE")]
    public class DmeDataSourceType
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID", OracleSequenceName = "SEQ_DME_DATASOURCE_TYPE")]
        public int Id { get; set; }
        public String SysCode { get; set; }
        public String Code { get; set; }
        public String Remark { get; set; }
    }
}
