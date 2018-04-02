using Dist.Dme.DAL.Conf;
using Dist.Dme.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dist.Dme.DAL.Context
{
    public class ContextBase : IDbContext
    {
        protected SqlSugarClient Db;

        public ContextBase(DbType dbType, String connectionConfig)
        {
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = connectionConfig,
                DbType = dbType,
                IsAutoCloseConnection = true
            });
            Db.Ado.IsEnableLogEvent = true;
            Db.Ado.LogEventStarting = (sql, pars) =>
            {
                Console.WriteLine(sql + "\r\n" + Db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                Console.WriteLine();
            };
        }

        public virtual SimpleClient Repository => throw new NotImplementedException();

        public SqlSugarClient GetDbContext()
        {
            return Db;
        }
    }
}
