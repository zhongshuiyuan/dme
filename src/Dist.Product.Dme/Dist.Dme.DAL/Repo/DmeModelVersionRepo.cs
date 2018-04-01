using Dist.Dme.DAL.Context;
using Dist.Dme.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.DAL.Repo
{
    /// <summary>
    /// 模型版本repository
    /// </summary>
    public class DmeModelVersionRepo : OracleContextBase
    {
        public SimpleClient<DmeModelVersion> Repository => new SimpleClient<DmeModelVersion>(base.GetDbContext());

    }
}
