using Dist.Dme.SRCE.Esri.DTO;
using Dist.Dme.SRCE.Esri.Utils;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using log4net;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.SRCE.Esri.AnalysisTools.Overlay
{
    /// <summary>
    /// 空间叠加分析通用工具
    /// </summary>
    public class OverlayCommonTool
    {
        private static ILog LOG = LogManager.GetLogger(typeof(OverlayCommonTool));
        /// <summary>
        /// 获取topo下的geometry union的数据
        /// </summary>
        /// <param name="featureClass">源要素类</param>
        /// <param name="queryClause">查询条件</param>
        /// <param name="queryGeometry">指定范围下</param>
        /// <param name="spatialRelType">空间参考类型</param>
        /// <param name="topoUnionGeometry">输出topo union后的geometry</param>
        /// <param name="oidList">输出OID集合</param>
        /// <param name="geometryField">geometry字段名称，默认为Shape</param>
        public static void GetTopounionGeometryByQuery(IFeatureClass featureClass, String queryClause,
          IGeometry queryGeometry, esriSpatialRelEnum spatialRelType,
          out IGeometry topoUnionGeometry, out IList<string> oidList, string geometryField = "Shape")
        {
            List<IFeature> featureList = new List<IFeature>();
            topoUnionGeometry = null;
            oidList = new List<string>();
            try
            {
                ISpatialFilter spatialFilter = new SpatialFilter();
                if (!string.IsNullOrEmpty(queryClause))
                {
                    spatialFilter.WhereClause = queryClause;
                }
                if (!string.IsNullOrEmpty(geometryField))
                {
                    spatialFilter.GeometryField = geometryField;
                }
                if (queryGeometry != null)
                {
                    spatialFilter.Geometry = queryGeometry;
                    spatialFilter.SpatialRel = spatialRelType;
                }
                ITopologicalOperator topologicalOperator = null;
                IFeatureCursor featureCursor = featureClass.Search(spatialFilter, false);
                IFeature feature = featureCursor.NextFeature();
                IGeometry geometryTemp;

                while (feature != null)
                {
                    geometryTemp = feature.Shape; // feature.ShapeCopy;
                    if (topologicalOperator == null)
                    {
                        topologicalOperator = geometryTemp as ITopologicalOperator;
                    } 
                    else
                    {
                        topologicalOperator = topologicalOperator.Union(geometryTemp) as ITopologicalOperator;
                    }
                    oidList.Add(feature.OID.ToString());
    
                    featureList.Add(feature);
                    feature = featureCursor.NextFeature();
                }

                topoUnionGeometry = topologicalOperator as IGeometry;
                WorkspaceUtil.ReleaseComObject(featureCursor);
            }
            catch (Exception e)
            {
                LOG.Error(e);
            }
        }
        /// <summary>
        /// 获取指定空间范围下相交的要素集合
        /// </summary>
        /// <param name="featureClass">被查询要素类</param>
        /// <param name="queryClause">查询条件</param>
        /// <param name="queryGeometry">指定空间范围</param>
        /// <param name="intersectFeatureDTOs">输出相交要素信息</param>
        /// <param name="sumIntersectArea">输出相交部分的总面积</param>
        /// <param name="geometryField">geometry字段名称，默认为Shape</param>
        public static void GetIntersectFeaturesByQuery(IFeatureClass featureClass, string queryClause,
         IGeometry queryGeometry, out IList<IntersectFeatureDTO> intersectFeatureDTOs,
         out double sumIntersectArea, string geometryField = "Shape")
        {
            List<IFeature> featureList = new List<IFeature>();
            intersectFeatureDTOs = new List<IntersectFeatureDTO>();
            sumIntersectArea = 0d;

            try
            {
                ISpatialFilter spatialFilter = new SpatialFilter();
                if (queryClause != null)
                {
                    spatialFilter.WhereClause = queryClause;
                }
                if (queryGeometry != null)
                {
                    spatialFilter.Geometry = queryGeometry;
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                }
                if (geometryField != null)
                {
                    spatialFilter.GeometryField = geometryField;
                }

                IFeatureCursor featureCursor = featureClass.Search(spatialFilter, false);
                IFeature feature = featureCursor.NextFeature();

                IntersectFeatureDTO intersectFeatureDTOTemp = null;

                while (feature != null)
                {
                    IGeometry intersectGeometry = OverlayCommonTool.GetIntersectGeometry(queryGeometry as IPolygon, feature.Shape as IPolygon);

                    if (null == intersectGeometry || intersectGeometry.IsEmpty)
                    {
                        continue;
                    }
                    intersectFeatureDTOTemp = new IntersectFeatureDTO
                    {
                        Geometry = feature.Shape,
                        OID = feature.OID,
                        GeometryCoord = GeometryUtil.ConvertGeometryToJson(intersectGeometry, out string message),
                        Area = GeometryUtil.GetArea(intersectGeometry),
                        QueryGeometry = queryGeometry,
                        IntersectGeometry = intersectGeometry
                    };

                    IArea area = intersectGeometry as IArea;
                    double intersectArea = area.Area;
                    sumIntersectArea += intersectArea;

                    featureList.Add(feature);
                    feature = featureCursor.NextFeature();
                }
                WorkspaceUtil.ReleaseComObject(featureCursor);
            }
            catch (Exception e)
            {
                LOG.Error(e);
            }
        }
        /// <summary>
        /// 获取两个Polygon的相交部分geometry
        /// </summary>
        /// <param name="polygon1"></param>
        /// <param name="polygon2"></param>
        /// <returns></returns>
        public static IGeometry GetIntersectGeometry(IPolygon polygon1, IPolygon polygon2)
        {
            try
            {
                bool isTouch = IsTouch(polygon1, polygon2);
                if (isTouch)
                {
                    return null;
                }

                ITopologicalOperator4 topo = polygon1 as ITopologicalOperator4;

                IGeometry intersectGeom = topo.Intersect(polygon2
                    , esriGeometryDimension.esriGeometry2Dimension);

                return intersectGeom;
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
            }
            return null;
        }
        /// <summary>
        /// sourceGeometry与targetGeometry是否相邻
        /// </summary>
        /// <param name="sourceGeometry">源</param>
        /// <param name="targetGeometry">目标</param>
        /// <returns></returns>
        public static bool IsTouch(IGeometry sourceGeometry, IGeometry targetGeometry)
        {
            try
            {
                IRelationalOperator relationalOperator = sourceGeometry as IRelationalOperator;
                return relationalOperator.Touches(targetGeometry);
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
            }
            return false;
        }
    }
}
