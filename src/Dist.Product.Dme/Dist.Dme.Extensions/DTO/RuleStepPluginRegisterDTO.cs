using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Extensions.DTO
{
    public class RuleStepPluginRegisterDTO
    {
        public string Name { get; set; }
        /// 分组，类别
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// assembly文件名称
        /// </summary>
        public string Assembly { get; set; }
        /// <summary>
        /// 类名全路径
        /// </summary>
        public string ClassId { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
