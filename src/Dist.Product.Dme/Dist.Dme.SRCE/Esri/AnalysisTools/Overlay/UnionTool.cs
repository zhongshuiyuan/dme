using ESRI.ArcGIS.AnalysisTools;
using ESRI.ArcGIS.Geoprocessor;
using NLog;
using System;

namespace Dist.Dme.SRCE.Esri.AnalysisTools.Overlay
{
    /// <summary>
    /// 联合。
    /// 要素类的叠合，输出层为保留原来两个输入图层的所有多边形
    /// </summary>
    public sealed class UnionTool
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

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
