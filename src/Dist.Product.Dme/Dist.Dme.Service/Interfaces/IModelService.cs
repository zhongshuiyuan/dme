﻿using Dist.Dme.Base.Framework.Interfaces;
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
        /// <returns></returns>
        object ListModels(Boolean detail, int isPublish);
        /// <summary>
        /// 添加模型
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        object AddModel(ModelAddReqDTO dto);
        /// <summary>
        /// 异步运行模型（背后依赖的是算法）
        /// </summary>
        /// <param name="versionCode">模型版本唯一编码</param>
        /// <returns></returns>
        DmeTask RunModelAsync(string versionCode);
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
        /// 压盖分析
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        object OverlayExecute(IDictionary<String, Object> parameters);
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
        /// 获取任务清单，以创建时间倒序
        /// </summary>
        /// <returns></returns>
        object ListTask();
        /// <summary>
        /// 获取任务步骤的执行结果
        /// </summary>
        /// <param name="taskCode"></param>
        /// <param name="ruleStepId"></param>
        /// <returns></returns>
        object GetTaskResult(string taskCode, int ruleStepId);
        /// <summary>
        /// 发布模型
        /// </summary>
        /// <param name="modelCode">模型唯一编码</param>
        /// <param name="isPublish">是否发布，1：发布；0：取消发布</param>
        /// <returns></returns>
        object PublishModel(string modelCode, int isPublish);
    }
}
