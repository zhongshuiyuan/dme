using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework.Interfaces
{
    /// <summary>
    /// 模型接口
    /// </summary>
    public interface IModel
    {
        IList<ModelHopMeta> Hops { get; set; }
        /// <summary>
        /// 包含的所有步骤
        /// </summary>
        IList<IStep> Steps { get; set; }
        /// <summary>
        /// 模型元数据
        /// </summary>
        IModelMeta ModelMeta { get; set; }
    }
}
