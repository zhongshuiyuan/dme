using Dist.Dme.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 算法响应DTO
    /// </summary>
    public class AlgorithmRespDTO
    {
        public int Id { get; set; }
        public String SysCode { get; set; }
        public String Name { get; set; }
        public String Alias { get; set; }
        public String Version { get; set; }
        public long CreateTime { get; set; }
        public String Remark { get; set; }
        public String Type { get; set; }
        public object Extension { get; set; }
        public IList<DmeAlgorithmMeta> Metas { get; set; }
    }
}
