using Dist.Dme.Algorithms.LandConflictDetection.DTO;
using Dist.Dme.Algorithms.Overlay.DTO;
using Dist.Dme.Base.Framework;
using Dist.Dme.Model.DTO;
using Dist.Dme.Model.Entity;
using Dist.Dme.Service.Impls;
using Dist.Dme.Service.Interfaces;
using Dist.Dme.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

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
        /// <param name="detail">是否获取详情信息，0：否；1：是</param>
        /// <returns></returns>
        [HttpGet]
        [Route("v1")]
        public Result ListModels([FromQuery(Name = "detail")] int detail = 0)
        {
            return base.Success(ModelService.ListModels(1 == detail));
        }
        /// <summary>
        /// 根据模型唯一编码获取模型
        /// </summary>
        /// <param name="code">模型唯一编码</param>
        /// <param name="detail">是否获取详情</param>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/{code}")]
        public Result GetModel(string code, [FromQuery(Name = "detail")] int detail =0)
        {
            if (!this.ModelService.IsBizGuid(code))
            {
                return base.Fail($"业务编码格式不正确[{code}]");
            }
            return base.Success(ModelService.GetModelMetadata(code, 1 == detail));
        }
       
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
            dto.Parameters.Add("FeatureClass_Source_First", @"D:\work\data\zgkg.mdb&BZY_YDGH_PY");
            dto.Parameters.Add("FeatureClass_Source_Second", @"D:\work\data\zgkg.mdb&BKY_YDGH_PY");
            dto.Parameters.Add("Yddm_Second", "YDDM");
            dto.Parameters.Add("Yddm_First", "YDDM");
            
            return (Result)ModelService.LandConflictExecute(dto.Parameters);
        }
        /// <summary>
        /// 叠加分析计算
        /// </summary>
        /// <param name="dto">参数输入</param>
        /// <returns></returns>
        [HttpPost]
        [Route("overlay/v1/execute")]
        public Result OverlayExecute([FromBody][Required]OverlayReqDTO dto)
        {
            if (null == dto)
            {
                dto = new OverlayReqDTO();
            }
            JObject obj = new JObject
            {
                { "Name", "TZFAFW" }
            };
            JObject sourceObj = new JObject();
            obj.Add("Source", sourceObj);
            sourceObj.Add("SysCode", "30143df1123449a896429854899f37f3");
            sourceObj.Add("IsLocal", 1);
            sourceObj.Add("Type", "PERSONAL_GEODATABASE");
            sourceObj.Add("Connection", @"{""Path"":""D:/work/dist/x_项目管理/f_福建省/x_厦门/02数据/控规调整样例.mdb""}");
            dto.Parameters.Add("SourceFeatureClass", obj.ToString());
            // dto.Parameters.Add("SourceFeatureClass", "{\"Name\":\"TZFAFW\",\"Source\":{\"SysCode\":\"30143df1123449a896429854899f37f3\",\"IsLocal\":1,\"Type\":\"PERSONAL_GEODATABASE\",\"Connection\":\"{\"Path\":\"D:\\work\\dist\\x_项目管理\f_福建省\\x_厦门\\02数据\\控规调整样例.mdb\"}\"}}");

            obj = new JObject
            {
                { "Name", "STKZXFW_YDFW" }
            };
            sourceObj = new JObject();
            obj.Add("Source", sourceObj);
            sourceObj.Add("SysCode", "791b05180d8c4e2186f7684ecf557457");
            sourceObj.Add("IsLocal", 0);
            sourceObj.Add("Type", "ENTERPRISE_GEODATABASE");
            JObject connObj = new JObject
            {
                { "name", "厦门空间库" },
                { "server", "192.168.1.166" },
                { "database", "orcl" },
                { "port", 1521},
                { "username", "xmgis"},
                { "encrypted", 0},
                { "password", "xmghj2014"}
            };
            sourceObj.Add("Connection", connObj.ToString());

            //sourceObj.Add("Connection", "{\"path\":\"D:\\work\\dist\\x_项目管理\\f_福建省\\x_厦门\\02数据\\控规调整样例.mdb\"}");
            dto.Parameters.Add("TargetFeatureClass", obj.ToString());
            // dto.Parameters.Add("TargetFeatureClass", "{\"Name\":\"STKZXFW_YDFW\", \"Source\":{\"SysCode\":\"30143df1123449a896429854899f37f3\",\"IsLocal\":1,\"Type\":\"PERSONAL_GEODATABASE\",\"Connection\":\"{\"name\":\"厦门空间库\",\"server\":\"192.168.200.38\",\"database\":\"orcl\",\"port\":1521,\"username\":\"xmgis\",\"encrypted\":0,\"password\":\"xmghj2014\"}\"}}");

            dto.Parameters.Add("AnalysisType", 0);
            dto.Parameters.Add("IsClearTemp", true);
            return base.Success(ModelService.OverlayExecute(dto.Parameters));
        }
        /// <summary>
        /// 运行模型计算
        /// </summary>
        /// <param name="modelVersionCode">模型版本唯一编码</param>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/run/{modelVersionCode}")]
        public Result Run(string modelVersionCode)
        {
            return base.Success(this.ModelService.RunModelAsync(modelVersionCode), "已开始模型的云计算......");
        }
        /// <summary>
        /// 注册模型
        /// </summary>
        /// <param name="dto">参数模型，当SysCode为空时，系统会自动附上一个唯一编码</param>
        /// <returns></returns>
        [HttpPost]
        [Route("register/v1")]
        public Result NewModel([FromBody]ModelAddReqDTO dto)
        {
            if (!this.ModelService.IsBizGuid(dto.SysCode))
            {
                return base.Fail($"业务编码格式不正确[{dto.SysCode}]");
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
        public Result CopyFromModelVersion(string modelVersionCode)
        {
            return base.Success(this.ModelService.CopyFromModelVersion(modelVersionCode));
        }
        /// <summary>
        /// 保存整个模型的规则步骤信息
        /// </summary>
        /// <param name="info">步骤信息</param>
        /// <returns></returns>
        [HttpPost, HttpPut]
        [Route("rulesteps/v1/info")]
        public Result SaveRuleStepInfos([FromBody] ModelRuleStepInfoDTO info)
        {
            if (!ModelState.IsValid)
            {
                return base.Error(ModelState);
            }
            return base.Success(this.ModelService.SaveRuleStepInfos(info));
        }
        /// <summary>
        /// 获取任务清单，以创建时间倒序
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("tasks/v1")]
        public Result ListTask()
        {
            return base.Success(this.ModelService.ListTask());
        }
        /// <summary>
        /// 获取任务的所有步骤计算结果
        /// </summary>
        /// <param name="taskCode">任务唯一编码</param>
        /// <returns></returns>
        [HttpGet]
        [Route("tasks/result/v1/{taskCode}")]
        public Result GetTaskOutput(string taskCode)
        {
            return base.Success(this.ModelService.GetTaskResult(taskCode, -1));
        }
        /// <summary>
        /// 获取任务指定步骤的计算结果
        /// </summary>
        /// <param name="taskCode">任务编码</param>
        /// <param name="ruleStepId">步骤id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("tasks/result/v1/{taskCode}/{ruleStepId}")]
        public Result GetTaskOutput(string taskCode, int ruleStepId)
        {
            return base.Success(this.ModelService.GetTaskResult(taskCode, ruleStepId));
        }
    }
}