using Dist.Dme.DAL.Conf;
using Dist.Dme.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.DAL
{
    public class DbContext
    {
        public DbContext()
        {
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.Oracle,
                IsAutoCloseConnection = true
            });
        }
        public SqlSugarClient Db;
        public SimpleClient<DmeModel> DmeModelRepo { get { return new SimpleClient<DmeModel>(Db); } }
    }
}
