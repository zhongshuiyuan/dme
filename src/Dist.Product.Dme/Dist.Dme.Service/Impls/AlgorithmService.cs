using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.DAL.Context;
using Dist.Dme.Model.DTO;
using Dist.Dme.Model.Entity;
using Dist.Dme.Service.Interfaces;
using log4net;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dist.Dme.Service.Impls
{
    /// <summary>
    /// 算法服务实现
    /// </summary>
    public class AlgorithmService : AbstractContext, IAlgorithmService
    {
        private static ILog LOG = LogManager.GetLogger(typeof(AlgorithmService));

        public object ListAlgorithms(bool hasMeta)
        {
            List<DmeAlgorithm> algs = base.DmeAlgorithmDb.GetList("CREATETIME", false);
            if (null == algs || 0 == algs.Count)
            {
                return algs;
            }
            if (hasMeta)
            {
                IList<AlgorithmRespDTO> algDTOs = new List<AlgorithmRespDTO>();
                AlgorithmRespDTO algorithmDTO = null;
                IList<DmeAlgorithmMeta> metas = null;
                foreach (var alg in algs)
                {
                    algorithmDTO = ClassValueCopier<AlgorithmRespDTO>.Copy(alg);
                    if(!string.IsNullOrEmpty(alg.Extension))
                    {
                        algorithmDTO.Extension = JsonConvert.DeserializeObject(alg.Extension);
                    }
                    algDTOs.Add(algorithmDTO);
                    metas = base.Db.Queryable<DmeAlgorithmMeta>().Where(meta => meta.AlgorithmId == alg.Id).ToList();
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
            DmeAlgorithm alg = base.Db.Queryable<DmeAlgorithm>().Single(a => a.SysCode == code);
            AlgorithmRespDTO algorithmDTO = ClassValueCopier<AlgorithmRespDTO>.Copy(alg);
            IList<DmeAlgorithmMeta>  metas = base.Db.Queryable<DmeAlgorithmMeta>().Where(meta => meta.AlgorithmId == alg.Id).ToList();
            if (metas !=null  && metas.Count >0)
            {
                algorithmDTO.Metas = metas;
            }
            return algorithmDTO;
        }
        public object AddAlgorithm(AlgorithmAddReqDTO dto)
        {
            DbResult<AlgorithmRespDTO> result = base.Db.Ado.UseTran<AlgorithmRespDTO>(() => 
            {
                DmeAlgorithm alg = base.Db.Queryable<DmeAlgorithm>().Where(a => a.SysCode == dto.SysCode).First();
                if (null == alg)
                { 
                    alg = ClassValueCopier<DmeAlgorithm>.Copy(dto);
                    alg.CreateTime = DateUtil.CurrentTimeMillis;
                    alg.Id = base.DmeAlgorithmDb.InsertReturnIdentity(alg);
                    AlgorithmRespDTO algorithmRespDTO = ClassValueCopier<AlgorithmRespDTO>.Copy(alg);
                    if (dto.Metas != null && dto.Metas.Count > 0)
                    {
                        algorithmRespDTO.Metas = new List<DmeAlgorithmMeta>();
                        DmeAlgorithmMeta meta = null;
                        foreach (var item in dto.Metas)
                        {
                            meta = ClassValueCopier<DmeAlgorithmMeta>.Copy(item);
                            meta.AlgorithmId = alg.Id;
                            meta.Id = base.DmeAlgorithmMetaDb.InsertReturnIdentity(meta);
                            algorithmRespDTO.Metas.Add(meta);
                        }
                    }
                    return algorithmRespDTO;
                }
                return this.GetAlgorithmByCode(alg.SysCode, true);
            });
            return result.Data;
        }

        public object ListAlgorithmMetadatasLocal()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                   .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IAlgorithm))))
                   .ToArray();
            if (null == types || 0 == types.Count())
            {
                return new List<IAlgorithm>();
            }
            IList<object> localAlgorithms = new List<object>();
            object temp = null;
            foreach (var type in types)
            {
                if (type.IsAbstract)
                {
                    // 抽象类排除
                    continue;
                }
                try
                {
                    temp = type.Assembly.CreateInstance(type.FullName, true);
                    if (null == temp)
                    {
                        continue;
                    }
                    localAlgorithms.Add(((IAlgorithm)temp).MetadataJSON);
                }
                catch (Exception ex)
                {
                    LOG.Error("获取本地算法对象失败", ex);
                    continue;
                }
            }
            return localAlgorithms;
        }
    }
}
