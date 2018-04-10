using Dist.Dme.DAL.Context;
using Dist.Dme.Model.Entity;
using Dist.Dme.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Service.Impls
{
    public class DataSourceService : AbstractContext, IDataSourceService
    {
        public List<DmeDatabaseType> ListDatabaseTypes()
        {
            return base.DmeDatabaseTypeDb.GetList();
        }
        public DmeDatabaseType GetDatabaseType(int id)
        {
            return base.DmeDatabaseTypeDb.GetById(id);
        }
    }
}
