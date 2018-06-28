using Dist.Dme.Base.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.DataSource
{
    /// <summary>
    /// 抽象类，用于抽象工厂公共部分
    /// </summary>
    public abstract class DMEDataSourceFactory : IDMEDataSourceFactory
    {
        public abstract IDMEDataSource Open(IPropertySetter properties, Boolean checkMetaInvalid = false);
        public abstract IDMEDataSource OpenFromConnectionStr(string connectionStr, Boolean checkMetaInvalid = false);
    }
}
