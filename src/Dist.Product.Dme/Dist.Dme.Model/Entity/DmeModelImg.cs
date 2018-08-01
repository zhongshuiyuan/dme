using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dist.Dme.Model.Entity
{
    [SugarTable("DME_MODEL_IMG")]
    public class DmeModelImg
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID", OracleSequenceName = "SEQ_DME_MODEL_IMG")]
        public int Id { get; set; }
        [SugarColumn(ColumnName = "MODEL_ID")]
        public int ModelId { get; set; }
        [SugarColumn(ColumnName = "VERSION_ID")]
        public int VersionId { get; set; }
        [Required]
        public string ImgCode { get; set; }
        public string Suffix { get; set; }
        public string SourceName { get; set; }
        public string ContentType { get; set; }
    }
}
