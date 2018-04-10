using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.Entity
{
    [SugarTable("DME_RULESTEP_HOP")]
    public class DmeRuleStepHop
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID", OracleSequenceName = "SEQ_DME_RULESTEP_HOP")]
        public int Id { get; set; }
        [SugarColumn(ColumnName = "MODEL_ID")]
        public int ModelId { get; set; }
        [SugarColumn(ColumnName = "VERSION_ID")]
        public int VersionId { get; set; }
        [SugarColumn(ColumnName = "STEP_FROM_ID")]
        public int StepFromId { get; set; }
        [SugarColumn(ColumnName = "STEP_TO_ID")]
        public int StepToId { get; set; }
        public int Enabled { get; set; }
    }
}
