using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 模型注册完成后响应的DTO
    /// </summary>
    public class ModelRegisterRespDTO
    {
        public int Id { get; set; }
        public String SysCode { get; set; }
        public String Name { get; set; }
        public String Remark { get; set; }
        public long CreateTime { get; set; }
        public int IsPublish { get; set; }
        public long PublishTime { get; set; }
        public string Category { get; set; }
        public int Status { get; set; }
        public string ModelTypeCode { get; set; }
        public IList<string> VersionCodes { get; set; }
    }
}
