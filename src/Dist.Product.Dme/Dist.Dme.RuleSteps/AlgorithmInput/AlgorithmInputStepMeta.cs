using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework.AlgorithmTypes;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.Model.Entity;
using Dist.Dme.RuleSteps;
using Dist.Dme.RuleSteps.AlgorithmInput.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Dist.Dme.Base.Framework.RuleSteps.AlgorithmInput
{
    /// <summary>
    /// 算法输入步骤元数据
    /// </summary>
    public class AlgorithmInputStepMeta : BaseRuleStepMeta, IRuleStepMeta
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 算法唯一编码
        /// </summary>
        private string AlgorithmCode { get; set; }
     
        protected override EnumRuleStepTypes MyRuleStepType => EnumRuleStepTypes.AlgorithmInput;
        /// <summary>
        /// 对外提供访问
        /// </summary>
        public object RuleStepType
        {
            get
            {
                return base.GetRuleStepTypeMeta(MyRuleStepType);
            }
        }
        public string RuleStepName { get; set; } = EnumUtil.GetEnumDisplayName(EnumRuleStepTypes.AlgorithmInput);

        public AlgorithmInputStepMeta(IRepository repository, DmeRuleStep step)
            : base(repository, step)
        {
        }
        public override object InParams
        {
            get
            {
                // new Property(nameof(this.FeatureClass_Source_First), "总规用地的图层信息", ValueTypeMeta.TYPE_MDB_FEATURECLASS, "", "", 1, "总规用地的图层信息，格式：mdb路径"+ SEPARATOR_FEATURE_PATH + "要素类"))
                base.InputParameters[nameof(AlgorithmInputStepMeta.AlgorithmCode)] =
                    new Property(nameof(AlgorithmInputStepMeta.AlgorithmCode), "算法唯一标识符", Common.EnumValueMetaType.TYPE_STRING, "", "", "算法唯一标识符，需要选择一个算法。");
              
                return base.InputParameters;
            }
        }

        /// <summary>
        /// 获取算法
        /// </summary>
        /// <returns></returns>
        public AlgorithmDTO GetAlgorithm()
        {
            var db = base.repository.GetDbContext();
            AlgorithmDTO algorithmDTO = new AlgorithmDTO
            {
                Algorithm = GetAlgorithmEntity()
            };
            algorithmDTO.MetaDefine = JsonConvert.DeserializeObject<AlgorithmMetaDefine>(algorithmDTO.Algorithm.Extension.ToString());
            String baseDir = AppDomain.CurrentDomain.BaseDirectory;
            String assemblyPath = Path.Combine(baseDir, algorithmDTO.MetaDefine.Assembly);
            Assembly assembly = Assembly.LoadFile(assemblyPath);
            if (null == assembly)
            {
                LOG.Warn($"程序集文件[{assemblyPath}]不存在");
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"程序集文件[{assemblyPath}]不存在");
            }
            IAlgorithm algorithm = (IAlgorithm)assembly.CreateInstance(algorithmDTO.MetaDefine.MainClass, true);
            if (null == algorithm)
            {
                LOG.Warn($"接口[{algorithmDTO.MetaDefine.MainClass}]创建实例为空");
                throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"程序集文件[{assemblyPath}]不存在");
            }
            algorithmDTO.AlgorithmInstance = algorithm;
            return algorithmDTO;
        }
       /// <summary>
       /// 获取算法实体
       /// </summary>
       /// <returns></returns>
        private DmeAlgorithm GetAlgorithmEntity()
        {
            var db = base.repository.GetDbContext();
            DmeRuleStepAttribute dmeRuleStepAttribute = db.Queryable<DmeRuleStepAttribute>().Single(rsa => rsa.RuleStepId == this.step.Id && rsa.AttributeCode == nameof(this.AlgorithmCode));
            if (null == dmeRuleStepAttribute)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, "没有找到步骤关联的算法属性值");
            }
            string algorithmCode = dmeRuleStepAttribute.AttributeValue.ToString();
            DmeAlgorithm dmeAlgorithm = db.Queryable<DmeAlgorithm>().Single(alg => alg.SysCode == algorithmCode);
            if (null == dmeAlgorithm)
            {
                throw new BusinessException((int)EnumSystemStatusCode.DME_FAIL, $"没有找到步骤关联的算法，编码[{algorithmCode}]");
            }
            return dmeAlgorithm;
        }
        /// <summary>
        /// 覆写父类的方法
        /// </summary>
        /// <returns></returns>
        public new IDictionary<string, Property> ReadAttributes()
        {
            var db = repository.GetDbContext();
            DmeAlgorithm dmeAlgorithm = GetAlgorithmEntity();
            // 参数值封装成Property
            // 算法步骤的属性元数据，从表DME_ALGORITHM_META获取
            var algorithmParaMetas = db.Queryable<DmeAlgorithmMeta>().Where(am => am.AlgorithmId == dmeAlgorithm.Id).ToList();
            if (0 == algorithmParaMetas?.Count)
            {
                return null;
            }
            IDictionary<string, Property> propertyDic = new Dictionary<string, Property>();
            foreach (var paraMetaItem in algorithmParaMetas)
            {
                propertyDic[paraMetaItem.Name] =
                    new Property(paraMetaItem.Name, paraMetaItem.Alias, EnumUtil.GetEnumObjByName<EnumValueMetaType>(paraMetaItem.DataType),
                    null, null, paraMetaItem.Remark, null, paraMetaItem.IsVisible, paraMetaItem.ReadOnly, paraMetaItem.Required, "");
            }
            // 把算法编码加进去
            propertyDic[nameof(this.AlgorithmCode)] = new Property(nameof(this.AlgorithmCode), nameof(this.AlgorithmCode), EnumValueMetaType.TYPE_STRING);
            IList<DmeRuleStepAttribute> ruleStepAttributes = repository.GetDbContext().Queryable<DmeRuleStepAttribute>().Where(rsa => rsa.RuleStepId == this.step.Id).ToList();
            if (ruleStepAttributes?.Count > 0)
            {
                IDictionary<string, Property> dictionary = new Dictionary<string, Property>();
                Property property = null;
                foreach (var subAtt in ruleStepAttributes)
                {
                    if (!propertyDic.ContainsKey(subAtt.AttributeCode) || null == subAtt.AttributeValue)
                    {
                        continue;
                    }
                    property = propertyDic[subAtt.AttributeCode];
                    property.IsNeedPrecursor = subAtt.IsNeedPrecursor;
                    if (1 == subAtt.IsNeedPrecursor)
                    {
                        // 前驱参数格式：${步骤编码:属性编码}（已过时）
                        property.Value = subAtt.AttributeValue.ToString();//subAtt.AttributeValue.ToString().Substring(2, subAtt.AttributeValue.ToString().LastIndexOf("}")-2);
                        dictionary[subAtt.AttributeCode] = property;
                        continue;
                    }
                    if ((int)EnumValueMetaType.TYPE_FEATURECLASS == property.DataType)
                    {
                        // 如果是要素类，则值的格式：{"name":"图层名","source":"数据源唯一编码"}
                        JObject jObject = JObject.Parse(subAtt.AttributeValue.ToString());
                        property.Value = jObject.GetValue("name").Value<string>();
                        property.DataSourceCode = jObject.GetValue("source").Value<string>();
                    }
                    else
                    {
                        property.Value = subAtt.AttributeValue;
                    }
                    dictionary[subAtt.AttributeCode] = property;
                }
                return dictionary;
            }
            return null;
        }
    }
}
