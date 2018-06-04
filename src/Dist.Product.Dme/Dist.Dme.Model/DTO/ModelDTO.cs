using Dist.Dme.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 模型
    /// </summary>
    public class ModelDTO
    {
        public int Id { get; set; }
        public String SysCode { get; set; }
        public String Name { get; set; }
        public String Remark { get; set; }
        public long CreateTime { get; set; }
        /// <summary>
        /// 模型版本集合
        /// </summary>
        public IList<ModelVersionDTO> Versions { get; set; } = new List<ModelVersionDTO>();
    }
}
