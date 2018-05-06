using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Utils;
using Dist.Dme.Model.Entity;
using Dist.Dme.Service.Impls;
using Dist.Dme.Service.Interfaces;
using Dist.Dme.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace Dist.Dme.WebApi.Controllers
{
    /// <summary>
    /// 数据源服务
    /// </summary>
    [Route("api/datasources")]
    public class DataSourcesController : BaseController
    {
        private IDataSourceService dataSourceService = new DataSourceService();

        /// <summary>
        /// 获取所有数据库类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/databasetypes")]
        public Result ListDatabaseTypes()
        {
            return base.Success(dataSourceService.ListDatabaseTypes());
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
            return base.Success(dataSourceService.GetDatabaseType(id));
        }
    }
}