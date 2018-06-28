using Dist.Dme.Base.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.DataSource
{
    public interface IDMEDataSource
    {
        IPropertySetter ConnectionProperties { get; }
        string ConnectionMeta { get; }
        IDMEDataSourceFactory DMEDataSourceFactory { get; }
        Boolean ValidConnection();
    }
}
