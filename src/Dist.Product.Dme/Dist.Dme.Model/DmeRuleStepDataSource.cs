using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model
{
    [SugarTable("DME_RULESTEP_DATASOURCE")]
    public class DmeRuleStepDataSource
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID", OracleSequenceName = "SEQ_DME_RULESTEP_DATASOURCE")]
        public int Id { get; set; }
        [SugarColumn(ColumnName = "RULESTEP_ID")]
        public int RuleStepId { get; set; }
        [SugarColumn(ColumnName = "MODEL_ID")]
        public int ModelId { get; set; }
        [SugarColumn(ColumnName = "VERSION_ID")]
        public int VersionId { get; set; }
        [SugarColumn(ColumnName = "DATASOURCE_ID")]
        public int DataSourceId { get; set; }
       
    }
}
