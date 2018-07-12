using NLog;
using System;
using System.Net;
using System.Net.Http;
// 需要依赖：Microsoft.AspNet.WebApi.Core.dll，其它的可能会缺失dll
using System.Web.Http.Filters;

namespace Dist.Dme.Base.Framework.Exception
{
    /// <summary>
    /// webapi异常处理标签，支持类和方法
    /// [WebApiExceptionFilterAttribute]
    /// </summary>
    public class WebApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        //重写基类的异常处理方法
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            //1.异常日志记录（正式项目里面一般是用log4net记录异常日志）
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "—" +
                actionExecutedContext.Exception.GetType().ToString() + "：" + actionExecutedContext.Exception.Message + "—堆栈信息：" +
                actionExecutedContext.Exception.StackTrace);

            //2.返回调用方具体的异常信息
            if (actionExecutedContext.Exception is NotImplementedException)
            {
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            }
            else if (actionExecutedContext.Exception is TimeoutException)
            {
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.RequestTimeout);
            }
            else if (actionExecutedContext.Exception is BusinessException)
            {
                var oResponse = new BusinessResponseMessage(((BusinessException)actionExecutedContext.Exception).Code)
                {
                    Content = new StringContent(((BusinessException)actionExecutedContext.Exception).Message),
                    ReasonPhrase = ((BusinessException)actionExecutedContext.Exception).Message
                };
                actionExecutedContext.Response = oResponse;
            }
            //.....这里可以根据项目需要返回到客户端特定的状态码。如果找不到相应的异常，统一返回服务端错误500
            else
            {
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

            base.OnException(actionExecutedContext);
        }
    }
}
