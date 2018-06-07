using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dist.Dme.DisFS.Interfaces
{
    /// <summary>
    /// 分布式文件存储适配器
    /// </summary>
    public interface IDisFSAdapter
    {
        /// <summary>
        /// 上传文件流
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        object UploadFromStream(string fileName, Stream stream);
        /// <summary>
        /// 从本地文件上传
        /// </summary>
        /// <param name="fileFullPath"></param>
        /// <returns></returns>
        object UploadFromPath(string fileFullPath);
    }
}
