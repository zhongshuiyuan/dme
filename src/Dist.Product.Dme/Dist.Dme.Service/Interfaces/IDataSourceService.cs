using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Service.Interfaces
{
    /// <summary>
    /// 数据源服务
    /// </summary>
    public interface IDataSourceService : IBizService
    {
        /// <summary>
        /// 获取所有数据库类型
        /// </summary>
        /// <returns></returns>
        List<DmeDatabaseType> ListDatabaseTypes();
        /// <summary>
        /// 获取单个数据库类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DmeDatabaseType GetDatabaseType(int id);
    }
}
