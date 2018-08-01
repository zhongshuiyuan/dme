using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        /// <param name="connectionString">连接字符串，格式：mongodb://ip:port</param>
        /// <param name="dataBase">数据库实例名称，大小写敏感</param>
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
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        public static GridFSBucket GetGridFSBucket(IMongoDatabase database)
        {
            return GridFSBucketCache.GetOrAdd(database.DatabaseNamespace.DatabaseName, new Lazy<GridFSBucket>(() =>
            {
                return new GridFSBucket(database);
            })).Value;
        }
        /// <summary>
        /// 获取mongo所有database
        /// </summary>
        /// <param name="connectionString">连接字符串，格式：mongodb://ip:port</param>
        /// <returns></returns>
        public static IList<string> ListDataBases(string connectionString)
        {
            IMongoClient mongoClient = GetMongodbClient(connectionString);
            return mongoClient.ListDatabaseNames().ToList();
        }
        /// <summary>
        /// 异步获取mongo所有database
        /// </summary>
        /// <param name="connectionString">连接字符串，格式：mongodb://ip:port</param>
        /// <returns></returns>
        public static Task<List<string>> ListDataBasesAsync(string connectionString)
        {
            IMongoClient mongoClient = GetMongodbClient(connectionString);
            return mongoClient.ListDatabaseNames().ToListAsync();
        }
        /// <summary>
        /// 获取指定database下的所有集合类
        /// </summary>
        /// <param name="connectionString">连接字符串，格式：mongodb://ip:port</param>
        /// <param name="dataBase">数据库实例，大小写敏感</param>
        /// <returns></returns>
        public static IList<string> ListCollections(string connectionString, string dataBase)
        {
            IMongoDatabase mongoDatabase = GetMongoDatabase(connectionString, dataBase);
            return mongoDatabase.ListCollectionNames().ToList();
        }
        /// <summary>
        /// 异步获取指定database下的所有集合类
        /// </summary>
        /// <param name="connectionString">连接字符串，格式：mongodb://ip:port</param>
        /// <param name="dataBase">数据库实例</param>
        /// <returns></returns>
        public static Task<List<string>> ListCollectionsAsync(string connectionString, string dataBase)
        {
            IMongoDatabase mongoDatabase = GetMongoDatabase(connectionString, dataBase);
            return mongoDatabase.ListCollectionNames().ToListAsync();
        }
        /// <summary>
        /// 获取数据库的bucket
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static GridFSBucket GetGridFSBucket(MongodbHost host)
        {
            if (string.IsNullOrWhiteSpace(host.ConnectionString)) throw new ArgumentException("MongoDB Connection String is Empty");
            if (string.IsNullOrWhiteSpace(host.DataBase)) throw new ArgumentException("MongoDB DataBase is Empty");

            return GridFSBucketCache.GetOrAdd(host.DataBase, new Lazy<GridFSBucket>(() =>
            {
                return new GridFSBucket(GetMongodbClient(host.ConnectionString).GetDatabase(host.DataBase));
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
            IMongoClient client = GetMongodbClient(host.ConnectionString);
            return client.GetDatabase(host.DataBase);
        }
        /// <summary>
        /// 获取mongo集合类
        /// </summary>
        /// <param name="host">mongo服务器信息</param>
        /// <returns></returns>
        public static IMongoCollection<T> GetMongodbCollection(MongodbHost host)
        {
            var mongoDataBase = GetMongoDatabase(host);
            if (string.IsNullOrEmpty(host.Collection))
            {
                return mongoDataBase.GetCollection<T>(typeof(T).Name);
            }
            return mongoDataBase.GetCollection<T>(host.Collection);
        }
        /// <summary>
        /// 获取集合类
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        public static IMongoCollection<T> GetMongodbCollection(IMongoDatabase database)
        {
            return database.GetCollection<T>(typeof(T).Name);
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
        /// 获取连接字符串
        /// </summary>
        public string ConnectionString
        {
            get
            {
                string conn = $"mongodb://{Server}:{Port}";
                if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                {
                    // 改变格式，PS：先不考虑密码加密
                    conn = $"mongodb://{UserName}:{Password}@{Server}:{Port}";
                }
                return conn;
            }
        }
        /// <summary>
        /// 服务器
        /// </summary>
        public string Server { get; set; } = "127.0.0.1";
        /// <summary>
        /// 端口，默认27017
        /// </summary>
        public int Port { get; set; } = 27017;
        /// <summary>
        /// 库
        /// </summary>
        public string DataBase { get; set; }
        /// <summary>
        /// 集合类名称
        /// </summary>
        public string Collection { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}
