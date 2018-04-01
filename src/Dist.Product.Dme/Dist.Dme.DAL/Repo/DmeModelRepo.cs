using Dist.Dme.DAL.Context;
using Dist.Dme.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.DAL.Repo
{
    /// <summary>
    /// 模型repository
    /// </summary>
    public class DmeModelRepo : OracleContextBase
    {
        public SimpleClient<DmeModel> Repository => new SimpleClient<DmeModel>(base.GetDbContext());

    }
}
