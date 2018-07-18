using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Extensions.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dist.Dme.Extensions
{
    /// <summary>
    /// 注册器
    /// </summary>
    public class Register
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        public static IList<AlgorithmRegisterDTO> Algorithms { get; set; } = new List<AlgorithmRegisterDTO>();
        public static IDictionary<string, RuleStepPluginRegisterDTO> RuleStepPluginsMap { get; set; } = new Dictionary<string, RuleStepPluginRegisterDTO>();

        static Register()
        {

            String baseDir = AppDomain.CurrentDomain.BaseDirectory;
            String registerFile = Path.Combine(new string[] { baseDir, "register.json" });
            if (!File.Exists(registerFile))
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"注册文件[{registerFile}]不存在");
            }
            String jsonText = File.ReadAllText(registerFile);
            JObject jObject = JObject.Parse(jsonText);
            Algorithms = JsonConvert.DeserializeObject<IList<AlgorithmRegisterDTO>>(jObject["Algorithms"].ToString());
            RuleStepPluginsMap = JsonConvert.DeserializeObject<IDictionary<string, RuleStepPluginRegisterDTO>>(jObject["RuleStepPlugins"].ToString());
        }
    }
}
