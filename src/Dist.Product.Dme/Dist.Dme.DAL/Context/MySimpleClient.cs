using NLog;
using SqlSugar;
using System;
using System.Collections.Generic;

namespace Dist.Dme.DAL.Context
{
    /// <summary>
    /// 扩展自己的db操作客户端
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MySimpleClient<T> : SimpleClient<T> where T : class, new()
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        public MySimpleClient(SqlSugarClient context) : base(context)
        {

        }
        public SqlSugarClient GetContext()
        {
            return base.Context;
        }
        /// <summary>
        /// SimpleClient中的方法满足不了你，你可以扩展自已的方法
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<T> GetByIds(dynamic[] ids)
        {
            return Context.Queryable<T>().In(ids).ToList(); ;
        }
        /// <summary>
        /// 按照属性排序检索所有
        /// </summary>
        /// <param name="orderProperty"></param>
        /// <param name="asc">是否升序</param>
        /// <returns></returns>
        public List<T> GetList(String orderProperty, bool asc)
        {
            return base.Context.Queryable<T>().OrderBy(orderProperty + (asc? " asc " : " desc ")).ToList();
        }
    }
}
