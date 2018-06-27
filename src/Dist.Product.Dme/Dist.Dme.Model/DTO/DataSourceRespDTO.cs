using Dist.Dme.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 数据源响应DTO
    /// </summary>
    public class DataSourceRespDTO
    {
        public string Name { get; set; }
        public string Desc { get; set; }
        public IList<DmeDataSource> Sources { get; set; } = new List<DmeDataSource>();
    }
}
