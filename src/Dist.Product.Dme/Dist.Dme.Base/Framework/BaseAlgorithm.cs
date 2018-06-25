using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework.Interfaces;
using Newtonsoft.Json;
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
        public object InParams
        {
            get
            {
                return this.InputParameters;
            }
        }
        public object OutParams
        {
            get
            {
                return this.OutputParameters;
            }
        }
        public object FeatureParams
        {
            get
            {
                return this.FeatureParameters;
            }
        }
        /// <summary>
        /// 初始化完成
        /// </summary>
        protected Boolean InitComplete { get; set; } = false;

        public abstract void Init(IDictionary<string, object> parameters);
        public abstract IAlgorithmDevType AlgorithmType { get; }
        public virtual object MetadataJSON
        {
            get
            {
                IDictionary<string, IDictionary<String, Property>> dictionary = new Dictionary<string, IDictionary<String, Property>>
                {
                    ["InputParameters"] = InputParameters,
                    ["OutputParameters"] = OutputParameters
                };
                return dictionary;
            }
        }

        /// <summary>
        /// 输入参数
        /// </summary>
        protected IDictionary<String, Property> InputParameters = new Dictionary<String, Property>();
        /// <summary>
        /// 输出参数
        /// </summary>
        protected IDictionary<String, Property> OutputParameters = new Dictionary<String, Property>();
        /// <summary>
        /// 特征参数
        /// </summary>
        protected IDictionary<String, Property> FeatureParameters = new Dictionary<String, Property>();
    }
}
