﻿using Dist.Dme.DisCache.Interfaces;
using Dist.Dme.DisFS.Adapters.Mongo;
using Dist.Dme.HSMessage.Conf;
using Dist.Dme.HSMessage.MQ.Kafka;
using Dist.Dme.HSMessage.Websocket.Fleck;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Extensions
{
    /// <summary>
    /// 服务工厂，存放Startup上已注册的服务
    /// </summary>
    public class ServiceFactory
    {
        /// <summary>
        /// mongo的连接信息
        /// </summary>
        public static MongodbHost MongoHost { get; set; }
        /// <summary>
        /// mongo客户端类
        /// </summary>
        public static IMongoClient MongoClient { get; set; }
        /// <summary>
        /// mongo数据库实例
        /// </summary>
        public static IMongoDatabase MongoDatabase { get; set; }
        /// <summary>
        /// 缓存服务
        /// </summary>
        public static ICacheService CacheService { get; set; }
        /// <summary>
        /// 消息设置
        /// </summary>
        public static KafkaSetting HSMessageSetting { get; set; }
        /// <summary>
        /// 消息消费方
        /// </summary>
        public static KafkaConsumer KafkaConsumer { get; set; }
        /// <summary>
        /// 消息生产方
        /// </summary>
        public static KafkaProducer KafkaProducer { get; set; }
        /// <summary>
        /// websocket服务器
        /// </summary>
        public static WebsocketFleckServer WebsocketServer { get; set; }

    }
}
