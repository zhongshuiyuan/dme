using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Dist.Dme.Base.Utils
{
    public sealed class GuidUtil
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
        /// <summary>
        /// 判断是否一个guid
        /// </summary>
        /// <param name="regexStr">正则表达式</param>
        /// <param name="expression">验证的字符</param>
        /// <returns></returns>
        public static bool IsGUID(string regexStr, string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                return false;
            }
            if (string.IsNullOrEmpty(regexStr))
            {
                return Guid.TryParse(expression, out Guid guid);
            }
            Regex guidReg = new Regex(regexStr);
            return guidReg.IsMatch(expression);
            //if (expression != null)
            //{
                // Regex guidRegEx = new Regex(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$");
                //return guidRegEx.IsMatch(expression);
            //}
        }
    }
}
