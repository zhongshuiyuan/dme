using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Dist.Dme.Base.Common
{
    public enum EnumDataSourceType
    {
        [Description("unknown type，未知类型")]
        UNKNOWN,
        [Description("shape file，栅格数据")]
        SHAPEFILE,
        [Description("coverage file，ArcInfo workstation原生数据格式")]
        COVERAGE,
        [Description("personal geodatabase，mdb文件")]
        PERSONAL_GEODATABASE,
        [Description("file geodatabase，gdb文件")]
        FILE_GEODATABASE,
        [Description("enterprise geodatabase，数据库连接，如oracle、sqlserver等")]
        ENTERPRISE_GEODATABASE,
        [Description("不规则三角网")]
        TIN,
        [Description("CAD数据")]
        CAD,
        [Description("oracle")]
        ORACLE,
        [Description("mongodb")]
        MONGODB
    }
}
