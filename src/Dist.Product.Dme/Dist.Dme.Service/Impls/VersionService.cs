using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.Model.DTO;
using Dist.Dme.Model.Entity;
using Dist.Dme.Service.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Service.Impls
{
    /// <summary>
    /// 版本管理服务
    /// </summary>
    public class VersionService : BaseBizService, IVersionService
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        public VersionService(IRepository repository)
        {
            base.Repository = repository;
        }
        public DmeVersion GetCurrentVersion()
        {
            return base.Repository.GetDbContext().Queryable<DmeVersion>().First();
        }
        public DmeVersion UpdateVersion(VersionDTO dto)
        {
            DmeVersion currentVersion = this.GetCurrentVersion();
            if (null == currentVersion)
            {
                currentVersion = new DmeVersion()
                {
                    MajorVersion = dto.MajorVersion,
                    MinorVersion = dto.MinorVersion,
                    RevisionVersion = dto.RevisionVersion,
                    UpgradeTime = DateUtil.CurrentTimeMillis
                };
                currentVersion = base.Db.Insertable<DmeVersion>(currentVersion).ExecuteReturnEntity();
            }
            else
            {
                currentVersion.MajorVersion = dto.MajorVersion;
                currentVersion.MinorVersion = dto.MinorVersion;
                currentVersion.RevisionVersion = dto.RevisionVersion;
                currentVersion.UpgradeTime = DateUtil.CurrentTimeMillis;
                base.Db.Updateable<DmeVersion>(currentVersion).ExecuteCommand();
            }
            return currentVersion;
        }
    }
}
