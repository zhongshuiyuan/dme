using Dist.Dme.DAL.Context;
using Dist.Dme.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.DAL.Repo
{
    /// <summary>
    /// 版本repository
    /// </summary>
    public class DmeVersionRepo : OracleContextBase
    {
        public SimpleClient<DmeVersion> Repository => new SimpleClient<DmeVersion>(base.GetDbContext());

    }
}
