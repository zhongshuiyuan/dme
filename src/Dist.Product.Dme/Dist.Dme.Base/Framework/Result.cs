using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework
{
    public enum STATUS
    {
        // 成功
        SUCCESS = 0,
        // 错误
        ERROR = 1,
        // 因为业务逻辑错误导致操作失败，如邮箱已存在，年龄不满足条件等。
        FAIL = 2
    }
    public class Result
    {
        /// <summary>
        /// 状态枚举对应的字符说明
        /// </summary>
        private String[] statusDesc = new String[] { "success", "error", "fail" };
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

        public Result(STATUS status, String message, int code, Object data)
        {
            this.Status = this.statusDesc[(int)status];
            this.Message = message;
            this.Code = code;
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
