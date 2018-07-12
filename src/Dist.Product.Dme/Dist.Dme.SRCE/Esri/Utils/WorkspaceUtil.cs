using Dist.Dme.Base.Utils;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesOleDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using NLog;
using System;
using System.IO;

namespace Dist.Dme.SRCE.Esri.Utils
{
    public sealed class WorkspaceUtil
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 通过sde连接信息打开工作空间
        /// 格式："SERVER=Kona;DATABASE=sde;INSTANCE=5151;USER=Editor;PASSWORD=Editor;VERSION=sde.DEFAULT" 
        /// </summary>
        /// <param name="connstr">sde连接信息</param>
        /// <returns></returns>
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
        public static string GetSDEConnectStr(IWorkspace pWorkspace)
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
        /// 打开MDB工作空间
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
        /// <summary>
        /// 通过直连方式打开SDE工作空间
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="server"></param>
        /// <param name="instance">sde:oracle11g:orcl</param>
        /// <param name="database">数据库实例名</param>
        /// <param name="version">默认版本，SDE.DEFAULT</param>
        /// <returns></returns>
        public static IWorkspace OpenSdeWorkspace(string user, string password, string server, string instance = "sde:oracle11g", string database = "ORCL", string version = "SDE.DEFAULT")
        {
            try
            {
                IPropertySet propertySet = new PropertySetClass();
                propertySet.SetProperty("SERVER", server);
                propertySet.SetProperty("INSTANCE", $"sde:oracle11g:{server}/{database}");
                // propertySet.SetProperty("DATABASE", database);
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
        /// 通过sde文件连接SDE
        /// </summary>
        /// <param name="sdeFile">sde的连接配置文件</param>
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
        /// <summary>
        /// 打开gdb文件
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 打开personal geodatabase工作空间
        /// </summary>
        /// <param name="database">mdb文件所在的路径</param>
        /// <returns></returns>
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
        /// <summary>
        /// 打开shape文件工作空间
        /// </summary>
        /// <param name="shapeFileFoderPath">shape文件所在的文件夹</param>
        /// <returns></returns>
        public static IWorkspace OpenShapeFileWorkspace(string shapeFileFoderPath)
        {
            IWorkspaceFactory workspaceFactory = new ShapefileWorkspaceFactoryClass(); ;
            IWorkspace pWorkspace = null;
            IPropertySet pSet = new PropertySetClass();
            pSet.SetProperty("DATABASE", shapeFileFoderPath);
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
        /// <summary>
        /// 打开coverage工作空间
        /// </summary>
        /// <param name="coverageFoderPath">coverage所在文件夹路径</param>
        /// <returns></returns>
        public static IWorkspace OpenCoverageWorkspace(string coverageFoderPath)
        {
            IWorkspaceFactory workspaceFactory = new ArcInfoWorkspaceFactory(); ;
            IWorkspace pWorkspace = null;
          
            try
            {
                pWorkspace = workspaceFactory.OpenFromFile(coverageFoderPath, 0);
            }
            catch (Exception ex)
            {
                LOG.Error("coverage文件打开错误，详情：" + ex.Message);
                return null;
            }
            return pWorkspace;
        }
        /// <summary>
        /// 打开CAD工作空间
        /// PS：打开cad要素时候，需要注意格式：cad文件名:要素类名称。cad文件名需要包含扩展名
        /// 如："buildings.dxf:polygon"
        /// </summary>
        /// <param name="cadFolderPath">cad所在的文件夹路径</param>
        /// <returns></returns>
        public static IWorkspace OpenCadWorkspace(string cadFolderPath)
        {
            IWorkspaceFactory pCADWorkSpaceFc = null;
            IWorkspace pWorkspace = null;
            try
            {
                pCADWorkSpaceFc = new CadWorkspaceFactoryClass();
                pWorkspace = pCADWorkSpaceFc.OpenFromFile(cadFolderPath, 0);
            }
            catch (Exception ex)
            {
                LOG.Error("cad文件打开错误，详情：" + ex.Message);
                // Console.WriteLine("空间数据库打开错误");
                return null;
            }
            return pWorkspace;
        }
    
        /// <summary>
        /// 打开栅格数据工作空间
        /// </summary>
        /// <param name="rasterFolderPath">栅格数据所在的文件夹路径</param>
        /// <returns></returns>
        public static IWorkspace OpenRasterWorkspace(string rasterFolderPath)
        {
            IWorkspaceFactory pRasterWorkSpaceFc = null;
            IWorkspace pWorkspace = null;
            try
            {
                pRasterWorkSpaceFc = new RasterWorkspaceFactoryClass();
                pWorkspace = pRasterWorkSpaceFc.OpenFromFile(rasterFolderPath, 0);
            }
            catch (Exception ex)
            {
                LOG.Error("raster文件打开错误，详情：" + ex.Message);
                // Console.WriteLine("空间数据库打开错误");
                return null;
            }
            return pWorkspace;
        }
        /// <summary>
        /// 打开不规则三角网数据工作空间
        /// </summary>
        /// <param name="tinFolderPath">TIN所在文件夹路径</param>
        /// <returns></returns>
        public static IWorkspace OpenTinWorkspace(string tinFolderPath)
        {
            IWorkspaceFactory pTinWorkSpaceFc = null;
            IWorkspace pWorkspace = null;
            try
            {
                pTinWorkSpaceFc = new TinWorkspaceFactoryClass();
                pWorkspace = pTinWorkSpaceFc.OpenFromFile(tinFolderPath, 0);
            }
            catch (Exception ex)
            {
                LOG.Error("TIN数据开错误" + ex.Message);
                return null;
            }
            return pWorkspace;
        }

        /// <summary>
        /// 打开本地Excel工作空间
        /// </summary>
        /// <param name="ExcelFilePath"></param>
        /// <returns></returns>
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
                LOG.Error(ex);
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
                LOG.Error(ex);
            }
            return pWorkspace;
        }

        public static IWorkspaceName OpenTextFileWorkspaceName(string TextFilePath)
        {

            IWorkspaceName pWorkspaceName = new WorkspaceNameClass
            {
                WorkspaceFactoryProgID = "esriDataSourcesOleDB.TextFileWorkspaceFactory"
            };
            try
            {
                pWorkspaceName.PathName = TextFilePath;
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
            }
            return pWorkspaceName;
        }

        public static IWorkspaceName OpenTinWorkspaceName(string TinFilePath)
        {

            IWorkspaceName pWorkspaceName = new WorkspaceNameClass
            {
                WorkspaceFactoryProgID = "esriDataSourcesFile.TinWorkspaceFactory"
            };
            try
            {
                pWorkspaceName.PathName = TinFilePath;
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
            }
            return pWorkspaceName;
        }

        public static IWorkspaceName OpenVpfWorkspaceName(string VpfFilePath)
        {
            IWorkspaceName pWorkspaceName = new WorkspaceNameClass
            {
                WorkspaceFactoryProgID = "esriDataSourcesFile.VpfWorkspaceFactory"
            };
            try
            {
                pWorkspaceName.PathName = VpfFilePath;

            }
            catch (Exception ex)
            {
                LOG.Error(ex);
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
                LOG.Error(ex);
            }
            return pCadDwgDataset;
        }

        #endregion

        #region 数据库内容操作
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
                LOG.Error(ex);
                return false;
            }
        }
        #endregion

        #region
        public static ICadDrawingDataset GetCADDataset(IWorkspaceName workspaceName, string cadFileName)
        {
            try
            {
                IDatasetName cadDatasetName = new CadDrawingNameClass
                {
                    Name = cadFileName,
                    WorkspaceName = workspaceName
                };

                //Open the CAD drawing
                IName name = (IName)cadDatasetName;
                return (ICadDrawingDataset)name.Open();
                //return cadDatasetName;
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
            }
            return null;
        }


        public static IFeatureDataset GetFeatureDataset(IWorkspaceName workspaceName, string featureDatasetName)
        {
            try
            {
                IDatasetName pFeatureDatasetName = new FeatureDatasetNameClass
                {
                    Name = featureDatasetName,
                    WorkspaceName = workspaceName
                };
                //Open the FeatureDataset
                IName name = (IName)pFeatureDatasetName;
                return (IFeatureDataset)name.Open();
            }
            catch (Exception ex)
            {
                LOG.Error($"打开数据集[{featureDatasetName}]失败", ex);
            }
            return null;
        }

        public static IFeatureDataset GetCoverageDataset(IWorkspaceName workspaceName, string coverageDatasetName)
        {
            try
            {
                IDatasetName pCoverageDatasetName = new CoverageNameClass
                {
                    Name = coverageDatasetName,
                    WorkspaceName = workspaceName
                };

                //Open the FeatureDataset
                IName name = (IName)pCoverageDatasetName;
                return (IFeatureDataset)name.Open();
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
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
        /// <summary>
        /// 获取数据集下的要素类
        /// </summary>
        /// <param name="workspace">工作空间</param>
        /// <param name="featureDatasetName">数据集名称</param>
        /// <param name="featureClassName">要素类名称</param>
        /// <returns></returns>
        public static IFeatureClass GetFeatureClass(IWorkspace workspace, string featureDatasetName, string featureClassName)
        {
            IDataset dataset = (IDataset)workspace;
            IEnumDataset enumDataset = dataset.Subsets;
            enumDataset.Reset();
            //get the datasets from enumerator     
            IDataset datasetNext = enumDataset.Next();
            //loop through the datasets    

            while (datasetNext != null)
            {
                if (datasetNext.Name == featureDatasetName)
                {
                    IEnumDataset enumDataset2 = datasetNext.Subsets;
                    enumDataset2.Reset();
                    IDataset dataset3 = enumDataset2.Next();
                    while (dataset3 != null)
                    {

                        if (dataset3.Name == featureClassName)
                        {
                            return (IFeatureClass)dataset3;
                        }
                        dataset3 = enumDataset2.Next();
                    }
                }

                datasetNext = enumDataset.Next();
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
                LOG.Error(ex);
                return null;
            }

            return annoLayer;
        }
        /// <summary>
        /// 获取要素类
        /// </summary>
        /// <param name="pWorkspace">工作空间</param>
        /// <param name="featureClsName">要素类名称</param>
        /// <returns></returns>
        public static IFeatureClass GetFeatureClass(IWorkspace pWorkspace, string featureClsName)
        {
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)pWorkspace;
            //open the featureclass
            return featureWorkspace.OpenFeatureClass(featureClsName);

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
                IDatasetName pRasterDatasetName = new RasterDatasetNameClass
                {
                    Name = RasterDatasetName,
                    WorkspaceName = workspaceName
                };

                //Open the FeatureDataset
                IName name = (IName)pRasterDatasetName;
                return (IRasterDataset)name.Open();
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
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
                LOG.Error(ex);
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
            {
                LOG.Error(ex);
            }
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

        public static IDataset GetFeatureDataset(IWorkspace pWorkspace, string featureDSName)
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
                    if (pDataset.Name == featureDSName)
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

        public static IFeatureClassName GetFeatureClassName(IFeatureDatasetName pFeatureDatasetName, string featureClassName)
        {
            IDatasetName pContainDSName;
            pContainDSName = (IDatasetName)pFeatureDatasetName;
            IEnumDatasetName pEnumDsName4FC = (IEnumDatasetName)pFeatureDatasetName.FeatureClassNames;
            IFeatureClassName pfeaClsName = (IFeatureClassName)pEnumDsName4FC.Next();

            IDatasetName pFC_DSName;
            try
            {
                while (pfeaClsName != null)
                {
                    pFC_DSName = (IDatasetName)pfeaClsName;
                    if (pFC_DSName.Name == featureClassName)
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
                pWorkspace = WorkspaceUtil.OpenShapeFileWorkspace(PathName);
            }
            else if (sourceLayer.IndexOf(".mdb") != -1)
            {
                PathName = sourceLayer.Substring(0, sourceLayer.IndexOf(".mdb") + 4);
                pWorkspace = WorkspaceUtil.OpenMdbWorspace(PathName);
            }
            else if (sourceLayer.IndexOf(".gdb") != -1)
            {
                PathName = sourceLayer.Substring(0, sourceLayer.IndexOf(".gdb") + 4);
                pWorkspace = WorkspaceUtil.OpenFileGdbWorkspace(PathName);
            }
            else if (sourceLayer.IndexOf(".sde") != -1)
            {
                PathName = sourceLayer.Substring(0, sourceLayer.IndexOf(".sde") + 4);
                pWorkspace = WorkspaceUtil.OpenSdeWorkspace(PathName, out string user, out string message);
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
 
                    IWorkspace pWorkspace = WorkspaceUtil.OpenWorkspace(sourceLayer);
                   
                    IFeatureClass featureClass = (pWorkspace as IFeatureWorkspace).OpenFeatureClass(pLayerName);
                    IWorkspace pOutWorkspace = WorkspaceUtil.OpenWorkspace(targetDatabase);
                    FeatureClassUtil.ExportToWorkspace(featureClass, pOutWorkspace);
                }
                catch (Exception ex)
                {
                    LOG.Error("拷贝图层失败，详情：" + ex.Message);
                    continue;
                }
            }
            LOG.Info("完成拷贝图层");
            return true;
        }
        /// <summary>
        /// 打开图层
        /// </summary>
        /// <param name="featureClassPath">要素类完成目录和要素类名称，格式：目录|要素类</param>
        /// <param name="featureClassPathSeparator">要素类路径分隔符</param>
        /// <param name="workspace">输出打开的工作空间</param>
        /// <param name="featureClass">输出打开的要素类</param>
        public static void OpenFeatureClass(string featureClassPath, string featureClassPathSeparator, out IFeatureWorkspace workspace, out IFeatureClass featureClass)
        {
            string[] paths = featureClassPath.Split(featureClassPathSeparator);
            if (2 == paths.Length)
            {
                workspace = WorkspaceUtil.OpenWorkspace(paths[0]) as IFeatureWorkspace;
                featureClass = workspace.OpenFeatureClass(paths[1]);
            }
            else
            {
                LOG.Error("数据源路径格式不正确");
                throw new Exception("数据源路径格式不正确，格式应为：mdb路径" + featureClassPathSeparator + "要素类");
            }
        }
        #endregion
        /// <summary>
        /// 释放对象
        /// </summary>
        /// <param name="obj"></param>
        public static void ReleaseComObject(object obj)
        {
            try
            {
                if ((obj != null))
                {
                    int nCount = 0;
                    do
                    {
                        nCount = System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                    }
                    while ((nCount > 0));
                }
                obj = null;
            }
            catch (Exception ex)
            {
                LOG.Error("释放对象出错", ex);
            }
        }
        /// <summary>
        /// 获取要素类的工作空间
        /// </summary>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        public static IWorkspace GetWorkspace(IFeatureClass featureClass)
        {
            IDataset dataset = featureClass as IDataset;
            if (dataset == null) return null;
            return dataset.Workspace;
        }
    }
}
