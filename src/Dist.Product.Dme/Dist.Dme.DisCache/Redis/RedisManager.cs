using Dist.Dme.DisCache.Define;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dist.Dme.DisCache.Redis
{
    public class RedisManager
    {
        /// <summary>
        /// redis读写分离配置文件信息
        /// </summary>
        private readonly RedisRWConfigInfo redisConfig;

        private static PooledRedisClientManager prcm;

        /// <summary>
        /// 静态构造方法，初始化链接池管理对象
        /// </summary>
        public RedisManager(RedisRWConfigInfo redisConfig)
        {
            this.redisConfig = redisConfig;
            CreateManager();
        }

        /// <summary>
        /// 创建链接池管理对象
        /// </summary>
        private void CreateManager()
        {
            string[] WriteServerConStr = SplitString(redisConfig.WriteServerList, ",");
            string[] ReadServerConStr = SplitString(redisConfig.ReadServerList, ",");
            prcm = new PooledRedisClientManager(ReadServerConStr, WriteServerConStr,
                             new RedisClientManagerConfig
                             {
                                 MaxWritePoolSize = redisConfig.MaxWritePoolSize,
                                 MaxReadPoolSize = redisConfig.MaxReadPoolSize,
                                 AutoStart = redisConfig.AutoStart,
                             });
        }

        private static string[] SplitString(string strSource, string split)
        {
            return strSource.Split(split.ToArray());
        }
        /// <summary>
        /// 客户端缓存操作对象
        /// </summary>
        public IRedisClient GetClient()
        {
            if (prcm == null)
                CreateManager();
            return prcm.GetClient();
        }
    }
}
