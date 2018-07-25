using Dist.Dme.Base.Common;
using Dist.Dme.Base.Common.Http;
using Dist.Dme.Base.DataSource.Define;
using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Base.Utils;
using Dist.Dme.DisFS.Adapters.Mongo;
using Dist.Dme.Extensions;
using Dist.Dme.Model.DTO;
using Dist.Dme.Service.Interfaces;
using Dist.Dme.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using NLog;
using System.ComponentModel.DataAnnotations;

namespace Dist.Dme.WebApi.Controllers
{
    /// <summary>
    /// 数据源服务
    /// </summary>
    [Route("api/datasources")]
    public class DataSourceController : BaseController
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        public IDataSourceService DataSourceService { get; private set; }
        public IMongoDatabase MongoDatabase { get; private set; }
        public MongodbHost MongodbHost { get; private set; }
        public DataSourceController(IDataSourceService dataSourceService, IMongoDatabase mongoDatabase, MongodbHost mongodbHost)
        {
            this.DataSourceService = dataSourceService;
            this.MongoDatabase = mongoDatabase;
            this.MongodbHost = mongodbHost;
        }
        /// <summary>
        /// 获取所有数据源类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CompressContent]
        [CacheFilter(CacheSecondDuration = 100)]
        [Route("v1/types")]
        public Result ListDatabaseTypes()
        {
            Register register = new Register();
            LOG.Info(">>>test log info");
            return base.Success(DataSourceService.ListDataSourceTypes());
        }
        /// <summary>
        /// 获取具体某个数据源类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/types/{id}")]
        public Result GetDatabaseType(int id)
        {
            return base.Success(DataSourceService.GetDatabaseType(id));
        }
        //[HttpGet]
        //[Route("v1/data")]
        //public void UploadDataset()
        //{
        //    ObjectId objectId = MongodbHelper<object>.UploadFileFromPath(this.MongodbHost, @"D:\work\dist\c_产品管理\g_规划协查\飞灵姐提供的重庆分析工具\demo数据\论坛版测试数据.mdb");
        //    System.Console.WriteLine(objectId);
        //}
        /// <summary>
        /// 获取已注册的数据源
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("v1")]
        public Result ListRegisteredDataSources()
        {
            return base.Success(this.DataSourceService.ListRegisteredDataSources());
        }
        /// <summary>
        /// 获取数据源连接的元数据信息
        /// </summary>
        /// <param name="typeCode">数据源类型编码，如：oracle</param>
        /// <returns></returns>
        [HttpGet]
        [Route("conn/meta/v1/{typeCode}")]
        public Result GetDatasourceConnMeta(string typeCode)
        {
            return base.Success(this.DataSourceService.GetDatasourceConnMeta(typeCode));
        }
        /// <summary>
        /// 注册数据源
        /// </summary>
        /// <param name="dto">数据源元数据信息</param>
        /// <returns></returns>
        [HttpPost]
        [Route("register/v1")]
        public Result RegistryDataSource([FromBody]DatasourceAddDTO dto)
        {
            try
            {
                // 验证数据源类型的合法性
                EnumUtil.GetEnumObjByName<EnumDataSourceType>(dto.Type);
            }
            catch (System.Exception)
            {
                return base.Error($"数据源类型不合法[{dto.Type}]");
            }
            return base.Success(this.DataSourceService.AddDataSource(dto));
        }
        /// <summary>
        /// 验证连接是否有效
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>true/false</returns>
        [HttpPost]
        [Route("conn/valid/v1")]
        public Result CheckConnectionValid([FromBody]DataSourceConnDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return base.Error(ModelState, "验证失败");
            }
            ValidResult validResult = (ValidResult)DataSourceService.CheckConnectionValid(dto);
            if(!validResult.IsValid)
            {
                if (validResult.Ex != null)
                {
                    return base.Error(validResult.Message, "验证不通过");
                }
                return base.Fail(validResult.Message, "验证不通过");
            }
            return base.Success(null, "验证通过");
        }
        /// <summary>
        /// 获取mongo下的所有database
        /// </summary>
        /// <param name="dataSourceCode">数据源唯一编码</param>
        /// <returns></returns>
        [HttpGet]
        [Route("mongo/v1/databases/{dataSourceCode}")]
        public Result ListMongoDataBases([Required] string dataSourceCode)
        {
            if (string.IsNullOrEmpty(dataSourceCode))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, "数据源编码不能为空");
            }
            return base.Success(this.DataSourceService.ListMongoDataBase(dataSourceCode));
        }
        /// <summary>
        /// 获取mongo下的所有database
        /// </summary>
        /// <param name="host">服务ip</param>
        /// <param name="port">端口号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("mongo/v1/databases")]
        public Result ListMongoDataBase([FromQuery] string host, [FromQuery] int port)
        {
            if (string.IsNullOrEmpty(host))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, "host不能为空");
            }
            if (!NetAssist.IsIP(host))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, "host不合法");
            }
            return base.Success(this.DataSourceService.ListMongoDataBase(host, port));
        }
        /// <summary>
        /// 获取mongo下的指定数据库的集合类
        /// </summary>
        /// <param name="dataSourceCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("mongo/v1/collections/{dataSourceCode}")]
        public Result ListMongoCollection([Required] string dataSourceCode)
        {
            if (string.IsNullOrEmpty(dataSourceCode))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, "数据源编码不能为空");
            }
            return base.Success(this.DataSourceService.ListMongoCollection(dataSourceCode));
        }
        /// <summary>
        /// 获取mongo下的指定数据库所有collections
        /// </summary>
        /// <param name="host">服务ip</param>
        /// <param name="port">端口号</param>
        /// <param name="dataBase">数据库实例名称，大小写敏感</param>
        /// <returns></returns>
        [HttpGet]
        [Route("mongo/v1/collections")]
        public Result ListMongoCollection([FromQuery] string host, [FromQuery] int port, [FromQuery] string dataBase)
        {
            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(dataBase))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, "host或dataBase不能为空");
            }
            if (!NetAssist.IsIP(host))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, "host不合法");
            }
            return base.Success(this.DataSourceService.ListMongoCollection(host, port, dataBase));
        }
    }
}