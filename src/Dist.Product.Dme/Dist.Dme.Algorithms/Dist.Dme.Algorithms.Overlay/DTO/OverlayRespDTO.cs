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
        public double SumArea { get; set; }
        /// <summary>
        /// 相交实体集合
        /// </summary>
        public IList<FeatureRespDTO> Features { get; set; } = new List<FeatureRespDTO>();

    }
    public class FeatureRespDTO
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
