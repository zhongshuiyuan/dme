using Dist.Dme.Base.Common;
using Dist.Dme.Base.DataSource;
using Dist.Dme.Base.DataSource.Oracle;
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
        public List<DmeDataSourceType> ListDataSourceTypes()
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
                        Desc = EnumUtil.GetEnumDescription(EnumUtil.GetEnumObjByName<EnumDataSourceType>(item.Type))
                    };
                    dictionary[item.Type] = tempDatasourceDTO;
                }
                tempDatasourceDTO.Sources.Add(item);
            }
            return dictionary;
        }
        public object GetDatasourceConnMeta(string typeCode)
        {
            typeCode = typeCode.ToUpper();
            EnumDataSourceType @enum = EnumUtil.GetEnumObjByName<EnumDataSourceType>(typeCode);
            IDMEDataSourceFactory factory = null;
            IDMEDataSource dataSource = null;
            string meta = "";
            switch (@enum)
            {
                case EnumDataSourceType.ORACLE:
                    factory = new DMEOracleFactory();
                    dataSource = factory.Open(null, false);
                    meta = dataSource.ConnectionMeta;
                    break;
                default:
                    meta = "";
                    break;
            }
            return meta;
        }
        public object CheckConnectionValid(DataSourceConnDTO dto)
        {
            dto.TypeCode = dto.TypeCode.ToUpper();
            EnumDataSourceType @enum = EnumUtil.GetEnumObjByName<EnumDataSourceType>(dto.TypeCode);
            IDMEDataSourceFactory factory = null;
            IDMEDataSource dataSource = null;
            bool valid = false;
            switch (@enum)
            {
                case EnumDataSourceType.UNKNOWN:
                    break;
                case EnumDataSourceType.SHAPEFILE:
                    break;
                case EnumDataSourceType.COVERAGE:
                    break;
                case EnumDataSourceType.PERSONAL_GEODATABASE:
                    break;
                case EnumDataSourceType.FILE_GEODATABASE:
                    break;
                case EnumDataSourceType.ENTERPRISE_GEODATABASE:
                    break;
                case EnumDataSourceType.TIN:
                    break;
                case EnumDataSourceType.CAD:
                    break;
                case EnumDataSourceType.ORACLE:
                    factory = new DMEOracleFactory();
                    dataSource = factory.OpenFromConnectionStr(dto.Connection, true);
                    valid = dataSource.ValidConnection();
                    break;
                default:
                    break;
            }
            return valid;
        }
    }
}
