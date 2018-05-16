using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    public abstract class BaseRequestDTO
    {
        /// <summary>
        /// 参数集
        /// </summary>
        public IDictionary<String, Object> Parameters { get; set; }
    }
}
