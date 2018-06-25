using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Algorithms.Overlay.DTO
{
    public class OverlayRespDTO
    {
        /// <summary>
        /// 相交的总面积
        /// </summary>
        public double SumIntersectArea { get; set; }
        /// <summary>
        /// 相交实体集合
        /// </summary>
        public IList<IntersectFeatureRespDTO> IntersectFeatures { get; set; } = new List<IntersectFeatureRespDTO>();

    }
    public class IntersectFeatureRespDTO
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
        public string CoordJson { get; set; }
    }
}
