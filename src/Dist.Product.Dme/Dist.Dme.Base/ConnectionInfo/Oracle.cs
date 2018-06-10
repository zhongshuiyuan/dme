using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.ConnectionInfo
{
    /// <summary>
    /// oracle连接信息
    /// </summary>
    public class Oracle
    {
        /// <summary>
        /// 自定义名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// oracle服务器
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// 数据库实例
        /// </summary>
        public string Database { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 密码是否加密，1：加密；0：明文
        /// </summary>
        public int Encrypted { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}
