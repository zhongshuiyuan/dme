using Dist.Dme.Base.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.DataSource.MongoDB
{
    public class DMEMongoFactory : BaseDMEDataSourceFactory, IDMEDataSourceFactory
    {
        public override IDMEDataSource Open(IPropertySetter properties, Boolean checkMetaValid = false)
        {
            return new DMEMongoDS(this, properties, checkMetaValid);
        }

        public override IDMEDataSource OpenFromConnectionStr(string connectionStr, Boolean checkMetaValid = false)
        {
            return new DMEMongoDS(this, connectionStr, checkMetaValid);
        }
    }
}
