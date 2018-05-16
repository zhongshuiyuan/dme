using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.Base.Utils
{
    public class GuidUtil
    {
        /// <summary>
        /// 创建一个GUID
        /// </summary>
        /// <param name="hyphens">是否有连接符</param>
        /// <param name="braces">是否有花括号</param>
        /// <param name="uppcase">是否大写</param>
        /// <returns></returns>
        public static String NewGuid(Boolean hyphens = false, Boolean braces = false, Boolean uppcase = false)
        {
            string guid = Guid.NewGuid().ToString("N");
            if (hyphens)
            {
                guid = Guid.NewGuid().ToString("D");
            }
            if (braces)
            {
                guid = "{" + guid + "}";
            }
            if (uppcase)
            {
                guid = guid.ToUpper();
            }
            return guid;
        }
    }
}
