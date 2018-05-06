using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Utils;
using Dist.Dme.DAL.Context;
using Dist.Dme.Model;
using Dist.Dme.Model.DTO;
using Dist.Dme.Model.Entity;
using Dist.Dme.Plugins.LandConflictDetection;
using Dist.Dme.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Service.Impls
{
    /// <summary>
    /// 模型服务
    /// </summary>
    public class ModelService : AbstractContext, IModelService
    {
        /// <summary>
        /// 用地差异分析
        /// </summary>
        private IAlgorithm landConflictDetectionAlgorithm = new LandConflictDetectionMain();

        public object GetLandConflictInputParameters()
        {
            return this.landConflictDetectionAlgorithm.GetInParameters();
        }
        public object GetLandConflictOutputParameters()
        {
            return this.landConflictDetectionAlgorithm.GetOutParameters();
        }

        public object LandConflictExecute(IDictionary<string, object> parameters)
        {
            this.landConflictDetectionAlgorithm.Init(parameters);
            return this.landConflictDetectionAlgorithm.Execute();
        }
    }
}
