using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Model.DTO;
using System;
using System.Collections.Generic;
using System.Text;

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
        /// <returns></returns>
        object ListModels(Boolean detail);
        /// <summary>
        /// 添加模型
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        object AddModel(ModelAddReqDTO dto);
        /// <summary>
        /// 执行模型（背后依赖的是算法）
        /// </summary>
        /// <param name="code">模型唯一编码</param>
        /// <param name="versionCode">版本唯一编码</param>
        /// <returns></returns>
        object ExecuteModel(string code, string versionCode);
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
        object LandConflictExecute(IDictionary<String, Object> parameters);

        /// <summary>
        /// 复制模型的一个版本
        /// </summary>
        /// <param name="versionCode">版本唯一编码</param>
        /// <returns></returns>
        object CopyModelVersion(string versionCode);
        /// <summary>
        /// 保存模型的规则步骤信息
        /// </summary>
        /// <param name="info">信息</param>
        /// <returns></returns>
        object SaveRuleStepInfos(ModelRuleStepInfoDTO info);
    }
}
