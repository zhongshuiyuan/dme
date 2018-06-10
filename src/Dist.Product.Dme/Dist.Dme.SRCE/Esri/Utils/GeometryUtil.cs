using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using log4net;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.SRCE.Esri.Utils
{
    /// <summary>
    /// geometry工具类
    /// </summary>
    public class GeometryUtil
    {
        private static log4net.ILog LOG = LogManager.GetLogger(typeof(GeometryUtil));
        /// <summary>
        /// 转换点空间参考
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="sourceSpatialRef"></param>
        /// <param name="targetSpatialRef"></param>
        /// <returns></returns>
        public static IPoint ProjectPoint(double x, double y, 
            ISpatialReference sourceSpatialRef,
            ISpatialReference targetSpatialRef)
        {
            IPoint pPoint = new PointClass();
            pPoint.PutCoords(x, y);

            pPoint.SpatialReference = sourceSpatialRef;
            pPoint.Project(targetSpatialRef);

            return pPoint;
        }
        /// <summary>
        /// 点坐标空间参考转换
        /// </summary>
        /// <param name="point"></param>
        /// <param name="targetSpatialRef"></param>
        /// <returns></returns>
        public static IPoint ProjectPoint(IPoint point,
           ISpatialReference targetSpatialRef)
        {
            IPoint pPoint = new PointClass();
            pPoint.PutCoords(point.X, point.Y);

            pPoint.SpatialReference = point.SpatialReference;
            pPoint.Project(targetSpatialRef);

            return pPoint;
        }
        /// <summary>
        /// 默认西安80坐标转换
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="sourceSpatialReference">原来点坐标的空间参考</param>
        /// <returns></returns>
        public static IPoint ProjectPoint(double x, double y, ISpatialReference sourceSpatialReference)
        {
            IPoint pPoint = new PointClass();
            pPoint.PutCoords(x, y);

            pPoint.SpatialReference = sourceSpatialReference;
            ISpatialReference spatialReference_New = SpatialRefUtil.GetProjectedCoordinate(esriSRProjCS4Type.esriSRProjCS_Xian1980_3_Degree_GK_CM_120E);
            pPoint.Project(spatialReference_New);

            return pPoint;
        }
        /// <summary>
        /// 转换geometry为坐标串
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="message">输出消息（目前是错误消息）</param>
        /// <returns>返回坐标串</returns>
        public static string ConvertGeometryToJson(IGeometry geometry, out string message)
        {
            string geomJsonStr = null;
            message = string.Empty;
            try
            {
                ITopologicalOperator topoGeom = geometry as ITopologicalOperator;
                topoGeom.Simplify();
                if (topoGeom is IPolygon polygon)
                {
                    polygon.Generalize(1);
                }

                if (geometry.SpatialReference == null || geometry.SpatialReference.Name == "Unknown")
                {
                    geometry.SpatialReference = SpatialRefUtil.GetProjectedCoordinate(esriSRProjCS4Type.esriSRProjCS_Xian1980_3_Degree_GK_CM_120E);
                }

                ESRI.ArcGIS.esriSystem.IJSONWriter jsonWriter = new ESRI.ArcGIS.esriSystem.JSONWriterClass();
                jsonWriter.WriteToString();

                ESRI.ArcGIS.Geometry.JSONConverterGeometryClass jsonCon = new ESRI.ArcGIS.Geometry.JSONConverterGeometryClass();
                jsonCon.WriteGeometry(jsonWriter, null, geometry, false);

                geomJsonStr = Encoding.UTF8.GetString(jsonWriter.GetStringBuffer());
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
                message = ex.ToString();
            }
            return geomJsonStr;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strJson"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ESRI.ArcGIS.Geometry.IGeometry ConvertToGeometry(string strJson, esriGeometryType type)
        {
            return ConvertToGeometry(strJson, type, false, false);
        }
        /// <summary>
        /// JSON字符串转成IGeometry  
        /// </summary>
        /// <param name="strJson"></param>
        /// <param name="type"></param>
        /// <param name="bHasZ"></param>
        /// <param name="bHasM"></param>
        /// <returns></returns>
        public static IGeometry ConvertToGeometry(string strJson, esriGeometryType type,
        bool bDefaultHasZs, bool bDefaultHasMs)
        {
            IJSONReader jsonReader = new JSONReaderClass();
            jsonReader.ReadFromString(strJson);

            JSONConverterGeometryClass jsonCon = new JSONConverterGeometryClass();
            return jsonCon.ReadGeometry(jsonReader, type, bDefaultHasZs, bDefaultHasMs);
        }
        /// <summary>
        /// 获取面图形面积
        /// </summary>
        /// <param name="polygon">图形</param>
        /// <param name="digits">精度位数，默认为3位</param>
        /// <returns>面积</returns>
        public static double GetArea(IPolygon polygon, int digits = 3)
        {
            if (polygon == null || polygon.IsEmpty) return 0;

            IArea pArea = polygon as IArea;
            double dbArea = pArea.Area;
            dbArea = System.Math.Round(dbArea, digits);
            return pArea.Area;
        }
        /// <summary>
        /// 获取图形面积
        /// </summary>
        /// <param name="geometry">图形</param>
        /// <param name="digits">精度位数，默认为3位</param>
        /// <returns></returns>
        public static double GetArea(IGeometry geometry, int digits = 3)
        {
            if (geometry == null || geometry.IsEmpty) return 0;

            IArea pArea = geometry as IArea;
            double dbArea = pArea.Area;
            dbArea = System.Math.Round(dbArea, digits);
            return pArea.Area;
        }
        /// <summary>
        /// 获取polygon1超出polygon2部分的面积
        /// </summary>
        /// <param name="polygon1"></param>
        /// <param name="polygon2"></param>
        /// <returns></returns>
        public static double GetDifferenceeArea(IPolygon polygon1, IPolygon polygon2)
        {
            try
            {
                ITopologicalOperator topo = polygon1 as ITopologicalOperator;
                IGeometry differGeom = topo.Difference(polygon2);
                IArea area = differGeom as IArea;
                return area.Area;
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
            }
            return 0;
        }
    }
}
