using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.DisFS.Adapters.Mongo;
using Dist.Dme.Model.Entity;
using Dist.Dme.Service.Interfaces;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Service.Impls
{
    public class StoreService : BaseBizService, IStoreService
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        public StoreService(IRepository repository, ILogService logService)
        {
            base.Repository = repository;
            base.LogService = logService;
        }

        public IList<string> ListMongoCollection(string datasourceCode)
        {
            DmeDataSource dmeDataSource = IsMongodbAndReturn(datasourceCode);
            MongodbHost host = JsonConvert.DeserializeObject<MongodbHost>(dmeDataSource.Connection);
            if (string.IsNullOrEmpty(host.DataBase))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, "mongodb的DataBase不能为空");
            }
            return MongodbManager<object>.ListCollections(host.Connection, host.DataBase);
        }

        public IList<string> ListMongoCollection(string host, int port, string dataBase)
        {
            string connectionString = $"mongodb://{host}:{port}";
            return MongodbManager<object>.ListCollections(connectionString, dataBase);
        }

        public IList<string> ListMongoDataBase(string datasourceCode)
        {
            DmeDataSource dmeDataSource = IsMongodbAndReturn(datasourceCode);
            MongodbHost host = JsonConvert.DeserializeObject<MongodbHost>(dmeDataSource.Connection);
            return MongodbManager<object>.ListDataBases(host.Connection);
        }
        /// <summary>
        /// 验证是否mongodb类型，并返回
        /// </summary>
        /// <param name="datasourceCode"></param>
        /// <returns></returns>
        private DmeDataSource IsMongodbAndReturn(string datasourceCode)
        {
            var db = base.Repository.GetDbContext();
            DmeDataSource dmeDataSource = db.Queryable<DmeDataSource>().Single(ds => ds.SysCode == datasourceCode);
            if (!nameof(EnumDataSourceType.MONGODB).Equals(dmeDataSource.Type))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"数据源类型[{dmeDataSource.Type}]不是mongodb类型，请选择mongodb数据源类型");
            }

            return dmeDataSource;
        }

        public IList<string> ListMongoDataBase(string host, int port)
        {
            string connectionString = $"mongodb://{host}:{port}";
            return MongodbManager<object>.ListDataBases(connectionString);
        }
    }
}
