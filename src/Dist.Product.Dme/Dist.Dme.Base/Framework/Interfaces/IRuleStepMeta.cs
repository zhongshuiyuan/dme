using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Framework.Interfaces
{
    public interface IRuleStepMeta
    {
        /// <summary>
        /// 规则步骤名称，如果不设置，则默认为规则类型的名称
        /// </summary>
        string RuleStepName { get; set; }
        /// <summary>
        /// 规则步骤类型
        /// </summary>
        IRuleStepType RuleStepType { get; }
        /// <summary>
        /// 保存步骤元数据信息
        /// </summary>
        /// <param name="modelId">模型id</param>
        /// <param name="modelVersionId">模型版本id</param>
        /// <param name="stepId">步骤id，如果是新步骤，则传入-1；否则传入真实的步骤id</param>
        /// <param name="guiLocationX">界面布局x坐标</param>
        /// <param name="guiLocationY">界面布局y坐标</param>
        /// <param name="stepName">步骤名称</param>
        /// <returns>返回保存的步骤标识id</returns>
        int SaveMeta(double guiLocationX, double guiLocationY, string stepName);
        /// <summary>
        /// 保存步骤属性数据
        /// </summary>
        /// <param name="modelId">模型id</param>
        /// <param name="modelVersionId">版本id</param>
        /// <param name="stepId">步骤id</param>
        /// <param name="attributes">属性值，键值对</param>
        Boolean SaveAttributes(IDictionary<string, object> attributes);
        /// <summary>
        /// 读取步骤属性
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="stepId">步骤id</param>
        /// <returns></returns>
        IDictionary<string, object> ReadAttributes();
        /// <summary>
        /// 获取规则步骤输入参数
        /// </summary>
        Object InParams { get; }
}
}
