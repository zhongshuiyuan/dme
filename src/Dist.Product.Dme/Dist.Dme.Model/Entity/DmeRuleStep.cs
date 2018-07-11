using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.Entity
{
    [SugarTable("DME_RULESTEP")]
    public class DmeRuleStep
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID", OracleSequenceName = "SEQ_DME_RULESTEP")]
        public int Id { get; set; }
        public String SysCode { get; set; }
        [SugarColumn(ColumnName = "MODEL_ID")]
        public int ModelId { get; set; }
        [SugarColumn(ColumnName = "VERSION_ID")]
        public int VersionId { get; set; }
        [SugarColumn(ColumnName = "GUI_LOCATION_X")]
        public double X { get; set; }
        [SugarColumn(ColumnName = "GUI_LOCATION_Y")]
        public double Y { get; set; }
        [SugarColumn(ColumnName = "STEP_TYPE_ID")]
        public int StepTypeId { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
    }
}
