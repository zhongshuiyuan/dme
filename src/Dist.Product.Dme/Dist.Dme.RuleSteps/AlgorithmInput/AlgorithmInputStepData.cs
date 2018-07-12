﻿using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework;
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

        public AlgorithmInputStepData(IRepository repository, int taskId, DmeRuleStep step) 
            : base(repository, taskId, step)
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
            List<DmeRuleStepAttribute> stepAttributes = base.repository.GetDbContext().Queryable<DmeRuleStepAttribute>().Where(rsa => rsa.RuleStepId == this.step.Id).ToList();
            if (0 == stepAttributes?.Count)
            {
                LOG.Warn("没有找到步骤关联的参数设置，停止执行");
                return new Result(EnumSystemStatusCode.DME_FAIL, "没有找到步骤关联的参数设置，停止执行", null);
            }
            //String baseDir = AppDomain.CurrentDomain.BaseDirectory;
            //String assemblyPath = Path.Combine(baseDir, dto.MetaDefine.Assembly);
            //Assembly assembly = Assembly.LoadFile(assemblyPath);
            //if (null == assembly)
            //{
            //    LOG.Warn($"程序集文件[{assemblyPath}]不存在");
            //    return new Result(EnumSystemStatusCode.DME_ERROR, $"程序集文件[{assemblyPath}]不存在", null);
            //}
            //IAlgorithm algorithm = (IAlgorithm)assembly.CreateInstance(dto.MetaDefine.MainClass, true);
            //if (null == algorithm)
            //{
            //    LOG.Warn($"接口[{dto.MetaDefine.MainClass}]创建实例为空");
            //    return new Result(EnumSystemStatusCode.DME_ERROR, $"程序集文件[{assemblyPath}]不存在", null);
            //}
            IDictionary<string, Property> inputParams = dto.AlgorithmInstance.InParams;
            // 载入参数值
            IDictionary<string, object> paraValues = new Dictionary<string, object>();
            foreach (var item in stepAttributes)
            {
                if (!inputParams.ContainsKey(item.AttributeCode))
                {
                    continue;
                }
                Property inputParaProperty = inputParams[item.AttributeCode];
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
                    paraValues[item.AttributeCode] = inputFeatureClassDTO;
                }
                else
                {
                    paraValues[item.AttributeCode] = item.AttributeValue;
                }
            }

            dto.AlgorithmInstance.Init(paraValues);
            Result result = dto.AlgorithmInstance.Execute();
            // 保存输出
            if (result != null && EnumUtil.GetEnumDisplayName(EnumSystemStatusCode.DME_SUCCESS) == result.Status)
            {
                base.SaveOutput(dto.AlgorithmInstance.OutParams);
                return new Result(EnumSystemStatusCode.DME_SUCCESS, "执行完毕", true);
            }
            return new Result(EnumSystemStatusCode.DME_FAIL, "执行失败，无异常信息", false);
        }

        public bool SaveAttributes(IDictionary<string, object> attributes)
        {
            return this.ruleStepMeta.SaveAttributes(attributes);
        }
    }
}
