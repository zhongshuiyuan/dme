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
using Dist.Dme.Model.DTO;
using Dist.Dme.Model.Entity;
using Dist.Dme.RuleSteps;
using Dist.Dme.RuleSteps.AlgorithmInput;
using Dist.Dme.Service.Interfaces;
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
                    IRuleStepData ruleStepData = RuleStepFactory.GetRuleStepData(ruleStepDTO.StepType.Code, base.Repository, -1, ruleStep);
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
                       new DmeDataSource  { SysCode = ds.SysCode, Connection = ds.Connection, CreateTime = ds.CreateTime, Id = ds.Id, Name = ds.Name, Remark = ds.Remark, Type = ds.Type }).ToList();
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
        public object ListModels(Boolean detail, int isPublish)
        {
            List<DmeModel> models = null;
            // 倒序
            if (-1 == isPublish)
            {
                models = base.Repository.GetDbContext().Queryable<DmeModel>().OrderBy(m => m.CreateTime, OrderByType.Desc).ToList();
            }
            else
            {
                // 是否发布
                models = base.Repository.GetDbContext().Queryable<DmeModel>().Where(m => m.IsPublish == isPublish).OrderBy(m => m.CreateTime, OrderByType.Desc).ToList();
            }

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
                    try
                    {
                        modelDTOs.Add(this.GetModelMetadata(m, detail));
                    }
                    catch(Exception ex)
                    {
                        LOG.Error(ex, ex.Message);
                    }
                }
                return modelDTOs;
            }
            else
            {
                return models;
            } 
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
            DbResult< DmeModel > dbResult = db.Ado.UseTran<DmeModel>(()=> 
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
                        PublishTime = DateUtil.CurrentTimeMillis
                    };
                    model = db.Insertable<DmeModel>(model).ExecuteReturnEntity();
                    if (null == model)
                    {
                        throw new Exception(String.Format("创建模型失败，原因未明，编码：[{0}]，名称：[{1}]。", dto.SysCode, dto.Name));
                    }
                    // 处理版本
                    this.HandleVersions(dto, db, model);
                    return model;
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
        /// <param name="dto"></param>
        /// <param name="db"></param>
        /// <param name="model"></param>
        private void HandleVersions(ModelAddReqDTO dto, SqlSugarClient db, DmeModel model)
        {
            IList<ModelVersionAddDTO> versions = dto.Versions;
            if (versions?.Count == 0)
            {
                throw new BusinessException("注册模型时，缺失版本信息。");
            }
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
                IRuleStepData ruleStepData = (IRuleStepData)assembly.CreateInstance(ruleStepPluginRegisterDTO.ClassId, true, BindingFlags.CreateInstance,null
                    ,new object[] { Repository, -1, step}, null, null);
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
        public DmeTask RunModel(string versionCode)
        {
            // 尽管使用了async关键字，如果不使用await，则还是同步操作
            //return await Task.Run<DmeTask>(() =>
            // {

            // });
            // 验证数据库是否存在指定模型版本信息
            SqlSugarClient db = base.Repository.GetDbContext();
            // 查询模型版本
            DmeModelVersion modelVersion = db.Queryable<DmeModelVersion>().Single(mv => mv.SysCode == versionCode);
            if (null == modelVersion)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"模型的版本[{versionCode}]不存在");
            }
            // Single方法，如果查询数据库多条数据，会抛出异常
            DmeModel model = db.Queryable<DmeModel>().Single(m => m.Id == modelVersion.ModelId);
            if (null == model)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"模型[{modelVersion.ModelId}]不存在");
            }
            // 查找关联的算法信息
            IList<DmeRuleStep> ruleSteps = db.Queryable<DmeRuleStep>().Where(rs => rs.ModelId == model.Id && rs.VersionId == modelVersion.Id).ToList();
            if (0 == ruleSteps?.Count)
            {
                LOG.Warn($"模型[{model.SysCode}]的版本[{versionCode}]下没有可执行步骤，停止运行");
                return null;
            }
            DmeTask newTask = null;
            db.Ado.UseTran<DmeTask>(() =>
            {
                // 每执行一次模型，生成一次任务
                newTask = new DmeTask
                {
                    SysCode = GuidUtil.NewGuid(),
                    CreateTime = DateUtil.CurrentTimeMillis,
                    Status = EnumUtil.GetEnumDisplayName(EnumSystemStatusCode.DME_RUNNING),
                    ModelId = model.Id,
                    VersionId = modelVersion.Id,
                    NodeServer = NetAssist.GetLocalHost()
                };
                newTask.Name = "task-" + newTask.CreateTime;
                newTask.LastTime = newTask.CreateTime;
                newTask = db.Insertable<DmeTask>(newTask).ExecuteReturnEntity();
                return newTask;
            });
            try
            {
                // 此时不阻塞，返回类型为Task，为了能捕获到线程异常信息
                RunModelAsyncEx(db, model, modelVersion, newTask, ruleSteps);
            }
            catch (Exception ex)
            {
                LOG.Error(ex, ex.Message);
                // 更改任务状态
                newTask.Status = EnumUtil.GetEnumDisplayName(EnumSystemStatusCode.DME_ERROR);
                newTask.LastTime = DateUtil.CurrentTimeMillis;
                db.Updateable<DmeTask>(newTask).UpdateColumns(task => new { task.Status, task.LastTime }).ExecuteCommand();
            }

            return newTask;
        }
        /// <summary>
        /// 构建步骤的链表信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ruleSteps"></param>
        /// <returns>多个链表</returns>
        private IList<RuleStepLinkedListNode<DmeRuleStep>> GetRuleStepNodeLinkedList(SqlSugarClient db, DmeModel model, DmeModelVersion modelVersion, IList<DmeRuleStep> ruleSteps)
        {
            IList<RuleStepLinkedListNode<DmeRuleStep>> newLinkedSteps = new List<RuleStepLinkedListNode<DmeRuleStep>>();
            // 一次性构建步骤实体字典
            IDictionary<int, RuleStepLinkedListNode<DmeRuleStep>> ruleStepDic = new Dictionary<int, RuleStepLinkedListNode<DmeRuleStep>>();
            foreach (var subStep in ruleSteps)
            {
                ruleStepDic[subStep.Id] = new RuleStepLinkedListNode<DmeRuleStep>(subStep);
                newLinkedSteps.Add(ruleStepDic[subStep.Id]);
            }
            //IList<RuleStepLinkedListNode<DmeRuleStep>> multiLinkedList = new List<RuleStepLinkedListNode<DmeRuleStep>>();
            IList<DmeRuleStepHop> hops = db.Queryable<DmeRuleStepHop>().Where(rsh => rsh.ModelId == model.Id && rsh.VersionId == modelVersion.Id).OrderBy(rsh => rsh.StepFromId).ToList();
            if (0 == hops?.Count)
            {
                return newLinkedSteps;
            }
           
            IDictionary<int, RuleStepLinkedListNode<DmeRuleStep>> newRuleStepLinkedNodeDic = new Dictionary<int, RuleStepLinkedListNode<DmeRuleStep>>();
            // 已经使用的步骤id集合
            IList<int> usedStepIds = new List<int>();
            RuleStepLinkedListNode<DmeRuleStep> linkedStepFromNode = null;
            RuleStepLinkedListNode<DmeRuleStep> linkedStepToNode = null;
            // 反过来构建？
            foreach (var hop in hops)
            {
                if (!ruleStepDic.ContainsKey(hop.StepFromId) || !ruleStepDic.ContainsKey(hop.StepToId) || 0 == hop.Enabled)
                {
                    continue;
                }
                linkedStepFromNode = ruleStepDic[hop.StepFromId];
                linkedStepToNode = ruleStepDic[hop.StepToId];
                linkedStepFromNode.Next.Add(linkedStepToNode);
                linkedStepToNode.Previous.Add(linkedStepFromNode);
            }
            return newLinkedSteps;
        }
        /// <summary>
        /// 异步处理模型后面的步骤运算
        /// </summary>
        /// <param name="db"></param>
        /// <param name="task">任务实体</param>
        /// <param name="modelVersion"></param>
        /// <param name="model"></param>
        /// <param name="ruleSteps"></param>
        [Obsolete]
        private async Task RunModelAsync(SqlSugarClient db, DmeModel model, DmeModelVersion modelVersion, DmeTask task,  IList<DmeRuleStep> ruleSteps)
        {
            await Task.Run<DmeTask>(() =>
            {
                // 查询步骤前后依赖关系
                // 形成链表
                IList<DmeRuleStepHop> hops = db.Queryable<DmeRuleStepHop>().Where(rsh => rsh.ModelId == model.Id && rsh.VersionId == modelVersion.Id).OrderBy("STEP_FROM_ID").ToList();
                IList <RuleStepLinkedListNode<DmeRuleStep>>  rulestepLinkedList = this.GetRuleStepNodeLinkedList(db, model, modelVersion, ruleSteps);

                return db.Ado.UseTran<DmeTask>(() =>
               {
                   try
                   {
                       IRuleStepData ruleStepData = null;
                       Result stepResult = null;
                       DmeRuleStepType ruleStepTypeTemp = null;
                       foreach (var subRuleStep in ruleSteps)
                       {
                           ruleStepTypeTemp = db.Queryable<DmeRuleStepType>().Single(rst => rst.Id == subRuleStep.StepTypeId);
                           ruleStepData = RuleStepFactory.GetRuleStepData(ruleStepTypeTemp.Code, this.Repository, task.Id, subRuleStep);
                           if (null == ruleStepData)
                           {
                               throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"步骤工厂无法创建编码为[{ruleStepTypeTemp.Code}]的流程实例节点");
                           }
                           stepResult = ruleStepData.Run();
                           //if (1 == subRuleStep.StepTypeId)
                           //{
                           //    // 算法输入
                           //    ruleStepData = new AlgorithmInputStepData(this.Repository, task.Id, subRuleStep);
                           //    // 执行计算
                           //    stepResult = ruleStepData.Run();
                           //}
                           if (stepResult.Code != EnumSystemStatusCode.DME_SUCCESS)
                           {
                               throw new BusinessException((int)stepResult.Code, stepResult.Message);
                           }
                       }
                       task.Status = EnumUtil.GetEnumDisplayName(EnumSystemStatusCode.DME_SUCCESS);
                       task.LastTime = DateUtil.CurrentTimeMillis;
                       db.Updateable<DmeTask>(task).ExecuteCommand();
                       return task;
                   }
                   catch (Exception ex)
                   {
                       // 更改任务执行的状态
                       if (ex is BusinessException)
                       {
                           task.Status = EnumUtil.GetEnumDisplayName(EnumUtil.GetEnumObjByValue<EnumSystemStatusCode>(((BusinessException)ex).Code));
                       }
                       else
                       {
                           task.Status = EnumUtil.GetEnumDisplayName(EnumSystemStatusCode.DME_ERROR);
                       }
                       task.LastTime = DateUtil.CurrentTimeMillis;
                       db.Updateable<DmeTask>(task).ExecuteCommand();
                       // 添加日志
                       this.LogService.AddLogAsync(Base.Common.Log.EnumLogType.ENTITY, Base.Common.Log.EnumLogLevel.ERROR, nameof(DmeTask), task.SysCode, "", ex, "", NetAssist.GetLocalHost());
                       throw ex;
                   }
               }).Data;
            });
        }
        /// <summary>
        /// 运行步骤节点
        /// </summary>
        /// <param name="db"></param>
        /// <param name="task"></param>
        /// <param name="node"></param>
        private void RunRuleStepNode(SqlSugarClient db, DmeTask task, RuleStepLinkedListNode<DmeRuleStep> node)
        {
            // 先计算前置节点
            if (node.Previous?.Count > 0)
            {
                foreach (var item in node.Previous)
                {
                    RunRuleStepNode(db, task, item);
                }
            }
            DmeTaskRuleStep dmeTaskRuleStep = null;
            try
            {
                // 先判断任务的状态，是否被停止
                DmeTask taskStatus = db.Queryable<DmeTask>().Single(t => t.SysCode == task.SysCode);
                if (null == taskStatus || taskStatus.Status.Equals(EnumUtil.GetEnumDisplayName(EnumSystemStatusCode.DME_STOP)))
                {
                    LOG.Info($"任务[{task.SysCode}]不存在或者已被停止");
                    return;
                }
                dmeTaskRuleStep = this.GetTaskRuleStep(db, task, node.Value, out string cacheKey);
                if (dmeTaskRuleStep != null && EnumUtil.GetEnumDisplayName(EnumSystemStatusCode.DME_SUCCESS).Equals(dmeTaskRuleStep.Status))
                {
                    // 释放
                    dmeTaskRuleStep = null;
                    LOG.Info($"任务[{task.SysCode}]下的步骤[{node.Value.SysCode}]已被计算过，并且状态为[success]");
                    return;
                }
                // 如果前置节点没有了，则计算当前节点内容
                DmeRuleStepType ruleStepTypeTemp = db.Queryable<DmeRuleStepType>().Single(rst => rst.Id == node.Value.StepTypeId);
                IRuleStepData ruleStepData = RuleStepFactory.GetRuleStepData(ruleStepTypeTemp.Code, this.Repository, task.Id, node.Value);
                if (null == ruleStepData)
                {
                    throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"步骤工厂无法创建编码为[{ruleStepTypeTemp.Code}]的流程实例节点");
                }
                dmeTaskRuleStep = new DmeTaskRuleStep
                {
                    SysCode = GuidUtil.NewGuid(),
                    TaskId = task.Id,
                    RuleStepId = node.Value.Id,
                    Status = EnumUtil.GetEnumDisplayName(EnumSystemStatusCode.DME_RUNNING),
                    CreateTime = DateUtil.CurrentTimeMillis,
                    LastTime = DateUtil.CurrentTimeMillis
                };
                dmeTaskRuleStep = db.Insertable<DmeTaskRuleStep>(dmeTaskRuleStep).ExecuteReturnEntity();
                // 任务步骤创建成功后，把相关信息记录在缓存中
                ServiceFactory.CacheService.AddAsync(cacheKey, dmeTaskRuleStep, 60);
                Result stepResult = ruleStepData.Run();
                UpdateRuleStep(db, dmeTaskRuleStep, cacheKey, stepResult);
                // 然后计算下一个步骤
                if (node?.Next.Count > 0)
                {
                    foreach (var item in node.Next)
                    {
                        RunRuleStepNode(db, task, item);
                    }
                }
            }
            catch (Exception ex)
            {
                dmeTaskRuleStep.Status = EnumUtil.GetEnumDisplayName(EnumSystemStatusCode.DME_ERROR);
                dmeTaskRuleStep.LastTime = DateUtil.CurrentTimeMillis;
                // 只更新状态和最后时间
                db.Updateable<DmeTaskRuleStep>(dmeTaskRuleStep).UpdateColumns(ts => new { ts.Status, ts.LastTime }).ExecuteCommand();
                this.LogService.AddLogAsync(Base.Common.Log.EnumLogType.ENTITY, EnumLogLevel.ERROR, nameof(DmeTaskRuleStep), dmeTaskRuleStep.SysCode, "", ex, "", NetAssist.GetLocalHost());
            } 
        }

        /// <summary>
        /// 从缓存或者db中获取任务步骤信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="task"></param>
        /// <param name="ruleStep"></param>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        private DmeTaskRuleStep GetTaskRuleStep(SqlSugarClient db, DmeTask task, DmeRuleStep ruleStep, out string cacheKey)
        {
            // 先从缓存查找
            cacheKey = HashUtil.Hash_2_MD5_32($"{task.SysCode}_{ruleStep.SysCode}");
            DmeTaskRuleStep dmeTaskRuleStep = null;
            try
            {
                dmeTaskRuleStep = ServiceFactory.CacheService.Get<DmeTaskRuleStep>(cacheKey);
                if (dmeTaskRuleStep != null)
                {
                    LOG.Info($"缓存中获取到任务步骤信息，任务id[{dmeTaskRuleStep.TaskId}]，步骤id[{dmeTaskRuleStep.RuleStepId}]");
                    return dmeTaskRuleStep;
                }
            }
            catch (Exception ex)
            {
                LOG.Warn("从缓存中获取任务步骤信息失败，详情：" + ex.Message);
            }
            // 从数据库中查找
            dmeTaskRuleStep = db.Queryable<DmeTaskRuleStep>().Single(tr => tr.TaskId == task.Id && tr.RuleStepId == ruleStep.Id);
            return dmeTaskRuleStep;
        }
        /// <summary>
        /// 更新任务和步骤信息到数据和缓存
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dmeTaskRuleStep"></param>
        /// <param name="cacheKey"></param>
        /// <param name="stepResult"></param>
        private static void UpdateRuleStep(SqlSugarClient db, DmeTaskRuleStep dmeTaskRuleStep, string cacheKey, Result stepResult)
        {
            dmeTaskRuleStep.Status = EnumUtil.GetEnumDisplayName(stepResult.Code);
            dmeTaskRuleStep.LastTime = DateUtil.CurrentTimeMillis;
            // 只更新状态和最后时间
            db.Updateable<DmeTaskRuleStep>(dmeTaskRuleStep).UpdateColumns(ts => new { ts.Status, ts.LastTime }).ExecuteCommand();
            // 刷新缓存
            ServiceFactory.CacheService.ReplaceAsync(cacheKey, dmeTaskRuleStep);
        }

        private async Task RunModelAsyncEx(SqlSugarClient db, DmeModel model, DmeModelVersion modelVersion, DmeTask task, IList<DmeRuleStep> ruleSteps)
        {
            await Task.Run<DmeTask>(() =>
            {
                return db.Ado.UseTran<DmeTask>(() =>
                {
                    try
                    {
                        // 查询步骤前后依赖关系
                        // 形成链表
                        IList<DmeRuleStepHop> hops = db.Queryable<DmeRuleStepHop>().Where(rsh => rsh.ModelId == model.Id && rsh.VersionId == modelVersion.Id).OrderBy("STEP_FROM_ID").ToList();
                        IList<RuleStepLinkedListNode<DmeRuleStep>> rulestepLinkedList = this.GetRuleStepNodeLinkedList(db, model, modelVersion, ruleSteps);
                        foreach (var item in rulestepLinkedList)
                        {
                            // 开始计算步骤
                            this.RunRuleStepNode(db, task, item);
                        }
                        task.Status = EnumUtil.GetEnumDisplayName(EnumSystemStatusCode.DME_SUCCESS);
                        task.LastTime = DateUtil.CurrentTimeMillis;
                        db.Updateable<DmeTask>(task).ExecuteCommand();
                        return task;
                    }
                    catch (Exception ex)
                    {
                        // 更改任务执行的状态
                        if (ex is BusinessException)
                        {
                            task.Status = EnumUtil.GetEnumDisplayName(EnumUtil.GetEnumObjByValue<EnumSystemStatusCode>(((BusinessException)ex).Code));
                        }
                        else
                        {
                            task.Status = EnumUtil.GetEnumDisplayName(EnumSystemStatusCode.DME_ERROR);
                        }
                        task.LastTime = DateUtil.CurrentTimeMillis;
                        db.Updateable<DmeTask>(task).ExecuteCommand();
                        // 添加日志
                        this.LogService.AddLogAsync(Base.Common.Log.EnumLogType.ENTITY, Base.Common.Log.EnumLogLevel.ERROR, nameof(DmeTask), task.SysCode, "", ex, "", NetAssist.GetLocalHost());
                        throw ex;
                    }
                }).Data;
            });
        }

        public object CopyModelVersion(string versionCode)
        {
            DmeModelVersion modelVersion = base.Repository.GetDbContext().Queryable<DmeModelVersion>().Where(mv => mv.SysCode == versionCode).Single();
            if (null == modelVersion)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"模型版本[{versionCode}]不存在，或模型版本编码无效");
            }
            var db = base.Repository.GetDbContext();
            return db.Ado.UseTran<DmeModelVersion>(()=>
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
                        newTempStep = ClassValueCopier<DmeRuleStep>.Copy(subStep, new String[] { "Id", "SysCode", "VersionId"});
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
                                    new string[] { "Id", "RuleStepId", "VersionId"});
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
            return base.Repository.GetDbContext().Queryable<DmeRuleStepType>().OrderBy(rst =>rst.Code, OrderByType.Asc).ToList();
        }
        public object SaveRuleStepInfos(ModelRuleStepInfoDTO info)
        {
            var db = base.Repository.GetDbContext();
            // 开始事务
            return db.Ado.UseTran<object>(()=>
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
                            IRuleStepData ruleStepData = RuleStepFactory.GetRuleStepData(step.TypeCode, base.Repository, -1, stepEntity);
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
        public async Task<Boolean> DeleteModel(string modelCode)
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
                    db.Updateable<DmeModel>(model).UpdateColumns(m => m.Status);
                    // 级联逻辑删除版本
                    var updateCount = db.Updateable<DmeModelVersion>()
                        .UpdateColumns(it => new DmeModelVersion() { Status = 0 })
                        .Where(it => it.ModelId == model.Id).ExecuteCommand();
                    LOG.Info($"共更新[{updateCount}]版本记录");
                    return true;
                }).Data;
            });
        }
    }
}
