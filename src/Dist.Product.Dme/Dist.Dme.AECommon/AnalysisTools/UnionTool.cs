using ESRI.ArcGIS.AnalysisTools;
using ESRI.ArcGIS.Geoprocessor;
using log4net;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.AECommon.AnalysisTools
{
    /// <summary>
    /// 联合。
    /// 多边形叠合，输出层为保留原来两个输入图层的所有多边形
    /// </summary>
    public sealed class UnionTool
    {
        private static ILog LOG = LogManager.GetLogger(typeof(UnionTool));

        /// <summary>
        /// 容差值
        /// </summary>
        public string ClusterTolerance { get; set; }
        /// <summary>
        /// 输入的图层集合，可以多个图层
        /// </summary>
        public String[] InputFeatures { get; set; }
        /// <summary>
        /// 联合后生成的图层
        /// </summary>
        public String OutputFeature { get; set; }

        public UnionTool()
        {
            ClusterTolerance = "0.001";
        }

        public object Excute()
        {
            try
            {
                Geoprocessor gp = new Geoprocessor
                {
                    OverwriteOutput = true
                };

                Union pUnionOper = new ESRI.ArcGIS.AnalysisTools.Union
                {
                    in_features = string.Join(";", InputFeatures),
                    out_feature_class = OutputFeature,
                    cluster_tolerance = ClusterTolerance
                };
                gp.ProgressChanged += new EventHandler<ProgressChangedEventArgs>(GPProgressChanged);
                gp.ToolExecuting += new EventHandler<ToolExecutingEventArgs>(GPToolExecuting);

                return gp.Execute(pUnionOper as IGPProcess, null);
            }
            catch (Exception ex)
            {
                LOG.Error("执行feature union出现异常，详情：" + ex.Message);
                throw ex;
            }
        }
        void GPProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }
        void GPToolExecuting(object sender, ToolExecutingEventArgs e)
        {
        }
    }
}
