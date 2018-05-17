using Dist.Dme.Base.Framework;
using Dist.Dme.Model.DTO;
using Dist.Dme.Plugins.LandConflictDetection.DTO;
using Dist.Dme.Service.Impls;
using Dist.Dme.Service.Interfaces;
using Dist.Dme.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public IModelService ModelService { get; private set; }
        public ModelsController(IModelService modelService)
        {
            this.ModelService = modelService;
        }
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
            dto.Parameters.Add("FeatureClass_Source_First", @"D:\work\data\zgkg.mdb|BZY_YDGH_PY");
            dto.Parameters.Add("FeatureClass_Source_Second", @"D:\work\data\zgkg.mdb|BKY_YDGH_PY");
            dto.Parameters.Add("Yddm_Second", "YDDM");
            dto.Parameters.Add("Yddm_First", "YDDM");
            
            return base.Success(ModelService.LandConflictExecute(dto.Parameters));
        }
        /// <summary>
        /// 注册模型
        /// </summary>
        /// <param name="dto">参数模型，当SysCode为空时，系统会自动附上一个唯一编码</param>
        /// <returns></returns>
        [HttpPost]
        [Route("/v1")]
        public Result AddModel([FromBody]ModelAddReqDTO dto)
        {
            if (!string.IsNullOrEmpty(dto.SysCode) && !Guid.TryParse(dto.SysCode, out Guid guid))
            {
                return base.Fail($"唯一编码格式无效[{dto.SysCode}]");
            }
            if (ModelState.IsValid)
            {
                return base.Success(ModelService.AddModel(dto));
            }
            // 参数验证失败
            return base.Error(ModelState);
        }
        /// <summary>
        /// 根据模型版本进行复制
        /// </summary>
        /// <param name="modelVersionCode">模型版本唯一编码</param>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/copy/{modelVersionCode}")]
        public Result Copy(string modelVersionCode)
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("rulestep/v1")]
        public Result NewRuleStep(string modelVersionCode)
        {
            throw new NotImplementedException();
        }
    }
}