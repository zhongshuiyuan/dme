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
        IList<IRuleStep> Steps { get; set; }
        /// <summary>
        /// 模型元数据
        /// </summary>
        IModelMeta ModelMeta { get; set; }
        /// <summary>
        /// 检测模型
        /// </summary>
        /// <param name="modelId">模型id</param>
        /// <param name="versionId">模型版本id</param>
        /// <returns></returns>
        object CheckModel(int modelId, int versionId);
    }
}
