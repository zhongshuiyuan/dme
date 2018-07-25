using Dist.Dme.Base.Common;
using Dist.Dme.Base.DataSource;
using Dist.Dme.Base.DataSource.Define;
using Dist.Dme.Base.DataSource.MongoDB;
using Dist.Dme.Base.DataSource.Oracle;
using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.DAL.Context;
using Dist.Dme.DisFS.Adapters.Mongo;
using Dist.Dme.Model.DTO;
using Dist.Dme.Model.Entity;
using Dist.Dme.Service.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Service.Impls
{
    public class DataSourceService : BaseBizService, IDataSourceService
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

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
            object meta = "";
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
            ValidResult result = new ValidResult();
            try
            {
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
                    case EnumDataSourceType.TIN:
                        break;
                    case EnumDataSourceType.CAD:
                        break;
                    case EnumDataSourceType.ORACLE:
                    case EnumDataSourceType.ENTERPRISE_GEODATABASE:
                        factory = new DMEOracleFactory();
                        dataSource = factory.OpenFromConnectionStr(dto.Connection, true);
                        result.IsValid = dataSource.ValidConnection();
                        break;
                    case EnumDataSourceType.MONGODB:
                        factory = new DMEMongoFactory();
                        dataSource = factory.OpenFromConnectionStr(dto.Connection, true);
                        result.IsValid = dataSource.ValidConnection();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                LOG.Error(ex, ex.Message);
                result.Message = ex.Message;
                result.Ex = ex;
            }
          
            return result;
        }
        public IList<string> ListMongoCollection(string datasourceCode)
        {
            DmeDataSource dmeDataSource = IsMongodbAndReturn(datasourceCode);
            MongodbHost host = JsonConvert.DeserializeObject<MongodbHost>(dmeDataSource.Connection);
            if (string.IsNullOrEmpty(host.DataBase))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, "mongodb的DataBase不能为空");
            }
            return MongodbManager<object>.ListCollections(host.ConnectionString, host.DataBase);
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
            return MongodbManager<object>.ListDataBases(host.ConnectionString);
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
