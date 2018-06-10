using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Dist.Dme.SRCE.Esri.Utils
{
    public sealed class ExportUtil
    {
        private static log4net.ILog LOG = LogManager.GetLogger(typeof(ExportUtil));
        #region 属性字段

        private static string msg = "";
        /// <summary>
        /// 错误信息
        /// </summary>
        public string LastErrorInfo
        {
            get { return msg; }
            set { msg = value; }
        }


        public IFeatureClass InputFeatureClass;

        public IWorkspace OutWorkspace;


        #endregion


        private IGeometry pGeometry = null;
        private string strSQL = "";
        private esriSpatialRelEnum SpatialRelEnum = esriSpatialRelEnum.esriSpatialRelIntersects;

        public ExportUtil()
        {
        }

        public ExportUtil(IGeometry m_pGeometry, string m_strSQL, esriSpatialRelEnum m_SpatialRelEnum)
        {
            pGeometry = m_pGeometry;
            strSQL = m_strSQL;
            SpatialRelEnum = m_SpatialRelEnum;
        }

        public void Excute()
        {
            IDataset inDataSet = InputFeatureClass as IDataset;
            IFeatureClassName inFCName = inDataSet.FullName as IFeatureClassName;
            IWorkspace inWorkspace = inDataSet.Workspace;
            IDataset outDataSet = OutWorkspace as IDataset;
            IWorkspaceName outWorkspaceName = outDataSet.FullName as IWorkspaceName;
            IFeatureClassName outFCName = new FeatureClassNameClass();
            IDatasetName dataSetName = outFCName as IDatasetName;
            dataSetName.WorkspaceName = outWorkspaceName;
            dataSetName.Name = inDataSet.Name;

            IFieldChecker fieldChecker = new FieldCheckerClass
            {
                InputWorkspace = inWorkspace,
                ValidateWorkspace = OutWorkspace
            };
            IFields fields = InputFeatureClass.Fields;
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

                        pSF.Geometry = pGeometry;
                        pSF.GeometryField = InputFeatureClass.ShapeFieldName;
                        pSF.SpatialRel = SpatialRelEnum;
                        pSF.WhereClause = strSQL;
                        break;
                    }
                }

                featureDataConverter = new FeatureDataConverterClass();
                featureDataConverter.ConvertFeatureClass(inFCName, pSF, null, outFCName, geometryDef, outFields, " ", 1000, 0);

                return;
            }
            catch (Exception ex)
            {
                LOG.Error("图层数据导出出错！" + ex.Message);
                throw ex;
            }

        }

        public void ExportoTMDB(DataTable m_DataTable, IWorkspace outWorkspace)
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
                            pFeatureLayer.FeatureClass, strLayerName);
                        pFeatureCursor = pFeatureClass.Insert(true);
                    }

                    pFeaturebuffer = pFeatureClass.CreateFeatureBuffer();
                    CopyFeature(pFeature, pFeaturebuffer);
                    pFeatureCursor.InsertFeature(pFeaturebuffer);
                    pFeatureCursor.Flush();
                }

                pWorkspaceEdit.StopEditing(true);


                //if (m_ProcessBarshowClass != null) m_ProcessBarshowClass.EndOperation();
                if (pFeatureCursor != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureCursor);
                if (pWorkspaceEdit != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(pWorkspaceEdit);
                if (outWorkspace != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(outWorkspace);

            }
            catch
            {
                if (pFeatureCursor != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureCursor);
                if (pWorkspaceEdit != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(pWorkspaceEdit);
                if (outWorkspace != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(outWorkspace);
                LOG.Error("[" + strLayerName + "]图层数据导出出错！");
            }
        }

        #region 导出要素类函数
        /// <summary>
        /// 复制要素
        /// </summary>
        /// <param name="pSourceFeature">源要素</param>
        /// <param name="pTargetFeatureBuffer">目标要素</param>
        /// <returns>true or false</returns>
        /// 直接复制
        private bool CopyFeature(IFeature pSourceFeature, IFeatureBuffer pTargetFeatureBuffer)
        {
            try
            {
                if (pSourceFeature == null || pTargetFeatureBuffer == null)
                {
                    return false;
                }

                if (!CopyAttributes(pSourceFeature, pTargetFeatureBuffer))
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
        /// <returns>true or false</returns>
        private bool CopyAttributes(IRow pSourceFeature, IRowBuffer pDestinationFeature)
        {
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
        /// <param name="pRefFeatCls">参考要素集</param>
        /// <param name="targetName">创建图层的名称</param> 
        /// <returns></returns>
        public IFeatureClass CreateFeatureClass(IFeatureWorkspace pTargetWorksps, esriFeatureType eFeatureType, IFeatureClass pRefFeatCls, string targetName)
        {
            if (pTargetWorksps == null || pRefFeatCls == null)
            {
                return null;
            }
            IDataset pDataset = (IDataset)pRefFeatCls;

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
                    IClone pClone = pRefFeatCls.Fields as IClone;
                    IFields pFields = (IFields)pClone.Clone();

                    //注记单独处理 add by zhouqch
                    if (eFeatureType == esriFeatureType.esriFTAnnotation)
                        pFeatCls = CreateAnnotationClass(targetName, pTargetWorksps, null, pRefFeatCls);
                    else
                        pFeatCls = pTargetWorksps.CreateFeatureClass(targetName, pFields, null, null, eFeatureType, pRefFeatCls.ShapeFieldName, null);


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
        private IFeatureClass CreateAnnotationClass(string targetName, IFeatureWorkspace pTargetWrksps, IFeatureDataset pTargetDs, IFeatureClass pRefAnnoFtCls)
        {
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
                    //string pTargetName = pDs.Name;
                    //if (pTargetDs.Workspace.Type != esriWorkspaceType.esriRemoteDatabaseWorkspace && pDs.Name.Split('.').Length > 1)
                    //{
                    //    pTargetName = pDs.Name.Split('.').GetValue(1).ToString();
                    //}
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
        #endregion
    }
}
