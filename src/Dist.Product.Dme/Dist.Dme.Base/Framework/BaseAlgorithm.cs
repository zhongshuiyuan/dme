using Dist.Dme.Base.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework
{
    public abstract class BaseAlgorithm : IAlgorithm
    {
        public abstract string SysCode { get; }
        public abstract string Name { get; }
        public abstract string Alias { get; }
        public abstract string Version { get; }
        public abstract string Remark { get; }

        public abstract Result Execute();
        public abstract object GetInParameters();
        public abstract object GetOutParameters();
        public abstract void Init(IDictionary<string, object> parameters);

        protected IDictionary<String, Property> InputParameters = new Dictionary<String, Property>();
        protected IDictionary<String, Property> OutputParameters = new Dictionary<String, Property>();

    }
}
