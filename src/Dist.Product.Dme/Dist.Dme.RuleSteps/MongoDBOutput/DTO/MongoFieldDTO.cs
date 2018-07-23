using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.RuleSteps.MongoDBOutput.DTO
{
    /// <summary>
    /// mongo字段信息
    /// </summary>
    public class MongoFieldDTO
    {
        /// <summary>
        /// 字段名称，如果IsNeedPrecursor=1，则Name格式为：步骤标识符:字段标识符
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否使用输入的字段名称作为mongo本身的字段名称，1：是；0：否
        /// </summary>
        public int IsUseName { get; set; }
        /// <summary>
        /// 如果IsUseName=false，那这个字段需要有指定的值，说明是重命名mongodb字段名称，否则使用Name
        /// </summary>
        public string NewName { get; set; }
        /// <summary>
        /// 是否前驱参数，0和1，默认为1
        /// </summary>
        public int IsNeedPrecursor { get; set; } = 1;
        /// <summary>
        /// IsNeedPrecursor = 0，则使用value的值，常量
        /// </summary>
        public object ConstantValue { get; set; }
    }
}
