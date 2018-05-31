using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework.Interfaces
{
    /// <summary>
    /// 存储库
    /// </summary>
    public interface IRepository
    {
        SqlSugarClient GetDbContext();
    }
}
