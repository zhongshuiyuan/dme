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
    /// 模型服务
    /// </summary>
    [Route("api/models")]
    public class ModelsController : BaseController
    {
        /// <summary>
        /// 模型服务
        /// </summary>
        private static IModelService ModelService => new ModelService();
        /// <summary>
        /// 获取所有模型
        /// </summary>
        /// <param name="refAlgorithm">是否获取关联的算法信息</param>
        /// <returns></returns>
        [HttpGet]
        [Route("/v1/")]
        public Result ListModels([FromQuery(Name = "refalgs")] int refAlgorithm = 0)
        {
            return base.Success(ModelService.ListModels(1 == refAlgorithm));
        }
        /// <summary>
        /// 获取用地冲突分析输入参数
        /// </summary>
        /// <returns></returns>
        //[HttpGet]
        //[Route("landconflict/v1/inputparas")]
        //public Result GetLandConflictInputParameters()
        //{
        //    return base.Success(ModelService.GetLandConflictInputParameters());
        //}
        /// <summary>
        /// 获取用地冲突分析输出参数
        /// </summary>
        /// <returns></returns>
        //[HttpGet]
        //[Route("landconflict/v1/outputparas")]
        //public Result GetLandConflictOutputParameters()
        //{
        //    return base.Success(ModelService.GetLandConflictOutputParameters());
        //}
        /// <summary>
        /// 获取用地冲突分析元数据信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("landconflict/v1/metadata")]
        public Result GetLandConflictMetadata()
        {
            return base.Success(ModelService.GetLandConflictMetadata());
        }
        /// <summary>
        /// 用地冲突分析计算
        /// </summary>
        /// <param name="dto">参数输入</param>
        /// <returns></returns>
        [HttpPost]
        [Route("landconflict/v1/execute")]
        public Result LandConflictExecute([FromBody][Required]LandConflictReqDTO dto)
        {
            dto.Parameters.Add("m_featureClass_source_first", @"D:\work\data\zgkg.mdb|BZY_YDGH_PY");
            dto.Parameters.Add("m_featureClass_source_second", @"D:\work\data\zgkg.mdb|BKY_YDGH_PY");
            dto.Parameters.Add("m_yddm_second", "YDDM");
            dto.Parameters.Add("m_yddm_first", "YDDM");
            
            return base.Success(ModelService.LandConflictExecute(dto.Parameters));
        }
        /// <summary>
        /// 注册模型
        /// </summary>
        /// <param name="dto">参数模型</param>
        /// <returns></returns>
        [HttpPost]
        [Route("/v1")]
        public Result AddModel([FromBody]ModelAddReqDTO dto)
        {
            if (ModelState.IsValid)
            {
                return base.Success(ModelService.AddModel(dto));
            }
            // 参数验证失败
            return base.Error(ModelState);
        }
    }
}