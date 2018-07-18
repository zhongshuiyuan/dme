using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.SRCE.Esri.DTO
{
    /// <summary>
    /// 相交的要素DTO
    /// </summary>
    public class IntersectFeatureDTO
    {
        /// <summary>
        /// 目标要素OID
        /// </summary>
        public int OID { get; set; }
        /// <summary>
        /// 相交那一部分的面积
        /// </summary>
        public double Area { get; set; }
        /// <summary>
        /// 相交那一部分的图形坐标串，JSON格式
        /// </summary>
        public string Coordinates { get; set; }
        /// <summary>
        /// 查询图形范围
        /// </summary>
        public IGeometry QueryGeometry { get; set; }
        /// <summary>
        /// 与QueryGeometry相交的目标geometry
        /// </summary>
        public IGeometry Geometry { get; set; }
        /// <summary>
        /// 相交那部分geometry
        /// </summary>
        public IGeometry IntersectGeometry { get; set; }
        /// <summary>
        /// geometry类型
        /// </summary>
        public string GeoType { get; set; }

    }
}
