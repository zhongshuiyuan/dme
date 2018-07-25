using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Service.Interfaces;
using Dist.Dme.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Dist.Dme.WebApi.Controllers
{
    [Route("api/stores")]
    public class StoreController : BaseController
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        protected IStoreService StoreService { get; private set; }
        public StoreController(IStoreService storeService)
        {
            this.StoreService = storeService;
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
            return base.Success(this.StoreService.ListMongoDataBase(dataSourceCode));
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
            return base.Success(this.StoreService.ListMongoDataBase(host, port));
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
            return base.Success(this.StoreService.ListMongoCollection(dataSourceCode));
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
            return base.Success(this.StoreService.ListMongoCollection(host, port, dataBase));
        }
    }
}
