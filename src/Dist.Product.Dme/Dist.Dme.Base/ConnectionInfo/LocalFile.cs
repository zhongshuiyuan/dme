using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.ConnectionInfo
{
    /// <summary>
    /// 本地文件，可以是mdb、gdb、cad，不包括shape文件，因为shapefile工作空间就是它所在的文件夹
    /// </summary>
    public class LocalFile
    {
        public string Path { get; set; }
    }
}
