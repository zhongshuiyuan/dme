using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.Entity
{
    /// <summary>
    /// 任务计算结果
    /// </summary>
    [SugarTable("DME_TASK_RESULT")]
    public class DmeTaskResult
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID", OracleSequenceName = "SEQ_DME_TASK_RESULT")]
        public int Id { get; set; }
        [SugarColumn(ColumnName = "TASK_ID")]
        public int TaskId { get; set; }
        [SugarColumn(ColumnName = "RULESTEP_ID")]
        public int RuleStepId { get; set; }
        [SugarColumn(ColumnName = "R_CODE")]
        public String ResultCode { get; set; }
        [SugarColumn(ColumnName = "R_TYPE")]
        public String ResultType { get; set; }
        [SugarColumn(ColumnName = "R_VALUE")]
        public object ResultValue { get; set; }
    }
}
