using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.Entity
{
    [SugarTable("DME_RULESTEP_TYPE")]
    public class DmeRuleStepType
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID", OracleSequenceName = "SEQ_DME_RULESTEP_TYPE")]
        public int Id { get; set; }
        public String Code { get; set; }
        public String Name { get; set; }
        public String Remark { get; set; }
    }
}
