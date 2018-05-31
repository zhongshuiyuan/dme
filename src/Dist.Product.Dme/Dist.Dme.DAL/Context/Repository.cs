using Dist.Dme.Base.Conf;
using Dist.Dme.Model.Entity;
using log4net;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.DAL.Context
{
    /// <summary>
    /// 业务上下文
    /// </summary>
    public class Repository : RepositoryBase
    {
        private static ILog LOG = LogManager.GetLogger(typeof(Repository));
        /// <summary>
        /// oracle 类型
        /// </summary>
        public Repository() : base(DbType.Oracle, SysConfig.DBConnectionString, true)
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
        /// 用来处理DmeAlgorithm元数据表的常用操作
        /// </summary>
        public MySimpleClient<DmeAlgorithmMeta> DmeAlgorithmMetaDb { get { return new MySimpleClient<DmeAlgorithmMeta>(base.GetDbContext()); } }
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
        public MySimpleClient<DmeTask> DmeJobDb { get { return new MySimpleClient<DmeTask>(base.GetDbContext()); } }
        /// <summary>
        /// 用来处理DmeModelVersion表的常用操作
        /// </summary>
        public MySimpleClient<DmeModelVersion> DmeModelVersionDb { get { return new MySimpleClient<DmeModelVersion>(base.GetDbContext()); } }
        /// <summary>
        /// 用来处理DmeVersion表的常用操作
        /// </summary>
        public MySimpleClient<DmeVersion> DmeVersionDb { get { return new MySimpleClient<DmeVersion>(base.GetDbContext()); } }
        /// <summary>
        /// 用来处理DmeUser表的常用操作
        /// </summary>
        public MySimpleClient<DmeUser> DmeUserDb { get { return new MySimpleClient<DmeUser>(base.GetDbContext()); } }
        /// <summary>
        /// 规则步骤关联
        /// </summary>
        public MySimpleClient<DmeRuleStep> DmeRuleStepDb { get { return new MySimpleClient<DmeRuleStep>(base.GetDbContext()); } }
        /// <summary>
        /// 规则步骤类型
        /// </summary>
        public MySimpleClient<DmeRuleStepType> DmeRuleStepTypeDb { get { return new MySimpleClient<DmeRuleStepType>(base.GetDbContext()); } }
        /// <summary>
        /// 规则步骤属性
        /// </summary>
        public MySimpleClient<DmeRuleStepAttribute> DmeRuleStepAttributeDb { get { return new MySimpleClient<DmeRuleStepAttribute>(base.GetDbContext()); } }

        
    }
}
