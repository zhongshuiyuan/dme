using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.Entity
{
    [SugarTable("DME_VERSION")]
    public class DmeVersion
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID", OracleSequenceName = "SEQ_DME_VERSION")]
        public int Id { get; set; }
        [SugarColumn(ColumnName = "MAJOR_VERSION")]
        public int MajorVersion { get; set; }
        [SugarColumn(ColumnName = "MINOR_VERSION")]
        public int MinorVersion { get; set; }
        [SugarColumn(ColumnName = "REVISION_VERSION")]
        public int RevisionVersion { get; set; }
        [SugarColumn(ColumnName = "UPGRADE_TIME")]
        public long UpgradeTime { get; set; }
    }
}
