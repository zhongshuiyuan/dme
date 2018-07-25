using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.DataSource.Define
{
    /// <summary>
    /// 验证结果
    /// </summary>
    public class ValidResult
    {
        public Boolean IsValid { get; set; } = false;
        public string Message { get; set; }
        public Exception Ex { get; set; }
    }
}
