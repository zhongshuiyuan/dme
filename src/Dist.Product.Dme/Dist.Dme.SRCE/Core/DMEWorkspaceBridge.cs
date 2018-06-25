using Dist.Dme.Base.Common;
using Dist.Dme.Base.ConnectionInfo;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Base.Utils;
using Dist.Dme.Model.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Dist.Dme.SRCE.Core
{
    /// <summary>
    /// 工作空间桥接层
    /// </summary>
    /// <typeparam name="TWorkspace"></typeparam>
    public class DMEWorkspaceBridge<TWorkspace, TFeatureClass> 
        where TWorkspace : class
        where TFeatureClass : class
    {
        /// <summary>
        /// 缓存，key为连接的MD5哈希
        /// </summary>
        static IDictionary<string, TWorkspace> TWorkspaceDic = new Dictionary<string, TWorkspace>();

        protected DMEWorkspace<TWorkspace, TFeatureClass> _dmeWorkspace;
        public void SetWorkspace(DMEWorkspace<TWorkspace, TFeatureClass> dmeWorkspace)
        {
            this._dmeWorkspace = dmeWorkspace;
        }
        /// <summary>
        /// 打开oracle远程工作空间
        /// </summary>
        /// <param name="conn">连接信息</param>
        /// <returns></returns>
        public virtual TWorkspace OpenWorkspace(OracleConn conn)
        {
            string md5Hash = MD5HashUtil.GetMD5Hash(JsonConvert.SerializeObject(conn));
            if (TWorkspaceDic.ContainsKey(md5Hash))
            {
                return TWorkspaceDic[md5Hash];
            }
           
            TWorkspace workspace = this._dmeWorkspace.Open(conn);
            TWorkspaceDic[md5Hash] = workspace;
            return workspace;
        }
        /// <summary>
        /// 打开本地文件工作空间
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public virtual TWorkspace OpenWorkspace(LocalConn conn)
        {
            string md5Hash = MD5HashUtil.GetMD5Hash(JsonConvert.SerializeObject(conn));
            if (TWorkspaceDic.ContainsKey(md5Hash))
            {
                return TWorkspaceDic[md5Hash];
            }
            TWorkspace workspace = this._dmeWorkspace.Open(conn);
            TWorkspaceDic[md5Hash] = workspace;
            return workspace;
        }
        /// <summary>
        /// 获取要素类对象
        /// </summary>
        /// <param name="workspace">工作空间</param>
        /// <param name="featureClassName">要素类名称</param>
        /// <returns></returns>
        public virtual TFeatureClass GetFeatureClass(TWorkspace workspace, string featureClassName)
        {
            return this._dmeWorkspace.GetFeatureClass(workspace, featureClassName);
        }
        /// <summary>
        /// 获取要素类对象
        /// </summary>
        /// <param name="dto">参数模型</param>
        /// <returns></returns>
        public virtual TFeatureClass GetFeatureClass(InputFeatureClassDTO dto)
        {
            TWorkspace workspace;
            if (GeoDatasourceType.ENTERPRISE_GEODATABASE == EnumUtil.GetEnumObjByName<GeoDatasourceType>(dto.Source.Type))
            {
                // 企业级数据库连接
                OracleConn conn = JsonConvert.DeserializeObject<OracleConn>(dto.Source.Connection);
                workspace = this.OpenWorkspace(conn);
            }
            else
            {
                LocalConn conn = JsonConvert.DeserializeObject<LocalConn>(dto.Source.Connection);
                conn.Type = EnumUtil.GetEnumObjByName<GeoDatasourceType>(dto.Source.Type);
                workspace = this.OpenWorkspace(conn);
            }
            if (null == workspace)
            {
                throw new BusinessException(SystemStatusCode.DME_ERROR, "获取源要素类工作空间失败");
            }
            TFeatureClass featureClass = this.GetFeatureClass(workspace, dto.Name);
            if (null == featureClass)
            {
                throw new BusinessException(SystemStatusCode.DME_ERROR, "获取源要素类对象失败");
            }
            return featureClass;
        }
    }
}
