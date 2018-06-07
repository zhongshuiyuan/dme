using Dist.Dme.DisFS.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dist.Dme.DisFS
{
    /// <summary>
    /// 分布式文件存储工厂类
    /// </summary>
    public class DisFSFactory
    {
        /// <summary>
        /// 上传文件流
        /// </summary>
        /// <param name="disFSAdapter">适配器</param>
        /// <param name="fileName"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public object UploadFromStream(IDisFSAdapter disFSAdapter, string fileName, Stream stream)
        {
            return disFSAdapter.UploadFromStream(fileName, stream);
        }
        /// <summary>
        /// 从本地路径上传文件
        /// </summary>
        /// <param name="disFSAdapter">适配器</param>
        /// <param name="fileFullPath"></param>
        /// <returns></returns>
        public object UploadFromPath(IDisFSAdapter disFSAdapter, string fileFullPath)
        {
            return disFSAdapter.UploadFromPath(fileFullPath);
        }
    }
}
