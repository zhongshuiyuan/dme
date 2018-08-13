using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Dist.Dme.Base.Common
{
    /// <summary>
    /// 属性类别
    /// </summary>
    public enum EnumAttributeType
    {
        /// <summary>
        /// 一般属性
        /// </summary>
        [DisplayName("normal")]
        NORMAL = 0,
        /// <summary>
        /// 运行时属性（需要运行模型的时候注入）
        /// </summary>
        [DisplayName("runtime")]
        RUNTIME = 1
    }
}
