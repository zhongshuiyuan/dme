using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.Entity
{
    [SugarTable("DME_TASK_RULESTEP")]
    public class DmeTaskRuleStep
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID", OracleSequenceName = "SEQ_DME_TASK_RULESTEP")]
        public int Id { get; set; }
        public string SysCode { get; set; }
        [SugarColumn(ColumnName = "TASK_ID")]
        public int TaskId { get; set; }
        [SugarColumn(ColumnName = "RULESTEP_ID")]
        public int RuleStepId { get; set; }
        public string Status { get; set; }
        public long CreateTime { get; set; }
        public long LastTime { get; set; }
    }
}
