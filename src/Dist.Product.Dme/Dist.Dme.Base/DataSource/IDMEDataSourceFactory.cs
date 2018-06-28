using Dist.Dme.Base.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.DataSource
{
    public interface IDMEDataSourceFactory
    {
        /// <summary>
        /// 注入属性注册器打开
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="checkMetaValid">是否检测连接元数据有效性</param>
        /// <returns></returns>
        IDMEDataSource Open(IPropertySetter properties, Boolean checkMetaValid = false);
        /// <summary>
        /// 通过连接字符串打开，json结构
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="checkMetaValid">是否检测连接元数据有效性</param>
        /// <returns></returns>
        IDMEDataSource OpenFromConnectionStr(string connectionStr, Boolean checkMetaValid = false);
    }
}
