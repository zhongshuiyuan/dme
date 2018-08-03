using Dist.Dme.Base.Common.Log;
using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.Model.Entity;
using Dist.Dme.Service.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dist.Dme.Service.Impls
{
    public class LogService : BaseBizService, ILogService
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        public LogService(IRepository repository)
        {
            base.Repository = repository;
            this.LogService = this;
        }

        public async void AddLogAsync(EnumLogType logType, EnumLogLevel logLevel, string objectType, string objectId, string remark = "", Exception ex = null, string userCode = "", string address = "", string apps = "DME")
        {
            await Task.Run(() =>
            {
                DmeLog log = new DmeLog
                {
                    LogType = EnumUtil.GetEnumName<EnumLogType>((int)logType),
                    LogLevel = EnumUtil.GetEnumName<EnumLogLevel>((int)logLevel),
                    ObjectType = objectType,
                    ObjectId = objectId,
                    UserCode = userCode,
                    Address = address,
                    Apps = apps,
                    Remark = remark,
                    CreateTime = DateUtil.CurrentTimeMillis
                };
                if (String.IsNullOrEmpty(remark) && ex != null)
                {
                    // 把异常信息赋予remark
                    if (ex is BusinessException)
                    {
                        log.Remark = $"Code:{((BusinessException)ex).Code}, Message:{ex.Message}";
                    }
                    else
                    {
                        log.Remark = ex.StackTrace + "\\r\\n" +ex.Message;
                    }
                }
                base.Repository.GetDbContext().Insertable<DmeLog>(log).ExecuteCommandAsync();
            });
        }
    }
}
