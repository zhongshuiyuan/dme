using Dist.Dme.Base.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework
{
    public class ModelHopMeta : BaseHopMeta<IRuleStep>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="enabled">是否可用，默认值为true</param>
        public ModelHopMeta(IRuleStep from, IRuleStep to, Boolean enabled = true)
        {
            base.From = from;
            base.To = to;
            base.Enabled = enabled;
        }
    }
}
