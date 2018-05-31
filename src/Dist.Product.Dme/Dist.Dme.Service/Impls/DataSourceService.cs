using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.DAL.Context;
using Dist.Dme.Model.Entity;
using Dist.Dme.Service.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Service.Impls
{
    public class DataSourceService : BaseBizService, IDataSourceService
    {
        private static ILog LOG = LogManager.GetLogger(typeof(DataSourceService));

        public DataSourceService(IRepository repository)
        {
            base.Repository = repository;
        }
        public List<DmeDatabaseType> ListDatabaseTypes()
        {
            return base.Repository.GetDbContext().Queryable<DmeDatabaseType>().ToList();
        }
        public DmeDatabaseType GetDatabaseType(int id)
        {
            return base.Repository.GetDbContext().Queryable<DmeDatabaseType>().Single(dt => dt.Id == id);
        }
    }
}
