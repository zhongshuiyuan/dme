using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Conf
{
    /// <summary>
    /// 系统类配置
    /// </summary>
    public class SysConfig
    {
        /// <summary>
        /// 数据库链接，配置源于web项目的appsetting.json
        /// </summary>
        public static string DBConnectionString = "Data Source=127.0.0.1/ORCL;User ID=dme;Password=pass;";
    }
}
