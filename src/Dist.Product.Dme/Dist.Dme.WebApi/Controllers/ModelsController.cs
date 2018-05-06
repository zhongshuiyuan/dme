using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Utils;
using Dist.Dme.Model.DTO;
using Dist.Dme.Model.Entity;
using Dist.Dme.Plugins.LandConflictDetection.DTO;
using Dist.Dme.Service.Impls;
using Dist.Dme.Service.Interfaces;
using Dist.Dme.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

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
        /// 获取用地冲突分析输入参数
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("landconflict/v1/inputparas")]
        public Result GetLandConflictInputParameters()
        {
            return base.Success(ModelService.GetLandConflictInputParameters());
        }
        /// <summary>
        /// 获取用地冲突分析输出参数
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("landconflict/v1/outputparas")]
        public Result GetLandConflictOutputParameters()
        {
            return base.Success(ModelService.GetLandConflictOutputParameters());
        }
        /// <summary>
        /// 用地冲突分析计算
        /// </summary>
        /// <param name="dto">参数输入</param>
        /// <returns></returns>
        [HttpPost]
        [Route("landconflict/v1/execute")]
        public Result LandConflictExecute([FromBody]LandConflictReqDTO dto)
        {
            dto.Parameters.Add("m_featureClass_source_first", @"D:\work\data\zgkg.mdb|BZY_YDGH_PY");
            dto.Parameters.Add("m_featureClass_source_second", @"D:\work\data\zgkg.mdb|BKY_YDGH_PY");
            dto.Parameters.Add("m_yddm_second", "YDDM");
            dto.Parameters.Add("m_yddm_first", "YDDM");
            
            return base.Success(ModelService.LandConflictExecute(dto.Parameters));
        }
    }
}