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

        public DataSourceInputStepData(IRepository repository, int taskId, DmeRuleStep step) : 
            base(repository, taskId, step)
        {
            ruleStepMeta = new DataSourceInputStepMeta(repository, step);
        }

        public IRuleStepMeta RuleStepMeta => ruleStepMeta;

        public Result Run()
        {
            throw new NotImplementedException();
        }

        public bool SaveAttributes(IDictionary<string, object> attributes)
        {
            return this.ruleStepMeta.SaveAttributes(attributes);
        }
    }
}
