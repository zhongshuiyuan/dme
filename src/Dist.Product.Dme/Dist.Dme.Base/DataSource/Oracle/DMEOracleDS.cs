using System;
using System.Collections.Generic;
using System.Text;
using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Base.Framework.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;

namespace Dist.Dme.Base.DataSource.Oracle
{
    /// <summary>
    /// 对oracle资源具体操作的方法
    /// </summary>
    public class DMEOracleDS : BaseDMEDataSource, IDMEOracleDS
    {
        private string name = string.Empty;
        private string server = "127.0.0.1";
        private string database = "ORCL";
        private int port = 1521;
        private string username = string.Empty;
        private int encrypted = 0;
        private string password = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="properties">
        /// {
        ///   "name":"厦门空间库",
        ///   "server":"192.168.1.166",
        ///   "database":"orcl",
        ///   "port":1521,
        ///   "username":"xmgis",
        ///   "encrypted":0,
        ///   "password":"xmghj2014"
        /// }
        /// <paramref name="checkMetaValid">是否检测连接元数据的有效性</paramref>
        /// </param>
        public DMEOracleDS(IDMEDataSourceFactory factory , IPropertySetter properties, Boolean checkMetaValid = false) : base(factory, properties, checkMetaValid)
        {
            if (checkMetaValid)
            {
                Check(properties);
            }
        }
        public DMEOracleDS(IDMEDataSourceFactory factory, string connectionStr, Boolean checkMetaValid = false) : base(factory, connectionStr, checkMetaValid)
        {
            if (checkMetaValid)
            {
                Check(connectionStr);
            }
        }
        public override string ConnectionMeta
        {
            get
            {
                JObject json = new JObject
                {
                    { nameof(this.name), name },
                    { nameof(this.server), server },
                    { nameof(this.database), database },
                    { nameof(this.port), port },
                    { nameof(this.username), username },
                    { nameof(this.encrypted), encrypted },
                    { nameof(this.password), password }
                };
                return json.ToString();
            }
        }

        public override bool ValidConnection()
        {
            string dbConnStr = $"Data Source={this.server}/{this.database};User ID={this.username};Password={this.password};";
            try
            {
                SqlSugarClient Db = new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = dbConnStr,
                    DbType = DbType.Oracle,
                    IsAutoCloseConnection = true,
                    //设为true，表示相同线程是同一个SqlSugarClient
                    IsShardSameThread = true
                });
                Db.Open();
                return true;
            }
            catch (Exception ex)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, "数据库验证失败，详情："+ex.Message);
            }
        }

        protected override bool Check(IPropertySetter properties)
        {
            // 检测数据源需要的参数
            if (null == base._propertySetter)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL_INIT, "连接属性为空");
            }
            this.name = (string)_propertySetter.GetProperty(nameof(this.name));
            if (string.IsNullOrEmpty(this.name))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL_INIT, $"缺失连接属性[{nameof(this.name)}]");
            }
            this.server = (string)_propertySetter.GetProperty(nameof(this.server));
            if (string.IsNullOrEmpty(this.server))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL_INIT, $"缺失连接属性[{nameof(this.server)}]");
            }
            this.database = (string)_propertySetter.GetProperty(nameof(this.database));
            if (string.IsNullOrEmpty(this.database))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL_INIT, $"缺失连接属性[{nameof(this.database)}]");
            }
            if (!_propertySetter.IsExist(nameof(this.port)))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL_INIT, $"缺失连接属性[{nameof(this.port)}]");
            }
            this.port = (int)_propertySetter.GetProperty(nameof(this.port));
            this.username = (string)_propertySetter.GetProperty(nameof(this.username));
            if (string.IsNullOrEmpty(this.username))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL_INIT, $"缺失连接属性[{nameof(this.username)}]");
            }
            if (!_propertySetter.IsExist(nameof(this.encrypted)))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL_INIT, $"缺失连接属性[{nameof(this.encrypted)}]");
            }
            this.encrypted = (int)_propertySetter.GetProperty(nameof(this.encrypted));
            this.password = (string)_propertySetter.GetProperty(nameof(this.password));
            if (string.IsNullOrEmpty(this.password))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL_INIT, $"缺失连接属性[{nameof(this.password)}]");
            }
            return true;
        }
        protected override bool Check(string connectionStr)
        {
            // 检测数据源需要的参数
            if (string.IsNullOrEmpty(connectionStr))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL_INIT, "连接属性为空");
            }
            JObject json = JObject.Parse(connectionStr);
            this.name = json[nameof(this.name)].Value<string>();
            if (string.IsNullOrEmpty(this.name))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL_INIT, $"缺失连接属性[{nameof(this.name)}]");
            }
            this.server = json[nameof(this.server)].Value<string>();
            if (string.IsNullOrEmpty(this.server))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL_INIT, $"缺失连接属性[{nameof(this.server)}]");
            }
            this.database = json[nameof(this.database)].Value<string>();
            if (string.IsNullOrEmpty(this.database))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL_INIT, $"缺失连接属性[{nameof(this.database)}]");
            }
            if (!json.ContainsKey(nameof(this.port)))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL_INIT, $"缺失连接属性[{nameof(this.port)}]");
            }
            this.port = json[nameof(this.port)].Value<int>();
            this.username = json[nameof(this.username)].Value<string>();
            if (string.IsNullOrEmpty(this.username))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL_INIT, $"缺失连接属性[{nameof(this.username)}]");
            }
            if (!json.ContainsKey(nameof(this.encrypted)))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL_INIT, $"缺失连接属性[{nameof(this.encrypted)}]");
            }
            this.encrypted = json[nameof(this.encrypted)].Value<int>();
            this.password = json[nameof(this.password)].Value<string>();
            if (string.IsNullOrEmpty(this.password))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL_INIT, $"缺失连接属性[{nameof(this.password)}]");
            }
            return true;
        }
    }
}
