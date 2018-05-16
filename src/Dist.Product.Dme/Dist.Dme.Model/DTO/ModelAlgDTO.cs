using Dist.Dme.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 模型与算法结合转换模型
    /// </summary>
    public class ModelAlgDTO
    {
        public int Id { get; set; }
        public String SysCode { get; set; }
        public String Name { get; set; }
        public String Remark { get; set; }
        public long CreateTime { get; set; }
        /// <summary>
        /// 模型关联的算法
        /// </summary>
        public IList<DmeAlgorithm> Algorithms { get; set; }
    }
}
