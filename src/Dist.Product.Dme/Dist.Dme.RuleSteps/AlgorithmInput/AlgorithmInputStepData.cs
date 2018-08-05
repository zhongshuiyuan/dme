using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Framework.RuleSteps.AlgorithmInput;
using Dist.Dme.Base.Utils;
using Dist.Dme.Model.DTO;
using Dist.Dme.Model.Entity;
using Dist.Dme.RuleSteps.AlgorithmInput.DTO;
using Newtonsoft.Json.Linq;
using NLog;
using System.Collections.Generic;

namespace Dist.Dme.RuleSteps.AlgorithmInput
{
    public class AlgorithmInputStepData : BaseRuleStepData, IRuleStepData
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        private AlgorithmInputStepMeta ruleStepMeta;

        public AlgorithmInputStepData(IRepository repository, DmeTask task, DmeRuleStep step) 
            : base(repository, task, step)
        {
            ruleStepMeta = new AlgorithmInputStepMeta(repository, step);
        }
        /// <summary>
        /// 对外访问
        /// </summary>
        public IRuleStepMeta RuleStepMeta => ruleStepMeta;

        public Result Run()
        {
            // 找到算法输入依赖的算法实体
            AlgorithmDTO dto = this.ruleStepMeta.GetAlgorithm();
            // 找到这个步骤注入的参数值
            var db = base.repository.GetDbContext();
            List<DmeRuleStepAttribute> stepAttributes = db.Queryable<DmeRuleStepAttribute>().Where(rsa => rsa.RuleStepId == this.step.Id).ToList();
            if (0 == stepAttributes?.Count)
            {
                LOG.Warn("没有找到步骤关联的参数设置，停止执行");
                return new Result(EnumSystemStatusCode.DME_FAIL, "没有找到步骤关联的参数设置，停止执行", null);
            }
            IDictionary<string, Property> inputParams = dto.AlgorithmInstance.InParams;
            // 载入参数值
            IDictionary<string, Property> paraValues = new Dictionary<string, Property>();
            foreach (var item in stepAttributes)
            {
                if (!inputParams.ContainsKey(item.AttributeCode))
                {
                    continue;
                }
                Property inputParaProperty = inputParams[item.AttributeCode];
                if (1 == item.IsNeedPrecursor)
                {
                    // 前驱参数
                    if (null == item.AttributeValue 
                        || string.IsNullOrEmpty(item.AttributeValue.ToString())
                        || !item.AttributeValue.ToString().Contains(":"))
                    {
                        throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"步骤[{step.SysCode}]的参数[{item.AttributeCode}]无效，值[{inputParaProperty.Value}]");
                    }
                    string preStepName = item.AttributeValue.ToString().Split(":")[0];
                    string preAttributeName = item.AttributeValue.ToString().Split(":")[1];
                    DmeRuleStepAttribute preRuleStepAttribute = db.Queryable<DmeRuleStepAttribute, DmeRuleStep > ((rsa, rs) => new object[] { rs.Id == rsa.RuleStepId })
                        .Where((rsa, rs) => rs.ModelId == step.ModelId && rs.Name == preStepName && rsa.AttributeCode == preAttributeName)
                        .Single();
                    if (null == preRuleStepAttribute)
                    {
                        throw new BusinessException((int)EnumSystemStatusCode.DME_ERROR, $"步骤[{step.SysCode}]的参数[{item.AttributeCode}]无效，找不到前驱参数信息");
                    }
                    paraValues[item.AttributeCode] = base.GetStepAttributeValue(preStepName, preAttributeName);// new Property(item.AttributeCode, item.AttributeCode, EnumValueMetaType.TYPE_UNKNOWN, preRuleStepAttribute.AttributeValue);
                }
                else
                {
                    if (inputParaProperty.DataType == (int)EnumValueMetaType.TYPE_FEATURECLASS)
                    {
                        // 这种类型，要注意解析数据源
                        JObject featureClassJson = JObject.Parse(item.AttributeValue.ToString());
                        string featureClassName = featureClassJson.GetValue("name").Value<string>();
                        string sourceId = featureClassJson.GetValue("source").Value<string>();
                        // 根据sourceId查找数据源实体
                        DmeDataSource dataSource = base.repository.GetDbContext().Queryable<DmeDataSource>().Single(ds => ds.SysCode == sourceId);

                        InputFeatureClassDTO inputFeatureClassDTO = new InputFeatureClassDTO
                        {
                            Name = featureClassName,
                            Source = ClassValueCopier<DataSourceDTO>.Copy(dataSource)
                        };
                        paraValues[item.AttributeCode] = new Property(item.AttributeCode, item.AttributeCode, EnumValueMetaType.TYPE_OBJECT, inputFeatureClassDTO);
                    }
                    else
                    {
                        paraValues[item.AttributeCode] = new Property(item.AttributeCode, item.AttributeCode, EnumValueMetaType.TYPE_UNKNOWN, item.AttributeValue);
                    }
                }
            }

            dto.AlgorithmInstance.Init(paraValues);
            Result result = dto.AlgorithmInstance.Execute();
            // 保存输出
            if (result != null && EnumUtil.GetEnumDisplayName(EnumSystemStatusCode.DME_SUCCESS).Equals(result.Status))
            {
                base.SaveOutput(dto.AlgorithmInstance.OutParams);
                return new Result(EnumSystemStatusCode.DME_SUCCESS, "执行完毕", true);
            }
            return new Result(EnumSystemStatusCode.DME_FAIL, "执行失败，无异常信息", false);
        }

        public bool SaveAttributes(IDictionary<string, Property> attributes)
        {
            return this.ruleStepMeta.SaveAttributes(attributes);
        }
    }
}
