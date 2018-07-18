using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.DisFS.Collection
{
    /// <summary>
    /// 任务结果的集合类
    /// </summary>
    public class TaskResultColl
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 任务id
        /// </summary>
        public int TaskId { get; set; }
        /// <summary>
        /// 规则步骤id
        /// </summary>
        public int RuleStepId { get; set; }
        /// <summary>
        /// 属性编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; set; }
    }
}
