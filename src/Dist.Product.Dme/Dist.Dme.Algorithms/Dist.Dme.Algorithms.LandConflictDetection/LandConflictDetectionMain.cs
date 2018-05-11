using Dist.Dme.AECommon.AnalysisTools;
using Dist.Dme.AECommon.Utils;
using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Framework.AlgorithmTypes;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using ESRI.ArcGIS;
using ESRI.ArcGIS.Geodatabase;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;

namespace Dist.Dme.Plugins.LandConflictDetection
{
    /// <summary>
    /// 用地冲突分析
    /// </summary>
    public class LandConflictDetectionMain : BaseAlgorithm, IAlgorithm
    {
        private static ILog LOG = LogManager.GetLogger(typeof(LandConflictDetectionMain));

        public override string SysCode => "302039a48cd545ce92b5ef9def690259";

        public override string Name => "LandConflictDetection_ZYKY";

        public override string Alias => "差异分析";

        public override string Version => "1.0.0";

        public override string Remark => "差异分析";
        /// <summary>
        /// 要素路径分隔符
        /// </summary>
        private static String SEPARATOR_FEATURE_PATH = "|";
        private IFeatureWorkspace m_localWorkspace_first;
        private string m_database_first;
        /// <summary>
        /// 总规用地图层路径（first和second寓意第一个图层和第二个图层，换成其他业务图层也可以，只要满足这样的规则）
        /// </summary>
        private string m_featureClass_source_first;
        /// <summary>
        /// 总规用地-用地代码
        /// </summary>
        private string m_yddm_first;
        private IFeatureWorkspace m_localWorkspace_second;
        /// <summary>
        /// 
        /// </summary>
        private string m_database_second;
        /// <summary>
        /// 控规用地图层路径
        /// </summary>
        private string m_featureClass_source_second;
        /// <summary>
        /// 控规用地-用地代码
        /// </summary>
        private string m_yddm_second;
        /// <summary>
        /// 打开的要素类对象
        /// </summary>
        private IFeatureClass m_featureClass_first = null, m_featureClass_second = null;

        private string sourceLayerFirstFullPath;
        private string sourceLayerSecondFullPath;
        /// <summary>
        /// 结果输出GDB路径
        /// </summary>
        public string ResultGDBPath { get; set; }
        /// <summary>
        /// 结果图层名称
        /// </summary>
        public string ReulstLayerName { get { return "FC_RESULT"; } }

        public LandConflictDetectionMain()
        {
            if (!ESRI.ArcGIS.RuntimeManager.Bind(ProductCode.EngineOrDesktop))
            {
                LOG.Error("arcgis license init error");
                throw new Exception("arcgis license init error");
            }
            LicenseUtil.CheckOutLicenseAdvanced();
            // 初始化输入参数
            base.InputParameters.Add("m_featureClass_source_first",
                new Property("m_featureClass_source_first", "总规用地的图层信息", ValueTypeMeta.TYPE_MDB_FEATURECLASS, "", "", 1, "总规用地的图层信息，格式：mdb路径"+ SEPARATOR_FEATURE_PATH + "要素类"));
            base.InputParameters.Add("m_featureClass_source_second",
                new Property("m_featureClass_source_second", "控规用地的图层信息", ValueTypeMeta.TYPE_MDB_FEATURECLASS, "", "", 1, "控规用地的图层路径信息，格式：mdb路径" + SEPARATOR_FEATURE_PATH + "要素类"));
            base.InputParameters.Add("m_yddm_first",
                new Property("m_yddm_zgyd", "总规用地代码属性", ValueTypeMeta.TYPE_STRING, "", "", 1, "总规用地代码属性"));
            base.InputParameters.Add("m_yddm_second",
                new Property("m_yddm_second", "控规用地代码属性", ValueTypeMeta.TYPE_STRING, "", "", 1, "控规用地代码属性"));

            // 初始化输出参数
            string resultGDBDir = System.AppDomain.CurrentDomain.BaseDirectory + "/result/";
            base.OutputParameters.Add("ResultGDBPath", new Property("ResultGDBPath", "输出结果", ValueTypeMeta.TYPE_GDB_PATH, resultGDBDir, resultGDBDir, 1, "分析结果为gdb文件"));
            base.OutputParameters.Add("ReulstLayerName", new Property("ReulstLayerName", "图层名称", ValueTypeMeta.TYPE_STRING, this.ReulstLayerName, this.ReulstLayerName, 1, "分析结果为gdb文件", 1));
        }

        public override Result Execute()
        {
            string resultGDBPath = this.ResultGDBPath;
            try
            {
                if (!Directory.Exists(this.ResultGDBPath))
                {
                    DirectoryInfo dirInfo = Directory.CreateDirectory(this.ResultGDBPath);
                }
                resultGDBPath = Path.Combine(this.ResultGDBPath, string.Format("result_{0}.gdb", DateUtil.CurrentTimeMillis));
                string pTempletMDBFile = AppDomain.CurrentDomain.BaseDirectory + "template/result.gdb";
                long beginMillisecond = DateUtil.CurrentTimeMillis;
                // 创建一份gdb
                DirectFileUtil.CopyDirectInfo(pTempletMDBFile, resultGDBPath);
                IWorkspace resultWorkspace = WorkspaceServices.OpenFileGdbWorkspace(resultGDBPath);
                LOG.Info("创建GDB消耗时间：[" + (DateUtil.CurrentTimeMillis - beginMillisecond) + "] 毫秒");
                // 拷贝源对比图层
                beginMillisecond = DateUtil.CurrentTimeMillis;
                //if (!WorkspaceServices.CopyLayer(new string[] { this.sourceLayerFirstFullPath, this.sourceLayerSecondFullPath }, resultGDBPath))
                //{
                //    LOG.Error("拷贝图层失败，中止计算");
                //    return new Result(STATUS.ERROR, "拷贝图层失败", SysStatusCode.DME3000, false);
                //}
                //LOG.Info("拷贝源对比图层消耗时间：[" + (DateUtil.CurrentTimeMillis() - beginMillisecond) + "] 毫秒");
                // 图层融合（union）
                UnionTool unionTool = new UnionTool
                {
                    InputFeatures = new String[] { this.sourceLayerFirstFullPath, this.sourceLayerSecondFullPath },
                    OutputFeature = resultGDBPath + "/" + this.ReulstLayerName
                };
                object result = unionTool.Excute();
                if (null == result)
                {
                    return new Result(STATUS.ERROR, "图层联合分析失败", SysStatusCode.DME3000, false);
                }
                // 对输出图层进行规则计算
                // TODO
                return new Result(STATUS.SUCCESS, "差异分析完成", SysStatusCode.DME1000, true);
            } catch (Exception ex)
            {
                LOG.Error("差异分析失败，详情：" + ex.Message);
                return new Result(STATUS.ERROR, "差异分析失败，详情：" + ex.Message, SysStatusCode.DME3000, false);
            }
        }

        public override object InParams
        {
            get
            {
                return base.InputParameters;
            }
        }

        public override object OutParams
        {
            get
            {
                return base.OutputParameters;
            }
        }
        public override object FeatureParams
        {
            get
            {
                return base.FeatureParameters;
            }
        }
        public override void Init(IDictionary<string, object> parameters)
        {
            if (!parameters.ContainsKey("m_featureClass_source_first"))
            {
                throw new Exception("没有找到输入参数【总规用地图层】");
            }
            if (!parameters.ContainsKey("m_yddm_first"))
            {
                throw new Exception("没有找到输入参数【总规用地代码属性】");
            }
            if (!parameters.ContainsKey("m_featureClass_source_second"))
            {
                throw new Exception("没有找到输入参数【控规用地图层】");
            }
            if (!parameters.ContainsKey("m_yddm_second"))
            {
                throw new Exception("没有找到输入参数【控规用地代码属性】");
            }
            if (!parameters.ContainsKey("ResultGDBPath"))
            {
                LOG.Info("没有指定新的输出路径，现在使用默认路径");
                this.ResultGDBPath = base.OutputParameters["ResultGDBPath"].DefaultValue.ToString();
            }
            else
            {
                LOG.Info("使用新的输出路径");
                this.ResultGDBPath = parameters["ResultGDBPath"].ToString();
            }

            this.m_featureClass_source_first = parameters["m_featureClass_source_first"].ToString();
            WorkspaceServices.OpenFeatureClass(this.m_featureClass_source_first, SEPARATOR_FEATURE_PATH, out this.m_localWorkspace_first, out this.m_featureClass_first);
            if (this.m_featureClass_first.FeatureDataset != null)
            {
                this.sourceLayerFirstFullPath = this.m_featureClass_source_first.Split(SEPARATOR_FEATURE_PATH)[0] + "/" + this.m_featureClass_first.FeatureDataset.Name + "/" + this.m_featureClass_source_first.Split(SEPARATOR_FEATURE_PATH)[1];
            }
            else
            {
                this.sourceLayerFirstFullPath = this.m_featureClass_source_first.Split(SEPARATOR_FEATURE_PATH)[0] + "/" + this.m_featureClass_source_first.Split(SEPARATOR_FEATURE_PATH)[1];
            }
            this.m_featureClass_source_second = parameters["m_featureClass_source_second"].ToString();
            WorkspaceServices.OpenFeatureClass(this.m_featureClass_source_second, SEPARATOR_FEATURE_PATH, out this.m_localWorkspace_second, out this.m_featureClass_second);
            if (this.m_featureClass_second.FeatureDataset != null)
            {
                this.sourceLayerSecondFullPath = this.m_featureClass_source_second.Split(SEPARATOR_FEATURE_PATH)[0] + "/" + this.m_featureClass_second.FeatureDataset.Name + "/" + this.m_featureClass_source_second.Split(SEPARATOR_FEATURE_PATH)[1];
            }
            else
            {
                this.sourceLayerSecondFullPath = this.m_featureClass_source_second.Split(SEPARATOR_FEATURE_PATH)[0] + "/" + this.m_featureClass_source_second.Split(SEPARATOR_FEATURE_PATH)[1];
            }

            this.m_yddm_first = parameters["m_yddm_first"].ToString();
            this.m_yddm_second = parameters["m_yddm_second"].ToString();
        }
        public override object MetadataJSON
        {
            get
            {
                IDictionary<string, System.Object> dictionary = new Dictionary<string, System.Object>
                {
                    ["SysCode"] = this.SysCode,
                    ["Name"] = this.Name,
                    ["Alias"] = this.Alias,
                    ["Version"] = this.Version,
                    ["Remark"] = this.Remark,
                    ["InputParameters"] = this.InputParameters,
                    ["OutputParameters"] = this.OutputParameters,
                    ["FeatureParameters"] = this.FeatureParameters
                };
                return dictionary;// JsonConvert.SerializeObject(dictionary);
            }
        }

        public override IAlgorithmDevType AlgorithmType => new AlgorithmDevTypeDLL();
    }
}
