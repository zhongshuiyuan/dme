using Dist.Dme.Base.DataSourceDefine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.SRCE.Core
{
    public abstract class DMEWorkspace<TWorkspace, TFeatureClass> 
        where TWorkspace : class
        where TFeatureClass : class
    {
        /// <summary>
        /// 打开oracle远程库工作空间
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public abstract TWorkspace Open(OracleConn conn);
        /// <summary>
        /// 打开本地文件的工作空间，可以是本地mdb、gdb等等
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public abstract TWorkspace Open(LocalConn conn);
        /// <summary>
        /// 获取要素类
        /// </summary>
        /// <param name="workspace">工作空间</param>
        /// <param name="featureClassName">要素名称</param>
        /// <returns></returns>
        public abstract TFeatureClass GetFeatureClass(TWorkspace workspace, string featureClassName);

    }
}
