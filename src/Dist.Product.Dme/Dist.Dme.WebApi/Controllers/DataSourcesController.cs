using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Utils;
using Dist.Dme.DisFS.Adapters.Mongo;
using Dist.Dme.Model.DTO;
using Dist.Dme.Service.Interfaces;
using Dist.Dme.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using NLog;

namespace Dist.Dme.WebApi.Controllers
{
    /// <summary>
    /// 数据源服务
    /// </summary>
    [Route("api/datasources")]
    public class DataSourcesController : BaseController
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        public IDataSourceService DataSourceService { get; private set; }
        public IMongoDatabase MongoDatabase { get; private set; }
        public MongodbHost MongodbHost { get; private set; }
        public DataSourcesController(IDataSourceService dataSourceService, IMongoDatabase mongoDatabase, MongodbHost mongodbHost)
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
        [Route("v1/types")]
        public Result ListDatabaseTypes()
        {
            LOG.Info(">>>test info");
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
            return base.Success(this.DataSourceService.CheckConnectionValid(dto), "验证通过");
        }
    }
}