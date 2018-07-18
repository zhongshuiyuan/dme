using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http.Filters;

namespace Dist.Dme.Base.Common.Http
{
    /// <summary>
    /// 自定义缓存过滤器特性
    /// </summary>
    public class CacheFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 缓存时间，单位：秒
        /// </summary>
        public int CacheSecondDuration { get; set; }
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Response.Headers.CacheControl = new CacheControlHeaderValue
            {
                MaxAge = TimeSpan.FromSeconds(CacheSecondDuration),
                MustRevalidate = true,
                Public = true
            };
        }
    }
}
