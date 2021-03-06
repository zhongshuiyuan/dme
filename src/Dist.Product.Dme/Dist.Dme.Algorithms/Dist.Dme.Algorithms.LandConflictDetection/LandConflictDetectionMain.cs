﻿using Dist.Dme.Base.Common;
using Dist.Dme.Base.Conf;
using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Framework.AlgorithmTypes;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.SRCE.Esri.AnalysisTools.Overlay;
using Dist.Dme.SRCE.Esri.Utils;
using ESRI.ArcGIS;
using ESRI.ArcGIS.Geodatabase;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;

namespace Dist.Dme.Algorithms.LandConflictDetection
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
        private static String SEPARATOR_FEATURE_PATH = "&"; 
        private IFeatureWorkspace m_localWorkspace_first;
        private string m_database_first;
        /// <summary>
        /// 总规用地图层路径（first和second寓意第一个图层和第二个图层，换成其他业务图层也可以，只要满足这样的规则）
        /// </summary>
        private string FeatureClass_Source_First;
        /// <summary>
        /// 总规用地-用地代码
        /// </summary>
        private string Yddm_First;
        private IFeatureWorkspace m_localWorkspace_second;
        /// <summary>
        /// 
        /// </summary>
        private string m_database_second;
        /// <summary>
        /// 控规用地图层路径
        /// </summary>
        private string FeatureClass_Source_Second;
        /// <summary>
        /// 控规用地-用地代码
        /// </summary>
        private string Yddm_Second;
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
            base.InputParametersMeta.Add(nameof(this.FeatureClass_Source_First),
                new Property(nameof(this.FeatureClass_Source_First), "总规用地的图层信息", Base.Common.EnumValueMetaType.TYPE_MDB_FEATURECLASS, "", "", "总规用地的图层信息，格式：mdb路径" + SEPARATOR_FEATURE_PATH + "要素类名称", null));
            base.InputParametersMeta.Add(nameof(this.FeatureClass_Source_Second),
                new Property(nameof(this.FeatureClass_Source_Second), "控规用地的图层信息", Base.Common.EnumValueMetaType.TYPE_MDB_FEATURECLASS, "", "", "控规用地的图层路径信息，格式：mdb路径" + SEPARATOR_FEATURE_PATH + "要素类名称", null));
            base.InputParametersMeta.Add(nameof(this.Yddm_First),
                new Property(nameof(this.Yddm_First), "总规用地代码属性", Base.Common.EnumValueMetaType.TYPE_STRING, "", "", "总规用地代码属性", null));
            base.InputParametersMeta.Add(nameof(this.Yddm_Second),
                new Property(nameof(this.Yddm_Second), "控规用地代码属性", Base.Common.EnumValueMetaType.TYPE_STRING, "", "", "控规用地代码属性", null));

            // 初始化输出参数
            string resultGDBDir = System.AppDomain.CurrentDomain.BaseDirectory + "/result/";
            base.OutputParametersMeta.Add(nameof(this.ResultGDBPath), new Property(nameof(this.ResultGDBPath), "输出结果", Base.Common.EnumValueMetaType.TYPE_GDB_PATH, resultGDBDir, resultGDBDir, "分析结果为gdb文件", null));
            base.OutputParametersMeta.Add(nameof(this.ReulstLayerName), new Property(nameof(this.ReulstLayerName), "图层名称", Base.Common.EnumValueMetaType.TYPE_STRING, this.ReulstLayerName, this.ReulstLayerName, "分析结果为gdb文件", null));
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
                string pTempletMDBFile = AppDomain.CurrentDomain.BaseDirectory + GlobalSystemConfig.PATH_TEMPLATE_FGDB;// "template/fgdb.gdb";
                long beginMillisecond = DateUtil.CurrentTimeMillis;
                // 创建一份gdb
                DirectFileUtil.CopyDirectInfo(pTempletMDBFile, resultGDBPath);
                IWorkspace resultWorkspace = WorkspaceUtil.OpenFileGdbWorkspace(resultGDBPath);
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
                    return new Result(EnumSystemStatusCode.DME_ERROR, "图层联合分析失败", EnumSystemStatusCode.DME_ERROR, false);
                }
                // 对输出图层进行规则计算
                // TODO
                return new Result(EnumSystemStatusCode.DME_SUCCESS, "差异分析完成", EnumSystemStatusCode.DME_SUCCESS, true);
            } catch (Exception ex)
            {
                LOG.Error("差异分析失败，详情：" + ex.Message);
                return new Result(EnumSystemStatusCode.DME_ERROR, "差异分析失败，详情：" + ex.Message, EnumSystemStatusCode.DME_ERROR, false);
            }
        }

        public override void Init(IDictionary<string, Property> parameters)
        {
            if (!parameters.ContainsKey(nameof(this.FeatureClass_Source_First)))
            {
                throw new Exception("没有找到输入参数【总规用地图层】");
            }
            if (!parameters.ContainsKey(nameof(this.Yddm_First)))
            {
                throw new Exception("没有找到输入参数【总规用地代码属性】");
            }
            if (!parameters.ContainsKey(nameof(this.FeatureClass_Source_Second)))
            {
                throw new Exception("没有找到输入参数【控规用地图层】");
            }
            if (!parameters.ContainsKey(nameof(this.Yddm_Second)))
            {
                throw new Exception("没有找到输入参数【控规用地代码属性】");
            }
            if (!parameters.ContainsKey(nameof(this.ResultGDBPath)))
            {
                LOG.Info("没有指定新的输出路径，现在使用默认路径");
                this.ResultGDBPath = base.OutputParametersMeta[nameof(this.ResultGDBPath)].DefaultValue.ToString();
            }
            else
            {
                LOG.Info("使用新的输出路径");
                this.ResultGDBPath = parameters[nameof(this.ResultGDBPath)].ToString();
            }

            this.FeatureClass_Source_First = parameters[nameof(this.FeatureClass_Source_First)].ToString();
            WorkspaceUtil.OpenFeatureClass(this.FeatureClass_Source_First, SEPARATOR_FEATURE_PATH, out this.m_localWorkspace_first, out this.m_featureClass_first);
            if (this.m_featureClass_first.FeatureDataset != null)
            {
                // 注意有要素集情况下的路径格式
                this.sourceLayerFirstFullPath = this.FeatureClass_Source_First.Split(SEPARATOR_FEATURE_PATH)[0] + "/" + this.m_featureClass_first.FeatureDataset.Name + "/" + this.FeatureClass_Source_First.Split(SEPARATOR_FEATURE_PATH)[1];
            }
            else
            {
                this.sourceLayerFirstFullPath = this.FeatureClass_Source_First.Split(SEPARATOR_FEATURE_PATH)[0] + "/" + this.FeatureClass_Source_First.Split(SEPARATOR_FEATURE_PATH)[1];
            }
            this.FeatureClass_Source_Second = parameters[nameof(this.FeatureClass_Source_Second)].ToString();
            WorkspaceUtil.OpenFeatureClass(this.FeatureClass_Source_Second, SEPARATOR_FEATURE_PATH, out this.m_localWorkspace_second, out this.m_featureClass_second);
            if (this.m_featureClass_second.FeatureDataset != null)
            {
                this.sourceLayerSecondFullPath = this.FeatureClass_Source_Second.Split(SEPARATOR_FEATURE_PATH)[0] + "/" + this.m_featureClass_second.FeatureDataset.Name + "/" + this.FeatureClass_Source_Second.Split(SEPARATOR_FEATURE_PATH)[1];
            }
            else
            {
                this.sourceLayerSecondFullPath = this.FeatureClass_Source_Second.Split(SEPARATOR_FEATURE_PATH)[0] + "/" + this.FeatureClass_Source_Second.Split(SEPARATOR_FEATURE_PATH)[1];
            }

            this.Yddm_First = parameters[nameof(this.Yddm_First)].ToString();
            this.Yddm_Second = parameters[nameof(this.Yddm_Second)].ToString();
        }
        public override object MetadataJSON
        {
            get
            {
                IDictionary<string, object> dictionary = new Dictionary<string, object>
                {
                    [nameof(this.SysCode)] = this.SysCode,
                    [nameof(this.Name)] = this.Name,
                    [nameof(this.Alias)] = this.Alias,
                    [nameof(this.Version)] = this.Version,
                    [nameof(this.Remark)] = this.Remark,
                    [nameof(this.InputParametersMeta)] = this.InputParametersMeta,
                    [nameof(this.OutputParametersMeta)] = this.OutputParametersMeta,
                    [nameof(this.FeatureParametersMeta)] = this.FeatureParametersMeta,
                    [nameof(AlgorithmType)] = this.AlgorithmType
                };
                return dictionary;// JsonConvert.SerializeObject(dictionary);
            }
        }

        public override IAlgorithmDevType AlgorithmType => new AlgorithmDevTypeDLL();
    }
}
