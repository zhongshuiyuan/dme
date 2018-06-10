using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Framework.AlgorithmTypes;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.SRCE.Esri.Utils;
using ESRI.ArcGIS;
using log4net;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Algorithms.Overlay
{
    public class OverlayMain : BaseAlgorithm, IAlgorithm
    {
        private static ILog LOG = LogManager.GetLogger(typeof(OverlayMain));

        public override string SysCode => "6e18c8abbf5448c7a4c15cfd4c6eed8c";

        public override string Name => "OverlayAnalysis";

        public override string Alias => "两个要素类进行压盖分析";

        public override string Version => "1.0.0";

        public override string Remark => "两个要素类进行压盖分析";

        public override IAlgorithmDevType AlgorithmType => new AlgorithmDevTypeDLL();

        public OverlayMain()
        {
            if (!ESRI.ArcGIS.RuntimeManager.Bind(ProductCode.EngineOrDesktop))
            {
                LOG.Error("arcgis license init error");
                throw new Exception("arcgis license init error");
            }
            LicenseUtil.CheckOutLicenseAdvanced();

        }
        public override Result Execute()
        {
            throw new NotImplementedException();
        }

        public override void Init(IDictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }
    }
}
