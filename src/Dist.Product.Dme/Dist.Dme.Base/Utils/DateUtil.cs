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
        public static long CurrentTimeMillis
        {
            get
            {
                return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
                // return DateTime.Now.Subtract(DateTime.Parse("1970-1-1")).TotalMilliseconds;
                //long currentTicks = DateTime.Now.Ticks;
                //DateTime dtFrom = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                //return (currentTicks - dtFrom.Ticks) / 10000;
            }
        }
    }
}
