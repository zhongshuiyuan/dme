﻿using Dist.Dme.Base.Common;
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
        public abstract object InParams { get; }
        public abstract object OutParams { get; }
        public abstract object FeatureParams { get; }
        public abstract void Init(IDictionary<string, object> parameters);
        public virtual object MetadataJSON
        {
            get
            {
                IDictionary<string, IDictionary<String, Property>> dictionary = new Dictionary<string, IDictionary<String, Property>>
                {
                    ["InputParameters"] = InputParameters,
                    ["OutputParameters"] = OutputParameters
                };
                return JsonConvert.SerializeObject(dictionary);
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
        /// 权重参数
        /// </summary>
        protected IDictionary<String, Property> FeatureParameters = new Dictionary<String, Property>();

    }
}