using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Model.Entity;
using NLog;
using System;
using System.Collections.Generic;

namespace Dist.Dme.RuleSteps.DataSourceInput
{
    public class DataSourceInputStepData : BaseRuleStepData, IRuleStepData
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        private DataSourceInputStepMeta ruleStepMeta;

        public DataSourceInputStepData(IRepository repository, DmeTask task, DmeRuleStep step) : 
            base(repository, task, step)
        {
            ruleStepMeta = new DataSourceInputStepMeta(repository, step);
        }

        public IRuleStepMeta RuleStepMeta => ruleStepMeta;

        public Result Run()
        {
            return new Result(Base.Common.EnumSystemStatusCode.DME_SUCCESS, "不需要计算", null);
        }

        public bool SaveAttributes(IDictionary<string, Property> attributes)
        {
            return this.ruleStepMeta.SaveAttributes(attributes);
        }
    }
}
