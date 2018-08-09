﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dist.Dme.Model.DTO
{
    /// <summary>
    /// 新创建任务的请求参数
    /// </summary>
    public class NewTaskReqDTO
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 模型版本编码
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string ModelVersionCode { get; set; }
        /// <summary>
        /// 周期表达式
        /// 由7段构成：秒 分 时 日 月 星期 年（年是可选的）
        /// "-" ：表示范围 MON-WED表示星期一到星期三
        /// "," ：表示列举 MON, WEB表示星期一和星期三
        /// "*" ：表是“每”，每月，每天，每周，每年等
        /// "/" :表示增量：0/15（处于分钟段里面） 每15分钟，在0分以后开始，3/20 每20分钟，从3分钟以后开始
        /// "?" :只能出现在日，星期段里面，表示不指定具体的值
        /// "L" :只能出现在日，星期段里面，是Last的缩写，一个月的最后一天，一个星期的最后一天（星期六）
        /// "W" :表示工作日，距离给定值最近的工作日
        /// "#" :表示一个月的第几个星期几，例如："6#3"表示每个月的第三个星期五（1=SUN...6=FRI,7=SAT）
        /// 
        /// 0 0 12 * * ?	每天中午12点触发
        /// 0 15 10 ? * *	每天上午10:15触发
        /// 0 15 10 * * ?	每天上午10:15触发
        /// 0 15 10 * * ? *	每天上午10:15触发
        /// 0 15 10 * * ? 2005	2005年的每天上午10:15触发
        /// 0 * 14 * * ?	在每天下午2点到下午2:59期间的每1分钟触发
        /// 0 0/5 14 * * ?	在每天下午2点到下午2:55期间的每5分钟触发
        /// 0 0/5 14,18 * * ?	在每天下午2点到2:55期间和下午6点到6:55期间的每5分钟触发
        /// 0 0-5 14 * * ?	在每天下午2点到下午2:05期间的每1分钟触发
        /// 0 10,44 14 ? 3 WED	每年三月的星期三的下午2:10和2:44触发
        /// 0 15 10 ? * MON-FRI	周一至周五的上午10:15触发
        /// 0 15 10 15 * ?	每月15日上午10:15触发
        /// 0 15 10 L * ?	每月最后一日的上午10:15触发
        /// 0 15 10 L-2 * ?	Fire at 10:15am on the 2nd-to-last last day of every month
        /// 0 15 10 ? * 6L	每月的最后一个星期五上午10:15触发
        /// 0 15 10 ? * 6L	Fire at 10:15am on the last Friday of every month
        /// 0 15 10 ? * 6L 2002-2005	2002年至2005年的每月的最后一个星期五上午10:15触发
        /// 0 15 10 ? * 6#3	每月的第三个星期五上午10:15触发
        /// 0 0 12 1/5 * ?	Fire at 12pm (noon) every 5 days every month, starting on the first day of the month.
        /// 0 11 11 11 11 ?	Fire every November 11th at 11:11am.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string CronExpression { get; set; }
        /// <summary>
        /// 是否立刻启动，0：否；1：是
        /// </summary>
        public int StartNow { get; set; } = 0;
        /// <summary>
        /// 多少秒之后启动
        /// </summary>
        public Double InSeconds { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>
        public string Remark { get; set; }
}
}