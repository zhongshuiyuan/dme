using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.DataSource.Define
{
    /// <summary>
    /// dme文件系统元数据
    /// </summary>
    public class DmeFileSystemMeta
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 后缀，带点
        /// </summary>
        public string Suffix { get; set; }
        /// <summary>
        /// 文档类型
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// mongo文档id
        /// </summary>
        public string ObjectId { get; set; }
    }
}
