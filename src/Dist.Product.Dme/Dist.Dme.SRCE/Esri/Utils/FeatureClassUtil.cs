using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;

namespace Dist.Dme.SRCE.Esri.Utils
{
    /// <summary>
    /// 要素类工具类
    /// </summary>
    public class FeatureClassUtil
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        /// <summary>  
        /// 根据传入的源要素类OldFeatureClass,新空间范围,要素存储工作空间,新要素类名  
        /// 产生具有相同字段结构和不同空间范围的要素类  
        /// </summary>  
        /// <param name="sourceFeatureClass">源要素类</param>  
        /// <param name="saveFeatureWorkspace">存储工作空间</param>  
        /// <param name="featureClassName">新要素类名</param>  
        /// <param name="pDomainEnv">新空间范围,可为null</param>  
        /// <returns></returns>  
        public IFeatureClass CloneFeatureClassInWorkspace(IFeatureClass sourceFeatureClass, IFeatureWorkspace saveFeatureWorkspace, string featureClassName, IEnvelope pDomainEnv)
        {
             IFields pFields = CloneFeatureClassFields(sourceFeatureClass, pDomainEnv);
            return saveFeatureWorkspace.CreateFeatureClass(featureClassName, pFields, null, null, esriFeatureType.esriFTSimple, sourceFeatureClass.ShapeFieldName, "");
        }
       
        /// <summary>  
        /// 将inFeatureClass要素类中所有符合pQueryFilter的要素复制到saveFeatureClass中,仅复制不做任何修改  
        /// </summary>  
        /// <param name="sourceFeatureClass">源要素类</param>  
        /// <param name="saveFeatureClass">存储要素类</param>  
        /// <param name="pQueryFilter">过滤参数</param>  
        /// <returns></returns>  
        public bool CloneFeatureClass(IFeatureClass sourceFeatureClass, IFeatureClass saveFeatureClass, IQueryFilter pQueryFilter)
        {
            //生成两个要素类字段的对应表  
            Dictionary<int, int> pFieldsDict = new Dictionary<int, int>();
            this.GetCommonFeatureClassFieldsDictionary(sourceFeatureClass, saveFeatureClass, ref pFieldsDict);
            IFeatureCursor pinFeatCursor = sourceFeatureClass.Search(pQueryFilter, false);
            long nCount = sourceFeatureClass.FeatureCount(pQueryFilter);
            IFeature pinFeat = pinFeatCursor.NextFeature();
            IFeatureCursor saveFeatureCursor = saveFeatureClass.Insert(true);
            //使用IFeatureBuffer在内存中产生缓存避免多次打开,关闭数据库  
            IFeatureBuffer psaveFeatBuf = null;
            IFeature psaveFeat = null;
            long n = 0;
            while (pinFeat != null)
            {
                try
                {
                    psaveFeatBuf = saveFeatureClass.CreateFeatureBuffer();
                    psaveFeat = psaveFeatBuf as IFeature;
                    if (sourceFeatureClass.FeatureType == esriFeatureType.esriFTAnnotation)
                    {
                        IAnnotationFeature pAF = (IAnnotationFeature)pinFeat;
                        IAnnotationFeature pNAF = (IAnnotationFeature)psaveFeat;
                        if (pAF.Annotation != null)
                        {
                            pNAF.Annotation = pAF.Annotation;
                        }
                    }
                    psaveFeat.Shape = pinFeat.Shape;
                    foreach (KeyValuePair<int, int> keyvalue in pFieldsDict)
                    {
                        if (pinFeat.get_Value(keyvalue.Key).ToString() == "")
                        {
                            if (psaveFeat.Fields.get_Field(keyvalue.Value).Type == esriFieldType.esriFieldTypeString)
                            {
                                psaveFeat.set_Value(keyvalue.Value, "");
                            }
                            else
                            {
                                psaveFeat.set_Value(keyvalue.Value, 0);
                            }
                        }
                        else
                        {
                            psaveFeat.set_Value(keyvalue.Value, pinFeat.get_Value(keyvalue.Key));
                        }
                    }
                    saveFeatureCursor.InsertFeature(psaveFeatBuf);
                }
                catch (Exception ex)
                {
                    LOG.Error(ex);
                }
                finally
                {
                    psaveFeat = null;
                    n++;
                    if (n % 2000 == 0)
                    {
                        saveFeatureCursor.Flush();
                    }
                    pinFeat = pinFeatCursor.NextFeature();
                }
            }
            saveFeatureCursor.Flush();
            return true;
        }
        /// <summary>
        /// 复制要素类字段
        /// </summary>
        /// <param name="sourceFeatureClass">源要素类</param>
        /// <param name="pDomainEnv">新空间范围,可为null</param>
        /// <returns></returns>
        private IFields CloneFeatureClassFields(IFeatureClass sourceFeatureClass, IEnvelope pDomainEnv)
        {
            IFields pFields = new FieldsClass(); 
             IFieldsEdit pFieldsEdit = (IFieldsEdit)pFields;
            //根据传入的要素类,将除了shape字段之外的字段复制  
            long nOldFieldsCount = sourceFeatureClass.Fields.FieldCount;
            long nOldGeoIndex = sourceFeatureClass.Fields.FindField(sourceFeatureClass.ShapeFieldName);
            for (int i = 0; i < nOldFieldsCount; i++)
            {
                if (i != nOldGeoIndex)
                {
                    pFieldsEdit.AddField(sourceFeatureClass.Fields.get_Field(i));
                }
                else
                {
                    IGeometryDef pGeomDef = new GeometryDefClass();
                    IGeometryDefEdit pGeomDefEdit = (IGeometryDefEdit)pGeomDef;
                    ISpatialReference pSR = null;
                    if (pDomainEnv != null)
                    {
                        pSR = new UnknownCoordinateSystemClass();
                        pSR.SetDomain(pDomainEnv.XMin, pDomainEnv.XMax, pDomainEnv.YMin, pDomainEnv.YMax);
                    }
                    else
                    {
                        IGeoDataset pGeoDataset = sourceFeatureClass as IGeoDataset;
                        pSR = CloneSpatialReference(pGeoDataset.SpatialReference);
                    }
                    //设置新要素类Geometry的参数  
                    pGeomDefEdit.GeometryType_2 = sourceFeatureClass.ShapeType;
                    pGeomDefEdit.GridCount_2 = 1;
                    pGeomDefEdit.set_GridSize(0, 10);
                    pGeomDefEdit.AvgNumPoints_2 = 2;
                    pGeomDefEdit.SpatialReference_2 = pSR;
                    //产生新的shape字段  
                    IField pField = new FieldClass();
                    IFieldEdit pFieldEdit = (IFieldEdit)pField;
                    pFieldEdit.Name_2 = "shape";
                    pFieldEdit.AliasName_2 = "shape";
                    pFieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
                    pFieldEdit.GeometryDef_2 = pGeomDef;
                    pFieldsEdit.AddField(pField);
                }
            }
            return pFields;
        }
        /// <summary>
        /// 复制空间参考系
        /// </summary>
        /// <param name="pSrcSpatialReference"></param>
        /// <returns>返回参考系</returns>
        private ISpatialReference CloneSpatialReference(ISpatialReference pSrcSpatialReference)
        {
            pSrcSpatialReference.GetDomain(out double xmin, out double xmax, out double ymin, out double ymax);
            ISpatialReference pSR = new UnknownCoordinateSystemClass();
            pSR.SetDomain(xmin, xmax, ymin, ymax);
            return pSR;
        }
        /// <summary>
        /// 获取两个要素类公共的字段字典信息
        /// </summary>
        /// <param name="featureClass1"></param>
        /// <param name="featureClass2"></param>
        /// <param name="fieldsDictionary">返回公共的字段字典信息</param>
        private void GetCommonFeatureClassFieldsDictionary(IFeatureClass featureClass1, IFeatureClass featureClass2, ref Dictionary<int, int> fieldsDictionary)
        {
            for (int i = 0; i < featureClass1.Fields.FieldCount; i++)
            {
                string tmpstrold = featureClass1.Fields.get_Field(i).Name.ToUpper();
                switch (tmpstrold)
                {
                    case "OBJECTID":
                    case "SHAPE":
                    case "SHAPE_LENGTH":
                    case "SHAPE_AREA":
                    case "FID":
                        {
                            //以上字段由系统自动生成  
                            break;
                        }
                    default:
                        {
                            for (int j = 0; j < featureClass2.Fields.FieldCount; j++)
                            {
                                string tmpstrnew = featureClass2.Fields.get_Field(j).Name.ToUpper();
                                if (tmpstrold == tmpstrnew)
                                {
                                    fieldsDictionary.Add(i, j);
                                    break;
                                }
                            }
                            break;
                        }
                }
            }
        }
        /// <summary>
        /// 导出要素类到目标工作空间
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="workspace"></param>
        /// <param name="queryGeometry"></param>
        /// <param name="spatialRelEnum"></param>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public static Boolean ExportToWorkspace(IFeatureClass featureClass, IWorkspace workspace, IGeometry queryGeometry = null, esriSpatialRelEnum spatialRelEnum = esriSpatialRelEnum.esriSpatialRelIntersects, string whereClause = "")
        {
            IDataset inDataSet = featureClass as IDataset;
            IFeatureClassName inFCName = inDataSet.FullName as IFeatureClassName;
            IWorkspace inWorkspace = inDataSet.Workspace;
            IDataset outDataSet = workspace as IDataset;
            IWorkspaceName outWorkspaceName = outDataSet.FullName as IWorkspaceName;
            IFeatureClassName outFCName = new FeatureClassNameClass();
            IDatasetName dataSetName = outFCName as IDatasetName;
            dataSetName.WorkspaceName = outWorkspaceName;
            dataSetName.Name = inDataSet.Name;

            IFieldChecker fieldChecker = new FieldCheckerClass
            {
                InputWorkspace = inWorkspace,
                ValidateWorkspace = workspace
            };
            IFields fields = featureClass.Fields;
            fieldChecker.Validate(fields, out IEnumFieldError enumFieldError, out IFields outFields);
            IFeatureDataConverter featureDataConverter = null;

            IField geometryField;
            try
            {
                IGeometryDef geometryDef = null;
                ISpatialFilter pSF = new SpatialFilterClass();

                for (int i = 0; i < outFields.FieldCount; i++)
                {
                    if (outFields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        geometryField = outFields.get_Field(i);
                        geometryDef = geometryField.GeometryDef;
                        IGeometryDefEdit targetFCGeoDefEdit = (IGeometryDefEdit)geometryDef;
                        targetFCGeoDefEdit.GridCount_2 = 1;
                        targetFCGeoDefEdit.set_GridSize(0, 0);
                        targetFCGeoDefEdit.SpatialReference_2 = geometryField.GeometryDef.SpatialReference;

                        pSF.Geometry = queryGeometry;
                        pSF.GeometryField = featureClass.ShapeFieldName;
                        pSF.SpatialRel = spatialRelEnum;
                        pSF.WhereClause = whereClause;
                        break;
                    }
                }

                featureDataConverter = new FeatureDataConverterClass();
                featureDataConverter.ConvertFeatureClass(inFCName, pSF, null, outFCName, geometryDef, outFields, " ", 1000, 0);
              
                return true;
            }
            catch (Exception ex)
            {
                LOG.Error("图层数据导出出错！" + ex.Message);
                throw ex;
            }
        }
        /// <summary>
        /// 导出DataTable到
        /// </summary>
        /// <param name="m_DataTable"></param>
        /// <param name="outWorkspace"></param>
        public static void ExportToWorkspace(DataTable m_DataTable, IWorkspace outWorkspace)
        {
            IFeatureLayer pFeatureLayer = null;
            IFeatureClass pFeatureClass = null;
            string strLayerName = "";
            IFeature pFeature = null;
            IFeatureBuffer pFeaturebuffer = null;
            IFeatureCursor pFeatureCursor = null;

            if (m_DataTable == null) return;
            if (m_DataTable.Rows.Count == 0) return;

            IWorkspaceEdit pWorkspaceEdit = (IWorkspaceEdit)outWorkspace;

            //ProcessBarshowClass m_ProcessBarshowClass = null;
            try
            {
                pWorkspaceEdit.StartEditing(false);

                for (int i = 0; i < m_DataTable.Rows.Count; i++)
                {
                    DataRow dr = m_DataTable.Rows[i];
                    pFeature = dr["TAG"] as IFeature;

                    string TempstrName = dr["图层名"].ToString();
                    if (TempstrName != strLayerName)
                    {
                        string strTitle = "正在导出要素类[" + TempstrName + "]...";
                        strLayerName = TempstrName;
                        if (i != 0)
                        {
                            pFeatureCursor.Flush();
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureCursor);
                            pWorkspaceEdit.StopEditOperation();
                            if (pFeatureClass != null)
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureClass);
                        }
                        //

                        pWorkspaceEdit.StartEditOperation();

                        pFeatureLayer = dr["ColFeaturLayer"] as IFeatureLayer;
                        pFeatureClass = CreateFeatureClass((IFeatureWorkspace)outWorkspace, pFeatureLayer.FeatureClass.FeatureType,
                            pFeatureLayer.FeatureClass, strLayerName, out string submsg);
                        pFeatureCursor = pFeatureClass.Insert(true);
                    }

                    pFeaturebuffer = pFeatureClass.CreateFeatureBuffer();
                    CopyFeature(pFeature, pFeaturebuffer, out string msg);
                    pFeatureCursor.InsertFeature(pFeaturebuffer);
                    pFeatureCursor.Flush();
                }

                pWorkspaceEdit.StopEditing(true);

                //if (m_ProcessBarshowClass != null) m_ProcessBarshowClass.EndOperation();
                if (pFeatureCursor != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureCursor);
                if (pWorkspaceEdit != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(pWorkspaceEdit);
                if (outWorkspace != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(outWorkspace);
            }
            catch (Exception ex)
            {
                if (pFeatureCursor != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureCursor);
                if (pWorkspaceEdit != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(pWorkspaceEdit);
                if (outWorkspace != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(outWorkspace);
                LOG.Error("[" + strLayerName + "]图层数据导出出错！", ex);
            }
        }

        /// <summary>
        /// 直接复制要素
        /// </summary>
        /// <param name="pSourceFeature">源要素</param>
        /// <param name="pTargetFeatureBuffer">目标要素</param>
        /// <param name="msg">输出消息</param>
        /// <returns>true or false</returns>
        private static bool CopyFeature(IFeature pSourceFeature, IFeatureBuffer pTargetFeatureBuffer, out string msg)
        {
            msg = "";
            try
            {
                if (pSourceFeature == null || pTargetFeatureBuffer == null)
                {
                    return false;
                }
                if (!CopyAttributes(pSourceFeature, pTargetFeatureBuffer, out string submsg))
                {
                    return false;
                }
                pTargetFeatureBuffer.Shape = pSourceFeature.ShapeCopy;
                return true;
            }
            catch (Exception ex)
            {
                if (ex.Message.ToUpper().Contains(("Z VALUE")))
                {
                    try
                    {   //zs-2009-4-21 去除或增加Geometry的Z值,否则会报错,因为目标坐标系可能有Z轴坐标
                        IGeometry pSourceGeometry = pSourceFeature.ShapeCopy;
                        IZAware pZAware = pSourceGeometry as IZAware;
                        if (pZAware.ZAware)
                        {
                            pZAware.ZAware = false;
                        }
                        else
                        {
                            pZAware.ZAware = true;
                            (pSourceGeometry as IZ).SetConstantZ(0);
                        }
                        pTargetFeatureBuffer.Shape = pSourceGeometry;
                        return true;
                    }
                    catch (Exception ex2)
                    {
                        msg = ex2.Message;
                        return false;
                    }
                }
                else
                {
                    msg = ex.Message;
                    return false;
                }
            }
        }
        /// <summary>
        /// 复制属性
        /// </summary>
        /// <param name="pSourceFeature">源要素</param>
        /// <param name="pDestinationFeature">目标要素</param>
        /// <param name="msg">输出消息</param>
        /// <returns>true or false</returns>
        private static bool CopyAttributes(IRow pSourceFeature, IRowBuffer pDestinationFeature, out string msg)
        {
            msg = "";
            bool bRet = false;
            IField pField = default(IField);
            int i = 0;
            IFields pFields = default(IFields);
            int FieldCount = 0;

            try
            {
                pFields = pDestinationFeature.Fields;
                for (FieldCount = 0; FieldCount <= pFields.FieldCount - 1; FieldCount++)
                {
                    pField = pFields.get_Field(FieldCount);

                    if (pField.Type != esriFieldType.esriFieldTypeOID && pField.Type != esriFieldType.esriFieldTypeGeometry)
                    {
                        i = pSourceFeature.Fields.FindField(pField.Name);
                        if (i > -1)
                        {
                            object objValue = pSourceFeature.get_Value(i);
                            if (objValue.ToString().Trim() != "")
                            {
                                pDestinationFeature.set_Value(FieldCount, pSourceFeature.get_Value(i));
                            }
                        }
                    }
                }
                bRet = true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                bRet = false;
            }
            return bRet;
        }
        /// <summary>
        /// 创建要素集
        /// </summary>
        /// <param name="pTargetWorksps">目标工作空间</param>
        /// <param name="eFeatureType">要事集类型</param>
        /// <param name="pRefFeatureClass">参考要素集</param>
        /// <param name="targetName">创建图层的名称</param>
        /// <param name="msg">输出消息</param>
        /// <returns></returns>
        public static IFeatureClass CreateFeatureClass(IFeatureWorkspace pTargetWorksps, esriFeatureType eFeatureType, IFeatureClass pRefFeatureClass, string targetName, out string msg)
        {
            msg = "";
            if (pTargetWorksps == null || pRefFeatureClass == null)
            {
                return null;
            }
            IDataset pDataset = (IDataset)pRefFeatureClass;

            if (targetName.Split('.').Length > 1)
            {
                targetName = targetName.Split('.').GetValue(1).ToString();
            }
            IFeatureClass pFeatCls = null;
            try
            {
                pFeatCls = pTargetWorksps.OpenFeatureClass(targetName); //存在则直接打开
            }
            catch
            {
                try
                {
                    //这里需要做Clone， by zhouqch
                    IClone pClone = pRefFeatureClass.Fields as IClone;
                    IFields pFields = (IFields)pClone.Clone();
                    //注记单独处理 add by zhouqch
                    if (eFeatureType == esriFeatureType.esriFTAnnotation)
                    {
                        pFeatCls = CreateAnnotationClass(targetName, pTargetWorksps, null, pRefFeatureClass, out string submsg);
                    }
                    else
                    {
                        pFeatCls = pTargetWorksps.CreateFeatureClass(targetName, pFields, null, null, eFeatureType, pRefFeatureClass.ShapeFieldName, null);
                    }
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    return null;
                }
            }
            return pFeatCls;
        }
        /// <summary>
        /// 根据参考注记层新建注记层
        /// </summary>
        /// <param name="targetName">创建注记图层的名称</param> 
        /// <param name="pTargetWrksps">目标工作空间</param>
        /// <param name="pTargetDs">所属目标数据集</param> 
        /// <param name="pRefAnnoFtCls">参考图层</param>
        /// <returns></returns>
        private static IFeatureClass CreateAnnotationClass(string targetName, IFeatureWorkspace pTargetWrksps, IFeatureDataset pTargetDs, IFeatureClass pRefAnnoFtCls, out string msg)
        {
            msg = "";
            try
            {
                IFeatureWorkspaceAnno pAnnoWrksps = pTargetWrksps as IFeatureWorkspaceAnno;
                if (pAnnoWrksps == null || pRefAnnoFtCls == null) return null;
                if (pRefAnnoFtCls.Extension is IAnnotationClassExtension pAnnoExten)
                {
                    IClone pClone = pAnnoExten.AnnoProperties as IClone;
                    IAnnotateLayerPropertiesCollection pAnnoProCol = (IAnnotateLayerPropertiesCollection)pClone.Clone();

                    esriUnits eUnits = pAnnoExten.ReferenceScaleUnits;
                    double dbScale = pAnnoExten.ReferenceScale;
                    IGraphicsLayerScale pGraScale = new GraphicsLayerScaleClass
                    {
                        ReferenceScale = dbScale,
                        Units = eUnits
                    };
                    pClone = pAnnoExten.SymbolCollection as IClone;
                    ISymbolCollection pSymbolCol = (ISymbolCollection)pClone.Clone();

                    pClone = pRefAnnoFtCls.Fields as IClone;
                    IFields pFields = (IFields)pClone.Clone();

                    IObjectClassDescription pOCDesc = new AnnotationFeatureClassDescriptionClass();

                    IDataset pDs = (IDataset)pRefAnnoFtCls;
                    return pAnnoWrksps.CreateAnnotationClass(targetName, pFields, pOCDesc.InstanceCLSID, pOCDesc.ClassExtensionCLSID, pRefAnnoFtCls.ShapeFieldName,
                                                       null, pTargetDs, null, pAnnoProCol, pGraScale, pSymbolCol, false);
                }
                return null;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }
        /// <summary>
        /// 导出要素类到shapefile
        /// </summary>
        /// <param name="sourceFeatureClass"></param>
        /// <param name="outputPath">shapefile路径，包括shapefile文件名</param>
        public static void ExportToShapefile(IFeatureClass sourceFeatureClass, string outputPath)
        {
            IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
            string parentPath = outputPath.Substring(0, outputPath.LastIndexOf('\\'));
            string fileName = outputPath.Substring(outputPath.LastIndexOf('\\') + 1, outputPath.Length - outputPath.LastIndexOf('\\') - 1);
            IWorkspaceName pWorkspaceName = pWorkspaceFactory.Create(parentPath, fileName, null, 0);
            // Cast for IName        
            IName name = (IName)pWorkspaceName;
            //Open a reference to the access workspace through the name object        
            IWorkspace pOutWorkspace = (IWorkspace)name.Open();

            IDataset pInDataset = sourceFeatureClass as IDataset;
            IFeatureClassName pInFCName = pInDataset.FullName as IFeatureClassName;
            IWorkspace pInWorkspace = pInDataset.Workspace;
            IDataset pOutDataset = pOutWorkspace as IDataset;
            IWorkspaceName pOutWorkspaceName = pOutDataset.FullName as IWorkspaceName;
            IFeatureClassName pOutFCName = new FeatureClassNameClass();
            IDatasetName pDatasetName = pOutFCName as IDatasetName;
            pDatasetName.WorkspaceName = pOutWorkspaceName;
            pDatasetName.Name = sourceFeatureClass.AliasName;
            IFieldChecker pFieldChecker = new FieldCheckerClass
            {
                InputWorkspace = pInWorkspace,
                ValidateWorkspace = pOutWorkspace
            };
            IFields pFields = sourceFeatureClass.Fields;
            pFieldChecker.Validate(pFields, out IEnumFieldError pEnumFieldError, out IFields pOutFields);
            IFeatureDataConverter pFeatureDataConverter = new FeatureDataConverterClass();
            pFeatureDataConverter.ConvertFeatureClass(pInFCName, null, null, pOutFCName, null, pOutFields, "", 100, 0);
        }
    }
}
