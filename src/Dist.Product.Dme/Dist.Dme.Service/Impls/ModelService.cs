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
    public class ModelService : AbstractContext, IModelService
    {
        private static ILog LOG = LogManager.GetLogger(typeof(ModelService));
        /// <summary>
        /// 用地差异分析
        /// </summary>
        private IAlgorithm landConflictDetectionAlgorithm = new LandConflictDetectionMain();

        public object GetLandConflictMetadata()
        {
            return this.landConflictDetectionAlgorithm.MetadataJSON;
        }

        public object GetModelMetadata(string modelCode, bool refAlgorithm)
        {
            throw new NotImplementedException();
        }

        public object LandConflictExecute(IDictionary<string, object> parameters)
        {
            this.landConflictDetectionAlgorithm.Init(parameters);
            return this.landConflictDetectionAlgorithm.Execute();
        }
        public object ListModels(Boolean refAlgorithm)
        {
            List<DmeModel> models = base.DmeModelDb.GetList("CREATETIME", false);
            if (null == models || 0 == models.Count)
            {
                return models;
            }
            if (refAlgorithm)
            {
                IList<ModelAlgDTO> modelAlgDTOs = new List<ModelAlgDTO>();
                ModelAlgDTO modelAlgDTO = null;
                IList<DmeRuleStep> rules = null;
                IList<DmeAlgorithm> algs = null;
                DmeAlgorithm algorithm = null;
                foreach (DmeModel m in models)
                {
                    modelAlgDTO = ClassValueCopier<ModelAlgDTO>.Copy(m);
                    modelAlgDTOs.Add(modelAlgDTO);
                    rules = base.DmeRulestepDb.GetContext().Queryable<DmeRuleStep>().Where("MODEL_ID = '" + m.Id + "'").ToList();
                    if (null == rules || 0 == rules.Count)
                    {
                        continue;
                    }
                    algs = new List<DmeAlgorithm>();
                    foreach (var rule in rules)
                    {
                        algorithm = base.DmeAlgorithmDb.GetById(rule.AlgorithmId);
                        if (null == algorithm)
                        {
                            continue;
                        }
                        algs.Add(algorithm);
                    }
                    modelAlgDTO.Algorithms = algs;
                }
                return modelAlgDTOs;
            }
            else
            {
                return models;
            } 
        }
        public object AddModel(ModelAddReqDTO dto)
        {
            // 使用事务
            DbResult< DmeModel > dbResult = base.Db.Ado.UseTran<DmeModel>(()=> 
            {
                // 如果没有找的，则会抛出异常。base.DmeModelDb.GetContext().Queryable<DmeModel>().Single(m => m.SysCode == dto.SysCode);
                DmeModel model = base.Db.Queryable<DmeModel>().Where(m => m.SysCode == dto.SysCode).First();
                if (null == model)
                {
                    model = ClassValueCopier<DmeModel>.Copy(dto);
                    model.CreateTime = DateUtil.CurrentTimeMillis;
                    int identity = base.DmeModelDb.InsertReturnIdentity(model);
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
                    base.DmeModelVersionDb.Insert(mv);
                }
                // 重复的模型不再添加，直接返回已存在的模型
                return model;
            });
            return dbResult.Data;
        }

        public object ExecuteModel(string code, string versionCode)
        {
            // Single方法，如果查询数据库没有数据，抛出异常
            DmeModel model = base.Db.Queryable<DmeModel>().Single(m => m.SysCode == code);
            // 查询模型版本
            DmeModelVersion modelVersion = base.Db.Queryable<DmeModelVersion>().Single(mv => mv.SysCode == versionCode);
            // 查找关联的算法信息
            IList<DmeRuleStep> ruleSteps = base.Db.Queryable<DmeRuleStep>().Where(rs => rs.ModelId == model.Id).Where(rs => rs.VersionId == modelVersion.Id).ToList();


            throw new NotImplementedException();
        }
    }
}
