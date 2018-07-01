﻿using Dist.Dme.Base.Common;
using Dist.Dme.Base.Framework.Exception;
using Dist.Dme.Base.Framework.Interfaces;
using Dist.Dme.Base.Utils;
using Dist.Dme.Model.Entity;
using log4net;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.RuleSteps
{
    /// <summary>
    /// 规则步骤元数据基类
    /// </summary>
    public abstract class BaseRuleStepMeta
    {
        private static ILog LOG = LogManager.GetLogger(typeof(BaseRuleStepMeta));

        public abstract string RuleStepName { get; set; }
        public abstract IRuleStepType RuleStepType { get; }
        public abstract object InParams { get; }
        protected IRepository repository;
        protected DmeRuleStep step;
        //protected int modelId;
        //protected int versionId;
        //protected int ruleStepId;

        public BaseRuleStepMeta(IRepository repository, DmeRuleStep step)
        {
            this.repository = repository;
            this.step = step;
            //this.modelId = modelId;
            //this.versionId = versionId;
            //this.ruleStepId = ruleStepId;
        }
        /// <summary>
        /// 步骤需要的输入参数
        /// </summary>
        protected IDictionary<String, Property> InputParameters { get; set; } = new Dictionary<String, Property>();

        public IDictionary<string, object> ReadAttributes()
        {
            IList<DmeRuleStepAttribute> attributes = repository.GetDbContext().Queryable<DmeRuleStepAttribute>().Where(rsa => rsa.RuleStepId == this.step.Id).ToList();
            if (attributes?.Count > 0)
            {
                IDictionary<string, object> dictionary = new Dictionary<string, object>();
                foreach (var item in attributes)
                {
                    dictionary[item.AttributeCode] = item.AttributeValue;
                }
                return dictionary;
            }
            return null;
        }
        public bool SaveAttributes(IDictionary<string, object> attributes)
        {
            if (attributes?.Count == 0)
            {
                LOG.Info("属性个数为 0，不再执行后面的操作。");
                return true;
            }
            var db = repository.GetDbContext();
            // 先删除这个步骤的属性，再重新添加
            return db.Ado.UseTran<Boolean>(() => 
            {
                // 删除的影响条目
                int deleteCount = db.Deleteable<DmeRuleStepAttribute>().Where(rsa => rsa.RuleStepId == this.step.Id).ExecuteCommand();
                LOG.Info($"删除规则[{this.step.Id}]下的{deleteCount} 条属性记录");
                // 保存步骤属性
                DmeRuleStep dmeRuleStep = db.Queryable<DmeRuleStep>().Single(rs => rs.Id == this.step.Id);
                if (null ==  dmeRuleStep)
                {
                    throw new BusinessException((int)SystemStatusCode.DME_FAIL, $"规则[{this.step.Id}]没有找到数据库记录");
                }
                // 不使用接口IList，Insertable<>批量插入存在问题
                List<DmeRuleStepAttribute> attributeEntities = new List<DmeRuleStepAttribute>();
                foreach (var item in attributes)
                {
                    DmeRuleStepAttribute dmeRuleStepAttribute = new DmeRuleStepAttribute
                    {
                        RuleStepId = this.step.Id,
                        ModelId = dmeRuleStep.ModelId,
                        VersionId = dmeRuleStep.VersionId,
                        AttributeCode = item.Key,
                        AttributeValue = item.Value
                    };
                    attributeEntities.Add(dmeRuleStepAttribute);
                }
                db.Insertable<DmeRuleStepAttribute>(attributeEntities).ExecuteCommand();
                return true;
            }).Data;
        }
        public int SaveMeta(double guiLocationX, double guiLocationY,
            string stepName)
        {
            // 步骤名如果为空，则使用当前步骤类型默认的名称
            if (string.IsNullOrEmpty(stepName))
            {
                stepName = this.RuleStepType.Name;
            }
            int identity = -1;
            SqlSugarClient db = repository.GetDbContext();
            return db.Ado.UseTran<int>(() => 
            {
                if (-1 == this.step.Id)
                {
                    LOG.Info($"步骤id为{this.step.Id}，为新创建步骤");
                    // 查找当前步骤类型
                    DmeRuleStepType dmeRuleStepType = db.Queryable<DmeRuleStepType>().Single(rst => rst.Code == this.RuleStepType.Code);
                    if (null == dmeRuleStepType)
                    {
                        throw new BusinessException((int)SystemStatusCode.DME_FAIL, $"步骤编码{this.RuleStepType.Code}不存在，请核实数据。");
                    }
                    DmeRuleStep dmeRuleStep = new DmeRuleStep
                    {
                        SysCode = GuidUtil.NewGuid(),
                        ModelId = this.step.ModelId,
                        VersionId = this.step.VersionId,
                        StepName = stepName,
                        GuiLocationX = guiLocationX,
                        GuiLocationY = guiLocationY,
                        StepTypeId = dmeRuleStepType.Id
                    };
                    identity = db.Insertable<DmeRuleStep>(dmeRuleStep).ExecuteReturnIdentity();
                }
                else
                {
                    LOG.Info($"步骤id为{this.step.Id}，更新步骤信息");
                    DmeRuleStep dmeRuleStep = db.Queryable<DmeRuleStep>().InSingle(this.step.Id);
                    if(null == dmeRuleStep)
                    {
                        throw new BusinessException((int)SystemStatusCode.DME_FAIL, $"步骤id[{this.step.Id}]不存在，请核实数据。");
                    }
                    if (!string.IsNullOrEmpty(stepName))
                    {
                        // 如果名称不为空，才更新原来的名称
                        dmeRuleStep.StepName = stepName;
                    }
                    dmeRuleStep.GuiLocationX = guiLocationX;
                    dmeRuleStep.GuiLocationY = guiLocationY;
                    db.Updateable<DmeRuleStep>(dmeRuleStep).ExecuteCommandHasChange();
                    identity = dmeRuleStep.Id;
                }
                return identity;
            }).Data;
        }
    }
}
