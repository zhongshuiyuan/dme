using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dist.Dme.Base.Utils
{
    /// <summary>
    /// 目录帮助类
    /// </summary>
    public sealed class DirectFileUtil
    {
        /// <summary>
        /// 拷贝文件夹
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="toDir"></param>
        public static void CopyDirectInfo(string sourceDir, string toDir)
        {
            if (!Directory.Exists(sourceDir))
            {
                throw new ApplicationException("Source directory does not exist");
            }
            if (!Directory.Exists(toDir))
            {
                Directory.CreateDirectory(toDir);
            }
            DirectoryInfo directInfo = new DirectoryInfo(sourceDir);
            //copy files
            FileInfo[] filesInfos = directInfo.GetFiles();
            foreach (FileInfo fileinfo in filesInfos)
            {
                string fileName = fileinfo.Name;
                File.Copy(fileinfo.FullName, toDir + @"/" + fileName, true);
            }
            //copy directory
            foreach (DirectoryInfo directoryPath in directInfo.GetDirectories())
            {
                string toDirPath = toDir + @"/" + directoryPath.Name;
                CopyDirectInfo(directoryPath.FullName, toDirPath);
            }
        }
    }
}
