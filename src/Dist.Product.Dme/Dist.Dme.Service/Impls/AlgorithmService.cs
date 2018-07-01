using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.DAL.Context;
using Dist.Dme.Model.DTO;
using Dist.Dme.Model.Entity;
using Dist.Dme.Service.Interfaces;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dist.Dme.Service.Impls
{
    /// <summary>
    /// 算法服务实现
    /// </summary>
    public class AlgorithmService : BaseBizService, IAlgorithmService
    {
        private static ILog LOG = LogManager.GetLogger(typeof(AlgorithmService));
        public AlgorithmService(IRepository repository)
        {
            base.Repository = repository;
        }

        public object ListAlgorithms(bool needMeta)
        {
            List<DmeAlgorithm> algs = base.Repository.GetDbContext().Queryable<DmeAlgorithm>().OrderBy(alg => alg.CreateTime, OrderByType.Desc).ToList();
            if (null == algs || 0 == algs.Count)
            {
                return algs;
            }
            if (needMeta)
            {
                IList<AlgorithmRespDTO> algDTOs = new List<AlgorithmRespDTO>();
                AlgorithmRespDTO algorithmDTO = null;
                IList<DmeAlgorithmMeta> metas = null;
                foreach (var alg in algs)
                {
                    algorithmDTO = ClassValueCopier<AlgorithmRespDTO>.Copy(alg);
                    if(alg.Extension != null && !string.IsNullOrEmpty(alg.Extension.ToString()))
                    {
                        algorithmDTO.Extension = alg.Extension;// JsonConvert.DeserializeObject(.ToString());
                    }
                    algDTOs.Add(algorithmDTO);
                    metas = base.Repository.GetDbContext().Queryable<DmeAlgorithmMeta>().Where(meta => meta.AlgorithmId == alg.Id).ToList();
                    if (null == metas || 0 == metas.Count)
                    {
                        continue;
                    }
                    algorithmDTO.Metas = metas;
                }
                return algDTOs;
            }
            return algs;
        }
        public AlgorithmRespDTO GetAlgorithmByCode(String code, bool hasMeta)
        {
            // single，如果找不到实体，则抛出异常
            DmeAlgorithm alg = base.Repository.GetDbContext().Queryable<DmeAlgorithm>().Single(a => a.SysCode == code);
            AlgorithmRespDTO algorithmDTO = ClassValueCopier<AlgorithmRespDTO>.Copy(alg);
            IList<DmeAlgorithmMeta>  metas = base.Repository.GetDbContext().Queryable<DmeAlgorithmMeta>().Where(meta => meta.AlgorithmId == alg.Id).ToList();
            if (metas !=null  && metas.Count >0)
            {
                algorithmDTO.Metas = metas;
            }
            return algorithmDTO;
        }
        public object AddAlgorithm(AlgorithmAddReqDTO dto)
        {
            DbResult<AlgorithmRespDTO> result = base.Repository.GetDbContext().Ado.UseTran<AlgorithmRespDTO>(() => 
            {
                DmeAlgorithm alg = base.Repository.GetDbContext().Queryable<DmeAlgorithm>().Where(a => a.SysCode == dto.SysCode).First();
                if (null == alg)
                {
                    alg = ClassValueCopier<DmeAlgorithm>.Copy(dto);
                    alg.CreateTime = DateUtil.CurrentTimeMillis;
                    alg.Extension = JsonConvert.SerializeObject(dto.Extension);
                    alg.Id = base.Repository.GetDbContext().Insertable<DmeAlgorithm>(alg).ExecuteReturnIdentity();
                }
                else
                {
                    // 进行更新操作
                    alg.Name = dto.Name;
                    alg.Alias = dto.Alias;
                    alg.Version = dto.Version;
                    alg.Remark = dto.Remark;
                    alg.Type = dto.Type;
                    alg.Extension = JsonConvert.SerializeObject(dto.Extension);
                    if (!base.Repository.GetDbContext().Updateable<DmeAlgorithm>().ExecuteCommandHasChange())
                    {
                        throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, "更新算法信息失败，无详情信息。");
                    }
                    if (dto.Metas != null && dto.Metas.Count > 0)
                    {
                        // 删除算法的输入输出参数这些元数据信息，必须ExecuteCommand，否则无效
                        base.Repository.GetDbContext().Deleteable<DmeAlgorithmMeta>().Where(am => am.AlgorithmId == alg.Id).ExecuteCommand();
                    }
                }
                // 重新注册算法参数元数据
                AlgorithmRespDTO algorithmRespDTO = ClassValueCopier<AlgorithmRespDTO>.Copy(alg);
                if (dto.Metas != null && dto.Metas.Count > 0)
                {
                    algorithmRespDTO.Metas = new List<DmeAlgorithmMeta>();
                    DmeAlgorithmMeta meta = null;
                    foreach (var item in dto.Metas)
                    {
                        meta = ClassValueCopier<DmeAlgorithmMeta>.Copy(item);
                        meta.AlgorithmId = alg.Id;
                        meta.Id = base.Repository.GetDbContext().Insertable<DmeAlgorithmMeta>(meta).ExecuteReturnIdentity();
                        algorithmRespDTO.Metas.Add(meta);
                    }
                }
                return algorithmRespDTO;
            });
            return result.Data;
        }

        public static IEnumerable<Type> GetType(Type interfaceType)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var t in type.GetInterfaces())
                    {
                        if (t == interfaceType)
                        {
                            yield return type;
                            break;
                        }
                    }
                }
            }
        }

        public object ListAlgorithmMetadatasLocal()
        {
            String baseDir = AppDomain.CurrentDomain.BaseDirectory;
            String registerFile = Path.Combine(new string[] { baseDir, "register.json" });
            if (!File.Exists(registerFile))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"注册文件[{registerFile}]不存在");
            }
            String jsonText = File.ReadAllText(registerFile);
            JObject jObject = JObject.Parse(jsonText);
           JToken[] jTokens = jObject["Algorithms"].ToArray<JToken>();
            if (null == jTokens || 0 == jTokens.Length)
            {
                return new List<IAlgorithm>();
            }
            IAlgorithm temp = null;
            // 元数据集合
            IList<object> localAlgorithmMetadatas = new List<object>();
            foreach (var item in jTokens)
            {
                try
                {
                    string assemblyPath = Path.Combine(baseDir, item["Assembly"].Value<string>());
                    Assembly assembly = Assembly.LoadFile(assemblyPath);
                    if (null == assembly)
                    {
                        LOG.Warn($"程序集文件[{assemblyPath}]不存在");
                        continue;
                    }
                    string fullName = item["MainClass"].Value<string>();
                    if (string.IsNullOrEmpty(fullName))
                    {
                        LOG.Warn($"接口名称缺失[MainClass]");
                        continue;
                    }
                    temp = (IAlgorithm)assembly.CreateInstance(fullName, true);
                    if (null == temp)
                    {
                        LOG.Warn($"接口[{fullName}]创建实例为空");
                        continue;
                    }
                    localAlgorithmMetadatas.Add(temp.MetadataJSON);
                }
                catch (Exception ex)
                {
                    LOG.Error("获取本地算法对象失败", ex);
                    continue;
                }
            }
          
            return localAlgorithmMetadatas;
        }

        public object RegistryAlgorithmFromLocal(string algCode)
        {
            String baseDir = AppDomain.CurrentDomain.BaseDirectory;
            String registerFile = Path.Combine(new string[] { baseDir, "register.json" });
            if (!File.Exists(registerFile))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"注册文件[{registerFile}]不存在");
            }
            String jsonText = File.ReadAllText(registerFile);
            JObject jObject = JObject.Parse(jsonText);
            JToken[] jTokens = jObject["Algorithms"].ToArray<JToken>();
            if (null == jTokens || 0 == jTokens.Length)
            {
                LOG.Warn("在注册文件中没有找到节点[Algorithms]的内容");
                return false;
            }
            IAlgorithm tempAlgorithm = null;
            foreach (var item in jTokens)
            {
                try
                {
                    string assemblyPath = Path.Combine(baseDir, item["Assembly"].Value<string>());
                    Assembly assembly = Assembly.LoadFile(assemblyPath);
                    if (null == assembly)
                    {
                        LOG.Warn($"程序集文件[{assemblyPath}]不存在");
                        continue;
                    }
                    string fullName = item["MainClass"].Value<string>();
                    if (string.IsNullOrEmpty(fullName))
                    {
                        LOG.Warn($"接口名称缺失[MainClass]");
                        continue;
                    }
                    tempAlgorithm = (IAlgorithm)assembly.CreateInstance(fullName, true);
                    if (null == tempAlgorithm)
                    {
                        LOG.Warn($"接口[{fullName}]创建实例为空");
                        continue;
                    }
                    if (string.IsNullOrEmpty(algCode) || algCode.Equals(tempAlgorithm.SysCode))
                    {
                        AlgorithmAddReqDTO algorithmAddReqDTO = new AlgorithmAddReqDTO
                        {
                            SysCode = tempAlgorithm.SysCode,
                            Name = tempAlgorithm.Name,
                            Alias = tempAlgorithm.Alias,
                            Version = tempAlgorithm.Version,
                            Remark = tempAlgorithm.Remark,
                            Type = tempAlgorithm.AlgorithmType.Code,
                            Extension = JsonConvert.SerializeObject(tempAlgorithm.AlgorithmType.Metadata)
                        };
                        algorithmAddReqDTO.Metas = new List<AlgorithmMetaReqDTO>();
                        // 输入参数
                        this.GetAlgParameters((IDictionary<String, Property>)tempAlgorithm.InParams, ParameterType.IN, algorithmAddReqDTO);
                        // 输出参数
                        this.GetAlgParameters((IDictionary<String, Property>)tempAlgorithm.OutParams, ParameterType.OUT, algorithmAddReqDTO);
                        // 特征参数
                        this.GetAlgParameters((IDictionary<String, Property>)tempAlgorithm.FeatureParams, ParameterType.IN_F, algorithmAddReqDTO);
                        // 持久化数据
                        this.AddAlgorithm(algorithmAddReqDTO);
                        if (string.IsNullOrEmpty(algCode))
                        {
                            // 表示所有，继续遍历其它算法
                            continue;
                        }
                        else
                        {
                            // 表示指定算法
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LOG.Error("注册本地算法失败", ex);
                    continue;
                }
            }
            return true;
            //var types = AppDomain.CurrentDomain.GetAssemblies()
            //      .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IAlgorithm))))
            //      .ToArray();
            //if (null == types || 0 == types.Count())
            //{
            //    LOG.Warn($"没有找到算法接口[{nameof(IAlgorithm)}]的相关实现体");
            //    return false;
            //}
            //IAlgorithm tempAlgorithm = null;
            //foreach (var type in types)
            //{
            //    if (type.IsAbstract)
            //    {
            //        // 抽象类排除
            //        continue;
            //    }
            //    try
            //    {
            //        tempAlgorithm = (IAlgorithm)type.Assembly.CreateInstance(type.FullName, true);
            //        if (null == tempAlgorithm)
            //        {
            //            continue;
            //        }
            //        if (string.IsNullOrEmpty(algCode) || algCode.Equals(tempAlgorithm.SysCode))
            //        {
            //            AlgorithmAddReqDTO algorithmAddReqDTO = new AlgorithmAddReqDTO
            //            {
            //                SysCode = tempAlgorithm.SysCode,
            //                Name = tempAlgorithm.Name,
            //                Alias = tempAlgorithm.Alias,
            //                Version = tempAlgorithm.Version,
            //                Remark = tempAlgorithm.Remark,
            //                Type = tempAlgorithm.AlgorithmType.Code,
            //                Extension = JsonConvert.SerializeObject(tempAlgorithm.AlgorithmType.Metadata)
            //            };
            //            algorithmAddReqDTO.Metas = new List<AlgorithmMetaReqDTO>();
            //            // 输入参数
            //            this.GetAlgParameters((IDictionary<String, Property>)tempAlgorithm.InParams, ParameterType.IN, algorithmAddReqDTO);
            //            // 输出参数
            //            this.GetAlgParameters((IDictionary<String, Property>)tempAlgorithm.OutParams, ParameterType.OUT, algorithmAddReqDTO);
            //            // 特征参数
            //            this.GetAlgParameters((IDictionary<String, Property>)tempAlgorithm.FeatureParams, ParameterType.IN_F, algorithmAddReqDTO);
            //            // 持久化数据
            //            this.AddAlgorithm(algorithmAddReqDTO);
            //            if (string.IsNullOrEmpty(algCode))
            //            {
            //                // 表示所有，继续遍历其它算法
            //                continue;
            //            }
            //            else
            //            {
            //                // 表示指定算法
            //                break;
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        LOG.Error("从本地注册算法对象失败", ex);
            //        continue;
            //    }
            //}
            //return true;
        }

        private void GetAlgParameters(IDictionary<String, Property> parameters, string parameterType, AlgorithmAddReqDTO algorithmAddReqDTO)
        {
            if (parameters != null && parameters.Count > 0)
            {
                AlgorithmMetaReqDTO tempAlgMeta = null;
                foreach (var item in parameters)
                {
                    tempAlgMeta = new AlgorithmMetaReqDTO
                    {
                        Name = item.Value.Name,
                        DataType = EnumUtil.GetEnumName<EnumValueMetaType>(item.Value.DataType),
                        Inout = parameterType,
                        IsVisible = item.Value.IsVisible,
                        Remark = item.Value.Remark,
                        Alias = item.Value.Alias,
                        ReadOnly = item.Value.ReadOnly
                    };
                    algorithmAddReqDTO.Metas.Add(tempAlgMeta);
                }
            }
        }
    }
}
