using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.Entity
{
    [SugarTable("DME_ALGORITHM_META")]
    public class DmeAlgorithmMeta
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID", OracleSequenceName = "SEQ_DME_ALGORITHM_METADATA")]
        public int Id { get; set; }
        public String Name { get; set; }
        public int DataType { get; set; }
        public String Inout { get; set; }
        [SugarColumn(ColumnName = "ALGORITHM_ID")]
        public int AlgorithmId { get; set; }
        public int IsVisible { get; set; }
        public String Remark { get; set; }
        public String Alias { get; set; }
        /// <summary>
        /// 是否只读。1：只读；0：可编辑
        /// </summary>
        public int ReadOnly { get; set; }
    }
}
