﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model
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
        public int AlgorithmId { get; set; }
        [SugarColumn(ColumnName = "GUI_LOCATION_X")]
        public int GuiLocationX { get; set; }
        [SugarColumn(ColumnName = "GUI_LOCATION_Y")]
        public int GuiLocationY { get; set; }
        public int StepTypeId { get; set; }
    }
}