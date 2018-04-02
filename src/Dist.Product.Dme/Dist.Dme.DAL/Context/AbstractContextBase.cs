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
        public AbstractContextBase() : base(DbType.Oracle, Config.ConnectionString)
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
