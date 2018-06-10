using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.ConnectionInfo
{
    /// <summary>
    /// 本地文件夹，可以是shape，因为shapefile工作空间就是它所在的文件夹
    /// </summary>
    public class LocalFolder
    {
        /// <summary>
        /// 目录
        /// </summary>
        public string Dir { get; set; }
    }
}
