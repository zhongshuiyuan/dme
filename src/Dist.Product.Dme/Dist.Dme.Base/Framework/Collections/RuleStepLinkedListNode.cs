using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework.Collections
{
    /// <summary>
    /// 规则步骤链表节点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class RuleStepLinkedListNode<T>
    {  
        public RuleStepLinkedListNode(T value)
        {
            this.Value = value;
        }
    
        public IList<RuleStepLinkedListNode<T>> Next { get; set; } = new List<RuleStepLinkedListNode<T>>();
        public IList<RuleStepLinkedListNode<T>> Previous { get; set; } = new List<RuleStepLinkedListNode<T>>();
        public T Value { get; set; }
    }
}
