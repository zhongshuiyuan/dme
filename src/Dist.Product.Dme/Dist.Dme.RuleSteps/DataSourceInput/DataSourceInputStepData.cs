using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Model.Entity;
using log4net;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.RuleSteps.DataSourceInput
{
    public class DataSourceInputStepData : BaseRuleStepData, IRuleStepData
    {
        private static ILog LOG = LogManager.GetLogger(typeof(DataSourceInputStepData));
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
