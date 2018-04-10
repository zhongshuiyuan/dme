using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.Entity
{
    [SugarTable("DME_USER")]
    public class DmeUser
    {
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ID", OracleSequenceName = "DME_USER")]
        public int Id { get; set; }
        public String SysCode { get; set; }
        public String LoginName { get; set; }
        public String LoginPwd { get; set; }
        public String Name { get; set; }
        public int Status { get; set; }
        public Int64 CreateTime { get; set; }
        public String Email { get; set; }
        public String Telephone { get; set; }
        public int UserType { get; set; }
    }
}
