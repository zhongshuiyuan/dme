using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 版本信息
    /// </summary>
    public class VersionDTO
    {
        /// <summary>
        /// 主版本号
        /// </summary>
        [Required]
        public int MajorVersion { get; set; }
        /// <summary>
        /// 次版本号
        /// </summary>
        [Required]
        public int MinorVersion { get; set; }
        /// <summary>
        /// 修订号
        /// </summary>
        [Required]
        public int RevisionVersion { get; set; }
    }
}
