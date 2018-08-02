using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 模型基本信息更新
    /// </summary>
    public class ModelBasicInfoUpdateDTO
    {
        /// <summary>
        /// 模型编码
        /// </summary>
        public string SysCode { get; set; }
        /// <summary>
        /// 模型修改名称
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 模型备注
        /// </summary>
        public String Remark { get; set; }
        /// <summary>
        /// 模型类型编码
        /// </summary>
        public String TypeCode { get; set; }
    }
}
