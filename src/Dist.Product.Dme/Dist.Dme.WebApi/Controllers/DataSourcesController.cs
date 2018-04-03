using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dist.Dme.Model;
using Dist.Dme.Service.Impls;
using Dist.Dme.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dist.Dme.WebApi.Controllers
{
    /// <summary>
    /// 数据源服务
    /// </summary>
    [Route("api/v1/datasources")]
    public class DataSourcesController : Controller
    {
        private IDataSourceService dataSourceService = new DataSourceService();

        /// <summary>
        /// 获取所有数据库类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("databasetypes")]
        public List<DmeDatabaseType> ListDatabaseTypes()
        {
            return dataSourceService.ListDatabaseTypes();
        }
        /// <summary>
        /// 获取具体某个数据库类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("databasetypes/{id}")]
        public DmeDatabaseType GetDatabaseType(int id)
        {
            return dataSourceService.GetDatabaseType(id);
        }
    }
}