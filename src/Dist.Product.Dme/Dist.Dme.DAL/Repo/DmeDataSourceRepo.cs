using Dist.Dme.DAL.Context;
using Dist.Dme.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.DAL.Repo
{
    /// <summary>
    /// 数据源repository
    /// </summary>
    public class DmeDataSourceRepo : OracleContextBase
    {
        public SimpleClient<DmeDataSource> Repository =>  new SimpleClient<DmeDataSource>(base.GetDbContext());

    }
}
