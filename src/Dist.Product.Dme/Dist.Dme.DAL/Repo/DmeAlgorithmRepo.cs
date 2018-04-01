using Dist.Dme.DAL.Context;
using Dist.Dme.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.DAL.Repo
{
    /// <summary>
    /// 算法repository
    /// </summary>
    public class DmeAlgorithmRepo : OracleContextBase
    {
        public SimpleClient<DmeAlgorithm> Repository => new SimpleClient<DmeAlgorithm>(base.GetDbContext());

    }
}
