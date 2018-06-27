using Dist.Dme.Base.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework.AlgorithmTypes
{
    /// <summary>
    /// 
    /// </summary>
    public class AlgorithmDevTypeDLL : IAlgorithmDevType
    {
        public string Code => "DLL";

        public string Name => "C#动态库";

        public object Metadata
        {
            get
            {
                AlgorithmMetaDefine md = new AlgorithmMetaDefine();
                IDictionary<string, string> metadata = new Dictionary<string, string>
                {
                    { nameof(md.Assembly), "DLL库名称" },
                    { nameof(md.MainClass), "主类名称"},
                    { nameof(md.MainMethod), "主方法"},
                    { nameof(md.Path), "DLL路径"}
                };

                return metadata;
            }
        }
    }
}
