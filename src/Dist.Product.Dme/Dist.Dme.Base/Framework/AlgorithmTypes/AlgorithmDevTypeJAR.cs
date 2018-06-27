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
                AlgorithmMetaDefine md = new AlgorithmMetaDefine();
                IDictionary<string, string> metadata = new Dictionary<string, string>
                {
                    { nameof(md.Assembly), "JAR包名称" },
                    { nameof(md.MainClass), "主类名称"},
                    { nameof(md.MainMethod), "主方法"},
                    { nameof(md.Path), "JAR包路径"}
                };

                return metadata;
            }
        }
    }
}
