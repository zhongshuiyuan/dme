using ESRI.ArcGIS.esriSystem;
using log4net;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.AECommon.Utils
{
    /// <summary>
    /// ESRI License 认证初始化
    /// </summary>
    public class LicenseUtil
    {
        private static log4net.ILog LOG = LogManager.GetLogger(typeof(LicenseUtil));

        private static ESRI.ArcGIS.esriSystem.IAoInitialize m_AoInitializeClass;

        static LicenseUtil()
        {
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);
            m_AoInitializeClass = new ESRI.ArcGIS.esriSystem.AoInitializeClass();
        }

        public static bool CheckOutLicenseAdvanced()
        {
            return CheckOutLicenseMain(esriLicenseProductCode.esriLicenseProductCodeEngineGeoDB);
        }

        //签出产品许可
        public static bool CheckOutLicenseMain(esriLicenseProductCode code)
        {
            try
            {
                if (m_AoInitializeClass.IsProductCodeAvailable(code) == esriLicenseStatus.esriLicenseAvailable)
                {
                    if (m_AoInitializeClass.Initialize(code) == esriLicenseStatus.esriLicenseCheckedOut)
                    {
                        m_AoInitializeClass.InitializedProduct();
                        //TraceHandler.AddDebugMessage("check out successed : " + aoInit.GetLicenseProductName(code));
                        return true;
                    }
                    //TraceHandler.AddErrorMessage("!!!check out error : " + aoInit.GetLicenseProductName(code));
                }
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
                //TraceHandler.AddException(ex);

            }

            return false;
        }

        //签出扩展许可
        private static bool CheckOutLicenseExtension(ref AoInitializeClass aoInit, esriLicenseExtensionCode code)
        {
            try
            {
                if (aoInit.CheckOutExtension(code) == esriLicenseStatus.esriLicenseCheckedOut)
                {
                    aoInit.GetLicenseExtensionName(code);
                    //TraceHandler.AddDebugMessage("check out successed :" + aoInit.GetLicenseExtensionName(code));
                    return true;
                }
                //TraceHandler.AddErrorMessage("!!!check out error :" + aoInit.GetLicenseExtensionName(code));
            }
            catch (Exception ex)
            {
                Console.WriteLine("初始化错误" + ex.ToString());
            }
            return false;
        }

        public static void ShutDown()
        {
            if (m_AoInitializeClass != null)
                m_AoInitializeClass.Shutdown();
        }
    }
}
