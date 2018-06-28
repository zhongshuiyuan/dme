using Dist.Dme.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 任务响应DTO
    /// </summary>
    public class TaskRespDTO
    {
        /// <summary>
        /// 任务
        /// </summary>
        public DmeTask Task { get; set; }
        /// <summary>
        /// 任务依赖的模型
        /// </summary>
        public DmeModel Model { get; set; }
        /// <summary>
        /// 模型版本
        /// </summary>
        public DmeModelVersion ModelVersion { get; set; }
    }
}
