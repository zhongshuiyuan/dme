using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Model.DTO;
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
        /// <summary>
        /// 添加数据源实例
        /// </summary>
        /// <param name="datasourceAddDTO">数据源信息</param>
        /// <returns></returns>
        object AddDataSource(DatasourceAddDTO datasourceAddDTO);
    }
}
