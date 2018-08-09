using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework.Define;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dist.Dme.RuleSteps.DataSourceInput
{
    /// <summary>
    /// 数据源步骤类型
    /// </summary>
    public class DataSourceInputStepMeta : BaseRuleStepMeta, IRuleStepMeta
    {
        public DataSourceInputStepMeta(IRepository repository, DmeRuleStep step)
          : base(repository, step)
        {
        }

        public RuleStepTypeMeta RuleStepType
        {
            get
            {
                return new RuleStepTypeMeta()
                {
                    Name = "DataSourceInput",
                    Category = "输入",
                    DisplayName = "数据源输入",
                    Description = "数据源输入"
                };
            }
        }
        public override IDictionary<string, Property> InParams
        {
            get
            {
                return base.InputParameters;
            }
        }

        public override IDictionary<string, Property> OutParams
        {
            get
            {
                return base.OutputParameters;
            }
        }

        /// <summary>
        /// 覆写父类的方法
        /// </summary>
        /// <returns></returns>
        public new IDictionary<string, Property> ReadAttributes()
        {
            IDictionary<string, Property> atts = new Dictionary<string, Property>();
            // 数据源步骤的属性不会存在数据模型[DME_RULESTEP_ATTRIBUTE]，而是存在数据模型[DME_RULESTEP_DATASOURCE]
            var db = base.repository.GetDbContext();
            // 选择数据源标识符
            IList<string> dataSources = db.Queryable<DmeRuleStepDataSource, DmeDataSource>((rsds, ds) => rsds.DataSourceId == ds.Id && rsds.RuleStepId == step.Id).Select<string>((rsds, ds) => ds.SysCode).ToList();
            if (dataSources?.Count > 0)
            {
                foreach (var item in dataSources)
                {
                    atts[nameof(base.Source)] = new Property(nameof(base.Source), nameof(base.Source), EnumValueMetaType.TYPE_STRING, item, "", "", null, 1, 0, 1, item);
                    // 一个数据源有且仅有一个关联
                    break;
                }
            }
            return atts;
        }
    }
}
