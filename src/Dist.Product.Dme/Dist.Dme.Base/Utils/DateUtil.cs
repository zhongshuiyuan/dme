using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Utils
{
    public class DateUtil
    {
        /// <summary>
        /// 获取当前时间毫秒数
        /// </summary>
        /// <returns></returns>
        public static double CurrentTimeMillis()
        {
            return DateTime.Now.Subtract(DateTime.Parse("1970-1-1")).TotalMilliseconds;
        }
    }
}
