using Dist.Dme.Base.DataSource.Define;
using Dist.Dme.Base.Framework;
using Dist.Dme.DisFS.Adapters.Mongo;
using Dist.Dme.Extensions;
using Dist.Dme.Service.Interfaces;
using Dist.Dme.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dist.Dme.WebApi.Controllers
{
    /// <summary>
    /// 文件控制器
    /// </summary>
    [Route("api/files")]
    public class FileController : BaseController
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        public IDataSourceService DataSourceService { get; private set; }
        public FileController(IDataSourceService dataSourceService)
        {
            this.DataSourceService = dataSourceService;
        }
        [HttpPost]
        [Route("v1/filesystem")]
        public Result UploadDmeFileSystem([FromForm][Required] string name, [FromForm]IFormFile[] files)
        {
            if (files?.Length > 0)
            {
                IList<DmeFileSystemMeta> metas = new List<DmeFileSystemMeta>();
                foreach (var file in files)
                {
                    try
                    {
                        string suffix = file.FileName.Substring(file.FileName.LastIndexOf("."));
                        GridFSUploadOptions options = new GridFSUploadOptions
                        {
                            Metadata = new BsonDocument(new Dictionary<string, object>() { ["ContentType"] = file.ContentType })
                        };
                        ObjectId objectId = MongodbHelper<object>.UploadFileFromStream(ServiceFactory.MongoDatabase, file.FileName, file.OpenReadStream(), options);
                        metas.Add(new DmeFileSystemMeta() {
                            Suffix = suffix,
                            ContentType = file.ContentType,
                           ObjectId = objectId.ToString()
                        });
                    }
                    catch (Exception ex)
                    {
                        return base.Fail($"文件数据库持久化失败，详情：{ex.Message}");
                    }
                }
                try
                {
                    return base.Success(this.DataSourceService.AddDmeFileSystemSource(name, metas));
                }
                catch (Exception ex)
                {
                    foreach (var item in metas)
                    {
                        LOG.Warn($"文件[{item.ObjectId}]撤销保存");
                        MongodbHelper<object>.DeleteFileById(ServiceFactory.MongoDatabase, item.ObjectId);
                    }
                    return base.Fail($"元数据数据库持久化失败，详情：{ex.Message}");
                }
            }
            return base.Fail("上传失败，没有获取到文件数据");
        }
    }
}
