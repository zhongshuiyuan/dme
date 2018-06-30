using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Concurrent;

namespace Dist.Dme.DisFS.Adapters.Mongo
{
    /// <summary>
    /// mongo管理类，客户端版本需要比服务器端版本低才能成功，否则出现版本兼容性问题。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class MongodbManager<T> where T : class
    {
        /// <summary>
        /// mongodb客户端缓存
        /// </summary>
        private static ConcurrentDictionary<string, Lazy<MongoClient>> MongoClientCache =
            new ConcurrentDictionary<string, Lazy<MongoClient>>();
        /// <summary>
        /// mongodb bucket缓存
        /// </summary>
        private static ConcurrentDictionary<string, Lazy<GridFSBucket>> GridFSBucketCache =
            new ConcurrentDictionary<string, Lazy<GridFSBucket>>();

        /// <summary>
        /// 获取mongodb客户端实例
        /// </summary>
        /// <param name="connectionString">连接字符串，格式：mongodb://ip:port</param>
        /// <returns></returns>
        public static IMongoClient GetMongodbClient(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("MongoDB Connection String is Empty");

            return MongoClientCache.GetOrAdd(connectionString,
               new Lazy<MongoClient>(() =>
               {
                   return new MongoClient(connectionString);
               })).Value;
        }
        /// <summary>
        /// 获取数据库的bucket
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dataBase"></param>
        /// <returns></returns>
        public static GridFSBucket GetGridFSBucket(string connectionString, string dataBase)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("MongoDB Connection String is Empty");
            if (string.IsNullOrWhiteSpace(dataBase)) throw new ArgumentException("MongoDB DataBase String is Empty");

            return GridFSBucketCache.GetOrAdd(dataBase, new Lazy<GridFSBucket>(()=> 
            {
                return new GridFSBucket(GetMongodbClient(connectionString).GetDatabase(dataBase));
            })).Value;
        }
        /// <summary>
        /// 获取数据库的bucket
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static GridFSBucket GetGridFSBucket(MongodbHost host)
        {
            if (string.IsNullOrWhiteSpace(host.Connection)) throw new ArgumentException("MongoDB Connection String is Empty");
            if (string.IsNullOrWhiteSpace(host.DataBase)) throw new ArgumentException("MongoDB DataBase is Empty");

            return GridFSBucketCache.GetOrAdd(host.DataBase, new Lazy<GridFSBucket>(() =>
            {
                return new GridFSBucket(GetMongodbClient(host.Connection).GetDatabase(host.DataBase));
            })).Value;
        }
        /// <summary>
        /// 获取mongo数据库实例
        /// </summary>
        /// <param name="connectionString">连接字符串，格式：mongodb://ip:port</param>
        /// <param name="dataBase">数据库名称</param>
        /// <returns></returns>
        public static IMongoDatabase GetMongoDatabase(string connectionString, string dataBase)
        {
            IMongoClient client = GetMongodbClient(connectionString);
            return client.GetDatabase(dataBase);
        }
        /// <summary>
        /// 获取mongo 数据库实例
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static IMongoDatabase GetMongoDatabase(MongodbHost host)
        {
            IMongoClient client = GetMongodbClient(host.Connection);
            return client.GetDatabase(host.DataBase);
        }
        /// <summary>
        /// 获取mongo集合类
        /// </summary>
        /// <param name="host">mongo服务器信息</param>
        /// <returns></returns>
        public static IMongoCollection<T> GetMongodbCollection(MongodbHost host)
        {
            IMongoClient client = GetMongodbClient((host.Connection));
            var mongoDataBase = client.GetDatabase(host.DataBase);
            return mongoDataBase.GetCollection<T>(typeof(T).Name);
        }
        /// <summary>
        /// 获取mongo集合类
        /// </summary>
        /// <param name="connectionString">连接字符串，格式：mongodb://ip:port</param>
        /// <param name="dataBase">数据库名称</param>
        /// <param name="table">表名称</param>
        /// <returns></returns>
        public static IMongoCollection<T> GetMongodbCollection(string connectionString, string dataBase, string table)
        {
            IMongoClient client = GetMongodbClient((connectionString));
            var mongoDataBase = client.GetDatabase(dataBase);
            return mongoDataBase.GetCollection<T>(table);
        }
        /// <summary>
        /// 获取mongo集合类，使用T的名称作为mongo的表名
        /// </summary>
        /// <param name="connectionString">连接字符串，格式：mongodb://ip:port</param>
        /// <param name="dataBase">数据库名称</param>
        /// <returns></returns>
        public static IMongoCollection<T> GetMongodbCollection(string connectionString, string dataBase)
        {
            IMongoClient client = GetMongodbClient((connectionString));
            var mongoDataBase = client.GetDatabase(dataBase);
            return mongoDataBase.GetCollection<T>(typeof(T).FullName);
        }
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
    }
}
