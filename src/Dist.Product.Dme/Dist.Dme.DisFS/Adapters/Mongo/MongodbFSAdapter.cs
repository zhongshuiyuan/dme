using Dist.Dme.DisFS.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dist.Dme.DisFS.Adapters.Mongo
{
    /// <summary>
    /// mongo文件存储
    /// </summary>
    public class MongodbFSAdapter : IDisFileAdapter
    {
        private MongodbHost host;
        public MongodbFSAdapter(MongodbHost host)
        {
            this.host = host;
        }

        public object UploadFromStream(string fileName, Stream stream)
        {
            return MongodbHelper<object>.UploadFileFromStream(this.host, fileName, stream);
        }
        public object UploadFromPath(string fileFullPath)
        {
            return MongodbHelper<object>.UploadFileFromPath(this.host, fileFullPath);
        }
    }
}
