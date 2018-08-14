using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Framework.AlgorithmTypes;
using Dist.Dme.Model.DTO;
using Dist.Dme.Service.Impls;
using Dist.Dme.Service.Interfaces;
using Dist.Dme.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dist.Dme.WebApi.Controllers
{
    /// <summary>
    /// 算法服务
    /// </summary>
    [Route("api/algorithms")]
    public class AlgorithmController : CommonController
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 算法服务
        /// </summary>
        //public IAlgorithmService  AlgorithmService { get; private set; }
        //public AlgorithmController(IAlgorithmService algorithmService)
        //{
        //    this.AlgorithmService = algorithmService;
        //}
        /// <summary>
        /// 获取所有算法。
        /// </summary>
        /// <param name="needMeta">是否获取算法的参数信息。0：表示否；1：表示是</param>
        /// <returns></returns>
        [HttpGet]
        [Route("metadata/v1")]
        public Result ListAlgorithm([FromQuery(Name = "needmeta")] int needMeta = 0)
        {
            return base.Success(base.algorithmService.ListAlgorithms(1 == needMeta));
        }
        /// <summary>
        /// 获取本地DLL算法元数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("metadata/v1/local")]
        public Result ListAlgorithmMetadatasLocal()
        {
            return base.Success(base.algorithmService.ListAlgorithmMetadatasLocal());
        }
        /// <summary>
        /// 注册算法
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("register/v1")]
        public Result RegistryAlgorithm([FromBody] AlgorithmAddReqDTO dto)
        {
            if (ModelState.IsValid)
            {
                return base.Success(base.algorithmService.AddAlgorithm(dto));
            }
            // 参数验证失败
            return base.Error(ModelState);
        }
        /// <summary>
        /// 删除算法
        /// </summary>
        /// <param name="code">算法唯一编码</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("v1/{code}")]
        public Result DeleteAlgorithm(string code)
        {
            return base.Success(base.algorithmService.DeleteAlgorithm(code));
        }
        /// <summary>
        /// 注册本地算法（从本地DLL获取）
        /// </summary>
        /// <param name="algCode">算法唯一编码。输入为空，说明是遍历全部</param>
        /// <returns></returns>
        [HttpGet]
        [Route("register/v1/local")]
        public Result RegistryAlgorithmFromLocal([FromQuery(Name = "algcode")]string algCode)
        {
            if (!string.IsNullOrEmpty(algCode) && !this.algorithmService.IsBizGuid(algCode))
            {
                return base.Fail($"算法编码格式无效[{algCode}]，格式如：97e1dd1f6c664128b93815f56256d0f1");
            }

            return base.Success(base.algorithmService.RegistryAlgorithmFromLocal(algCode));
        }
        /// <summary>
        /// 算法开发平台类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("devtypes/v1")]
        public Result ListAlgorithmType()
        {
            return base.Success(AlgorithmTypesFactory.ListAlgorithmDevType());
        }
        /// <summary>
        /// 算法的执行器，需要注入算法的参数
        /// </summary>
        /// <param name="algCode">算法唯一编码</param>
        /// <param name="dto">参数信息值，键值对格式</param>
        /// <returns></returns>
        [HttpPost]
        [Route("execute/v1/{algCode}")]
        public Result ExecuteAlgorithm(string algCode, [FromBody]BaseRequestDTO dto)
        {
            AlgorithmRespDTO algInfo = (AlgorithmRespDTO)base.algorithmService.GetAlgorithmByCode(algCode, true);
            
            return base.Success("");
        }
    }
}