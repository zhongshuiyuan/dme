using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Conf
{
    /// <summary>
    /// 系统类配置
    /// </summary>
    public sealed class GlobalSystemConfig
    {
        /// <summary>
        /// 数据库链接，配置源于web项目的appsetting.json
        /// </summary>
        public static string DBConnectionString = "Data Source=127.0.0.1/ORCL;User ID=dme;Password=pass;";
        /// <summary>
        /// 模板目录，/template
        /// </summary>
        public static string DIR_TEMPLATE = "/template";
        /// <summary>
        /// 运行时目录，/runtime
        /// </summary>
        public static string DIR_RUNTIME = "/runtime";
        /// <summary>
        /// 临时目录
        /// </summary>
        public static string DIR_TEMP = "/temp";
        /// <summary>
        /// 文件geodatabase模板路径，/template/fgdb.gdb
        /// </summary>
        public static string PATH_TEMPLATE_FGDB = "/template/fgdb.gdb";
        /// <summary>
        /// 个人geodatabase模板路径，/template/pgdb.mdb
        /// </summary>
        public static string PATH_TEMPLATE_PGDB = "/template/pgdb.mdb";
    }
}
