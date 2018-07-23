using Dist.Dme.Algorithms.Overlay.DTO;
using Dist.Dme.Base.Common;
using Dist.Dme.Base.Conf;
using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Framework.AlgorithmTypes;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.Model.DTO;
using Dist.Dme.SRCE.Core;
using Dist.Dme.SRCE.Esri;
using Dist.Dme.SRCE.Esri.AnalysisTools.Overlay;
using Dist.Dme.SRCE.Esri.DTO;
using Dist.Dme.SRCE.Esri.Utils;
using ESRI.ArcGIS;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Dist.Dme.Algorithms.Overlay
{
    /// <summary>
    /// 分析类型
    /// </summary>
    public enum AnalysisType
    {
        [Description("压盖分析")]
        COVER = 0,
        [Description("超出分析")]
        OVERTOP = 1
    }
    /// <summary>
    /// 叠加分析
    /// </summary>
    public class OverlayMain : BaseAlgorithm, IAlgorithm
    {
        private static ILog LOG = LogManager.GetLogger(typeof(OverlayMain));

        public override string SysCode => "6e18c8abbf5448c7a4c15cfd4c6eed8c";

        public override string Name => "OverlayAnalysis";

        public override string Alias => "两个要素类进行叠加分析";

        public override string Version => "1.0.0";

        public override string Remark => "两个要素类进行叠加分析";

        public override IAlgorithmDevType AlgorithmType => new AlgorithmDevTypeDLL();
        /// <summary>
        /// 源要素类
        /// </summary>
        private IFeatureClass SourceFeatureClass;
        private InputFeatureClassDTO sourceFeatureClassDTO;
        /// <summary>
        /// 目标要素类
        /// </summary>
        private IFeatureClass TargetFeatureClass;
        private InputFeatureClassDTO targetFeatureClassDTO;
        /// <summary>
        /// 分析类型
        /// </summary>
        private AnalysisType AnalysisType;
        /// <summary>
        /// 是否清理临时目录或者文件
        /// </summary>
        private Boolean IsClearTemp = false;

        public OverlayMain()
        {
            if (!ESRI.ArcGIS.RuntimeManager.Bind(ProductCode.EngineOrDesktop))
            {
                LOG.Error("arcgis license init error");
                throw new Exception("arcgis license init error");
            }
            LicenseUtil.CheckOutLicenseAdvanced();

            // 初始化输入参数
            base.InputParametersMeta.Add(nameof(this.SourceFeatureClass),
                new Property(nameof(this.SourceFeatureClass), "源要素类，叠加的图层", EnumValueMetaType.TYPE_FEATURECLASS, new InputFeatureClassDTO(), new InputFeatureClassDTO(), "源要素类，叠加的图层", null));
            base.InputParametersMeta.Add(nameof(this.TargetFeatureClass),
                         new Property(nameof(this.TargetFeatureClass), "目标要素类，被叠加的图层", EnumValueMetaType.TYPE_FEATURECLASS, new InputFeatureClassDTO(), new InputFeatureClassDTO(), "目标要素类，被叠加的图层", null));
            base.InputParametersMeta.Add(nameof(this.AnalysisType),
                        new Property(nameof(this.AnalysisType), "分析类型", EnumValueMetaType.TYPE_INTEGER, null, (int)AnalysisType.COVER, "分析类型选择",
                        new object[] {
                            new Property(nameof(AnalysisType.COVER), EnumUtil.GetEnumDescription(AnalysisType.COVER), EnumValueMetaType.TYPE_INTEGER, (int)AnalysisType.COVER, null, EnumUtil.GetEnumDescription(AnalysisType.COVER)),
                            new Property(nameof(AnalysisType.OVERTOP), EnumUtil.GetEnumDescription(AnalysisType.OVERTOP), EnumValueMetaType.TYPE_INTEGER, (int)AnalysisType.OVERTOP, null, EnumUtil.GetEnumDescription(AnalysisType.OVERTOP))}));
            base.InputParametersMeta.Add(nameof(IsClearTemp), new Property(nameof(IsClearTemp), nameof(IsClearTemp), EnumValueMetaType.TYPE_BOOLEAN, false, false, "是否清理临时目录或文件", new object[] { true, false}));
            // 指定输出参数类型
            base.OutputParametersMeta.Add(nameof(Result), new Property(nameof(Result), "输出结果", EnumValueMetaType.TYPE_JSON, null, null, "输出结果，json格式", null, 1, 1));
        }
        public override Result Execute()
        {
            if (!base.InitComplete)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL_INIT, "初始化工作未完成");
            }
            DMEWorkspaceBridge<IWorkspace, IFeatureClass> dmeWorkspaceBridge = new DMEWorkspaceBridge<IWorkspace, IFeatureClass>();
            dmeWorkspaceBridge.SetWorkspace(new EsriWorkspace());
            // 目前先以ORACLE和MDB为数据源类型
            // 获取source feature class
            this.SourceFeatureClass = dmeWorkspaceBridge.GetFeatureClass(this.sourceFeatureClassDTO);
            // 获取target feature class
            this.TargetFeatureClass = dmeWorkspaceBridge.GetFeatureClass(this.targetFeatureClassDTO);

            if (AnalysisType.COVER == this.AnalysisType)
            {
                OverlayCommonTool.GetTopounionGeometryByQuery(this.SourceFeatureClass, null, null, esriSpatialRelEnum.esriSpatialRelUndefined,
               out IGeometry sourceGeom, out IList<string> oidList);
                //找出touch的要素oid，排除掉touch的图形
                OverlayCommonTool.GetTopounionGeometryByQuery(this.TargetFeatureClass, null, sourceGeom, esriSpatialRelEnum.esriSpatialRelTouches, out IGeometry touchGeom, out IList<string> touchOIDList);
                //通过intersect查找压盖部分的要素
                string queryClause = null;
                if (touchOIDList != null && touchOIDList.Count > 0)
                {
                    queryClause = $"OBJECTID  NOT in ({string.Join(",", ((List<string>)touchOIDList).ToArray())})";
                }
                // 求取空间相交的部分
                OverlayCommonTool.GetIntersectFeaturesByQuery(this.TargetFeatureClass, queryClause, sourceGeom,
                    out IList<IntersectFeatureDTO> intersectFeatureDTOs, out double sumIntersectArea);

                if (intersectFeatureDTOs?.Count > 0)
                {
                    OverlayRespDTO overlayRespDTO = new OverlayRespDTO
                    {
                        SumArea = sumIntersectArea
                    };
                    foreach (var item in intersectFeatureDTOs)
                    {
                        FeatureRespDTO intersectFeatureRespDTO = new FeatureRespDTO
                        {
                            OID = item.OID,
                            Area = item.Area,
                            Coordinates = item.Coordinates,
                            GeoType = item.GeoType
                        };
                        overlayRespDTO.Features.Add(intersectFeatureRespDTO);
                    }
                    Property resultProp = base.OutputParametersMeta[nameof(Result)];
                    resultProp.Value = overlayRespDTO;
                    return new Result(EnumSystemStatusCode.DME_SUCCESS, "运行完成", EnumSystemStatusCode.DME_SUCCESS, null);
                }
            }
            else if (AnalysisType.OVERTOP == this.AnalysisType)
            {
                // 先拷贝一份mdb模板
                string sTemplate = System.AppDomain.CurrentDomain.BaseDirectory + GlobalSystemConfig.PATH_TEMPLATE_PGDB;// @"\template\pgdb.mdb";
                string sPath = System.AppDomain.CurrentDomain.BaseDirectory + GlobalSystemConfig.DIR_TEMP + "/" + GuidUtil.NewGuid() + ".mdb";
                File.Copy(sTemplate, sPath);
                IWorkspace mdbWorkspace = WorkspaceUtil.OpenMdbWorspace(sPath);
                if (FeatureClassUtil.ExportToWorkspace(this.SourceFeatureClass, mdbWorkspace))
                {
                    // 获取导出的临时要素类信息
                    IFeatureClass tempExpFeatureClass = WorkspaceUtil.GetFeatureClass(mdbWorkspace, this.sourceFeatureClassDTO.Name);
                    // 进行擦除操作
                    OverlayCommonTool.Erase(tempExpFeatureClass, this.TargetFeatureClass);
                    // 计算完保存结果，如何保存？
                    if (tempExpFeatureClass != null)
                    {
                        OverlayRespDTO overlayRespDTO = new OverlayRespDTO();
                        // 总面积
                        double sumArea = 0;
                        IFeatureCursor pFeatureCursor = tempExpFeatureClass.Search(null, false);
                        IFeature pFeature = null;
                        //获得“Area”字段
                        //IFields fields = pFeatureCursor.Fields;
                        //int areaIndex = fields.FindField("Area");
                        while ((pFeature = pFeatureCursor.NextFeature()) != null)
                        {
                            double area = ((IArea)pFeature.Shape).Area;// (double)pFeature.get_Value(areaIndex);
                            sumArea += area;
                            FeatureRespDTO featureRespDTO = new FeatureRespDTO
                            {
                                OID = pFeature.OID,
                                Area = area,
                                Coordinates = GeometryUtil.ConvertGeometryToJson(pFeature.Shape, out string msg)
                            };
                            overlayRespDTO.Features.Add(featureRespDTO);
                            pFeature = pFeatureCursor.NextFeature();
                        }
                        overlayRespDTO.SumArea = sumArea;
                        Property resultProp = base.OutputParametersMeta[nameof(Result)];
                        resultProp.Value = overlayRespDTO;
                        return new Result(EnumSystemStatusCode.DME_SUCCESS, "运行完成", EnumSystemStatusCode.DME_SUCCESS, null);
                    }
                }
                // 删除临时文件
                if (IsClearTemp && File.Exists(sPath))
                {
                    File.Delete(sPath);
                }
            }
            else
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, "分析类型不匹配");
            }
           
            return new Result(EnumSystemStatusCode.DME_SUCCESS, "运行完成，但没有运算结果", EnumSystemStatusCode.DME_SUCCESS, null);
        }

        public override void Init(IDictionary<string, Property> parameters)
        {
            if (0 == base.InParams?.Count)
            {
                LOG.Error("未设置输入参数");
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, "未设置输入参数");
            }
            if (0 == base.OutParams?.Count)
            {
                LOG.Error("未设置输出参数");
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, "未设置输出参数");
            }
            if (!parameters.ContainsKey(nameof(this.SourceFeatureClass)))
            {
                LOG.Error($"缺失参数[{nameof(this.SourceFeatureClass)}]");
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"缺失参数[{nameof(this.SourceFeatureClass)}]");
            }
            if (!parameters.ContainsKey(nameof(this.TargetFeatureClass)))
            {
                LOG.Error($"缺失参数[{nameof(this.SourceFeatureClass)}]");
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"缺失参数[{nameof(this.TargetFeatureClass)}]");
            }
            if (!parameters.ContainsKey(nameof(this.AnalysisType)))
            {
                LOG.Error($"缺失参数[{nameof(this.AnalysisType)}]");
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"缺失参数[{nameof(this.AnalysisType)}]");
            }
            if (parameters.ContainsKey(nameof(IsClearTemp)))
            {
                this.IsClearTemp = Boolean.Parse(parameters[nameof(IsClearTemp)].ToString());
            }
            // 从步骤层传过来的数据已经被格式化成对象了（JSON->实体对象）
            this.sourceFeatureClassDTO = (InputFeatureClassDTO)parameters[nameof(this.SourceFeatureClass)].Value;//JsonConvert.DeserializeObject<InputFeatureClassDTO>(parameters[nameof(this.SourceFeatureClass)].ToString());
            this.targetFeatureClassDTO = (InputFeatureClassDTO)parameters[nameof(this.TargetFeatureClass)].Value;//JsonConvert.DeserializeObject<InputFeatureClassDTO>(parameters[nameof(this.TargetFeatureClass)].ToString());
            this.AnalysisType = EnumUtil.GetEnumObjByValue<AnalysisType>(int.Parse(parameters[nameof(this.AnalysisType)].Value.ToString()));

            base.InitComplete = true;
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
                return dictionary;
            }
        }
    }
}
