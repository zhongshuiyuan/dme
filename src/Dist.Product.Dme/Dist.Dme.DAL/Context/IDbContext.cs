using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.DAL.Context
{
    public interface IDbContext
    {
        SqlSugarClient GetDbContext();
    }
}
