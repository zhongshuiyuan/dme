using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework;
using System;
using System.Collections.Generic;

namespace Dist.Dme.Plugins.LandConflictDetection
{
    public class Main : IAlgorithm
    {
        public string SysCode => "302039a48cd545ce92b5ef9def690259";

        public string Name => "ZYKY";

        public string Alias => "总规控规差异分析";

        public string Version => "1.0.0";

        public string Remark => "总规控规差异分析";

        private IDictionary<String, Property> inParameters = new Dictionary<String, Property>();
        /// <summary>
        /// 总规用地图层路径
        /// </summary>
        private string m_featurePath_zgyd;
        /// <summary>
        /// 控规用地图层路径
        /// </summary>
        private string m_featurePath_kgyd;

        public bool Execute()
        {
            throw new NotImplementedException();
        }

        public object GetInParameters()
        {
            this.inParameters.Add("m_featurePath_zgyd", new Property("m_featurePath_zgyd", ValueTypeMeta.TYPE_STRING, "", "", 1, "总规用地的图层路径"));
            this.inParameters.Add("m_featurePath_kgyd", new Property("m_featurePath_kgyd", ValueTypeMeta.TYPE_STRING, "", "", 1, "控规用地的图层路径"));

            return inParameters;
        }

        public object GetOutParameters()
        {
            throw new NotImplementedException();
        }

        public bool Init()
        {
            throw new NotImplementedException();
        }
    }
}
