using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Service.Interfaces
{
    /// <summary>
    /// 存储服务，不单单是数据库，还有mongodb
    /// </summary>
    public interface IStoreService
    {
        /// <summary>
        /// 获取mongo的数据库集合
        /// </summary>
        /// <param name="datasourceCode">数据源编码</param>
        /// <returns></returns>
        IList<string> ListMongoDataBase(string datasourceCode);
      
        /// <summary>
        /// 获取mongo的数据库集合
        /// </summary>
        /// <param name="host">服务器地址</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        IList<string> ListMongoDataBase(string host, int port);
        /// <summary>
        /// 获取mongo指定数据库下的集合类
        /// </summary>
        /// <param name="datasourceCode">数据源编码</param>
        /// <returns></returns>
        IList<string> ListMongoCollection(string datasourceCode);
        /// <summary>
        /// 获取mongo指定数据库下的集合类
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="dataBase"></param>
        /// <returns></returns>
        IList<string> ListMongoCollection(string host, int port, string dataBase);
    }
}
