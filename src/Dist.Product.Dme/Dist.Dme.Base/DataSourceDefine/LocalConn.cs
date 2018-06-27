using Dist.Dme.Base.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.DataSourceDefine
{
    /// <summary>
    /// 本地文件，可以是mdb、gdb、cad
    /// 本地文件夹，可以是shape，因为shapefile工作空间就是它所在的文件夹
    /// </summary>
    public class LocalConn
    {
        /// <summary>
        /// 类型
        /// </summary>
        public GeoDatasourceType Type { get; set; }
        /// <summary>
        /// 路径，可以是文件路径，或者文件夹路径
        /// </summary>
        public string Path { get; set; }
    }
}
