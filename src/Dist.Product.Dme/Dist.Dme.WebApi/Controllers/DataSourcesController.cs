using Dist.Dme.Base.Framework;
using Dist.Dme.DisFS.Adapters.Mongo;
using Dist.Dme.Model.DTO;
using Dist.Dme.Service.Interfaces;
using Dist.Dme.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dist.Dme.WebApi.Controllers
{
    /// <summary>
    /// 数据源服务
    /// </summary>
    [Route("api/datasources")]
    public class DataSourcesController : BaseController
    {
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
        /// 获取所有数据库类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/databasetypes")]
        public Result ListDatabaseTypes()
        {
            return base.Success(DataSourceService.ListDatabaseTypes());
        }
        /// <summary>
        /// 获取具体某个数据库类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/databasetypes/{id}")]
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
        /// 获取注册的数据源
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/")]
        public Result ListRegisteredDataSources()
        {
            return base.Success(this.DataSourceService.ListRegisteredDataSources());
        }
        /// <summary>
        /// 注册数据源
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/registry")]
        public Result RegistryDataSource([FromBody]DatasourceAddDTO dto)
        {
            return base.Success(this.DataSourceService.AddDataSource(dto));
        }
    }
}