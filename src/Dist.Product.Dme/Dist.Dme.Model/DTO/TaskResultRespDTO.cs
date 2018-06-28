using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 任务结果响应DTO
    /// </summary>
    public class TaskResultRespDTO
    {
        public int RuleStepId { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public object Value { get; set; }
    }
}
