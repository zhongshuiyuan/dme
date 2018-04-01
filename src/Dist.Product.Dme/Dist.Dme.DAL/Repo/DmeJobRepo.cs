using Dist.Dme.DAL.Context;
using Dist.Dme.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.DAL.Repo
{
    /// <summary>
    /// 任务repository
    /// </summary>
    public class DmeJobRepo : OracleContextBase
    {
        public SimpleClient<DmeJob> Repository => new SimpleClient<DmeJob>(base.GetDbContext());

    }
}
