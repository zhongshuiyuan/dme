using Dist.Dme.Base.Common;
using Dist.Dme.Base.Conf;
using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Base.Utils;
using Dist.Dme.DisFS.Adapters.Mongo;
using Dist.Dme.Extensions;
using Dist.Dme.Model.DTO;
using Dist.Dme.Model.Entity;
using Dist.Dme.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dist.Dme.WebApi.Controllers
{
    /// <summary>
    /// 模型服务
    /// </summary>
    [Route("api/models")]
    public class ModelController : CommonController
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 模型服务
        /// </summary>
        
        //public ModelController(IModelService modelService)
        //{
        //    this.ModelService = modelService;
        //}
        /// <summary>
        /// 添加模型类型
        /// </summary>
        /// <param name="types">类型名称数组</param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/types")]
        public async Task<Result> AddModelTypesAsync([FromBody]string[] types)
        {
            return  base.Success(await base.modelService.AddModelTypesAsync(types));
        }
        /// <summary>
        /// 更新模型类型
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("v1/types")]
        public Result UpdateModelTypes([FromBody] ModelTypeUpdateDTO dto )
        {
            return base.Success(base.modelService.UpdateModelTypes(dto));
        }
        /// <summary>
        /// 删除模型类型
        /// </summary>
        /// <param name="code">类型唯一编码</param>
        /// <returns>true/false</returns>
        [HttpDelete]
        [Route("v1/types/{code}")]
        public Result DeleteModelTypes(string code)
        {
            return base.Success(base.modelService.DeleteModelTypes(code));
        }
        /// <summary>
        ///  获取模型类型列表
        /// </summary>
        /// <param name="orderFieldName">排序字段名称（仅限一个）</param>
        /// <param name="orderType">排序类型，升序：0，降序：1</param>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/types")]
        public Result ListModelTypes([FromQuery(Name = "orderfieldname")]string orderFieldName, [FromQuery(Name ="ordertype")]int orderType = 0)
        {
            if (orderType < 0 || orderType > 1)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"输入参数orderType值[{orderType}]有误，不允许小于0或大于1");
            }
            return base.Success(base.modelService.ListModelTypes(orderFieldName, orderType));
        }
        /// <summary>
        /// 获取所有模型
        /// </summary>
        /// <param name="detail">是否获取详情信息，0：否；1：是</param>
        /// <param name="isPublish">是否被发布的模型，1：已发布；0：未发布；-1：全部</param>
        /// <param name="status">模型状态，0：被删除；1：正常</param>
        /// <returns></returns>
        [HttpGet]
        [Route("v1")]
        public Result ListModels(
            [FromQuery(Name = "detail")] int detail = 0, 
            [FromQuery(Name = "ispublish")] int isPublish = 1,
            [FromQuery(Name = "status")] int status = 1)
        {
            return base.Success(modelService.ListModels(1 == detail, isPublish, EnumUtil.GetEnumObjByValue<EnumModelStatus>(status)));
        }
        /// <summary>
        /// 根据模型唯一编码获取模型
        /// </summary>
        /// <param name="code">模型唯一编码</param>
        /// <param name="detail">是否获取详情，0：否；1：是</param>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/{code}")]
        public Result GetModel(string code, [FromQuery(Name = "detail")] int detail =0)
        {
            if (!this.modelService.IsBizGuid(code))
            {
                return base.Fail($"业务编码格式不正确[{code}]");
            }
            return base.Success(base.modelService.GetModelMetadata(code, 1 == detail));
        }
        /// <summary>
        /// 获取所有的规则步骤类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("rulesteptypes/v1")]
        public Result ListRuleStepTypes()
        {
            return base.Success(base.modelService.ListRuleStepTypes());
        }
        /// <summary>
        /// 更新模型基本信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("v1/basicinfo")]
        public Result UpdateModelBasicInfo([FromBody]ModelBasicInfoUpdateDTO dto)
        {
            return base.Success(base.modelService.UpdateModelBasicInfo(dto));
        }
        /// <summary>
        /// 添加模型版本
        /// </summary>
        /// <param name="dto">参数模型</param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/version")]
        public Result AddModelVersion([FromBody]ModelVersionSimpleAddDTO dto)
        {
            return base.Success(base.modelService.AddModelVersion(dto));
        }
        /// <summary>
        /// 更新模型版本信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("v1/version")]
        public Result UpdateModelVersion([FromBody]ModelVersionUpdateDTO dto)
        {
            return base.Success(base.modelService.UpdateModelVersion(dto));
        }
        /// <summary>
        /// 模型注册
        /// </summary>
        /// <param name="dto">参数模型，当SysCode为空时，系统会自动附上一个唯一编码</param>
        /// <returns></returns>
        [HttpPost]
        [Route("register/v1")]
        public Result RegisterModelSimple([FromBody]ModelAddReqDTO dto)
        {
            if (!string.IsNullOrEmpty(dto.SysCode) && !this.modelService.IsBizGuid(dto.SysCode))
            {
                return base.Fail($"业务编码格式不正确[{dto.SysCode}]");
            }
            if (ModelState.IsValid)
            {
                return base.Success(base.modelService.AddModel(dto));
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
            return base.Success(base.modelService.CopyFromModelVersion(modelVersionCode));
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
            return base.Success(base.modelService.SaveRuleStepInfos(info));
        }
        
        /// <summary>
        /// 设置模型的发布状态
        /// </summary>
        /// <param name="modelCode">模型唯一编码</param>
        /// <param name="isPublish">是否发布，1：发布：0：取消发布</param>
        /// <returns></returns>
        [HttpGet]
        [Route("publish/v1/{modelCode}/{isPublish}")]
        public Result PublishModel(string modelCode, int isPublish)
        {
            return base.Success(base.modelService.PublishModel(modelCode, isPublish));
        }
        /// <summary>
        /// 上传模型图片
        /// </summary>
        /// <param name="modelVersionCode">模型版本唯一编码</param>
        /// <param name="file">文档，注意：前端传过来的form表单数据key为file</param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/img/{modelVersionCode}")]
        public Result UploadModelImg(string modelVersionCode, [FromForm]IFormFile file)
        {
            if (file?.Length > 0)
            {
                string suffix = file.FileName.Substring(file.FileName.LastIndexOf("."));
                string localFileName = GuidUtil.NewGuid() + suffix;
                GridFSUploadOptions options = new GridFSUploadOptions
                {
                    Metadata = new BsonDocument(new Dictionary<string, object>() { ["ContentType"]= file.ContentType})
                };
                ObjectId objectId = MongodbHelper<object>.UploadFileFromStream(ServiceFactory.MongoDatabase, localFileName, file.OpenReadStream(), options);
                try
                {
                    return base.Success(base.modelService.AddModelImg(modelVersionCode, file.FileName, suffix, file.ContentType, objectId.ToString()));
                }
                catch (Exception ex)
                {
                    LOG.Error(ex, "模型图片数据库持久化失败，详情："+ex.Message);
                    if (objectId != null)
                    {
                        LOG.Warn($"文件[{objectId}]撤销保存");
                        MongodbHelper<object>.DeleteFileById(ServiceFactory.MongoDatabase, objectId);
                    }
                }
            }
            return base.Fail("上传失败，没有获取到图片数据");
        }
        /// <summary>
        /// 获取模型图片
        /// </summary>
        /// <param name="modelVersionCode">模型版本编码</param>
        /// <returns>FileContentResult</returns>
        [HttpGet]
        [Route("v1/img/{modelVersionCode}")]
        public IActionResult GetModelImg(string modelVersionCode)
        {
            String baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string defaultImgPath = baseDir + GlobalSystemConfig.PATH_DEFAULT_IMG;
            string contentType = "image/jpeg";
            byte[] bytes = null;

            DmeModelImg dmeModelImg = base.modelService.GetModelImg(modelVersionCode);
            if (null == dmeModelImg)
            {
                LOG.Warn($"模型版本[{modelVersionCode}]还没设置图片，使用默认图片");
                if (!System.IO.File.Exists(defaultImgPath))
                {
                    throw new BusinessException($"默认图片[{defaultImgPath}]不存在");
                }
                bytes = System.IO.File.ReadAllBytes(defaultImgPath);
            }
            else
            {
                //从mongodb中读取流，DownloadFileToStream有问题
                bytes = MongodbHelper<object>.DownloadFileAsByteArray(ServiceFactory.MongoDatabase, new ObjectId(dmeModelImg.ImgCode));
                //imgStream = FileUtil.BytesToStream(bytes);
                contentType = dmeModelImg.ContentType;
            }
            // byte[] buffer = new byte[imgStream.Length];
            //读取图片字节流
            // imgStream.Read(buffer, 0, Convert.ToInt32(imgStream.Length));
            var response = File(bytes, contentType);
            // imgStream.Close();
            return response;
        }
        /// <summary>
        /// 逻辑删除模型
        /// </summary>
        /// <param name="modelCode">模型编码</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("v1/{modelCode}")]
        public async Task<Result> DeleteModelAsync(string modelCode)
        {
            Boolean result = await modelService.DeleteModelAsync(modelCode);
            if (result)
            {
                return base.Success(result);
            }
            return base.Fail("删除模型失败，详情请管理员查看具体日志信息。");
        }
        /// <summary>
        /// 还原模型
        /// </summary>
        /// <param name="modelCode">模型唯一编码</param>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/restore/{modelCode}")]
        public async Task<Result> RestoreModelAsync(string modelCode)
        {
            Boolean result = await base.modelService.RestoreModelAsync(modelCode);
            if (result)
            {
                return base.Success(result);
            }
            return base.Fail("删除模型失败，详情请管理员查看具体日志信息。");
        }
        /// <summary>
        /// 获取模型运行时参数
        /// </summary>
        /// <param name="modelVersionCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/runtime/attributes/{modelVersionCode}")]
        public Result GetRuntimeAttributes(string modelVersionCode)
        {
            return base.Success(base.modelService.GetRuntimeAttributes(modelVersionCode));
        }
    }
}