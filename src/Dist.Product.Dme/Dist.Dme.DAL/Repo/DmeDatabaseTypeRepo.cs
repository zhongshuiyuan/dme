using Dist.Dme.DAL.Context;
using Dist.Dme.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.DAL.Repo
{
    /// <summary>
    /// 数据库类型repository
    /// </summary>
    public class DmeDatabaseTypeRepo : OracleContextBase
    {
        public SimpleClient<DmeDatabaseType> Repository => new SimpleClient<DmeDatabaseType>(base.GetDbContext());

    }
}
