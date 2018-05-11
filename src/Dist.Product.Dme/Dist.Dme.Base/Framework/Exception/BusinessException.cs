using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework.Exception
{
    /// <summary>
    /// 自定义业务异常信息
    /// </summary>
    public class BusinessException : SystemException
    {
        /// <summary>
        /// 业务异常编码
        /// </summary>
        public int Code;

        public BusinessException() : base()
        {

        }
        public BusinessException(string message) : base(message)
        {
        }
        public BusinessException(int code, string message) : base(message)
        {
            this.Code = code;
        }
    }
}
