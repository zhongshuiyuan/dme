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
        public IDictionary<string, Property> InParams
        {
            get
            {
                return this.InputParametersMeta;
            }
        }
        public IDictionary<string, Property> OutParams
        {
            get
            {
                return this.OutputParametersMeta;
            }
        }
        public IDictionary<string, Property> FeatureParams
        {
            get
            {
                return this.FeatureParametersMeta;
            }
        }
        /// <summary>
        /// 初始化完成
        /// </summary>
        protected Boolean InitComplete { get; set; } = false;

        public abstract void Init(IDictionary<string, Property> parameters);
        public abstract IAlgorithmDevType AlgorithmType { get; }
        public virtual object MetadataJSON
        {
            get
            {
                IDictionary<string, IDictionary<String, Property>> dictionary = new Dictionary<string, IDictionary<String, Property>>
                {
                    ["InputParameters"] = InputParametersMeta,
                    ["OutputParameters"] = OutputParametersMeta
                };
                return dictionary;
            }
        }

        /// <summary>
        /// 输入参数元数据
        /// </summary>
        protected IDictionary<String, Property> InputParametersMeta = new Dictionary<String, Property>();
        /// <summary>
        /// 输出参数元数据
        /// </summary>
        protected IDictionary<String, Property> OutputParametersMeta = new Dictionary<String, Property>();
        /// <summary>
        /// 特征参数元数据
        /// </summary>
        protected IDictionary<String, Property> FeatureParametersMeta = new Dictionary<String, Property>();
    }
}
