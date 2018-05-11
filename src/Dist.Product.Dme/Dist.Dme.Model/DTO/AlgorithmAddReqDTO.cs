using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 
    /// </summary>
    public class AlgorithmAddReqDTO
    {
        [Required]
        public String SysCode { get; set; }
        [Required]
        public String Name { get; set; }
        public String Alias { get; set; }
        public String Version { get; set; }
        public String Remark { get; set; }
        public String Path { get; set; }
        [Required]
        public IList<AlgorithmMetaReqDTO> Metas { get; set; }
    }
}
