using Dist.Dme.SRCE.Esri.DTO;
using Dist.Dme.SRCE.Esri.Utils;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using NLog;
using System;
using System.Collections.Generic;

namespace Dist.Dme.SRCE.Esri.AnalysisTools.Overlay
{
    /// <summary>
    /// 空间叠加分析通用工具
    /// </summary>
    public class OverlayCommonTool
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
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
                LOG.Error($"{nameof(GetTopounionGeometryByQuery)}抛异常", e);
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
                        CoordJson = GeometryUtil.ConvertGeometryToJson(intersectGeometry, out string message),
                        Area = GeometryUtil.GetArea(intersectGeometry),
                        QueryGeometry = queryGeometry,
                        IntersectGeometry = intersectGeometry
                    };
                    intersectFeatureDTOs.Add(intersectFeatureDTOTemp);
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
        /// <summary>
        /// 获取一个面相对于另一个面差异的部分，实现擦除分析功能（Erase）
        /// </summary>
        /// <param name="polygon1">是个面对象</param>
        /// <param name="polygon2">是个面对象</param>
        /// <returns>获取polygon1中去掉polygon1和polygon2重合部分的部分</returns>
        public static IGeometry GetDifference(IPolygon polygon1, IPolygon polygon2)
        {
            try
            {
                ITopologicalOperator topo = polygon1 as ITopologicalOperator;
                IGeometry differGeom = topo.Difference(polygon2);

                return differGeom;
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
            }
            return null;
        }
        /// <summary>
        /// 两个要素类进行擦除操作
        /// </summary>
        /// <param name="sourceFeatureClass">源要素类</param>
        /// <param name="eraseFeatureClass">用于擦除的要素类，必须是个面要素类</param>
        public static void Erase(IFeatureClass sourceFeatureClass, IFeatureClass eraseFeatureClass)
        {
            if (eraseFeatureClass.ShapeType != esriGeometryType.esriGeometryPolygon)
            {
                throw new Exception("EraseFeatureClass不是个面要素类");
            }
            if (esriGeometryType.esriGeometryPoint == sourceFeatureClass.ShapeType)
            {
                throw new Exception("SourceFeatureClass不能是个点要素类");
            }
            // 先要临时复制一个要素类

            // 遍历用于擦除的要素
            IFeatureCursor eraseFeatureCursor = eraseFeatureClass.Search(null, false);
            IFeature eraseFeature = null;
            while ((eraseFeature = eraseFeatureCursor.NextFeature()) != null)
            {
                ISpatialFilter spatialFilter = new SpatialFilterClass
                {
                    Geometry = eraseFeature.Shape,
                    SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects
                };
                IFeatureCursor sourceFeatureCursor = sourceFeatureClass.Update(spatialFilter, false);
                IFeature sourceFeature = null;
                while ((sourceFeature = sourceFeatureCursor.NextFeature()) != null)
                {
                    IGeometry geometry = sourceFeature.ShapeCopy;
                    ITopologicalOperator topoOper = geometry as ITopologicalOperator;
                    IGeometry geoDifference = topoOper.Difference(eraseFeature.Shape);
                    sourceFeature.Shape = geoDifference;
                    sourceFeatureCursor.UpdateFeature(sourceFeature);
                }
                // 释放游标
                WorkspaceUtil.ReleaseComObject(sourceFeatureCursor);
            }
        }
    }
}
