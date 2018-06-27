using Dist.Dme.Base.Common;
using Dist.Dme.Base.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Dist.Dme.Base.Framework
{
    public class Result
    {
        /// <summary>
        /// 状态
        /// </summary>
        public String Status { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        public String Message { get; set; }
        /// <summary>
        /// 获取结果状态码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public Object Data { get; set; }

        public Result(SystemStatusCode status, String message, int code, Object data)
        {
            this.Status = EnumUtil.GetEnumDisplayName(status);
            this.Message = message;
            this.Code = code;
            this.Data = data;
        }
        public Result(SystemStatusCode status, String message, Object data)
        {
            this.Status = EnumUtil.GetEnumDisplayName(status);
            this.Message = message;
            this.Code = (int)status;
            this.Data = data;
        }
        /// <summary>
        ///  将业务状态和业务信息转换成JSON字符串，业务状态的key为：status，业务信息的key为：data
        /// </summary>
        /// <returns></returns>
        public String ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
