using System;
using System.Collections.Generic;
using System.Text;
using Dist.Dme.Base.Common;
using Dist.Dme.Base.DataSource.MongoDB;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Base.Framework.Interfaces;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using SqlSugar;

namespace Dist.Dme.Base.DataSource.MongoDB
{
    /// <summary>
    /// 对oracle资源具体操作的方法
    /// </summary>
    public class DMEMongoDS : BaseDMEDataSource, IDMEMongoDS
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        // private string connection = string.Empty;
        private string server = "127.0.0.1";
        private int port = 27017;
        // private string dataBase = "";
        private string userName = "";
        private int encrypted = 0;
        private string password = "";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="properties">
        /// {
        ///   {
        ///  "server": "192.168.1.166",
        ///  "port":27017,
        ///   "dataBase": "DME",
        ///  "userName":"",
        ///  "encrypted":0,
        ///  "password":""
        /// }
        /// <paramref name="checkMetaValid">是否检测连接元数据的有效性</paramref>
        /// </param>
        public DMEMongoDS(IDMEDataSourceFactory factory , IPropertySetter properties, Boolean checkMetaValid = false) : base(factory, properties, checkMetaValid)
        {
            if (checkMetaValid)
            {
                Check(properties);
            }
        }
        public DMEMongoDS(IDMEDataSourceFactory factory, string connectionStr, Boolean checkMetaValid = false) : base(factory, connectionStr, checkMetaValid)
        {
            if (checkMetaValid)
            {
                Check(connectionStr);
            }
        }
        public override object ConnectionMeta
        {
            get
            {
                IDictionary<string, object> dic = new Dictionary<string, object>
                {
                    { nameof(this.server), server },
                    { nameof(this.port), port },
                    { nameof(this.encrypted), encrypted },
                    { nameof(this.userName), userName },
                    { nameof(this.password), password }
                };
                return dic;
            }
        }

        public override bool ValidConnection()
        {
            // 如果有用户和密码，格式："mongodb://用户名:密码@IP:端口";
            // 如果没有用户和密码，格式： "mongodb://IP:端口";
            try
            {
                if (string.IsNullOrWhiteSpace(this.server)) throw new ArgumentException("MongoDB server  is Empty");
                string connection = $"mongodb://{server}:{port}";
                if (!string.IsNullOrEmpty(this.userName) && !string.IsNullOrEmpty(this.password))
                {
                    // 改变格式，PS：先不考虑密码加密
                    connection = $"mongodb://{userName}:{password}@{server}:{port}";
                }
                MongoClient mongoClient = new MongoClient(connection);
                if (null == mongoClient)
                {
                    LOG.Warn("创建mongo客户端失败，为null");
                    return false;
                }
                mongoClient.ListDatabaseNames();
                //mongoClient.StartSession();
                return true;
            }
            catch (Exception ex)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, "mongo验证失败，详情："+ex.Message);
            }
        }

        protected override bool Check(IPropertySetter properties)
        {
            // 检测数据源需要的参数
            if (null == base._propertySetter)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, "属性设置器为空");
            }
            if (!_propertySetter.IsExist(nameof(this.server)))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"缺失服务器属性[{nameof(this.server)}]");
            }
            this.server = (string)_propertySetter.GetProperty(nameof(this.server));
            if (string.IsNullOrEmpty(this.server))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"连接属性[{nameof(this.server)}]不能为空");
            }
            if (!_propertySetter.IsExist(nameof(this.port)))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"缺失端口属性[{nameof(this.port)}]");
            }
            this.port = (int)_propertySetter.GetProperty(nameof(this.port));
          
            // 以下是可选的
            if (_propertySetter.IsExist(nameof(this.userName)))
            {
                this.userName = (string)_propertySetter.GetProperty(nameof(this.userName));
            }
            if (_propertySetter.IsExist(nameof(this.encrypted)))
            {
                this.encrypted = (int)_propertySetter.GetProperty(nameof(this.encrypted));
            }
            if (_propertySetter.IsExist(nameof(this.password)))
            {
                this.password = (string)_propertySetter.GetProperty(nameof(this.password));
            }

            return true;
        }
        protected override bool Check(string connectionStr)
        {
            // 检测数据源需要的参数
            if (string.IsNullOrEmpty(connectionStr))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, "连接字符串connectionStr为空");
            }
            JObject json = JObject.Parse(connectionStr);
            if (!json.ContainsKey(nameof(this.server)))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"缺失服务器属性[{nameof(this.server)}]");
            }
            this.server = json[nameof(this.server)].Value<string>();
            if (string.IsNullOrEmpty(this.server))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, "服务器地址server不能为空");
            }
            if (!json.ContainsKey(nameof(this.port)))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"缺失端口属性[{nameof(this.port)}]");
            }
            this.port = json[nameof(this.port)].Value<int>();

            // 以下是可选的
            if (json.ContainsKey(nameof(this.userName)))
            {
                this.userName = json[nameof(this.userName)].Value<string>();
            }
            if (json.ContainsKey(nameof(this.password)))
            {
                this.password = json[nameof(this.password)].Value<string>();
            }
            if (json.ContainsKey(nameof(this.encrypted)))
            {
                this.encrypted = json[nameof(this.encrypted)].Value<int>();
            }

            return true;
        }
    }
}
