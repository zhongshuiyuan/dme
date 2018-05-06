using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.DataSourcesOleDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using log4net;

namespace Dist.Dme.AECommon.Utils
{
    public sealed class WorkspaceServices
    {
        private static log4net.ILog LOG = LogManager.GetLogger(typeof(WorkspaceServices));
        public static IWorkspace OpenSdeWorkspace(string connstr)
        {
            try
            {
                IWorkspaceFactory2 workspaceFactory = new SdeWorkspaceFactoryClass();
                IWorkspace workspace = workspaceFactory.OpenFromString(connstr, 0);
                return workspace;
            }
            catch (Exception ex)
            {
                LOG.Error("SDE空间数据库打开错误，详情：" + ex.Message);
                // Console.WriteLine("SDE空间数据库打开错误，详情："+ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 获取workapce 连接字符串
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <returns></returns>
        public static string GetConnectStr(IWorkspace pWorkspace)
        {
            if (pWorkspace == null) return "";
            string str = "SERVER={0};INSTANCE={1};VERSION=sde.DEFAULT;USER={2};PASSWORD={3}";
            string server = pWorkspace.ConnectionProperties.GetProperty("SERVER").ToString();
            string instance = pWorkspace.ConnectionProperties.GetProperty("INSTANCE").ToString();
            string user = pWorkspace.ConnectionProperties.GetProperty("USER").ToString();
            string passworkd = pWorkspace.ConnectionProperties.GetProperty("PASSWORD").ToString();
            string result = string.Format(str, server, instance, user, passworkd);
            return result;
        }

        /// <summary>
        /// 创建MDB工作空间
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <returns></returns>
        public static IWorkspace OpenMdbWorspace(string strFilePath)
        {
            if (strFilePath == string.Empty)
            {
                LOG.Error("参数strFilePath不能为空");
                throw new Exception("参数strFilePath不能为空");
            }
            try
            {
                IWorkspaceFactory2 pWrksFactory = new AccessWorkspaceFactoryClass();
                //int index = strFilePath.LastIndexOf('\\');
                //string strDic = strFilePath.Substring(0, index);
                //string strName = strFilePath.Substring(index + 1, strFilePath.Length - index - 1);
                //IWorkspaceName pWrkspsName = pWrksFactory.Create(strDic, strName, null, 0);
               //IWorkspace pWrksps = ((IName)pWrkspsName).Open() as IWorkspace;
               return pWrksFactory.OpenFromFile(strFilePath, 0);
               //return pWrksps;
            }
            catch (Exception ex)
            {
                LOG.Error("mdb工作空间打开失败，详情：" + ex.Message);
                // Console.WriteLine("创建mdb工作空间失败，详情：" + ex.Message);
                return null;
            }
        }

        #region 空间数据库操作服务
        public static IWorkspace OpenSdeWorkspace(string user, string password, string server, string instance, string database, string version)
        {
            try
            {
                IPropertySet propertySet = new PropertySetClass();
                propertySet.SetProperty("SERVER", server);
                propertySet.SetProperty("INSTANCE", instance);
                propertySet.SetProperty("DATABASE", database);
                propertySet.SetProperty("USER", user);
                propertySet.SetProperty("PASSWORD", password);
                propertySet.SetProperty("VERSION", version);
                IWorkspaceFactory2 workspaceFactory = new SdeWorkspaceFactoryClass();
                IWorkspace workspace = workspaceFactory.Open(propertySet, 0);

                return workspace;
            }
            catch (Exception ex)
            {
                LOG.Error("SDE空间数据库打开错误，详情：" + ex.Message);
                // Console.WriteLine("SDE空间数据库打开错误，详情："+ex.Message);
                return null;
            }

        }

        /// <summary>
        /// 连接SDE
        /// </summary>
        /// <param name="sdeFile"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IWorkspace OpenSdeWorkspace(string sdeFile, out string sdeUser, out string message)
        {
            message = string.Empty;
            sdeUser = string.Empty;

            IWorkspace pWorkSpace = null;
            IWorkspaceFactory pSdeFactory = null;
            try
            {
                if (string.IsNullOrEmpty(sdeFile))
                {
                    message = "获取连接文件出错";
                    return null;
                }
                if (!File.Exists(sdeFile))
                {
                    message = "获取连接文件出错";
                    return null;
                }

                pSdeFactory = new SdeWorkspaceFactoryClass();

                IPropertySet pPropertySet = pSdeFactory.ReadConnectionPropertiesFromFile(sdeFile);
                if (pPropertySet == null)
                {
                    message = "读取连接文件出错";
                    return null;
                }

                string strServer = pPropertySet.GetProperty("SERVER").ToString();
                string strPort = pPropertySet.GetProperty("INSTANCE").ToString();
                sdeUser = pPropertySet.GetProperty("USER").ToString();

                //if (NetAssist.CheckSDEConnecTion(strServer, Convert.ToInt32(strPort), out message) == false)
                //{
                //    return null;
                //}
                if (NetAssist.PingIP(strServer) == false)
                {
                    message = "无法连接服务器";
                    return null;
                }

                if (strPort == "5151")
                {
                    if (NetAssist.PingPort(strServer, Convert.ToInt32(strPort)) == false)
                    {
                        message = "无法连接服务器";
                        return null;
                    }
                }

                pWorkSpace = pSdeFactory.OpenFromFile(sdeFile, 0);
            }
            catch (Exception ex)
            {
                LOG.Error("空间数据库打开错误，详情：" + ex.Message);
                // Console.WriteLine("空间数据库打开错误");
                message = ex.Message;
                return null;
            }
            return pWorkSpace;
        }

        public static IWorkspace OpenFileGdbWorkspace(string database)
        {
            try
            {
                ESRI.ArcGIS.esriSystem.IPropertySet propertySet = new ESRI.ArcGIS.esriSystem.PropertySetClass();
                propertySet.SetProperty("DATABASE", database);
                IWorkspaceFactory workspaceFactory = new ESRI.ArcGIS.DataSourcesGDB.FileGDBWorkspaceFactoryClass();
                return workspaceFactory.Open(propertySet, 0);
            }
            catch (Exception ex)
            {
                LOG.Error("空间数据库打开错误，详情：" + ex.Message);
                // Console.WriteLine("空间数据库打开错误");
                return null;
            }
        }

        public static IWorkspace OpenPGDBWorkspace(string database)
        {
            try
            {
                ESRI.ArcGIS.esriSystem.IPropertySet propertySet = new ESRI.ArcGIS.esriSystem.PropertySetClass();
                propertySet.SetProperty("DATABASE", database);
                IWorkspaceFactory workspaceFactory = new ESRI.ArcGIS.DataSourcesGDB.AccessWorkspaceFactoryClass();
                return workspaceFactory.Open(propertySet, 0);
            }
            catch (Exception ex)
            {
                LOG.Error("空间数据库打开错误，详情：" + ex.Message);
                // Console.WriteLine("空间数据库打开错误");
                return null;
            }
        }

        public static IWorkspace OpenShapeFileWorkspace(string dirName)
        {
            IWorkspaceFactory workspaceFactory = null;
            try
            {
                workspaceFactory = null;
                workspaceFactory = new ShapefileWorkspaceFactory();
            }
            catch (Exception ex)
            {
                LOG.Error("shape文件打开错误，详情：" + ex.Message);
                // Console.WriteLine("空间数据库打开错误");
                return null;
            }
            return workspaceFactory.OpenFromFile(dirName, 0);
        }

        public static IWorkspace OpenShpFileWorkspace(string workspacePath)
        {
            IWorkspaceFactory workspaceFactory = new ShapefileWorkspaceFactoryClass(); ;
            IWorkspace pWorkspace = null;
            IPropertySet pSet = new PropertySetClass();
            pSet.SetProperty("DATABASE", workspacePath);
            try
            {
                pWorkspace = workspaceFactory.Open(pSet, 0);
            }
            catch (Exception ex)
            {
                LOG.Error("shape文件打开错误，详情：" + ex.Message);
                // Console.WriteLine("空间数据库打开错误");
                return null;
            }
            return pWorkspace;
        }

        public static IWorkspace OpenCadWorkspace(string cadFilePath)
        {
            IWorkspaceFactory pCADWorkSpaceFc = null;
            IWorkspace pWorkspace = null;
            try
            {
                pCADWorkSpaceFc = new CadWorkspaceFactoryClass();
                pWorkspace = pCADWorkSpaceFc.OpenFromFile(cadFilePath, 0);
            }
            catch (Exception ex)
            {
                LOG.Error("cad文件打开错误，详情：" + ex.Message);
                // Console.WriteLine("空间数据库打开错误");
                return null;
            }
            return pWorkspace;
        }

        public static IWorkspace OpenArcInfoWorkspace(string dirPath)
        {
            IPropertySet propertySet = new PropertySetClass();
            propertySet.SetProperty("DATABASE", dirPath);
            IWorkspaceFactory pFactory = new ArcInfoWorkspaceFactoryClass();
            IWorkspace pWorkspace = null;
            try
            {
                pWorkspace = pFactory.Open(propertySet, 0);
            }
            catch (Exception ex)
            {
                LOG.Error("arcinfo文件打开错误，详情：" + ex.Message);
                // Console.WriteLine("空间数据库打开错误");
                return null;
            }
            pFactory = null;
            return pWorkspace;
        }

        public static IWorkspace OpenRasterWorkspace(string rasterFilePath)
        {
            IWorkspaceFactory pRasterWorkSpaceFc = null;
            IWorkspace pWorkspace = null;
            try
            {
                pRasterWorkSpaceFc = new RasterWorkspaceFactoryClass();
                pWorkspace = pRasterWorkSpaceFc.OpenFromFile(rasterFilePath, 0);
            }
            catch (Exception ex)
            {
                LOG.Error("raster文件打开错误，详情：" + ex.Message);
                // Console.WriteLine("空间数据库打开错误");
                return null;
            }
            return pWorkspace;
        }

        public static ITinWorkspace OpenTinWorkspace(string tinPath)
        {
            IWorkspaceFactory pTinWorkSpaceFc = null;
            ITinWorkspace pWorkspace = null;
            try
            {
                pTinWorkSpaceFc = new TinWorkspaceFactoryClass();
                pWorkspace = (ITinWorkspace)pTinWorkSpaceFc.OpenFromFile(tinPath, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("空间数据库打开错误");
                return null;
            }
            return pWorkspace;
        }
        public static IWorkspace OpenCADWorkspace(string CADFolderPath)
        {
            IWorkspaceFactory pCADWorkSpaceFc = null;
            IWorkspace pWorkspace = null;
            try
            {
                pCADWorkSpaceFc = new CadWorkspaceFactoryClass();
                pWorkspace = pCADWorkSpaceFc.OpenFromFile(CADFolderPath, 0);
            }
            catch (Exception ex)
            {
                ;
            }
            return pWorkspace;
        }

        /// <summary>
        /// for ArcInfo coverages and Info tables.
        /// </summary>
        /// <param name="DirPath"></param>
        /// <returns></returns>
        public static IWorkspace OpenArcinfoWorkspace(string DirPath)
        {
            IWorkspaceFactory pArcinfoWorkSpaceFc = null;
            IWorkspace pWorkspace = null;
            try
            {
                pArcinfoWorkSpaceFc = new ArcInfoWorkspaceFactoryClass();
                pWorkspace = pArcinfoWorkSpaceFc.OpenFromFile(DirPath, 0);
            }
            catch (Exception ex)
            {
                ;
            }
            return pWorkspace;

        }

        public static IWorkspace OpenExcelWorkspace(string ExcelFilePath)
        {
            IWorkspaceFactory pExcelWorkSpaceFc = null;
            IWorkspace pWorkspace = null;
            try
            {
                pExcelWorkSpaceFc = new ExcelWorkspaceFactoryClass();
                pWorkspace = pExcelWorkSpaceFc.OpenFromFile(ExcelFilePath, 0);
            }
            catch (Exception ex)
            {
                ;// MessageBox.Show(ex.Message);
            }
            return pWorkspace;
        }

        public static IWorkspace OpenSDCWorkSpace(string filePath)
        {
            IWorkspaceFactory workspaceFactory = null;
            IWorkspace pWorkspace = null;
            try
            {
                workspaceFactory = null;
                workspaceFactory = new SDCWorkspaceFactoryClass();
                pWorkspace = workspaceFactory.OpenFromFile(filePath, 0);
                return pWorkspace;
            }
            catch (Exception ex)
            {
                ;// MessageBox.Show(ex.Message);
            }
            return pWorkspace;
        }

        public static IWorkspaceName OpenTextFileWorkspaceName(string TextFilePath)
        {

            IWorkspaceName pWorkspaceName = new WorkspaceNameClass();
            pWorkspaceName.WorkspaceFactoryProgID = "esriDataSourcesOleDB.TextFileWorkspaceFactory";
            try
            {
                pWorkspaceName.PathName = TextFilePath;
            }
            catch (Exception ex)
            {
                ;// MessageBox.Show(ex.Message);
            }
            return pWorkspaceName;
        }

        public static IWorkspaceName OpenTinWorkspaceName(string TinFilePath)
        {

            IWorkspaceName pWorkspaceName = new WorkspaceNameClass();
            pWorkspaceName.WorkspaceFactoryProgID = "esriDataSourcesFile.TinWorkspaceFactory";
            try
            {
                pWorkspaceName.PathName = TinFilePath;
            }
            catch (Exception ex)
            {
                ;// MessageBox.Show(ex.Message);
            }
            return pWorkspaceName;
        }

        public static IWorkspaceName OpenVpfWorkspaceName(string VpfFilePath)
        {
            IWorkspaceName pWorkspaceName = new WorkspaceNameClass();
            pWorkspaceName.WorkspaceFactoryProgID = "esriDataSourcesFile.VpfWorkspaceFactory";
            try
            {
                pWorkspaceName.PathName = VpfFilePath;

            }
            catch (Exception ex)
            {
                ;// MessageBox.Show(ex.Message);
            }
            return pWorkspaceName;
        }

        public static ICadDrawingDataset GetCADDataset(ICadDrawingWorkspace pCadDwgWorkspace, string DwgFileName)
        {
            ICadDrawingDataset pCadDwgDataset = null;
            try
            {
                pCadDwgDataset = pCadDwgWorkspace.OpenCadDrawingDataset(DwgFileName);
            }
            catch (Exception ex)
            {
                ;// MessageBox.Show(ex.Message);
            }
            return pCadDwgDataset;
        }

        #endregion

        #region//数据库内容操作
        public static bool IsTableExist(IWorkspace wrkSpace, string featClass)
        {
            try
            {
                IFeatureWorkspace featWrkSpace = wrkSpace as IFeatureWorkspace;
                if (featWrkSpace == null) return false;
                IFeatureClass featureClass = featWrkSpace.OpenFeatureClass(featClass);
                if (featureClass == null) return false;
                else
                {
                    featureClass = null;
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region
        public static ICadDrawingDataset GetCADDataset(IWorkspaceName workspaceName, string cadFileName)
        {
            try
            {
                IDatasetName cadDatasetName = new CadDrawingNameClass();
                cadDatasetName.Name = cadFileName;
                cadDatasetName.WorkspaceName = workspaceName;

                //Open the CAD drawing
                IName name = (IName)cadDatasetName;
                return (ICadDrawingDataset)name.Open();
                //return cadDatasetName;
            }
            catch (Exception ex)
            {
                ;// MessageBox.Show(ex.Message);
            }
            return null;
        }


        public static IFeatureDataset GetFeatureDataset(IWorkspaceName workspaceName, string FeatureDatasetName)
        {
            try
            {
                IDatasetName pFeatureDatasetName = new FeatureDatasetNameClass();
                pFeatureDatasetName.Name = FeatureDatasetName;
                pFeatureDatasetName.WorkspaceName = workspaceName;

                //Open the FeatureDataset
                IName name = (IName)pFeatureDatasetName;
                return (IFeatureDataset)name.Open();
            }
            catch (Exception ex)
            {
                ;// MessageBox.Show(ex.Message);
            }
            return null;
        }

        public static IFeatureDataset GetCoverageDataset(IWorkspaceName workspaceName, string CoverageDatasetName)
        {
            try
            {
                IDatasetName pCoverageDatasetName = new CoverageNameClass();
                pCoverageDatasetName.Name = CoverageDatasetName;
                pCoverageDatasetName.WorkspaceName = workspaceName;

                //Open the FeatureDataset
                IName name = (IName)pCoverageDatasetName;
                return (IFeatureDataset)name.Open();
            }
            catch (Exception ex)
            {
                ;// MessageBox.Show(ex.Message);
            }
            return null;
        }

        public static IFeatureClassName GetFeatureClassName(IDatasetName pDatasetName)
        {
            IFeatureClassName pFeatureClassName = new FeatureClassNameClass();
            try
            {
                pFeatureClassName.FeatureDatasetName = pDatasetName;


            }
            catch { }
            return pFeatureClassName;

        }

        public static IFeatureClass GetFeatureClass(IWorkspace workspace, string strFeaDataset, string strFeaclass)
        {
            IDataset dataset = (IDataset)workspace;
            IEnumDataset enumDataset = dataset.Subsets;
            enumDataset.Reset();
            //get the datasets from enumerator     
            IDataset dataset2 = enumDataset.Next();
            //loop through the datasets    

            while (dataset2 != null)
            {

                if (dataset2.Name == strFeaDataset)
                {
                    IEnumDataset enumDataset2 = dataset2.Subsets;
                    enumDataset2.Reset();
                    IDataset dataset3 = enumDataset2.Next();
                    while (dataset3 != null)
                    {

                        if (dataset3.Name == strFeaclass)
                        {
                            return (IFeatureClass)dataset3;
                        }
                        dataset3 = enumDataset2.Next();
                    }
                }

                dataset2 = enumDataset.Next();
            }
            return null;


        }
        public static IAnnotationLayer GetAnnotationLayer(IFeatureWorkspace featWrk, IFeatureDataset featDS, string annolayerName)
        {
            IAnnotationLayer annoLayer = null;
            try
            {
                IAnnotationLayerFactory annoLayerFact = new FDOGraphicsLayerFactoryClass();
                annoLayer = annoLayerFact.OpenAnnotationLayer(featWrk, featDS, annolayerName);
            }
            catch (Exception ex)
            {
                return null;

            }

            return annoLayer;
        }
        public static IFeatureClass GetFeatureClass(IWorkspace pWorkspace, string FeaClsName)
        {

            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)pWorkspace;
            //open the featureclass
            return featureWorkspace.OpenFeatureClass(FeaClsName);

        }

        public static IFeatureClassName GetCoverageFeatureClassName(IDatasetName pDatasetName)
        {
            IFeatureClassName pCoverageFeatureClassName = new CoverageFeatureClassNameClass();
            try
            {
                pCoverageFeatureClassName.FeatureDatasetName = pDatasetName;

            }
            catch { }
            return pCoverageFeatureClassName;

        }
        public static IFeatureClassName GetFgdbFeatureClassName(IDatasetName pDatasetName)
        {
            IFeatureClassName pFgdbFeatureClassName = new FgdbFeatureClassNameClass();
            try
            {
                pFgdbFeatureClassName.FeatureDatasetName = pDatasetName;

            }
            catch { }
            return pFgdbFeatureClassName;

        }
        public static IFeatureClassName GetRasterCatalogName(IDatasetName pDatasetName)
        {
            IFeatureClassName pRasterCatalogName = new RasterCatalogNameClass();
            try
            {
                pRasterCatalogName.FeatureDatasetName = pDatasetName;

            }
            catch { }
            return pRasterCatalogName;

        }

        public static IRasterDataset GetRasterDataset(IWorkspaceName workspaceName, string RasterDatasetName)
        {
            try
            {
                IDatasetName pRasterDatasetName = new RasterDatasetNameClass();
                pRasterDatasetName.Name = RasterDatasetName;
                pRasterDatasetName.WorkspaceName = workspaceName;

                //Open the FeatureDataset
                IName name = (IName)pRasterDatasetName;
                return (IRasterDataset)name.Open();
            }
            catch (Exception ex)
            {
                ;// MessageBox.Show(ex.Message);
            }
            return null;
        }
        public static IRasterDataset GetRasterDataset(IRasterWorkspace pRasterWorkspace, string RasterDatasetName)
        {
            //this example opens a raster from a raster format
            try
            {
                IRasterDataset rasterDataset = pRasterWorkspace.OpenRasterDataset(RasterDatasetName);
                return rasterDataset;
            }
            catch (Exception ex)
            {
                ;// MessageBox.Show(ex.Message);
                return null;
            }
        }
        public static IRasterDataset GetRasterDataset(IWorkspace pSDEWorkspace, string strRasterDataset)
        {

            IEnumDataset pEnumdataset = pSDEWorkspace.get_Datasets(esriDatasetType.esriDTRasterDataset);

            IDataset pDataset = pEnumdataset.Next();
            while (pDataset != null)
            {
                if (pDataset.Name == strRasterDataset)
                {

                    return (IRasterDataset)pDataset;

                }
                pDataset = pEnumdataset.Next();
            }

            pEnumdataset = null;
            pDataset = null;
            return null;
        }
        public static IDataset GetRasterCatalog(IWorkspace workspace, string RasterCatalogName)
        {
            IEnumDataset pEnumdataset = workspace.get_Datasets(esriDatasetType.esriDTRasterCatalog);

            IDataset pDataset = pEnumdataset.Next();
            while (pDataset != null)
            {
                if (pDataset.Name == RasterCatalogName)
                {

                    return pDataset;

                }
                pDataset = pEnumdataset.Next();
            }

            pEnumdataset = null;
            pDataset = null;
            return null;
        }
        public static IRasterBand GetRasterBand(IRasterWorkspace workspace, string strRasterDataset, string strBandName)
        {
            IRasterWorkspace pRasterWorkspace = workspace;
            IRasterDataset pRasterDataset;
            IRasterBandCollection pRasterBandColl;
            IEnumRasterBand pEnumRasterBand;
            IRasterBand pRasterBand = null;
            try
            {
                pRasterDataset = pRasterWorkspace.OpenRasterDataset(strRasterDataset);
                pRasterBandColl = (IRasterBandCollection)pRasterDataset;
                pEnumRasterBand = pRasterBandColl.Bands;
                pRasterBand = pRasterBandColl.get_BandByName(strBandName);
            }
            catch (Exception ex)
            { }
            pRasterWorkspace = null;
            pRasterDataset = null;
            pRasterBandColl = null;
            pEnumRasterBand = null;
            return pRasterBand;
        }

        public static IRasterBand GetRasterBand(IWorkspace pSDEWorkspace, string strRasterDataset, string strBandName)
        {
            IEnumDataset pEnumdataset = pSDEWorkspace.get_Datasets(esriDatasetType.esriDTRasterDataset);
            IRasterBandCollection pRasterBandCollection = null;
            IEnumRasterBand pEnumRasterBand = null;
            IRasterBand pRasterBand = null;
            IDataset pDataset = pEnumdataset.Next();
            while (pDataset != null)
            {
                if (pDataset.Name == strRasterDataset)
                {

                    pRasterBandCollection = (IRasterBandCollection)pDataset;
                    pEnumRasterBand = pRasterBandCollection.Bands;
                    pRasterBand = pEnumRasterBand.Next();
                    while (pRasterBand != null)
                    {
                        if (pRasterBand.Bandname == strBandName)
                        {
                            return pRasterBand;
                        }
                        pRasterBand = pEnumRasterBand.Next();
                    }
                }
                pDataset = pEnumdataset.Next();
            }
            pRasterBandCollection = null;
            pEnumRasterBand = null;
            pRasterBand = null;
            pEnumdataset = null;
            pDataset = null;
            return null;
        }

        public static IDataset GetFeatureDataset(IWorkspace pWorkspace, string FeatureDSName)
        {
            IDataset pDataset = null;
            IEnumDataset pEnumDataset;
            try
            {
                pEnumDataset = pWorkspace.get_Datasets(esriDatasetType.esriDTFeatureDataset);
                pEnumDataset.Reset();
                pDataset = pEnumDataset.Next();
                while (pDataset != null)
                {
                    if (pDataset.Name == FeatureDSName)
                    {
                        return pDataset;
                    }
                    pDataset = pEnumDataset.Next();
                }


            }
            catch
            {

            }
            return null;

        }

        public static IFeatureClassName GetFeatureClassName(IFeatureDatasetName pFeatureDatasetName, string FeatureClassName)
        {
            IDatasetName pContainDSName;
            pContainDSName = (IDatasetName)pFeatureDatasetName;
            IEnumDatasetName pEnumDsName4FC;
            pEnumDsName4FC = (IEnumDatasetName)pFeatureDatasetName.FeatureClassNames;
            IFeatureClassName pfeaClsName;
            pfeaClsName = (IFeatureClassName)pEnumDsName4FC.Next();
            IDatasetName pFC_DSName;
            try
            {
                while (pfeaClsName != null)
                {
                    pFC_DSName = (IDatasetName)pfeaClsName;
                    if (pFC_DSName.Name == FeatureClassName)
                    {
                        return pfeaClsName;
                    }

                    pfeaClsName = (IFeatureClassName)pEnumDsName4FC.Next();
                }
            }
            catch
            {

            }
            return null;
        }
        /// <summary>
        /// 根据源图层后缀，自动判断打开对应的工作空间
        /// </summary>
        /// <param name="sourceLayer"></param>
        /// <returns></returns>
        public static IWorkspace OpenWorkspace(String sourceLayer)
        {
            string PathName = System.IO.Path.GetDirectoryName(sourceLayer);
            IWorkspace pWorkspace = null;
            if (sourceLayer.IndexOf(".shp") != -1)
            {
                pWorkspace = WorkspaceServices.OpenShapeFileWorkspace(PathName);
            }
            else if (sourceLayer.IndexOf(".mdb") != -1)
            {
                PathName = sourceLayer.Substring(0, sourceLayer.IndexOf(".mdb") + 4);
                pWorkspace = WorkspaceServices.OpenMdbWorspace(PathName);
            }
            else if (sourceLayer.IndexOf(".gdb") != -1)
            {
                PathName = sourceLayer.Substring(0, sourceLayer.IndexOf(".gdb") + 4);
                pWorkspace = WorkspaceServices.OpenFileGdbWorkspace(PathName);
            }
            else if (sourceLayer.IndexOf(".sde") != -1)
            {
                string user = "", message = "";
                PathName = sourceLayer.Substring(0, sourceLayer.IndexOf(".sde") + 4);
                pWorkspace = WorkspaceServices.OpenSdeWorkspace(PathName, out user, out message);
            }
            return pWorkspace;
        }
        /// <summary>
        /// 拷贝图层到目标数据库
        /// </summary>
        /// <param name="sourceLayers">原始图层</param>
        /// <param name="targetDatabase">本地mdb或者gdb</param>
        public static bool CopyLayer(string[] sourceLayers, string targetDatabase)
        {
            foreach (string sourceLayer in sourceLayers)
            {
                try
                {
                    string pLayerName = System.IO.Path.GetFileName(sourceLayer);
 
                    IWorkspace pWorkspace = WorkspaceServices.OpenWorkspace(sourceLayer);
                   
                    IFeatureClass featureClass = (pWorkspace as IFeatureWorkspace).OpenFeatureClass(pLayerName);
                    IWorkspace pOutWorkspace = WorkspaceServices.OpenWorkspace(targetDatabase);

                    ExportUtil exportUtil = new ExportUtil();
                    exportUtil.InputFeatureClass = featureClass;
                    exportUtil.OutWorkspace = pOutWorkspace;
                    exportUtil.Excute();
                }
                catch (Exception ex)
                {
                    LOG.Error("拷贝图层失败，详情：" + ex.Message);
                    return false;
                }
            }
            LOG.Info("完成拷贝图层");
            return true;
        }
        #endregion
    }
}
