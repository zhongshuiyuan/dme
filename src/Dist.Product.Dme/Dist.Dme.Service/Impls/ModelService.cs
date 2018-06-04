using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.DAL.Context;
using Dist.Dme.Model.DTO;
using Dist.Dme.Model.Entity;
using Dist.Dme.Plugins.LandConflictDetection;
using Dist.Dme.Service.Interfaces;
using log4net;
using SqlSugar;
using System;
using System.Collections.Generic;

namespace Dist.Dme.Service.Impls
{
    /// <summary>
    /// 模型服务
    /// </summary>
    public class ModelService : BaseBizService, IModelService
    {
        private static ILog LOG = LogManager.GetLogger(typeof(ModelService));
        /// <summary>
        /// 用地差异分析
        /// </summary>
        private IAlgorithm landConflictDetectionAlgorithm = new LandConflictDetectionMain();
        /// <summary>
        /// 自动注入参数
        /// </summary>
        /// <param name="repository"></param>
        public ModelService(IRepository repository) {
            base.Repository = repository;
        }
        public object GetLandConflictMetadata()
        {
            return this.landConflictDetectionAlgorithm.MetadataJSON;
        }

        public object GetModelMetadata(string modelCode, Boolean detail)
        {
            DmeModel model = base.Repository.GetDbContext().Queryable<DmeModel>().Single(m => m.SysCode == modelCode);
            return this.GetModelMetadata(model, detail);
        }
        /// <summary>
        /// 内部方法，获取模型元数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        private ModelDTO GetModelMetadata(DmeModel model, Boolean detail)
        {
            ModelDTO modelDTO = ClassValueCopier<ModelDTO>.Copy(model);

            if (!detail)
            {
                return modelDTO;
            }
            // 获取模型版本
            IList<DmeModelVersion> versions = base.Repository.GetDbContext().Queryable<DmeModelVersion>().Where(mv => mv.ModelId == model.Id).ToList();
            if (null == versions || 0 == versions.Count)
            {
                return modelDTO;
            }
            
            foreach (var v in versions)
            {
                ModelVersionDTO versionDTO = ClassValueCopier<ModelVersionDTO>.Copy(v);
                modelDTO.Versions.Add(versionDTO);
                IList<DmeRuleStep> ruleStepEntities = base.Repository.GetDbContext().Queryable<DmeRuleStep>().Where(rs => rs.ModelId == model.Id && rs.VersionId == v.Id).ToList();
                if (null == ruleStepEntities || 0 == ruleStepEntities.Count)
                {
                    continue;
                }
                RuleStepDTO ruleStepDTO = null;
                foreach (var rule in ruleStepEntities)
                {
                    ruleStepDTO = ClassValueCopier<RuleStepDTO>.Copy(rule);
                    versionDTO.Steps.Add(ruleStepDTO);
                    // 获取步骤类型实体
                    ruleStepDTO.StepType = base.Repository.GetDbContext().Queryable<DmeRuleStepType>().Where(rst => rst.Id == rule.StepTypeId).Single();
                    // 检索步骤的属性值信息
                    IList<DmeRuleStepAttribute> attributes = base.Repository.GetDbContext().Queryable<DmeRuleStepAttribute>().Where(rsa => rsa.RuleStepId == rule.Id).ToList();
                    if (null == attributes || 0 == attributes.Count)
                    {
                        continue;
                    }
                    foreach (var att in attributes)
                    {
                        ruleStepDTO.Attributes.Add(new KeyValuePair<string, object>(att.AttributeCode, att.AttributeValue));
                    }
                }
            }
           
            return modelDTO;
        }

        public object LandConflictExecute(IDictionary<string, object> parameters)
        {
            this.landConflictDetectionAlgorithm.Init(parameters);
            return this.landConflictDetectionAlgorithm.Execute();
        }
        public object ListModels(Boolean detail)
        {
            // 倒序
            List<DmeModel> models = base.Repository.GetDbContext().Queryable<DmeModel>().OrderBy(m => m.CreateTime, OrderByType.Desc).ToList();
            if (null == models || 0 == models.Count)
            {
                return models;
            }
            if (detail)
            {
                // 获取模型的详情信息
                IList<ModelDTO> modelDTOs = new List<ModelDTO>();
                foreach (DmeModel m in models)
                {
                    modelDTOs.Add(this.GetModelMetadata(m, detail));
                }
                return modelDTOs;
            }
            else
            {
                return models;
            } 
        }

        public object AddModel(ModelAddReqDTO dto)
        {
            if (string.IsNullOrEmpty(dto.SysCode))
            {
                // 自动赋予一个唯一编码
                dto.SysCode = GuidUtil.NewGuid();
            }
            // 使用事务
            DbResult< DmeModel > dbResult = base.Repository.GetDbContext().Ado.UseTran<DmeModel>(()=> 
            {
                // 查询单条没有数据返回NULL, Single超过1条会报错，First不会
                // base.DmeModelDb.GetContext().Queryable<DmeModel>().Single(m => m.SysCode == dto.SysCode);
                DmeModel model = base.Repository.GetDbContext().Queryable<DmeModel>().Where(m => m.SysCode == dto.SysCode).Single();
                if (null == model)
                {
                    model = ClassValueCopier<DmeModel>.Copy(dto);
                    model.CreateTime = DateUtil.CurrentTimeMillis;
                    int identity = base.Repository.GetDbContext().Insertable<DmeModel>(model).ExecuteReturnIdentity();
                    if (identity <= 0)
                    {
                        throw new Exception(String.Format("创建模型失败，编码：[{0}]，名称：[{1}]", dto.SysCode, dto.Name));
                    }
                    // 同时默认添加一个版本
                    DmeModelVersion mv = new DmeModelVersion
                    {
                        SysCode = GuidUtil.NewGuid(),
                        Name = "版本1",
                        ModelId = identity,
                        CreateTime = DateUtil.CurrentTimeMillis
                    };
                    base.Repository.GetDbContext().Insertable<DmeModelVersion>(mv).ExecuteReturnEntity();
                }
                // 重复的模型不再添加，直接返回已存在的模型
                return model;
            });
            return dbResult.Data;
        }

        public object ExecuteModel(string code, string versionCode)
        {
            // Single方法，如果查询数据库没有数据，抛出异常
            DmeModel model = base.Repository.GetDbContext().Queryable<DmeModel>().Single(m => m.SysCode == code);
            // 查询模型版本
            DmeModelVersion modelVersion = base.Repository.GetDbContext().Queryable<DmeModelVersion>().Single(mv => mv.SysCode == versionCode);
            // 查找关联的算法信息
            IList<DmeRuleStep> ruleSteps = base.Repository.GetDbContext().Queryable<DmeRuleStep>().Where(rs => rs.ModelId == model.Id).Where(rs => rs.VersionId == modelVersion.Id).ToList();

            throw new NotImplementedException();
        }

        public object CopyModelVersion(string versionCode)
        {
            DmeModelVersion modelVersion = base.Repository.GetDbContext().Queryable<DmeModelVersion>().Where(mv => mv.SysCode == versionCode).Single();
            if (null == modelVersion)
            {
                throw new BusinessException(SystemStatusCode.DME2000, $"模型版本[{versionCode}]不存在，或模型版本编码无效");
            }

            throw new NotImplementedException();
        }

        public object SaveRuleStepInfos(ModelRuleStepInfoDTO info)
        {
            // 开始事务
            return base.Repository.GetDbContext().Ado.UseTran<object>(()=>
            {
                // 根据模型版本号，获取模型版本信息
                DmeModelVersion modelVersion = base.Repository.GetDbContext().Queryable<DmeModelVersion>().Single(mv => mv.SysCode == info.ModelVersionCode);
                // 清除模型的步骤信息
                base.Repository.GetDbContext().Deleteable<DmeRuleStep>(rs => rs.ModelId == modelVersion.ModelId && rs.VersionId == modelVersion.Id).ExecuteCommand();
                // 清除步骤属性信息
                base.Repository.GetDbContext().Deleteable<DmeRuleStepAttribute>(rsa => rsa.ModelId == modelVersion.ModelId && rsa.VersionId == modelVersion.Id).ExecuteCommand();
                // 根据key建立起关系
                if (info.RuleSteps?.Count > 0)
                {
                    // 客户端传过来的key和业务id映射关系
                    IDictionary<string, int> key2BizId = new Dictionary<string, int>();
                    foreach (var step in info.RuleSteps)
                    {
                        DmeRuleStep stepEntity = new DmeRuleStep
                        {
                            SysCode = GuidUtil.NewGuid(),
                            ModelId = modelVersion.ModelId,
                            VersionId = modelVersion.Id
                        };
                        stepEntity.Id = base.Repository.GetDbContext().Insertable<DmeRuleStep>(stepEntity).ExecuteReturnIdentity();
                        key2BizId[step.Key] = stepEntity.Id;
                        // 处理步骤属性
                        if (step.Attributes?.Count > 0)
                        {
                            IList<DmeRuleStepAttribute> attributeEntities = new List<DmeRuleStepAttribute>();
                            foreach (var att in step.Attributes)
                            {
                                attributeEntities.Add(new DmeRuleStepAttribute
                                {
                                    RuleStepId = stepEntity.Id,
                                    ModelId = stepEntity.ModelId,
                                    VersionId = stepEntity.VersionId,
                                    AttributeCode = att.Key,
                                    AttributeValue = att.Value
                                });
                            }
                            base.Repository.GetDbContext().Insertable<DmeRuleStepAttribute>(attributeEntities).ExecuteCommand();
                        }
                    }
                    // 处理步骤的向量关系
                    if (info.Vectors?.Count > 0)
                    {
                        IList<DmeRuleStepHop> ruleStepHops = new List<DmeRuleStepHop>();
                        foreach (var vector in info.Vectors)
                        {
                            // 只要向量的信息不完整，都不需要保存连接信息
                            if (!key2BizId.ContainsKey(vector.FromKey) || !key2BizId.ContainsKey(vector.ToKey))
                            {
                                continue;
                            }
                            ruleStepHops.Add(new DmeRuleStepHop
                            {
                                ModelId = modelVersion.ModelId,
                                VersionId = modelVersion.Id,
                                StepFromId = key2BizId[vector.FromKey],
                                StepToId = key2BizId[vector.ToKey],
                                Enabled = vector.Enabled
                            });
                        }
                        base.Repository.GetDbContext().Insertable<DmeRuleStepHop>(ruleStepHops).ExecuteCommandAsync();
                    }
                }
                return true;
            }).Data;
        }
        public object CopyFromModelVersion(string modelVersionCode)
        {
            // 开始事务
            return base.Repository.GetDbContext().Ado.UseTran<object>(() =>
            {
                // 根据模型版本号，获取模型版本信息
                DmeModelVersion modelVersion = base.Repository.GetDbContext().Queryable<DmeModelVersion>().Single(mv => mv.SysCode == modelVersionCode);
                DmeModelVersion newVersion = new DmeModelVersion
                {
                    CreateTime = DateUtil.CurrentTimeMillis,
                    ModelId = modelVersion.ModelId
                };
                newVersion.Name = $"版本-{newVersion.CreateTime}";
                newVersion.SysCode = GuidUtil.NewGuid();
                newVersion.Id = base.Repository.GetDbContext().Insertable<DmeModelVersion>(newVersion).ExecuteReturnIdentity();
                // 获取被复制的版本的步骤信息
                IList<DmeRuleStep> oldRuleSteps = base.Repository.GetDbContext().Queryable<DmeRuleStep>().Where(rs => rs.ModelId == modelVersion.ModelId && rs.VersionId == modelVersion.Id).ToList();
                if (oldRuleSteps?.Count > 0)
                {
                    foreach (var step in oldRuleSteps)
                    {
                        DmeRuleStep newRuleStep = ClassValueCopier<DmeRuleStep>.Copy(step, new String[] { "Id", "SysCode", "VersionId" });
                        newRuleStep.SysCode = GuidUtil.NewGuid();
                        newRuleStep.VersionId = newVersion.Id;
                        newRuleStep.Id = base.Repository.GetDbContext().Insertable<DmeRuleStep>(newRuleStep).ExecuteReturnIdentity();
                        // 获取步骤的属性信息
                        IList<DmeRuleStepAttribute> oldRuleStepAttributes = base.Repository.GetDbContext().Queryable<DmeRuleStepAttribute>().Where(rsa => rsa.ModelId == modelVersion.ModelId && rsa.VersionId == modelVersion.Id).ToList();
                        if (oldRuleStepAttributes?.Count > 0)
                        {
                            List<DmeRuleStepAttribute> newRuleStepAtts = new List<DmeRuleStepAttribute>();
                            foreach (var att in oldRuleStepAttributes)
                            {
                                DmeRuleStepAttribute newRuleStepAtt = ClassValueCopier<DmeRuleStepAttribute>.Copy(att, new String[] { "Id", "RuleStepId", "VersionId" });
                                newRuleStepAtt.RuleStepId = newRuleStep.Id;
                                newRuleStepAtt.VersionId = newVersion.Id;
                                newRuleStepAtts.Add(newRuleStepAtt);
                            }
                            base.Repository.GetDbContext().Insertable<DmeRuleStepAttribute>(newRuleStepAtts.ToArray()).ExecuteCommand();
                        }
                    }
                }
                return true;
            }).Data;
        }
    }
}
