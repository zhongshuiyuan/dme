using Dist.Dme.Base.Conf;
using Dist.Dme.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.DAL.Context
{
    /// <summary>
    /// 业务上下文
    /// </summary>
    public abstract class AbstractContext : ContextBase
    {
        /// <summary>
        /// oracle 类型
        /// </summary>
        public AbstractContext() : base(DbType.Oracle, SysConfig.DBConnectionString, true)
        {
          
        }
        /// <summary>
        /// 用来处理DmeModel表的常用操作
        /// </summary>
        public MySimpleClient<DmeModel> DmeModelDb { get { return new MySimpleClient<DmeModel>(base.GetDbContext()); } }
        /// <summary>
        /// 用来处理DmeAlgorithm表的常用操作
        /// </summary>
        public MySimpleClient<DmeAlgorithm> DmeAlgorithmDb { get { return new MySimpleClient<DmeAlgorithm>(base.GetDbContext()); } }
        /// <summary>
        /// 用来处理DmeDatabaseType表的常用操作
        /// </summary>
        public MySimpleClient<DmeDatabaseType> DmeDatabaseTypeDb { get { return new MySimpleClient<DmeDatabaseType>(base.GetDbContext()); } }
        /// <summary>
        /// 用来处理DmeDataSource表的常用操作
        /// </summary>
        public MySimpleClient<DmeDataSource> DmeDataSourceDb { get { return new MySimpleClient<DmeDataSource>(base.GetDbContext()); } }
        /// <summary>
        /// 用来处理DmeJob表的常用操作
        /// </summary>
        public MySimpleClient<DmeJob> DmeJobDb { get { return new MySimpleClient<DmeJob>(base.GetDbContext()); } }
        /// <summary>
        /// 用来处理DmeModelVersion表的常用操作
        /// </summary>
        public MySimpleClient<DmeModelVersion> DmeModelVersionDb { get { return new MySimpleClient<DmeModelVersion>(base.GetDbContext()); } }
        /// <summary>
        /// 用来处理DmeVersion表的常用操作
        /// </summary>
        public MySimpleClient<DmeVersion> DmeVersionDb { get { return new MySimpleClient<DmeVersion>(base.GetDbContext()); } }
    }
}
