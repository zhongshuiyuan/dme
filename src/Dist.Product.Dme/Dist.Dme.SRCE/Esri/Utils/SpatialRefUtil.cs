using ESRI.ArcGIS.Geometry;
using NLog;
using System;

namespace Dist.Dme.SRCE.Esri.Utils
{
    /// <summary>
    /// 空间参考工具类
    /// </summary>
    public class SpatialRefUtil
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 通过prj文件创建SpatialRef
        /// </summary>
        /// <param name="prjFile"></param>
        /// <returns></returns>
        public static ISpatialReference CreateSpatialRefByPrjFile(string prjFile)
        {
            try
            {
                ISpatialReferenceFactory srf = new SpatialReferenceEnvironmentClass();
                ISpatialReference spaRef = srf.CreateESRISpatialReferenceFromPRJFile(prjFile);
                return spaRef;
            }
            catch (Exception ex)
            {
                LOG.Error($"创建空间参考文件失败[{prjFile}]", ex);
            }
            return null;
        }
        /// <summary>
        /// 通过投影类型获取投影坐标系
        /// </summary>
        /// <param name="pcsType"></param>
        /// <returns></returns>
        public static ISpatialReference GetProjectedCoordinate(esriSRProjCS4Type pcsType)
        {
            ISpatialReferenceFactory2 pSpatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            ISpatialReference pSpatialReference = pSpatialReferenceFactory.CreateProjectedCoordinateSystem((int)pcsType);
            return pSpatialReference;
        }
    }
}
