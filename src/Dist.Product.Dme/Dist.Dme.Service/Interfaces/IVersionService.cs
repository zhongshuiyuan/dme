using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Model.DTO;
using Dist.Dme.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Service.Interfaces
{
    /// <summary>
    /// 用户服务
    /// </summary>
    public interface IVersionService : IBizService
    {
        /// <summary>
        /// 获取当前版本
        /// </summary>
        /// <returns></returns>
        DmeVersion GetCurrentVersion();
        /// <summary>
        /// 更新版本号
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        DmeVersion UpdateVersion(VersionDTO dto);
    }
}
