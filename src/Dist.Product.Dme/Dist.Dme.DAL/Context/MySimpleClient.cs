using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.DAL.Context
{
    /// <summary>
    /// 扩展自己的db操作客户端
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MySimpleClient<T> : SimpleClient<T> where T : class, new()
    {
        public MySimpleClient(SqlSugarClient context) : base(context)
        {

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
    }
}
