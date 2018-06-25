using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Dist.Dme.SRCE.Core
{
    /// <summary>
    /// 平台类型
    /// </summary>
    public enum PlatformType
    {
        /// <summary>
        /// 超图平台
        /// </summary>
        [Description("超图平台")]
        SUPERMAP = 1500,
        /// <summary>
        /// esri平台
        /// </summary>
        [Description("ArcGIS平台")]
        ESRI = 1501
    }
}
