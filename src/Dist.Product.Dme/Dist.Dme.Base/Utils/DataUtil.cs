using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Dist.Dme.Base.Utils
{
    /// <summary>
    /// 数据工具
    /// </summary>
    public class DataUtil
    {
        /// <summary>
        /// IDictionary转NameValueCollection
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static NameValueCollection ToNameValueCollection( IDictionary<string, string> dict)
        {
            return dict?.Aggregate(new NameValueCollection(),
                (seed, current) => {
                    seed.Add(current.Key, current.Value);
                    return seed;
                });
        }
    }
}
