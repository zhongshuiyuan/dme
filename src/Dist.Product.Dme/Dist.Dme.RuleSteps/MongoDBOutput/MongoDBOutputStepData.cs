using Dist.Dme.Base.Framework;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Model.Entity;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.RuleSteps.MongoDBOutput
{
    public class MongoDBOutputStepData : BaseRuleStepData, IRuleStepData
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        private MongoDBOutputStepMeta ruleStepMeta;

        public MongoDBOutputStepData(IRepository repository, int taskId, DmeRuleStep step) :
         base(repository, taskId, step)
        {
            ruleStepMeta = new MongoDBOutputStepMeta(repository, step);
        }
        public IRuleStepMeta RuleStepMeta => ruleStepMeta;

        public Result Run()
        {
            throw new NotImplementedException();
        }

        public bool SaveAttributes(IDictionary<string, object> attributes)
        {
            return ruleStepMeta.SaveAttributes(attributes);
        }
    }
}
