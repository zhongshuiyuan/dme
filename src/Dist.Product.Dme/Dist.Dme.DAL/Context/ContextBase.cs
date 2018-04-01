using Dist.Dme.DAL.Conf;
using Dist.Dme.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.DAL.Context
{
    public class ContextBase : IDbContext
    {
        protected SqlSugarClient Db;

        public ContextBase(DbType dbType)
        {
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = dbType,
                IsAutoCloseConnection = true
            });
        }

        public SqlSugarClient GetDbContext()
        {
            return Db;
        }
    }
}
