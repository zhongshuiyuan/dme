using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework
{
    public abstract class BaseHopMeta<T>
    {
        /// <summary>
        /// 起点
        /// </summary>
        public T From { get; set; }
        /// <summary>
        /// 终点
        /// </summary>
        public T To { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public Boolean Enabled { get; set; }
    }
}
