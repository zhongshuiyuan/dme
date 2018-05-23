using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.Entity
{
    [SugarTable("DME_RULESTEP_ATTRIBUTE")]
    public class DmeRuleStepAttribute
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID", OracleSequenceName = "SEQ_DME_RULESTEP_ATTRIBUTE")]
        public int Id { get; set; }
        [SugarColumn(ColumnName = "RULESTEP_ID")]
        public int RuleStepId { get; set; }
        [SugarColumn(ColumnName = "MODEL_ID")]
        public int ModelId { get; set; }
        [SugarColumn(ColumnName = "VERSION_ID")]
        public int VersionId { get; set; }
        [SugarColumn(ColumnName = "ATTRIBUTE_CODE")]
        public String AttributeCode { get; set; }
        [SugarColumn(ColumnName = "ATTRIBUTE_VALUE")]
        public object AttributeValue { get; set; }
    }
}
