using Dist.Dme.Base.Framework.AlgorithmTypes;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.RuleSteps.AlgorithmInput.DTO
{
    public class AlgorithmDTO
    {
        public DmeAlgorithm Algorithm { get; set; }
        public AlgorithmMetaDefine MetaDefine { get; set; }
        public IAlgorithm AlgorithmInstance { get; set; }
    }
}
