using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.Model.Entity;
using System;
using System.Collections.Generic;
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
        public string RuleStepName { get; set; } = EnumUtil.GetEnumDisplayName(EnumRuleStepTypes.DataSourceInput);
        protected override EnumRuleStepTypes MyRuleStepType => EnumRuleStepTypes.DataSourceInput;
        public object RuleStepType
        {
            get
            {
                return base.GetRuleStepTypeMeta(MyRuleStepType);
            }
        }
        public override object InParams
        {
            get
            {
                return base.InputParameters;
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
