using Dist.Dme.Algorithms.LandConflictDetection;
using Dist.Dme.Algorithms.Overlay;
using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.DAL.Context;
using Dist.Dme.DisFS.Adapters.Mongo;
using Dist.Dme.DisFS.Collection;
using Dist.Dme.Extensions;
using Dist.Dme.Model.DTO;
using Dist.Dme.Model.Entity;
using Dist.Dme.RuleSteps.AlgorithmInput;
using Dist.Dme.Service.Interfaces;
using log4net;
using MongoDB.Driver;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        private IAlgorithm OverlayAlg = new OverlayMain();
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
        public object OverlayExecute(IDictionary<String, Object> parameters)
        {
            this.OverlayAlg.Init(parameters);
            Result result = this.OverlayAlg.Execute();
            if (result.Status == EnumUtil.GetEnumDisplayName(SystemStatusCode.DME_SUCCESS))
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

        public DmeTask RunModelAsync(string versionCode)
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
                throw new BusinessException((int)SystemStatusCode.DME_ERROR, $"模型的版本[{versionCode}]不存在");
            }
            // Single方法，如果查询数据库多条数据，会抛出异常
            DmeModel model = db.Queryable<DmeModel>().Single(m => m.Id == modelVersion.ModelId);
            if (null == model)
            {
                throw new BusinessException((int)SystemStatusCode.DME_ERROR, $"模型[{modelVersion.ModelId}]不存在");
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
                    Status = EnumUtil.GetEnumDisplayName(SystemStatusCode.DME_RUNNING),
                    ModelId = model.Id,
                    VersionId = modelVersion.Id
                };
                newTask.Name = "task-" + newTask.CreateTime;
                newTask.LastTime = newTask.CreateTime;
                newTask = db.Insertable<DmeTask>(newTask).ExecuteReturnEntity();
                return newTask;
            });
            // 此时不阻塞
            RunModelAsync(db, newTask, modelVersion, model, ruleSteps);
            return newTask;
        }
        /// <summary>
        /// 异步处理模型后面的步骤运算
        /// </summary>
        /// <param name="db"></param>
        /// <param name="task">任务实体</param>
        /// <param name="modelVersion"></param>
        /// <param name="model"></param>
        /// <param name="ruleSteps"></param>
        private async void RunModelAsync(SqlSugarClient db, DmeTask task, DmeModelVersion modelVersion, DmeModel model, IList<DmeRuleStep> ruleSteps)
        {
            await Task.Run<DmeTask>(() =>
            {
                // 查询步骤前后依赖关系
                // TODO 暂时不处理
                IList<DmeRuleStepHop> hops = db.Queryable<DmeRuleStepHop>().Where(rsh => rsh.ModelId == model.Id && rsh.VersionId == modelVersion.Id).OrderBy("STEP_FROM_ID").ToList();

                return db.Ado.UseTran<DmeTask>(() =>
               {
                   try
                   {
                       IRuleStepData ruleStepData = null;
                       Result stepResult = null;
                       foreach (var subRuleStep in ruleSteps)
                       {
                           // 注入每一个步骤的参数值
                           if (1 == subRuleStep.StepTypeId)
                           {
                               // 算法输入
                               ruleStepData = new AlgorithmInputStepData(this.Repository, task.Id, model.Id, modelVersion.Id, subRuleStep.Id);
                           }
                           // 执行计算
                           stepResult = ruleStepData.Run();
                           if (stepResult.Code != (int)SystemStatusCode.DME_SUCCESS)
                           {
                               throw new BusinessException(stepResult.Code, stepResult.Message);
                           }
                       }
                       task.Status = EnumUtil.GetEnumDisplayName(SystemStatusCode.DME_SUCCESS);
                       task.LastTime = DateUtil.CurrentTimeMillis;
                       db.Updateable<DmeTask>(task).ExecuteCommand();
                       return task;
                   }
                   catch (Exception ex)
                   {
                       // 更改任务执行的状态
                       if (ex is BusinessException)
                       {
                           task.Status = EnumUtil.GetEnumDisplayName(EnumUtil.GetEnumObjByValue<SystemStatusCode>(((BusinessException)ex).Code));
                       }
                       else
                       {
                           task.Status = EnumUtil.GetEnumDisplayName(SystemStatusCode.DME_ERROR);
                       }
                       task.LastTime = DateUtil.CurrentTimeMillis;
                       db.Updateable<DmeTask>(task).ExecuteCommand();
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
                throw new BusinessException((int)SystemStatusCode.DME_FAIL, $"模型版本[{versionCode}]不存在，或模型版本编码无效");
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
        public object ListTask()
        {
            SqlSugarClient db = base.Repository.GetDbContext();
            IList<DmeTask> tasks = db.Queryable<DmeTask>().OrderBy(t => t.CreateTime, OrderByType.Desc).ToList();
            if (0 == tasks?.Count)
            {
                return 0;
            }
            IList<TaskRespDTO> taskRespDTOs = new List<TaskRespDTO>();
            foreach (var item in tasks)
            {
                TaskRespDTO dto = new TaskRespDTO
                {
                    Task = item,
                    Model = db.Queryable<DmeModel>().Single(m => m.Id == item.ModelId),
                    ModelVersion = db.Queryable<DmeModelVersion>().Single(mv => mv.Id == item.VersionId)
                };
                taskRespDTOs.Add(dto);
            }
            return taskRespDTOs;
        }
        
        public object GetTaskResult(string taskCode, int ruleStepId)
        {
            SqlSugarClient db = base.Repository.GetDbContext();
            DmeTask task = db.Queryable<DmeTask>().Single(t => t.SysCode == taskCode);
            if (null == task)
            {
                throw new BusinessException((int)SystemStatusCode.DME_FAIL, $"任务不存在[{taskCode}]");
            }
            // 查询任务的结果输出
            IList<DmeTaskResult> taskResults = null;
            if (-1 == ruleStepId)
            {
                // 全部步骤
                taskResults = db.Queryable<DmeTaskResult>().Where(tr => tr.TaskId == task.Id).ToList();
            }
            else
            {
                // 指定步骤
                taskResults = db.Queryable<DmeTaskResult>().Where(tr => tr.TaskId == task.Id && tr.RuleStepId == ruleStepId).ToList();
            }
            if (null == taskResults || 0 == taskResults.Count)
            {
                return null;
            }
            IList<TaskResultRespDTO> taskResultRespDTOs = new List<TaskResultRespDTO>();
            TaskResultRespDTO temp = null;
            foreach (var item in taskResults)
            {
                temp = new TaskResultRespDTO
                {
                    RuleStepId = item.RuleStepId,
                    Code = item.ResultCode,
                    Type = item.ResultType
                };
                // 解析步骤类型
                DmeRuleStep ruleStep = db.Queryable<DmeRuleStep>().Single(rs => rs.Id == item.RuleStepId);

                //   Value = item.ResultValue
                ValueMetaType @enum = EnumUtil.GetEnumObjByName<ValueMetaType>(temp.Type);
                switch (@enum)
                {
                    case ValueMetaType.TYPE_UNKNOWN:
                        break;
                    case ValueMetaType.TYPE_NUMBER:
                        break;
                    case ValueMetaType.TYPE_STRING:
                        break;
                    case ValueMetaType.TYPE_DATE:
                        break;
                    case ValueMetaType.TYPE_BOOLEAN:
                        break;
                    case ValueMetaType.TYPE_INTEGER:
                        break;
                    case ValueMetaType.TYPE_BIGNUMBER:
                        break;
                    case ValueMetaType.TYPE_SERIALIZABLE:
                        break;
                    case ValueMetaType.TYPE_BINARY:
                        break;
                    case ValueMetaType.TYPE_TIMESTAMP:
                        break;
                    case ValueMetaType.TYPE_INET:
                        break;
                    case ValueMetaType.TYPE_LOCAL_FILE:
                        break;
                    case ValueMetaType.TYPE_MDB_FEATURECLASS:
                        break;
                    case ValueMetaType.TYPE_GDB_PATH:
                        break;
                    case ValueMetaType.TYPE_FOLDER:
                        break;
                    case ValueMetaType.TYPE_STRING_LIST:
                        break;
                    case ValueMetaType.TYPE_SDE_FEATURECLASS:
                        break;
                    case ValueMetaType.TYPE_FEATURECLASS:
                        break;
                    case ValueMetaType.TYPE_JSON:
                        // 从mongo中获取
                        var filter = Builders<TaskResultColl>.Filter.And(
                            Builders<TaskResultColl>.Filter.Eq("TaskId", item.TaskId),
                            Builders<TaskResultColl>.Filter.Eq("RuleStepId", item.RuleStepId),
                            Builders<TaskResultColl>.Filter.Eq("Code", item.ResultCode));
                        IList<TaskResultColl> colls = MongodbHelper<TaskResultColl>.FindList(ServiceFactory.MongoHost, filter);
                        if (colls != null && colls.Count > 0)
                        {
                            temp.Value = colls[0].Value;
                        }
                        break;
                    default:
                        break;
                }

                taskResultRespDTOs.Add(temp);
            }
            return taskResultRespDTOs;
        }
    }
}
