using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Model.DTO;
using Dist.Dme.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dist.Dme.Service.Interfaces
{
    /// <summary>
    /// 模型服务
    /// </summary>
    public interface IModelService : IBizService
    {
        /// <summary>
        /// 获取模型集合
        /// </summary>
        /// <param name="detail">是否获取模型的详情信息</param>
        /// <param name="isPublish">是否被发布，1：已发布；0：未发布；-1：全部</param>
        /// <param name="enumModelStatus">模型状态，0：被删除；1：正常</param>
        /// <returns></returns>
        object ListModels(Boolean detail, int isPublish, EnumModelStatus enumModelStatus);
        /// <summary>
        /// 添加模型，只是注册简单的信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        object AddModel(ModelAddReqDTO dto);
        /// <summary>
        /// 根据模型唯一编码，获取模型的元数据信息
        /// </summary>
        /// <param name="modelCode">模型编码</param>
        /// <param name="detail">是否获取详情</param>
        /// <returns></returns>
        object GetModelMetadata(String modelCode, Boolean detail);
        /// <summary>
        /// 获取用地冲突分析元数据信息
        /// </summary>
        /// <returns></returns>
        object GetLandConflictMetadata();
        /// <summary>
        /// 用地冲突分析计算
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        object LandConflictExecute(IDictionary<String, Property> parameters);
        /// <summary>
        /// 压盖分析
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        object OverlayExecute(IDictionary<String, Property> parameters);
        /// <summary>
        /// 复制模型的一个版本
        /// </summary>
        /// <param name="versionCode">版本唯一编码</param>
        /// <returns></returns>
        object CopyModelVersion(string versionCode);

        /// <summary>
        /// 获取所有的规则步骤类型
        /// </summary>
        /// <returns></returns>
        object ListRuleStepTypes();

        /// <summary>
        /// 保存模型的规则步骤信息
        /// </summary>
        /// <param name="info">信息</param>
        /// <returns></returns>
        object SaveRuleStepInfos(ModelRuleStepInfoDTO info);
        /// <summary>
        /// 复制一个模型版本
        /// </summary>
        /// <param name="modelVersionCode"></param>
        /// <returns></returns>
        object CopyFromModelVersion(string modelVersionCode);
        /// <summary>
        /// 发布模型
        /// </summary>
        /// <param name="modelCode">模型唯一编码</param>
        /// <param name="isPublish">是否发布，1：发布；0：取消发布</param>
        /// <returns></returns>
        object PublishModel(string modelCode, int isPublish);
        /// <summary>
        /// 校验模型
        /// </summary>
        /// <param name="modelCode">模型唯一编码</param>
        /// <returns></returns>
        object ValidModel(string modelCode);
        /// <summary>
        /// 添加模型图片
        /// </summary>
        /// <param name="modelVersionCode"></param>
        /// <param name="sourceName"></param>
        /// <param name="suffix"></param>
        /// <param name="contentType">类型</param>
        /// <param name="objectId"></param>
        /// <returns></returns>
        object AddModelImg(string modelVersionCode, string sourceName, string suffix, string contentType, string objectId);
        /// <summary>
        /// 获取模型图片
        /// </summary>
        /// <param name="modelVersionCode"></param>
        /// <returns></returns>
        DmeModelImg GetModelImg(string modelVersionCode);
        /// <summary>
        /// 逻辑删除模型，级联逻辑删除版本
        /// </summary>
        /// <param name="modelCode">模型编码</param>
        /// <returns></returns>
        Task<Boolean> DeleteModelAsync(string modelCode);
        /// <summary>
        /// 恢复模型
        /// </summary>
        /// <param name="modelCode">模型编码</param>
        /// <returns></returns>
        Task<bool> RestoreModelAsync(string modelCode);
        /// <summary>
        /// 添加模型类型
        /// </summary>
        /// <param name="types">类型名称数组</param>
        /// <returns></returns>
        Task<object> AddModelTypesAsync(string[] types);
        /// <summary>
        /// 获取模型类型列表，如果orderFieldName为空，则以创建时间倒序
        /// </summary>
        /// <param name="orderFieldName">排序字段名称</param>
        /// <param name="orderType">排序类型，0：升序；1：降序</param>
        /// <returns></returns>
        object ListModelTypes(string orderFieldName, int orderType);
        /// <summary>
        /// 更新模型类型
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        object UpdateModelTypes(ModelTypeUpdateDTO dto);
        /// <summary>
        /// 更新模型基本信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        object UpdateModelBasicInfo(ModelBasicInfoUpdateDTO dto);
        /// <summary>
        /// 更新模型版本信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        object UpdateModelVersion(ModelVersionUpdateDTO dto);
    }
}
