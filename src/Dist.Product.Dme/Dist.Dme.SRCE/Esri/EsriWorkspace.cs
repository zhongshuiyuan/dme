﻿using Dist.Dme.Base.DataSource.Define;
using Dist.Dme.SRCE.Core;
using Dist.Dme.SRCE.Esri.Utils;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dist.Dme.SRCE.Esri
{
    /// <summary>
    /// esri平台工作空间
    /// </summary>
    public class EsriWorkspace : DMEWorkspace<IWorkspace, IFeatureClass>
    {
        public override IWorkspace Open(OracleConn conn)
        {
            return WorkspaceUtil.OpenSdeWorkspace(conn.Username, conn.Password, conn.Server);
        }

        public override IWorkspace Open(LocalConn conn)
        {
            IWorkspace workspace = null;
            switch (conn.Type)
            {
                case Base.Common.EnumDataSourceType.SHAPEFILE:
                    workspace = WorkspaceUtil.OpenShapeFileWorkspace(conn.Path);
                    break;
                case Base.Common.EnumDataSourceType.COVERAGE:
                    workspace = WorkspaceUtil.OpenCoverageWorkspace(conn.Path);
                    break;
                case Base.Common.EnumDataSourceType.PERSONAL_GEODATABASE:
                    workspace = WorkspaceUtil.OpenMdbWorspace(conn.Path);
                    break;
                case Base.Common.EnumDataSourceType.FILE_GEODATABASE:
                    workspace = WorkspaceUtil.OpenFileGdbWorkspace(conn.Path);
                    break;
                case Base.Common.EnumDataSourceType.TIN:
                    workspace = WorkspaceUtil.OpenTinWorkspace(conn.Path);
                    break;
                case Base.Common.EnumDataSourceType.CAD:
                    workspace = WorkspaceUtil.OpenCadWorkspace(conn.Path);
                    break;
                default:
                    throw new Exception($"不支持当前本地文件格式[{conn.Path}]");
            }
            return workspace;
        }
        public override IFeatureClass GetFeatureClass(IWorkspace workspace, string featureClassName)
        {
            return WorkspaceUtil.GetFeatureClass(workspace, featureClassName);
        }
    }
}
