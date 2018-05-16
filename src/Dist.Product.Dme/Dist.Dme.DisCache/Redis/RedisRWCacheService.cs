using Dist.Dme.DisCache.Interfaces;
using log4net;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dist.Dme.DisCache.Redis
{
    /// <summary>
    /// redis读写分离服务
    /// </summary>
    public class RedisRWCacheService : RedisBase, ICacheService
    {
        /// <summary>  
        /// 针对Log4net的实例  
        /// </summary>  
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RedisRWCacheService));
        /// <summary>  
        /// The seconds time out.  
        /// 默认缓存过期时间单位秒  
        /// </summary>  
        private int secondsTimeOut = 24 * 60 * 60;

        public RedisRWCacheService(IRedisClient redisClient)
            : base(redisClient)
        {
        }
        /// <summary>  
        /// 给某个键对应的数据设置过期时间  
        /// </summary>  
        /// <param name="key">键</param>  
        /// <param name="seconds">过期时间</param>  
        public void Expire(string key, int seconds)
        {
            try
            {
                var dtTimeOut = DateTime.Now.AddSeconds(seconds);
                base.redisClient.ExpireEntryAt(key, dtTimeOut);
            }
            catch (Exception ex)
            {
                var message = string.Format("设置过期时间出错");
                Logger.Error(message, ex);
            }
        }
        /// <summary>  
        /// 设置单个实体  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="key">缓存建</param>  
        /// <param name="t">缓存值</param>  
        /// <param name="timeout">过期时间，单位秒,-1：不过期，0：默认过期时间：一天</param>  
        public bool Set<T>(string key, T t, int timeout = -1)
        {
            try
            {
                if (timeout >= 0)
                {
                    if (timeout > 0)
                    {
                        this.secondsTimeOut = timeout;
                    }

                    var dtTimeOut = DateTime.Now.AddSeconds(this.secondsTimeOut);
                    return base.redisClient.Set(key, t, dtTimeOut);
                }

                return base.redisClient.Set(key, t);
            }
            catch (Exception ex)
            {
                string message = string.Format("设置Redis缓存出错");
                Logger.Error(message, ex);
                return false;
            }
        }

        /// <summary>  
        ///     获取单个实体  
        /// </summary>  
        /// <typeparam name="T">对象类型</typeparam>  
        /// <param name="key">键值</param> 
        public T Get<T>(string key) where T : class
        {
            try
            {
                return base.redisClient.Get<T>(key);
            }
            catch (Exception ex)
            {
                string message = string.Format("获取Redis缓存出错");
                Logger.Error(message, ex);
                return default(T);
            }
        }

        /// <summary>  
        ///     删除  
        /// </summary>  
        /// <param name="key">键值</param>  
        public bool Remove(string key)
        {
            try
            {
                return base.redisClient.Remove(key);
            }
            catch (Exception ex)
            {
                string message = string.Format("删除Redis缓存出错");
                Logger.Error(message, ex);
                return false;
            }
        }

        /// <summary>  
        /// 删除所有  
        /// </summary>  
        public void RemoveAll()
        {
            // var keyList = base.redisClient.GetAllKeys();
            base.redisClient.FlushDb();//.RemoveAll(keyList);
        }

        /// <summary>  
        /// 获取Redis的所有key  
        /// </summary>  
        public List<string> ListKey()
        {
            return base.redisClient.GetAllKeys();
        }

        /// <summary>  
        ///     添加一个对象  
        /// </summary>  
        /// <typeparam name="T">对象类型</typeparam>  
        /// <param name="key">键</param>  
        /// <param name="t"></param>  
        /// <param name="timeout">过期时间(单位为秒) -1：不过期，0：默认过期时间 一天；具体有效秒</param>  
        public bool Add<T>(string key, T t, int timeout = -1)
        {
            try
            {
                if (timeout >= 0)
                {
                    if (timeout > 0)
                    {
                        this.secondsTimeOut = timeout;
                    }
                    var dtTimeOut = DateTime.Now.AddSeconds(this.secondsTimeOut);
                    base.redisClient.ExpireEntryAt(key, dtTimeOut);
                }

                return base.redisClient.Add(key, t);
            }
            catch (Exception ex)
            {
                string message = string.Format("添加Redis缓存出错");
                Logger.Error(message, ex);
                return false;
            }
        }

        /// <summary>  
        /// 根据IEnumerable数据添加链表  
        /// </summary>  
        /// <typeparam name="T">对象类型</typeparam>  
        /// <param name="key">键</param>  
        /// <param name="values">值</param>  
        /// <param name="timeout">过期时间（秒），-1：不过期；0：默认过期时间:一天；有效具体秒数</param>  
        public void AddList<T>(string key, IEnumerable<T> values, int timeout = -1)
        {
            try
            {
                IRedisTypedClient<T> iredisClient = base.redisClient.As<T>();
                IRedisList<T> redisList = iredisClient.Lists[key];
                redisList.AddRange(values);
                if (timeout > 0)
                {
                    if (timeout > 0)
                    {
                        this.secondsTimeOut = timeout;
                    }
                    var dtTimeOut = DateTime.Now.AddSeconds(this.secondsTimeOut);
                    base.redisClient.ExpireEntryAt(key, dtTimeOut);
                }

                iredisClient.Save();
            }
            catch (Exception ex)
            {
                string message = string.Format("添加链表出错");
                Logger.Error(message, ex);
            }
        }

        /// <summary>  
        /// 添加单个实体到链表中  
        /// </summary>  
        /// <typeparam name="T">对象类型</typeparam>  
        /// <param name="key">键</param>  
        /// <param name="Item"></param>  
        /// <param name="timeout">过期时间 -1：不过期，0：默认过期时间：一天</param>  
        public void AddEntityToList<T>(string key, T Item, int timeout = -1)
        {
            try
            {
                IRedisTypedClient<T> iredisClient = base.redisClient.As<T>();
                IRedisList<T> redisList = iredisClient.Lists[key];
                redisList.Add(Item);
                iredisClient.Save();
                if (timeout >= 0)
                {
                    if (timeout > 0)
                    {
                        this.secondsTimeOut = timeout;
                    }
                    var dtTimeOut = DateTime.Now.AddSeconds(this.secondsTimeOut);
                    base.redisClient.ExpireEntryAt(key, dtTimeOut);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("添加单个的实体到链表中出错");
                Logger.Error(message, ex);
            }
        }

        /// <summary>  
        /// 获取链表  
        /// </summary>  
        /// <typeparam name="T">对象类型</typeparam>  
        /// <param name="key">键</param>  
        public IEnumerable<T> GetList<T>(string key)
        {
            try
            {
                IRedisTypedClient<T> iredisClient = base.redisClient.As<T>();
                return iredisClient.Lists[key];
            }
            catch (Exception ex)
            {
                string message = string.Format("获取链表出错");
                Logger.Error(message, ex);
                return null;
            }
        }

        /// <summary>  
        /// 在链表中删除单个实体  
        /// </summary>  
        /// <typeparam name="T">对象类型</typeparam>  
        /// <param name="key">键</param>  
        public void RemoveEntityFromList<T>(string key, T t)
        {
            try
            {
                IRedisTypedClient<T> iredisClient = base.redisClient.As<T>();
                IRedisList<T> redisList = iredisClient.Lists[key];
                redisList.RemoveValue(t);
                iredisClient.Save();
            }
            catch (Exception ex)
            {
                string message = string.Format("删除链表中的单个实体出错");
                Logger.Error(message, ex);
            }
        }

        /// <summary>  
        /// 根据key移除整个链表  
        /// </summary>  
        /// <param name="key">键</param>  
        public void RemoveAllList<T>(string key)
        {
            try
            {
                IRedisTypedClient<T> iredisClient = base.redisClient.As<T>();
                IRedisList<T> redisList = iredisClient.Lists[key];
                redisList.RemoveAll();
                iredisClient.Save();
            }
            catch (Exception ex)
            {
                string message = string.Format("删除链表集合");
                Logger.Error(message, ex);
            }
        }

        public bool Exists(string key)
        {
            return base.redisClient.ContainsKey(key);
        }

        public Task<bool> ExistsAsync(string key)
        {
            throw new NotImplementedException();
        }

        public bool Add(string key, object value)
        {
            return this.Set(key, value);
        }

        public Task<bool> AddAsync(string key, object value)
        {
            throw new NotImplementedException();
        }

        public bool Add(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            try
            {
                base.redisClient.Add(key, value);
                base.redisClient.ExpireEntryIn(key, expiressAbsoulte);
                return true;
            }
            catch (Exception ex)
            {
                string message = string.Format("添加链表出错");
                Logger.Error(message, ex);
                return false;
            }
        }

        public Task<bool> AddAsync(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            throw new NotImplementedException();
        }

        public bool Add(string key, object value, TimeSpan expiresIn, bool isSliding = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddAsync(string key, object value, TimeSpan expiresIn, bool isSliding = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAsync(string key)
        {
            throw new NotImplementedException();
        }

        public void RemoveAll(IEnumerable<string> keys)
        {
            base.redisClient.RemoveAll(keys);
        }

        public Task RemoveAllAsync(IEnumerable<string> keys)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAsync<T>(string key) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<object> GetAsync(string key)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, object> GetAll(IEnumerable<string> keys)
        {
            return base.redisClient.GetAll<object>(keys);
        }

        public Task<IDictionary<string, object>> GetAllAsync(IEnumerable<string> keys)
        {
            throw new NotImplementedException();
        }

        public bool Replace(string key, object value)
        {
            return this.Set(key, value);
        }

        public Task<bool> ReplaceAsync(string key, object value)
        {
            throw new NotImplementedException();
        }

        public bool Replace(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            base.redisClient.Set(key, value);
            base.redisClient.ExpireEntryIn(key, expiressAbsoulte);
            return true;
        }

        public Task<bool> ReplaceAsync(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            throw new NotImplementedException();
        }

        public bool Replace(string key, object value, TimeSpan expiresIn, bool isSliding = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ReplaceAsync(string key, object value, TimeSpan expiresIn, bool isSliding = false)
        {
            throw new NotImplementedException();
        }
    }
}
