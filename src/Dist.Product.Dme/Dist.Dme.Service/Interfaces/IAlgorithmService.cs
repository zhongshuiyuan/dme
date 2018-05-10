using System;
using System.Collections.Generic;
using System.Text;
using Dist.Dme.Model.DTO;

namespace Dist.Dme.Service.Interfaces
{
    /// <summary>
    /// 算法服务
    /// </summary>
    public interface IAlgorithmService
    {
        /// <summary>
        /// 获取算法列表
        /// </summary>
        /// <param name="hasMeta">是否需要获取算法元数据信息</param>
        /// <returns></returns>
        object ListAlgorithm(Boolean hasMeta);
        /// <summary>
        /// 注册算法
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        object AddAlgorithm(AlgorithmAddReqDTO dto);
    }
}
