using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Common
{
    public class Result
    {
        /// <summary>
        /// 业务状态：成功
        /// </summary>
        public const String SUCCESS = "success";
        /// <summary>
        /// 业务状态：失败 因为业务逻辑错误导致操作失败，如邮箱已存在，年龄不满足条件等。
        /// </summary>
        public const String FAIL = "fail";
        /// <summary>
        ///  业务状态：错误 因为一些不可预计的、系统级的错误导致的操作失败，如数据库断电，服务器内存溢出等。
        /// </summary>
        public const String ERROR = "error";
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="status">业务状态</param>
        /// <param name="data">业务信息</param>
        public Result(String status, Object data)
        {
            this.Status = status;
            this.Data = data;
        }
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="status">业务状态</param>
        /// <param name="data">业务信息</param>
        /// <param name="message">显示信息</param>
        public Result(String status, Object data, String message)
        {
            this.Status = status;
            this.Data = data;
            this.Message = message;
        }
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="status">业务状态</param>
        /// <param name="data">业务信息</param>
        /// <param name="message">显示消息</param>
        /// <param name="code">业务编码</param>
        public Result(String status, Object data, String message, int code)
        {
            this.Status = status;
            this.Data = data;
            this.Message = message;
            this.Code = code;
        }
       /// <summary>
       ///  将业务状态和业务信息转换成JSON字符串，业务状态的key为：status，业务信息的key为：data
       /// </summary>
       /// <returns></returns>
        public String ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        ///  * 获取业务状态
        /// * 如果状态为系统定义的成功/失败/错误，则直接调用ReturnValue.SUCCESS、FAILED、ERROR得到状态值。
        ///  * 如果状态为用户传入的自定义值，则直接返回该值
        /// </summary>
        public String Status { get; set; }
         /// <summary>
        ///  设置业务信息,业务信息类型：<br>
        ///  1.从业务异常中提取出的业务错误信息。<br>
        ///  2.自定义的业务信息。<br>
        ///  3.业务对象、业务对象集合
        /// </summary>
        public Object Data { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public String Message { get; set; }
        /// <summary>
        /// 获取结果状态码
        /// </summary>
        public int Code { get; set; }
    }
}
