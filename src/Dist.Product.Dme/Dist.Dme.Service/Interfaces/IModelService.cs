﻿using Dist.Dme.Model.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Service.Interfaces
{
    /// <summary>
    /// 模型服务
    /// </summary>
    public interface IModelService
    {
        /// <summary>
        /// 获取模型集合
        /// </summary>
        /// <param name="refAlgorithm">是否获取关联的算法</param>
        /// <returns></returns>
        object ListModels(Boolean refAlgorithm);
        /// <summary>
        /// 添加模型
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        object AddModel(ModelAddReqDTO dto);
        /// <summary>
        /// 根据模型唯一编码，获取模型的元数据信息
        /// </summary>
        /// <param name="modelCode">模型编码</param>
        /// <param name="refAlgorithm">是否获取关联的算法</param>
        /// <returns></returns>
        object GetModelMetadata(String modelCode, Boolean refAlgorithm);
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
    }
}