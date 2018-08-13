using SuperMap.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dist.Dme.SuperMap
{
    public class SuperMapHelper
    {
        public static void OpenWorkspace()
        {
            // 打开工作空间，取出工作空间中名为“World”的数据集，查询其描述信息
            Workspace workspace = new Workspace();
            WorkspaceConnectionInfo workspaceConnectionInfo = new WorkspaceConnectionInfo
            {
                Type = WorkspaceType.SXWU
            };
            String file = @"D:\gitrep\dme\src\database\supermap-data\ws-rule.smwu";
            workspaceConnectionInfo.Server = file;
            workspace.Open(workspaceConnectionInfo);
            Datasource datasource = workspace.Datasources[0];
            DatasetVector dataset = (DatasetVector)datasource.Datasets["World"];
            Console.WriteLine("数据集的描述信息为：" + dataset.Description);

            // 保存工作空间
            workspace.Save();
            // 另存工作空间
            String file_saveAs = @"D:\gitrep\dme\src\database\supermap-data\world_saveAs.sxwu";
            WorkspaceConnectionInfo workspaceConnectionInfo_saveAs = new WorkspaceConnectionInfo(file_saveAs);
            if (workspace.SaveAs(workspaceConnectionInfo_saveAs))
            {
                Console.WriteLine("另存工作空间成功！");
            }

            // 释放资源
            dataset.Close();
            workspaceConnectionInfo.Dispose();
            workspaceConnectionInfo_saveAs.Dispose();
            workspace.Close();
            workspace.Dispose();
        }
    }
}
