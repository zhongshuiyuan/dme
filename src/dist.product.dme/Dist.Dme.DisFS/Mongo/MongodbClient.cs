using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.DisFS.Mongo
{
    /// <summary>
    /// mongo客户端类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class MongodbClient<T> where T : class
    {
        #region +MongodbInfoClient 获取mongodb实例
        /// <summary>
        /// 获取mongodb实例
        /// </summary>
        /// <param name="host">连接字符串，库，表</param>
        /// <returns></returns>
        public static IMongoCollection<T> MongodbInfoClient(MongodbHost host)
        {
            MongoClient client = new MongoClient(host.Connection);
            var dataBase = client.GetDatabase(host.DataBase);
            return dataBase.GetCollection<T>(host.Table);
        }
        #endregion
    }
    /// <summary>
    /// mongo参数定义
    /// </summary>
    public class MongodbHost
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string Connection { get; set; }
        /// <summary>
        /// 库
        /// </summary>
        public string DataBase { get; set; }
        /// <summary>
        /// 表
        /// </summary>
        public string Table { get; set; }
    }
}
