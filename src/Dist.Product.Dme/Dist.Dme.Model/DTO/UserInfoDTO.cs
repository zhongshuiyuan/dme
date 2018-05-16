using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 用户信息DTO
    /// </summary>
    public class UserInfoDTO
    {
        public int Id { get; set; }
        public String SysCode { get; set; }
        public String LoginName { get; set; }
        public String Name { get; set; }
        public int Status { get; set; }
        public Int64 CreateTime { get; set; }
        public String Email { get; set; }
        public String Telephone { get; set; }
        public int UserType { get; set; }
    }
}
