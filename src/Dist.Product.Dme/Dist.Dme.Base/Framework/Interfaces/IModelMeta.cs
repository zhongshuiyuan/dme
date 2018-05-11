using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework.Interfaces
{
    /// <summary>
    /// 模型元数据接口
    /// </summary>
    public interface IModelMeta
    {
        /// <summary>
        /// 获取唯一标识
        /// </summary>
        /// <returns></returns>
        String SysCode { get; }
        /// <summary>
        /// 名称
        /// </summary>
        String Name { get; }
        /// <summary>
        /// 备注信息
        /// </summary>
        String Remark { get; }
        /// <summary>
        /// 创建者
        /// </summary>
        String Creator { get; set; }
    }
}
