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
        List<DmeDataSourceType> ListDataSourceTypes();
        /// <summary>
        /// 获取单个数据库类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DmeDataSourceType GetDatabaseType(int id);
        /// <summary>
        /// 添加数据源实例
        /// </summary>
        /// <param name="datasourceAddDTO">数据源信息</param>
        /// <returns></returns>
        object AddDataSource(DatasourceAddDTO datasourceAddDTO);
        /// <summary>
        /// 获取已注册的数据源
        /// </summary>
        /// <returns></returns>
        object ListRegisteredDataSources();
        /// <summary>
        /// 获取数据源连接元数据
        /// </summary>
        /// <param name="typeCode">数据源类型唯一编码</param>
        /// <returns></returns>
        object GetDatasourceConnMeta(string typeCode);
        /// <summary>
        /// 检查连接的有效性
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        object CheckConnectionValid(DataSourceConnDTO dto);
    }
}
