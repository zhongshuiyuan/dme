using Dist.Dme.DAL.Conf;
using Dist.Dme.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.DAL.Context
{
    /// <summary>
    /// 上下文抽象类
    /// </summary>
    public abstract class AbstractContextBase : ContextBase
    {
        /// <summary>
        /// oracle 类型
        /// </summary>
        public AbstractContextBase() : base(DbType.Oracle)
        {
          
        }
    }
}
