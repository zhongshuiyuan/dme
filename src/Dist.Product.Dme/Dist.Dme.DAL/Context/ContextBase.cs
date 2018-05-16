﻿using Dist.Dme.Base.Conf;
using Dist.Dme.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Dist.Dme.DAL.Context
{
    using log4net;
    using SqlSugar;

    public class ContextBase : IDbContext
    {
        private static ILog LOG = LogManager.GetLogger(typeof(ContextBase));
        protected SqlSugarClient Db;

        public ContextBase(DbType dbType, String connectionConfig, bool IsAutoCloseConnection)
        {
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = connectionConfig,
                DbType = dbType,
                IsAutoCloseConnection = IsAutoCloseConnection,
                //设为true，表示相同线程是同一个SqlSugarClient
                IsShardSameThread = true 
            });
            Db.Ado.IsEnableLogEvent = true;
            Db.Ado.LogEventStarting = (sql, pars) =>
            {
                Console.WriteLine(sql + "\r\n" + Db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                Console.WriteLine();
            };
            // AOP LOG
            Db.Aop.OnLogExecuting = (sql, pars) => //SQL executing event (pre-execution)
            {
            
            };
            Db.Aop.OnLogExecuted = (sql, pars) => //SQL executed event
            {
             
            };
            Db.Aop.OnError = (exp) =>//SQL execution error event
            {
              
            };
            Db.Aop.OnExecutingChangeSql = (sql, pars) => //SQL executing event (pre-execution,SQL script can be modified)
            {
                return new KeyValuePair<string, SugarParameter[]>(sql, pars);
            };

        }
        /// <summary>
        /// 开启事务
        /// </summary>
        public void BeginTran()
        {
            Db.Ado.BeginTran();
        }
        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTran()
        {
            Db.Ado.CommitTran();
        }
        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTran()
        {
            Db.Ado.RollbackTran();
        }
        // <summary>
        /// 初始化ORM连接对象
        /// </summary>
        /// <param name="commandTimeOut">等待超时时间, 默认为30秒 (单位: 秒)</param>
        /// <param name="dbType">数据库类型, 默认为Oracle</param>
        /// <param name="isAutoCloseConnection">是否自动关闭数据库连接, 默认不是, 如果设置为true, 则会在每次操作完数据库后, 即时关闭, 如果一个方法里面多次操作了数据库, 建议保持为false, 否则可能会引发性能问题</param>
        public SqlSugarClient InitDB(int commandTimeOut = 30, DbType dbType = DbType.Oracle, bool isAutoCloseConnection = false)
        {
            var db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = SysConfig.DBConnectionString,
                DbType = dbType,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = isAutoCloseConnection
            });
            db.Ado.CommandTimeOut = commandTimeOut;
            return db;
        }
        /// <summary>
        /// 执行数据库操作
        /// </summary>
        /// <typeparam name="Result">返回值类型 泛型</typeparam>
        /// <param name="func">方法委托</param>
        /// <param name="commandTimeOut">超时时间, 单位为秒, 默认为30秒</param>
        /// <param name="dbType">数据库类型, 默认为Oracle</param>
        /// <returns>泛型返回值</returns>
        public Result Exec<Result>(Func<SqlSugarClient, Result> func, int commandTimeOut = 30, DbType dbType = DbType.Oracle)
        {
            if (func == null) throw new Exception("委托为null, 停止执行");

            using (var db = InitDB(commandTimeOut, dbType))
            {
                try
                {
                    return func(db);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    db.Dispose();
                }
            }
        }
        /// <summary>
        /// 带事务处理的执行数据库操作
        /// </summary>
        /// <typeparam name="Result">返回值类型 泛型</typeparam>
        /// <param name="func">方法委托</param>
        /// <param name="commandTimeOut">超时时间, 单位为秒, 默认为30秒</param>
        /// <param name="dbType">数据库类型, 默认为Oracle</param>
        /// <returns>泛型返回值</returns>
        public Result ExecTran<Result>(Func<SqlSugarClient, Result> func, int commandTimeOut = 30, DbType dbType = DbType.Oracle)
        {
            if (func == null) throw new Exception("委托为null, 事务处理停止执行");
            using (var db = InitDB(commandTimeOut, dbType))
            {
                try
                {
                    db.Ado.BeginTran(IsolationLevel.Unspecified);
                    var result = func(db);
                    db.Ado.CommitTran();
                    return result;
                }
                catch (Exception ex)
                {
                    db.Ado.RollbackTran();
                    throw ex;
                }
                finally
                {
                    db.Dispose();
                }
            }
        }
        public SqlSugarClient GetDbContext()
        {
            return Db;
        }
    }
}
