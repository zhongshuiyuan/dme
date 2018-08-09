using Dist.Dme.Algorithms.LandConflictDetection;
using Dist.Dme.Algorithms.Overlay;
using Dist.Dme.Base.Common;
using Dist.Dme.Base.Common.Log;
using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Framework.Collections;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.DAL.Context;
using Dist.Dme.DisFS.Adapters.Mongo;
using Dist.Dme.DisFS.Collection;
using Dist.Dme.Extensions;
using Dist.Dme.Extensions.DTO;
using Dist.Dme.HSMessage.Define;
using Dist.Dme.HSMessage.MQ.Kafka;
using Dist.Dme.Model.DTO;
using Dist.Dme.Model.Entity;
using Dist.Dme.RuleSteps;
using Dist.Dme.RuleSteps.AlgorithmInput;
using Dist.Dme.Scheduler;
using Dist.Dme.Service.Interfaces;
using Dist.Dme.Service.Scheduler;
using MongoDB.Driver;
using NLog;
using SqlSugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Dist.Dme.Service.Impls
{
    /// <summary>
    /// 模型服务
    /// </summary>
    public class ModelService : BaseBizService, IModelService
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 用地差异分析
        /// </summary>
        private IAlgorithm landConflictDetectionAlgorithm = new LandConflictDetectionMain();
        private IAlgorithm OverlayAlg = new OverlayMain();
        /// <summary>
        /// 自动注入参数
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="logService"></param>
        public ModelService(IRepository repository, ILogService logService) {
            base.Repository = repository;
            base.LogService = logService;
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
            var db = base.Repository.GetDbContext();
            // 获取模型版本
            IList<DmeModelVersion> versions = db.Queryable<DmeModelVersion>().Where(mv => mv.ModelId == model.Id).ToList();
            if (null == versions || 0 == versions.Count)
            {
                return modelDTO;
            }

            foreach (var v in versions)
            {
                ModelVersionDTO versionDTO = ClassValueCopier<ModelVersionDTO>.Copy(v);
                modelDTO.Versions.Add(versionDTO);
                IList<DmeRuleStep> ruleStepEntities = db.Queryable<DmeRuleStep>().Where(rs => rs.ModelId == model.Id && rs.VersionId == v.Id).ToList();
                if (null == ruleStepEntities || 0 == ruleStepEntities.Count)
                {
                    continue;
                }
                RuleStepDTO ruleStepDTO = null;
                foreach (var ruleStep in ruleStepEntities)
                {
                    ruleStepDTO = ClassValueCopier<RuleStepDTO>.Copy(ruleStep);
                    versionDTO.Steps.Add(ruleStepDTO);
                    // 获取步骤类型实体
                    ruleStepDTO.StepType = db.Queryable<DmeRuleStepType>().Where(rst => rst.Id == ruleStep.StepTypeId).Single();
                    IRuleStepData ruleStepData = RuleStepFactory.GetRuleStepData(ruleStepDTO.StepType.Code, base.Repository, null, ruleStep);
                    IDictionary<string, Property> attributeDic = ruleStepData.RuleStepMeta.ReadAttributes();
                    if (attributeDic?.Count > 0)
                    {
                        foreach (var item in attributeDic.Values)
                        {
                            ruleStepDTO.Attributes.Add(item);
                        }
                    }
                    // 检索步骤的属性值信息
                    //IList<DmeRuleStepAttribute> attributes = db.Queryable<DmeRuleStepAttribute>().Where(rsa => rsa.RuleStepId == ruleStep.Id).ToList();
                    //if (null == attributes || 0 == attributes.Count)
                    //{
                    //    continue;
                    //}
                    //foreach (var att in attributes)
                    //{
                    //    ruleStepDTO.Attributes.Add(new KeyValuePair<string, object>(att.AttributeCode, att.AttributeValue));
                    //}
                }
                // 每个模型版本下的数据源（多表关联查询）
                // 当不需要用LEFT JOIN或者 RIGHT JOIN 只是单纯的INNER JOIN时我们还提供了更简单的语法实现多表查询
                IList<DmeDataSource> datasources = db.Queryable<DmeDataSource, DmeRuleStepDataSource>((ds, rsds) => ds.Id == rsds.DataSourceId).Select(ds =>
                       new DmeDataSource { SysCode = ds.SysCode, Connection = ds.Connection, CreateTime = ds.CreateTime, Id = ds.Id, Name = ds.Name, Remark = ds.Remark, Type = ds.Type }).ToList();
                if (datasources?.Count > 0)
                {
                    foreach (var datasourceItem in datasources)
                    {
                        if (versionDTO.DataSources.ContainsKey(datasourceItem.SysCode))
                        {
                            continue;
                        }
                        versionDTO.DataSources[datasourceItem.SysCode] = datasourceItem;
                    }
                }
                // 每个模型版本下的节点向量信息
                IList<DmeRuleStepHop> hops = db.Queryable<DmeRuleStepHop>()
                     .Where(rsh => rsh.ModelId == model.Id && rsh.VersionId == v.Id)
                     .OrderBy(rsh => rsh.StepFromId)
                     .ToList();
                if (hops?.Count > 0)
                {
                    DmeRuleStep stepFromTemp = null;
                    DmeRuleStep stepToTemp = null;
                    foreach (var item in hops)
                    {
                        stepFromTemp = db.Queryable<DmeRuleStep>().InSingle(item.StepFromId);
                        stepToTemp = db.Queryable<DmeRuleStep>().InSingle(item.StepToId);
                        if (null == stepFromTemp || null == stepToTemp)
                        {
                            LOG.Warn($"开始步骤[{item.StepFromId}]，或者结束步骤[{item.StepToId}]找不到对应实体信息");
                            continue;
                        }
                        versionDTO.Hops.Add(new RuleStepHopDTO(stepFromTemp.Name, stepToTemp.Name, item.Enabled, item.Name));
                    }
                }
            }

            return modelDTO;
        }

        public object LandConflictExecute(IDictionary<string, Property> parameters)
        {
            this.landConflictDetectionAlgorithm.Init(parameters);
            return this.landConflictDetectionAlgorithm.Execute();
        }
        public object ListModels(Boolean detail, int isPublish, EnumModelStatus enumModelStatus)
        {
            List<DmeModel> models = null;
            // 倒序
            if (-1 == isPublish)
            {
                models = base.Repository.GetDbContext().Queryable<DmeModel>().Where(m => m.Status == (int)enumModelStatus).OrderBy(m => m.CreateTime, OrderByType.Desc).ToList();
            }
            else
            {
                // 是否发布
                models = base.Repository.GetDbContext().Queryable<DmeModel>().Where(m => m.IsPublish == isPublish && m.Status == (int)enumModelStatus).OrderBy(m => m.CreateTime, OrderByType.Desc).ToList();
            }

            if (null == models || 0 == models.Count)
            {
                return models;
            }
            IList<ModelDTO> modelDTOs = new List<ModelDTO>();
            ModelTypeDTO modelTypeDTO = null;
            ModelDTO modelDTO = null;
            // 获取模型的详情信息
            foreach (DmeModel m in models)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(m.ModelTypeCode))
                    {
                        DmeModelType dmeModelType = base.Db.Queryable<DmeModelType>().InSingle(m.ModelTypeId);
                        modelTypeDTO = ClassValueCopier<ModelTypeDTO>.Copy(dmeModelType);
                    }
                    modelDTO = this.GetModelMetadata(m, detail);
                    modelDTO.Type = modelTypeDTO;
                    modelDTOs.Add(modelDTO);
                }
                catch (Exception ex)
                {
                    LOG.Error(ex, ex.Message);
                }
            }
            return modelDTOs;
        }
        public object OverlayExecute(IDictionary<String, Property> parameters)
        {
            this.OverlayAlg.Init(parameters);
            Result result = this.OverlayAlg.Execute();
            if (result.Status == EnumUtil.GetEnumDisplayName(EnumSystemStatusCode.DME_SUCCESS))
            {
                IDictionary<string, Property> outParameters = this.OverlayAlg.OutParams;
                return outParameters;
            }
            return null;
        }
        public object AddModel(ModelAddReqDTO dto)
        {
            if (string.IsNullOrEmpty(dto.SysCode))
            {
                // 自动赋予一个唯一编码
                dto.SysCode = GuidUtil.NewGuid();
            }
            var db = base.Repository.GetDbContext();
            // 使用事务
            DbResult<ModelRegisterRespDTO> dbResult = db.Ado.UseTran<ModelRegisterRespDTO>(() =>
            {
                // 查询单条没有数据返回NULL, Single超过1条会报错，First不会
                // base.DmeModelDb.GetContext().Queryable<DmeModel>().Single(m => m.SysCode == dto.SysCode);
                DmeModel model = db.Queryable<DmeModel>().Where(m => m.SysCode == dto.SysCode).Single();
              if (null == model)
              {
                  model = new DmeModel
                  {
                      SysCode = dto.SysCode,
                      Name = dto.Name,
                      Remark = dto.Remark,
                      CreateTime = DateUtil.CurrentTimeMillis,
                      IsPublish = 1,
                      PublishTime = DateUtil.CurrentTimeMillis,
                      Status = 1
                  };
                  // 模型类型
                  DmeModelType modelType = db.Queryable<DmeModelType>().Single(mt => mt.SysCode == dto.TypeCode);
                  model.ModelTypeId = modelType.Id;
                  model.ModelTypeCode = modelType.SysCode;
                  model = db.Insertable<DmeModel>(model).ExecuteReturnEntity();
                  if (null == model)
                  {
                      throw new Exception(String.Format("创建模型失败，原因未明，编码：[{0}]，名称：[{1}]。", dto.SysCode, dto.Name));
                  }
                 
                  ModelRegisterRespDTO respDTO = ClassValueCopier<ModelRegisterRespDTO>.Copy(model);
                    // 处理版本
                  respDTO.VersionCodes = this.HandleVersions(db, model, dto);
                  return respDTO;
              }
              else
              {
                  throw new BusinessException($"模型[{dto.SysCode}]已存在，不能重复注册");
              }
          });
          return dbResult.Data;
        }

        /// <summary>
        /// 处理版本信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="model"></param>
        /// <param name="dto"></param>
        /// <returns>返回版本编码数组</returns>
        private IList<string> HandleVersions(SqlSugarClient db, DmeModel model, ModelAddReqDTO dto)
        {
            IList<ModelVersionAddDTO> versions = dto.Versions;
            if (versions?.Count == 0)
            {
                throw new BusinessException("注册模型时，缺失版本信息。");
            }
            IList<string> versionCodes = new List<string>();
            foreach (var subVersion in versions)
            {
                DmeModelVersion mv = new DmeModelVersion
                {
                    SysCode = GuidUtil.NewGuid(),
                    Name = subVersion.Name,
                    ModelId = model.Id,
                    CreateTime = DateUtil.CurrentTimeMillis
                };
                mv = db.Insertable<DmeModelVersion>(mv).ExecuteReturnEntity();
                versionCodes.Add(mv.SysCode);
                // 添加步骤信息
                IList<RuleStepAddDTO> stepsAdd = subVersion.Steps;
                if (stepsAdd?.Count == 0)
                {
                    continue;
                }
                // 步骤名称与步骤实体的映射
                IDictionary<string, DmeRuleStep> ruleStepMap = new Dictionary<string, DmeRuleStep>();
                foreach (var subStepAdd in stepsAdd)
                {
                    RuleStepTypeDTO ruleStepTypeDTO = subStepAdd.StepType;
                    if (null == ruleStepTypeDTO)
                    {
                        throw new BusinessException("注册模型，缺失步骤类型元数据。");
                    }
                    DmeRuleStepType dmeRuleStepType = db.Queryable<DmeRuleStepType>().Single(rst => rst.Code == ruleStepTypeDTO.Code);
                    DmeRuleStep step = new DmeRuleStep
                    {
                        SysCode = GuidUtil.NewGuid(),
                        ModelId = model.Id,
                        VersionId = mv.Id,
                        X = subStepAdd.X,
                        Y = subStepAdd.Y,
                        Name = subStepAdd.Name,
                        Remark = subStepAdd.Remark,
                        StepTypeId = dmeRuleStepType.Id
                    };
                    step = db.Insertable<DmeRuleStep>(step).ExecuteReturnEntity();
                    ruleStepMap[subStepAdd.Name] = step;
                    // 处理步骤属性
                    this.HandleAttributes(db, model, mv, subStepAdd, step);
                }
                // 处理步骤之间的连接关系
                this.HandleHop(db, subVersion, model, mv, ruleStepMap);
            }
            return versionCodes;
        }

        /// <summary>
        /// 处理步骤属性信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="model"></param>
        /// <param name="version"></param>
        /// <param name="subStepAdd"></param>
        /// <param name="step"></param>
        private void HandleAttributes(SqlSugarClient db, DmeModel model, DmeModelVersion version, RuleStepAddDTO subStepAdd, DmeRuleStep step)
        {
            IList<AttributeReqDTO> properties = subStepAdd.Attributes;
            if (properties?.Count == 0)
            {
                LOG.Warn("没有可处理的步骤属性信息");
                return;
            }
            if (Register.RuleStepPluginsMap.ContainsKey(subStepAdd.StepType.Code))
            {
                RuleStepPluginRegisterDTO ruleStepPluginRegisterDTO = Register.RuleStepPluginsMap[subStepAdd.StepType.Code];
                String baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string assemblyPath = Path.Combine(baseDir, ruleStepPluginRegisterDTO.Assembly);
                Assembly assembly = Assembly.LoadFile(assemblyPath);
                IRuleStepData ruleStepData = (IRuleStepData)assembly.CreateInstance(ruleStepPluginRegisterDTO.ClassId, true, BindingFlags.CreateInstance, null
                    , new object[] { Repository, -1, step }, null, null);
                if (null == ruleStepData)
                {
                    LOG.Warn($"无法创建步骤实体[{ruleStepPluginRegisterDTO.ClassId}]");
                    return;
                }
                IDictionary<string, Property> attributes = new Dictionary<string, Property>();
                // DmeRuleStepAttribute dmeRuleStepAttribute = null;
                foreach (var p in properties)
                {
                    //if (nameof(EnumValueMetaType.TYPE_FEATURECLASS).Equals(p.DataTypeCode)
                    //    || nameof(EnumValueMetaType.TYPE_MDB_FEATURECLASS).Equals(p.DataTypeCode)
                    //    || nameof(EnumValueMetaType.TYPE_SDE_FEATURECLASS).Equals(p.DataTypeCode))
                    //{
                    //    attributes[p.Name] = new Property(p.Name, p.Name, EnumValueMetaType.TYPE_UNKNOWN, "{\"name\":\"" + p.Value + "\",\"source\":\"" + p.DataSourceCode + "\"}",
                    //        null, null, null, 1, 0, 1, p.DataSourceCode, p.IsNeedPrecursor);
                    //    // 要素类的属性，注意值的存储格式
                    //    //dmeRuleStepAttribute = new DmeRuleStepAttribute
                    //    //{
                    //    //    RuleStepId = step.Id,
                    //    //    ModelId = model.Id,
                    //    //    VersionId = version.Id,
                    //    //    IsNeedPrecursor = p.IsNeedPrecursor,
                    //    //    AttributeCode = p.Name,
                    //    //    AttributeValue = "{\"name\":\"" + p.Value + "\",\"source\":\"" + p.DataSourceCode + "\"}"
                    //    //};
                    //}
                    //else
                    //{
                    //    //dmeRuleStepAttribute = new DmeRuleStepAttribute
                    //    //{
                    //    //    RuleStepId = step.Id,
                    //    //    ModelId = model.Id,
                    //    //    VersionId = version.Id,
                    //    //    IsNeedPrecursor = p.IsNeedPrecursor,
                    //    //    AttributeCode = p.Name,
                    //    //    AttributeValue = p.Value
                    //    //};
                    //    attributes[p.Name] = new Property(p.Name, p.Name, EnumValueMetaType.TYPE_UNKNOWN, p.Value,
                    //       null, null, null, 1, 0, 1, p.DataSourceCode, p.IsNeedPrecursor);
                    //}
                    EnumValueMetaType enumValueMetaType;
                    if (string.IsNullOrEmpty(p.DataTypeCode))
                    {
                        enumValueMetaType = EnumValueMetaType.TYPE_UNKNOWN;
                    }
                    else
                    {
                        enumValueMetaType = EnumUtil.GetEnumObjByName<EnumValueMetaType>(p.DataTypeCode);
                    }
                    attributes[p.Name] = new Property(p.Name, p.Name, enumValueMetaType, p.Value,
                          null, null, null, 1, 0, 1, p.DataSourceCode, p.IsNeedPrecursor);
                    //if (1 == p.IsNeedPrecursor)
                    //{
                    //    dmeRuleStepAttribute.AttributeValue = "${" + p.Value + "}";
                    //}
                    // attributes.Add(dmeRuleStepAttribute);
                }
                ruleStepData.SaveAttributes(attributes);
            }

            // 属性
            //if (nameof(EnumRuleStepTypes.DataSourceInput).Equals(subStepAdd.StepType.Code))
            //{
            //    // 数据源类型的步骤，下面有且仅有一个属性
            //    DmeRuleStepDataSource dmeRuleStepDataSource = new DmeRuleStepDataSource
            //    {
            //        ModelId = model.Id,
            //        VersionId = version.Id,
            //        RuleStepId = step.Id
            //    };
            //    DmeDataSource dmeDataSource = db.Queryable<DmeDataSource>().Single(ds => ds.SysCode == properties[0].DataSourceCode);
            //    dmeRuleStepDataSource.DataSourceId = dmeDataSource.Id;
            //    db.Insertable<DmeRuleStepDataSource>(dmeRuleStepDataSource).ExecuteCommand();
            //}
            //else
            //{
            //    List<DmeRuleStepAttribute> attributes = new List<DmeRuleStepAttribute>();
            //    DmeRuleStepAttribute dmeRuleStepAttribute = null;
            //    foreach (var p in properties)
            //    {
            //        if (nameof(EnumValueMetaType.TYPE_FEATURECLASS).Equals(p.DataTypeCode)
            //            || nameof(EnumValueMetaType.TYPE_MDB_FEATURECLASS).Equals(p.DataTypeCode)
            //            || nameof(EnumValueMetaType.TYPE_SDE_FEATURECLASS).Equals(p.DataTypeCode))
            //        {
            //            // 要素类的属性，注意值的存储格式
            //            dmeRuleStepAttribute = new DmeRuleStepAttribute
            //            {
            //                RuleStepId = step.Id,
            //                ModelId = model.Id,
            //                VersionId = version.Id,
            //                IsNeedPrecursor = p.IsNeedPrecursor,
            //                AttributeCode = p.Name,
            //                AttributeValue = "{\"name\":\"" + p.Value + "\",\"source\":\"" + p.DataSourceCode + "\"}"
            //            };
            //        }
            //        else
            //        {
            //            dmeRuleStepAttribute = new DmeRuleStepAttribute
            //            {
            //                RuleStepId = step.Id,
            //                ModelId = model.Id,
            //                VersionId = version.Id,
            //                IsNeedPrecursor = p.IsNeedPrecursor,
            //                AttributeCode = p.Name,
            //                AttributeValue = p.Value
            //            };
            //        }
            //        //if (1 == p.IsNeedPrecursor)
            //        //{
            //        //    dmeRuleStepAttribute.AttributeValue = "${" + p.Value + "}";
            //        //}
            //        attributes.Add(dmeRuleStepAttribute);
            //    }
            //    db.Insertable<DmeRuleStepAttribute>(attributes).ExecuteCommand();
            //}
        }

        /// <summary>
        /// 处理步骤连线信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="versionDTO"></param>
        /// <param name="model"></param>
        /// <param name="version"></param>
        /// <param name="ruleStepMap"></param>
        private void HandleHop(SqlSugarClient db, ModelVersionAddDTO versionDTO, DmeModel model, DmeModelVersion version, IDictionary<string, DmeRuleStep> ruleStepMap)
        {
            IList<RuleStepHopDTO> ruleStepHopDTOs = versionDTO.Hops;
            if (ruleStepHopDTOs?.Count == 0)
            {
                return;
            }
            List<DmeRuleStepHop> hops = new List<DmeRuleStepHop>();
            foreach (var hopDTO in ruleStepHopDTOs)
            {
                DmeRuleStepHop hop = new DmeRuleStepHop
                {
                    ModelId = model.Id,
                    VersionId = version.Id,
                    StepFromId = ruleStepMap[hopDTO.StepFromName].Id,
                    StepToId = ruleStepMap[hopDTO.StepToName].Id,
                    Enabled = hopDTO.Enabled,
                    Name = hopDTO.Name
                };
                hops.Add(hop);
            }
            db.Insertable<DmeRuleStepHop>(hops).ExecuteCommand();
        }
       
        public object CopyModelVersion(string versionCode)
        {
            DmeModelVersion modelVersion = base.Repository.GetDbContext().Queryable<DmeModelVersion>().Where(mv => mv.SysCode == versionCode).Single();
            if (null == modelVersion)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"模型版本[{versionCode}]不存在，或模型版本编码无效");
            }
            var db = base.Repository.GetDbContext();
            return db.Ado.UseTran<DmeModelVersion>(() =>
            {
                // 复制为新版本
                DmeModelVersion newVersion = new DmeModelVersion
                {
                    SysCode = GuidUtil.NewGuid(),
                    ModelId = modelVersion.ModelId,
                    CreateTime = DateUtil.CurrentTimeMillis
                };
                newVersion.Name = $"版本-{DateUtil.CurrentTimeMillis}";
                newVersion = db.Insertable<DmeModelVersion>(newVersion).ExecuteReturnEntity();
                // 复制步骤信息
                IList<DmeRuleStep> steps = db.Queryable<DmeRuleStep>().Where(rs => rs.ModelId == modelVersion.ModelId && rs.VersionId == modelVersion.Id).ToList();
                if (steps?.Count > 0)
                {
                    // List<DmeRuleStep> copiedSteps = new List<DmeRuleStep>();
                    DmeRuleStep newTempStep = null;
                    DmeRuleStepAttribute newTempStepAttr = null;
                    foreach (var subStep in steps)
                    {
                        newTempStep = ClassValueCopier<DmeRuleStep>.Copy(subStep, new String[] { "Id", "SysCode", "VersionId" });
                        newTempStep.SysCode = GuidUtil.NewGuid();
                        newTempStep.VersionId = newVersion.Id;
                        newTempStep = db.Insertable<DmeRuleStep>(newTempStep).ExecuteReturnEntity();
                        // copiedSteps.Add(copiedTempStep);
                        // 复制步骤的参数属性信息
                        IList<DmeRuleStepAttribute> oldAttributes = db.Queryable<DmeRuleStepAttribute>().Where(rsa => rsa.RuleStepId == subStep.Id && rsa.VersionId == modelVersion.Id).ToList();
                        if (oldAttributes?.Count > 0)
                        {
                            List<DmeRuleStepAttribute> newStepAttrs = new List<DmeRuleStepAttribute>();
                            foreach (var subAttr in oldAttributes)
                            {
                                newTempStepAttr = ClassValueCopier<DmeRuleStepAttribute>.Copy(subAttr,
                                    new string[] { "Id", "RuleStepId", "VersionId" });
                                newTempStepAttr.RuleStepId = newTempStep.Id;
                                newTempStepAttr.VersionId = newVersion.Id;
                                newStepAttrs.Add(newTempStepAttr);
                            }
                            int insertCount = db.Insertable<DmeRuleStepAttribute>(newStepAttrs).ExecuteCommand();
                            LOG.Info($"成功复制[{insertCount}]个步骤[{subStep.Id}]的参数信息");
                        }
                        // 复制步骤关联的数据源引用信息
                        IList<DmeRuleStepDataSource> rsDataSources = db.Queryable<DmeRuleStepDataSource>().
                           Where(rsds => rsds.RuleStepId == subStep.Id && rsds.ModelId == modelVersion.ModelId && rsds.VersionId == modelVersion.Id).ToList();
                        if (rsDataSources?.Count > 0)
                        {
                            List<DmeRuleStepDataSource> newRuleStepDataSources = new List<DmeRuleStepDataSource>();
                            foreach (var subDs in rsDataSources)
                            {
                                newRuleStepDataSources.Add(new DmeRuleStepDataSource()
                                {
                                    RuleStepId = newTempStep.Id,
                                    ModelId = subDs.ModelId,
                                    VersionId = newVersion.Id,
                                    DataSourceId = subDs.DataSourceId
                                });
                            }
                            db.Insertable<DmeRuleStepDataSource>(newRuleStepDataSources).ExecuteCommand();
                        }
                    }
                }

                return newVersion;
            });
        }

        public object ListRuleStepTypes()
        {
            return base.Repository.GetDbContext().Queryable<DmeRuleStepType>().OrderBy(rst => rst.Code, OrderByType.Asc).ToList();
        }
        public object SaveRuleStepInfos(ModelRuleStepInfoDTO info)
        {
            var db = base.Repository.GetDbContext();
            // 开始事务
            return db.Ado.UseTran<object>(() =>
            {
                // 根据模型版本号，获取模型版本信息
                DmeModelVersion modelVersion = base.Repository.GetDbContext().Queryable<DmeModelVersion>().Single(mv => mv.SysCode == info.ModelVersionCode);
                // 清除模型的步骤信息
                db.Deleteable<DmeRuleStep>(rs => rs.ModelId == modelVersion.ModelId && rs.VersionId == modelVersion.Id).ExecuteCommand();
                // 清除步骤属性信息
                db.Deleteable<DmeRuleStepAttribute>(rsa => rsa.ModelId == modelVersion.ModelId && rsa.VersionId == modelVersion.Id).ExecuteCommand();
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
                        stepEntity.Id = db.Insertable<DmeRuleStep>(stepEntity).ExecuteReturnIdentity();
                        key2BizId[step.Key] = stepEntity.Id;
                        // 处理步骤属性
                        if (step.Attributes?.Count > 0)
                        {
                            IRuleStepData ruleStepData = RuleStepFactory.GetRuleStepData(step.TypeCode, base.Repository, null, stepEntity);
                            ruleStepData.SaveAttributes(step.Attributes);
                            //IList<DmeRuleStepAttribute> attributeEntities = new List<DmeRuleStepAttribute>();
                            //foreach (var att in step.Attributes)
                            //{
                            //    attributeEntities.Add(new DmeRuleStepAttribute
                            //    {
                            //        RuleStepId = stepEntity.Id,
                            //        ModelId = stepEntity.ModelId,
                            //        VersionId = stepEntity.VersionId,
                            //        AttributeCode = att.Key,
                            //        AttributeValue = att.Value
                            //    });
                            //}
                            //db.Insertable<DmeRuleStepAttribute>(attributeEntities).ExecuteCommand();
                        }
                    }
                    // 处理步骤的向量关系
                    if (info.Vectors?.Count > 0)
                    {
                        IList<DmeRuleStepHop> ruleStepHops = new List<DmeRuleStepHop>();
                        foreach (var vector in info.Vectors)
                        {
                            // 只要向量的信息不完整，都不需要保存连接信息
                            if (!key2BizId.ContainsKey(vector.StepFromName) || !key2BizId.ContainsKey(vector.StepToName))
                            {
                                continue;
                            }
                            ruleStepHops.Add(new DmeRuleStepHop
                            {
                                ModelId = modelVersion.ModelId,
                                VersionId = modelVersion.Id,
                                StepFromId = key2BizId[vector.StepFromName],
                                StepToId = key2BizId[vector.StepToName],
                                Enabled = vector.Enabled
                            });
                        }
                        db.Insertable<DmeRuleStepHop>(ruleStepHops).ExecuteCommandAsync();
                    }
                }
                return true;
            }).Data;
        }
        public object CopyFromModelVersion(string modelVersionCode)
        {
            // 开始事务
            return base.Repository.GetDbContext().Ado.UseTran<DmeModelVersion>(() =>
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
                return newVersion;
            }).Data;
        }

        public object PublishModel(string modelCode, int enabled)
        {
            // 只更新列：IsPublish和PublishTime
            return
                base.Repository.GetDbContext().Updateable<DmeModel>().
                      UpdateColumns(m => new DmeModel() { IsPublish = enabled, PublishTime = DateUtil.CurrentTimeMillis }).
                      Where(m => m.SysCode == modelCode).ExecuteCommandHasChange();
        }

        public object ValidModel(string modelCode)
        {
            throw new NotImplementedException();
        }
        public object AddModelImg(string modelVersionCode, string sourceName, string suffix, string contentType, string objectId)
        {
            var db = Repository.GetDbContext();
            DmeModelVersion modelVersion = db.Queryable<DmeModelVersion>().Single(mv => mv.SysCode == modelVersionCode);
            if (null == modelVersion)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"模型版本[{modelVersionCode}]不存在");
            }
            DmeModelImg modelImg = db.Queryable<DmeModelImg>().Single(mi => mi.ModelId == modelVersion.ModelId && mi.VersionId == modelVersion.Id);
            if (null == modelImg)
            {
                modelImg = new DmeModelImg
                {
                    ModelId = modelVersion.ModelId,
                    VersionId = modelVersion.Id,
                    Suffix = suffix,
                    SourceName = sourceName,
                    ContentType = contentType,
                    ImgCode = objectId
                };
                modelImg = db.Insertable<DmeModelImg>(modelImg).ExecuteReturnEntity();
            }
            else
            {
                modelImg.ImgCode = objectId;
                modelImg.Suffix = suffix;
                modelImg.ContentType = contentType;
                modelImg.SourceName = sourceName;
                db.Updateable<DmeModelImg>(modelImg).ExecuteCommand();
            }
            return modelImg;
        }
        public DmeModelImg GetModelImg(string modelVersionCode)
        {
            var db = Repository.GetDbContext();
            DmeModelVersion modelVersion = db.Queryable<DmeModelVersion>().Single(mv => mv.SysCode == modelVersionCode);
            if (null == modelVersion)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"模型版本[{modelVersionCode}]不存在");
            }
            return db.Queryable<DmeModelImg>().Single(mi => mi.ModelId == modelVersion.ModelId && mi.VersionId == modelVersion.Id);
        }
        public async Task<Boolean> DeleteModelAsync(string modelCode)
        {
            return await Task.Run<Boolean>(() => {
                var db = Repository.GetDbContext();
                return db.Ado.UseTran<Boolean>(() =>
                {
                    DmeModel model = db.Queryable<DmeModel>().Single(m => m.SysCode == modelCode);
                    if (null == model)
                    {
                        throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"模型[{modelCode}]不存在");
                    }
                    model.Status = 0;
                    db.Updateable<DmeModel>(model).UpdateColumns(m => m.Status).ExecuteCommand();
                    // 级联逻辑删除版本
                    var updateCount = db.Updateable<DmeModelVersion>()
                        .UpdateColumns(it => new DmeModelVersion() { Status = 0 })
                        .Where(it => it.ModelId == model.Id).ExecuteCommand();
                    LOG.Info($"共更新[{updateCount}]条版本记录");
                    // 暂停模型相关的任务
                    List<DmeTask> tasks = db.Queryable<DmeTask>().Where(t => t.ModelId == model.Id).ToList();
                    if (tasks?.Count > 0)
                    {
                        foreach (var subTask in tasks)
                        {
                            DmeQuartzScheduler<TaskRunnerJob>.PauseJob(subTask.SysCode, model.ModelTypeCode);
                        }
                    }
                    return true;
                }).Data;
            });
        }
        public async Task<bool> RestoreModelAsync(string modelCode)
        {
            return await Task.Run<Boolean>(() => {
                var db = Repository.GetDbContext();
                return db.Ado.UseTran<Boolean>(() =>
                {
                    DmeModel model = db.Queryable<DmeModel>().Single(m => m.SysCode == modelCode);
                    if (null == model)
                    {
                        throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"模型[{modelCode}]不存在");
                    }
                    model.Status = 1;
                    db.Updateable<DmeModel>(model).UpdateColumns(m => m.Status).ExecuteCommand();
                    // 级联逻辑恢复版本
                    var updateCount = db.Updateable<DmeModelVersion>()
                        .UpdateColumns(mv => new DmeModelVersion() { Status = 1 })
                        .Where(it => it.ModelId == model.Id).ExecuteCommand();
                    LOG.Info($"共更新[{updateCount}]条版本记录");
                    return true;
                }).Data;
            });
        }
        public async Task<object> AddModelTypesAsync(string[] types)
        {
            if (0 == types?.Length)
            {
                LOG.Warn("传入的模型类型数据为空");
                return null;
            }
            List<DmeModelType> modelTypes = new List<DmeModelType>();
            foreach (var item in types)
            {
                DmeModelType modelType = base.Db.Queryable<DmeModelType>().Single(mt => mt.Name == item);
                if (modelType != null)
                {
                    continue;
                }
                modelType = new DmeModelType
                {
                    Name = item,
                    SysCode = GuidUtil.NewGuid(),
                    CreateTime = DateUtil.CurrentTimeMillis,
                    LastTime = DateUtil.CurrentTimeMillis
                };
                modelType = await base.Db.Insertable<DmeModelType>(modelType).ExecuteReturnEntityAsync();
                modelTypes.Add(modelType);
            }
            return modelTypes;
        }
        public object ListModelTypes(string orderFieldName, int orderType)
        {
            if (string.IsNullOrEmpty(orderFieldName))
            {
                return base.Db.Queryable<DmeModelType>().OrderBy(mt => mt.CreateTime, OrderByType.Desc).ToList();
            }
            return base.Db.Queryable<DmeModelType>().OrderBy(orderFieldName + " " + (0 == orderType ? nameof(OrderByType.Asc) : nameof(OrderByType.Desc))).ToList();
        }
        public object UpdateModelTypes(ModelTypeUpdateDTO dto)
        {
            base.Db.Updateable<DmeModelType>()
                .UpdateColumns(mt => new DmeModelType { Name = dto.NewName, LastTime = DateUtil.CurrentTimeMillis })
                .Where(mt2 => mt2.SysCode == dto.SysCode).ExecuteCommand();
            return true;
        }
        public object UpdateModelBasicInfo(ModelBasicInfoUpdateDTO dto)
        {
            DmeModel model = base.Db.Queryable<DmeModel>().Single(m => m.SysCode == dto.SysCode);
            if (null == model)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"模型[{dto.SysCode}]不存在");
            }
            DmeModelType modelType = base.Db.Queryable<DmeModelType>().Single(mt => mt.SysCode == dto.TypeCode);
            if (null == modelType)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"模型类型[{dto.TypeCode}]不存在");
            }
            model.Name = dto.Name;
            model.Remark = dto.Remark;
            model.ModelTypeId = modelType.Id;
            model.ModelTypeCode = modelType.SysCode;
            base.Db.Updateable<DmeModel>(model).ExecuteCommand();
            return true;
        }
        public object UpdateModelVersion(ModelVersionUpdateDTO dto)
        {
            base.Db.Updateable<DmeModelVersion>().UpdateColumns(mv => new DmeModelVersion { Name = dto.NewName }).Where(mv => mv.SysCode == dto.SysCode).ExecuteCommand();
            return true;
        }
    }
}
