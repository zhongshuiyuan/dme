using Dist.Dme.Base.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework.AlgorithmTypes
{
    /// <summary>
    /// 
    /// </summary>
    public class AlgorithmDevTypeJAR : IAlgorithmDevType
    {
        public string Code => "JAR";

        public string Name => "JAR依赖";

        public object Metadata
        {
            get
            {
                IList<AlgorithmMetaDefine> metadata = new List<AlgorithmMetaDefine>
                {
                    new AlgorithmMetaDefine("assembly", "", "JAR名称"),
                    new AlgorithmMetaDefine("mainClass", "", "主类名称"),
                    new AlgorithmMetaDefine("mainMethod", "", "主方法"),
                    new AlgorithmMetaDefine("path", "", "DLL路径")
                };

                return metadata;
            }
        }
    }
}
