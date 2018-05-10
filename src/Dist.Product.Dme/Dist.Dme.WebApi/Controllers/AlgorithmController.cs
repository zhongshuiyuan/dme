using Dist.Dme.Base.Framework;
using Dist.Dme.Model.DTO;
using Dist.Dme.Plugins.LandConflictDetection.DTO;
using Dist.Dme.Service.Impls;
using Dist.Dme.Service.Interfaces;
using Dist.Dme.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Dist.Dme.WebApi.Controllers
{
    /// <summary>
    /// 算法服务
    /// </summary>
    [Route("api/algorithms")]
    public class AlgorithmController : BaseController
    {
        /// <summary>
        /// 算法服务
        /// </summary>
        private static IAlgorithmService AlgorithmService => new AlgorithmService();
        /// <summary>
        /// 获取所有算法
        /// </summary>
        /// <param name="hasMeta">是否获取算法的参数信息</param>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/")]
        public Result ListModels([FromQuery(Name = "hasmeta")] int hasMeta = 0)
        {
            return base.Success(AlgorithmService.ListAlgorithm(1 == hasMeta));
        }
        /// <summary>
        /// 注册算法
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/")]
        public Result AddAlgorithm([FromBody] AlgorithmAddReqDTO dto)
        {
            return base.Success(AlgorithmService.AddAlgorithm(dto));
        }
        
    }
}