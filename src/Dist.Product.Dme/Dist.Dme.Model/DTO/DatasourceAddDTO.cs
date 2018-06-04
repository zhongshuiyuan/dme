using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 数据源添加DTO
    /// </summary>
    public class DatasourceAddDTO
    {
        public String Name { get; set; }
        public int IsLocal { get; set; }
        public String Type { get; set; }
        public String Connection { get; set; }
        public int CreateTime { get; set; }
        public String Remark { get; set; }
    }
}
