using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.DisCache.Define
{
    /*----------------------------------------------------------------
              // Copyright (C)  数慧系统技术有限公司
             // 版权所有。 
             //
             // 文件名：
             // 文件功能描述：redis参数
             //
             // 
             // 创建标识：
             //
             // 修改标识：
             // 修改描述：
 //----------------------------------------------------------------*/

    public class RedisCacheProvider
    {
        public int DataBase { get; set; }
        public string HostName { get; set; }
        public string InstanceName { get; set; }
        public int Port { get; set; }
    }
}
