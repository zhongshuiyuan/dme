using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 模型版本更新
    /// </summary>
    public class ModelVersionUpdateDTO
    {
        public string SysCode { get; set; }
        public string NewName { get; set; }
        public int Status { get; set; } = 1;
    }
}
