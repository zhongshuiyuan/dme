using Dist.Dme.Base.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.DataSource
{
    public abstract class BaseDMEDataSource : IDMEDataSource
    {
        /// <summary>
        /// 是否检测连接元数据的有效性
        /// </summary>
        private Boolean _checkMetaInvalid = false;
        protected string _connectionStr = "";
        protected IPropertySetter _propertySetter = null;
        protected IDMEDataSourceFactory _dataSourceFactory = null;
        /// <summary>
        /// 检测参数环境
        /// </summary>
        /// <returns></returns>
        protected abstract Boolean Check(IPropertySetter properties);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <returns></returns>
        protected abstract Boolean Check(string connectionStr);
        /// <summary>
        /// 验证连接有效性
        /// </summary>
        /// <returns></returns>
        public abstract bool ValidConnection();

        /// <summary>
        /// 获取连接的JSON
        /// </summary>
        public abstract object ConnectionMeta { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="properties"></param>
        /// <param name="checkMetaInvalid">是否检测连接元数据的有效性</param>
        public BaseDMEDataSource(IDMEDataSourceFactory factory, IPropertySetter properties, Boolean checkMetaInvalid = false)
        {
            this._propertySetter = properties;
            this._dataSourceFactory = factory;
            this._checkMetaInvalid = checkMetaInvalid;
        }
        /// <summary>
        /// 是否检测连接元数据的有效性
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="connectionStr"></param>
        /// <param name="checkMetaInvalid"></param>
        public BaseDMEDataSource(IDMEDataSourceFactory factory, string connectionStr, Boolean checkMetaInvalid = false)
        {
            this._connectionStr = connectionStr;
            this._dataSourceFactory = factory;
            this._checkMetaInvalid = checkMetaInvalid;
        }

        public IPropertySetter ConnectionProperties
        {
            get
            {
                return this._propertySetter;
            }
        }

        public IDMEDataSourceFactory DMEDataSourceFactory
        {
            get
            {
                return this._dataSourceFactory;
            }
        }

    }
}
