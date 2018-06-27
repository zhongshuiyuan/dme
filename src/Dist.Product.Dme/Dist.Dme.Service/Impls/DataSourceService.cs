using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.DAL.Context;
using Dist.Dme.DisFS.Adapters.Mongo;
using Dist.Dme.Model.DTO;
using Dist.Dme.Model.Entity;
using Dist.Dme.Service.Interfaces;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Service.Impls
{
    public class DataSourceService : BaseBizService, IDataSourceService
    {
        private static ILog LOG = LogManager.GetLogger(typeof(DataSourceService));
        private MongodbHost mongoHost;

        public DataSourceService(IRepository repository, MongodbHost host)
        {
            base.Repository = repository;
            this.mongoHost = host;
        }
        public List<DmeDataSourceType> ListDatabaseTypes()
        {
            return base.Repository.GetDbContext().Queryable<DmeDataSourceType>().ToList();
        }
        public DmeDataSourceType GetDatabaseType(int id)
        {
            return base.Repository.GetDbContext().Queryable<DmeDataSourceType>().Single(dt => dt.Id == id);
        }

        public object AddDataSource(DatasourceAddDTO datasourceAddDTO)
        {
            DmeDataSource dataSource = ClassValueCopier<DmeDataSource>.Copy(datasourceAddDTO);
            dataSource.CreateTime = DateUtil.CurrentTimeMillis;
            dataSource.SysCode = GuidUtil.NewGuid();
            return base.Repository.GetDbContext().Insertable<DmeDataSource>(dataSource).ExecuteReturnEntity();
        }
        public object ListRegisteredDataSources()
        {
            List<DmeDataSource> datasources = this.Repository.GetDbContext().Queryable<DmeDataSource>().OrderBy("type").ToList();
            if (0 == datasources?.Count)
            {
                return null;
            }
            // 按照数据源类别进行归类
            IDictionary<string, DataSourceRespDTO> dictionary = new Dictionary<string, DataSourceRespDTO>();
            DataSourceRespDTO tempDatasourceDTO = null;
            foreach (var item in datasources)
            {
                if (dictionary.ContainsKey(item.Type))
                {
                    tempDatasourceDTO = dictionary[item.Type];
                }
                else
                {
                    tempDatasourceDTO = new DataSourceRespDTO
                    {
                        Name = item.Type,
                        Desc = EnumUtil.GetEnumDescription(EnumUtil.GetEnumObjByName<DataSourceTypes>(item.Type))
                    };
                    dictionary[item.Type] = tempDatasourceDTO;
                }
                tempDatasourceDTO.Sources.Add(item);
            }
            return dictionary;
        }
    }
}
