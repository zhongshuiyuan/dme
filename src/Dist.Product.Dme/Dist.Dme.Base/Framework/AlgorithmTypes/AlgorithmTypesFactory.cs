using Dist.Dme.Base.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dist.Dme.Base.Framework.AlgorithmTypes
{
    public class AlgorithmTypesFactory
    {
        /// <summary>
        /// 获取算法开发平台类型
        /// </summary>
        /// <returns></returns>
        public static object ListAlgorithmDevType()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                     .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IAlgorithmDevType))))
                     .ToArray();
            if (null == types || 0 == types.Count())
            {
                return new List<IAlgorithmDevType>();
            }
            IList<IAlgorithmDevType> algorithmTypes = new List<IAlgorithmDevType>();
            object temp = null;
            foreach (var type in types)
            {
                temp = type.Assembly.CreateInstance(type.FullName, true);
                if (null == temp)
                {
                    continue;
                }
                algorithmTypes.Add((IAlgorithmDevType)temp);
            }
            return algorithmTypes;
        }
    }
}
