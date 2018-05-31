using System;
using System.Collections.Generic;
using System.Text;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Model.DTO;

namespace Dist.Dme.Service.Interfaces
{
    /// <summary>
    /// 算法服务
    /// </summary>
    public interface IAlgorithmService : IBizService
    {
        /// <summary>
        /// 获取单个算法
        /// </summary>
        /// <param name="code">算法唯一编码</param>
        /// <param name="hasMeta">是否需要获取算法元数据信息</param>
        /// <returns></returns>
        AlgorithmRespDTO GetAlgorithmByCode(String code, bool hasMeta);
        /// <summary>
        /// 获取算法列表
        /// </summary>
        /// <param name="needMeta">是否需要获取算法元数据信息</param>
        /// <returns></returns>
        object ListAlgorithms(Boolean needMeta);
        /// <summary>
        /// 注册算法
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        object AddAlgorithm(AlgorithmAddReqDTO dto);
        /// <summary>
        /// 获取本地dll的所有算法对象元数据信息
        /// </summary>
        /// <returns></returns>
        object ListAlgorithmMetadatasLocal();
        /// <summary>
        /// 从本地查找算法，并注册算法
        /// </summary>
        /// <param name="algCode">算法唯一编码</param>
        /// <returns></returns>
        object RegistryAlgorithmFromLocal(string algCode);
    }
}
