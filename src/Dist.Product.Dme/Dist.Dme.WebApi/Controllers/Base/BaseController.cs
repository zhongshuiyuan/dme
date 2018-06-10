using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Framework.Exception;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Net;

namespace Dist.Dme.WebApi.Controllers.Base
{
    /// <summary>
    /// WebApiExceptionFilterAttribute：统一处理异常信息
    /// </summary>
    [WebApiExceptionFilterAttribute]
    public class BaseController : Controller
    {
        /// <summary>
        /// 获得Header
        /// </summary>
        /// <param name="key">Header的键</param>
        /// <returns>获得的值, 获取失败返回null</returns>
        /// <param name="request">Request对象, 在Requst还没有初始化的时候需要手动传入, 相反则无需传, 如在过滤器里面, 需要传此参数, 否则只会永远返回null, 传了此参数, 会优先使用此参数</param>
        protected string GetHeaderValue(string key, HttpRequest request = null)
        {
            var isHeader = false;
            StringValues header;
            if (request != null)
            {
                isHeader = request.Headers.TryGetValue(key, out header);
            }
            else
            {
                if (Request != null)
                {
                    isHeader = Request.Headers.TryGetValue(key, out header);
                }
                else
                {
                    return null;
                }
            }
            if (isHeader) return header[0];
            return null;
        }

        /// <summary>
        /// 获得Get请求参数
        /// </summary>
        /// <param name="key">参数名称</param>
        /// <param name="request">Request对象, 在Requst还没有初始化的时候需要手动传入, 相反则无需传, 如在过滤器里面, 需要传此参数, 否则只会永远返回null, 传了此参数, 会优先使用此参数</param>
        /// <returns>获得的值, 获取失败返回null</returns>
        protected string QueryString(string key, HttpRequest request = null)
        {
            var isQueryString = false;
            StringValues queryString;
            if (request != null)
            {
                isQueryString = request.Query.TryGetValue(key, out queryString);
            }
            else
            {
                if (Request != null)
                {
                    isQueryString = Request.Query.TryGetValue(key, out queryString);
                }
                else
                {
                    return null;
                }
            }
            if (isQueryString) return queryString[0];
            return null;
        }

        /// <summary>
        /// 获得Post请求参数
        /// </summary>
        /// <param name="key">参数名称</param>
        /// <param name="request">Request对象, 在Requst还没有初始化的时候需要手动传入, 相反则无需传, 如在过滤器里面, 需要传此参数, 否则只会永远返回null, 传了此参数, 会优先使用此参数</param>
        /// <returns>获得的值, 获取失败返回null</returns>
        protected string Form(string key, HttpRequest request = null)
        {
            var isForm = false;
            StringValues form;
            if (request != null)
            {
                isForm = request.Form.TryGetValue(key, out form);
            }
            else
            {
                if (Request != null)
                {
                    isForm = Request.Form.TryGetValue(key, out form);
                }
                else
                {
                    return null;
                }
            }
            if (isForm) return form[0];
            return null;
        }
        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected Result Success(Object data)
        {
            return new Result(STATUS.SUCCESS, "操作成功", SystemStatusCode.DME_SUCCESS, data);
        }
        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected Result Fail(Object data)
        {
            return new Result(STATUS.FAIL, "操作失败", SystemStatusCode.DME_FAIL, data);
        }
        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected Result Error(Object data)
        {
            return new Result(STATUS.ERROR, "操作错误", SystemStatusCode.DME_ERROR, data);
        }
    }
}