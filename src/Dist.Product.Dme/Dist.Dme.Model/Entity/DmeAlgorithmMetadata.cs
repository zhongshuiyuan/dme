using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.Entity
{
    [SugarTable("DME_ALGORITHM_METADATA")]
    public class DmeAlgorithmMetadata
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID", OracleSequenceName = "SEQ_DME_ALGORITHM_METADATA")]
        public int Id { get; set; }
        public String Name { get; set; }
        public String Code { get; set; }
        public int Type { get; set; }
        public String Inout { get; set; }
        [SugarColumn(ColumnName = "ALGORITHM_ID")]
        public int AlgorithmId { get; set; }
    }
}
