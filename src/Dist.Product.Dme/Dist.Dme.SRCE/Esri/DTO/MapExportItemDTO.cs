using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.SRCE.Esri.DTO
{
    /// <summary>
    /// 地图输出信息
    /// </summary>
    public class MapExportItemDTO
    {
        public IGeometry ResultGeometry { get; set; }
        public IGeometry SourceGeometry { get; set; }
        public IGeometry TargetGeometry { get; set; }
        public string SourceFeatureClassName { get; set; }
        public string TargetFeatureClassName { get; set; }
        public string SourceFeatureID { get; set; }
        public string TargetFeatureID { get; set; }
        public List<string> OtherVisibleFeatureClassName { get; set; }
    }
}
