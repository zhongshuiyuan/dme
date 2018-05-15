using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.DisCache.Redis
{
    /// <summary>
    /// RedisBase类，是redis操作的基类，继承自IDisposable接口，主要用于释放内存
    /// </summary>
    public abstract class RedisBase : IDisposable
    {
        protected readonly IRedisClient redisClient;
        private bool _disposed = false;
        public RedisBase(IRedisClient redisClient)
        {
            this.redisClient = redisClient;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    redisClient.Dispose();
                }
            }
            this._disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// 保存数据DB文件到硬盘
        /// </summary>
        public void Save()
        {
            redisClient.Save();
        }
        /// <summary>
        /// 异步保存数据DB文件到硬盘
        /// </summary>
        public void SaveAsync()
        {
            redisClient.SaveAsync();
        }
    }
}
