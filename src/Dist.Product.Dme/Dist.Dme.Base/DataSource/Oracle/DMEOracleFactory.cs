using System;
using System.Collections.Generic;
using System.Text;
using Dist.Dme.Base.Framework.Interfaces;

namespace Dist.Dme.Base.DataSource.Oracle
{
    public class DMEOracleFactory : BaseDMEDataSourceFactory, IDMEDataSourceFactory
    {
        public override IDMEDataSource Open(IPropertySetter properties, Boolean checkMetaValid = false)
        {
            return new DMEOracleDS(this, properties, checkMetaValid);
        }

        public override IDMEDataSource OpenFromConnectionStr(string connectionStr, Boolean checkMetaValid = false)
        {
            return new DMEOracleDS(this, connectionStr, checkMetaValid);
        }
    }
}
