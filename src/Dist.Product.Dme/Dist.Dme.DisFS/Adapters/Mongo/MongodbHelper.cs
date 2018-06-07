using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dist.Dme.DisFS.Adapters.Mongo
{
    /// <summary>
    /// mongodb帮助类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class MongodbHelper<T> where T : class, new()
    {
        private static ILog LOG = LogManager.GetLogger(typeof(T));
        #region Add 添加一条数据
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="t">添加的实体</param>
        /// <param name="host">mongodb连接信息</param>
        /// <returns></returns>
        public static int Add(MongodbHost host, T t)
        {
            try
            {
                var client = MongodbManager<T>.GetMongodbCollection(host);
                client.InsertOne(t);
                return 1;
            }
            catch
            {
                return 0;
            }
        }
        #endregion

        #region AddAsync 异步添加一条数据
        /// <summary>
        /// 异步添加一条数据
        /// </summary>
        /// <param name="t">添加的实体</param>
        /// <param name="host">mongodb连接信息</param>
        /// <returns></returns>
        public static async Task<int> AddAsync(MongodbHost host, T t)
        {
            try
            {
                var client = MongodbManager<T>.GetMongodbCollection(host);
                await client.InsertOneAsync(t);
                return 1;
            }
            catch
            {
                return 0;
            }
        }
        #endregion

        #region InsertMany 批量插入
        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="t">实体集合</param>
        /// <returns></returns>
        public static int InsertMany(MongodbHost host, List<T> t)
        {
            try
            {
                var client = MongodbManager<T>.GetMongodbCollection(host);
                client.InsertMany(t);
                return 1;
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
                return 0;
            }
        }
        #endregion

        #region InsertManyAsync 异步批量插入
        /// <summary>
        /// 异步批量插入
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="t">实体集合</param>
        /// <returns></returns>
        public static async Task<int> InsertManyAsync(MongodbHost host, List<T> t)
        {
            try
            {
                var client = MongodbManager<T>.GetMongodbCollection(host);
                await client.InsertManyAsync(t);
                return 1;
            }
            catch
            {
                return 0;
            }
        }
        #endregion

        #region Update 修改一条数据
        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <param name="t">添加的实体</param>
        /// <param name="host">mongodb连接信息</param>
        /// <returns></returns>
        public static UpdateResult Update(MongodbHost host, T t, string id)
        {
            try
            {
                var client = MongodbManager<T>.GetMongodbCollection(host);
                //修改条件
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
                //要修改的字段
                var list = new List<UpdateDefinition<T>>();
                foreach (var item in t.GetType().GetProperties())
                {
                    if (item.Name.ToLower() == "id") continue;
                    list.Add(Builders<T>.Update.Set(item.Name, item.GetValue(t)));
                }
                var updatefilter = Builders<T>.Update.Combine(list);
                return client.UpdateOne(filter, updatefilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region UpdateAsync 异步修改一条数据
        /// <summary>
        /// 异步修改一条数据
        /// </summary>
        /// <param name="t">添加的实体</param>
        /// <param name="host">mongodb连接信息</param>
        /// <returns></returns>
        public static async Task<UpdateResult> UpdateAsync(MongodbHost host, T t, string id)
        {
            try
            {
                var client = MongodbManager<T>.GetMongodbCollection(host);
                //修改条件
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
                //要修改的字段
                var list = new List<UpdateDefinition<T>>();
                foreach (var item in t.GetType().GetProperties())
                {
                    if (item.Name.ToLower() == "id") continue;
                    list.Add(Builders<T>.Update.Set(item.Name, item.GetValue(t)));
                }
                var updatefilter = Builders<T>.Update.Combine(list);
                return await client.UpdateOneAsync(filter, updatefilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region UpdateManay 批量修改数据
        /// <summary>
        /// 批量修改数据
        /// </summary>
        /// <param name="dic">要修改的字段</param>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">修改条件</param>
        /// <returns></returns>
        public static UpdateResult UpdateManay(MongodbHost host, Dictionary<string, string> dic, FilterDefinition<T> filter)
        {
            try
            {
                var client = MongodbManager<T>.GetMongodbCollection(host);
                T t = new T();
                //要修改的字段
                var list = new List<UpdateDefinition<T>>();
                foreach (var item in t.GetType().GetProperties())
                {
                    if (!dic.ContainsKey(item.Name)) continue;
                    var value = dic[item.Name];
                    list.Add(Builders<T>.Update.Set(item.Name, value));
                }
                var updatefilter = Builders<T>.Update.Combine(list);
                return client.UpdateMany(filter, updatefilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region UpdateManayAsync 异步批量修改数据
        /// <summary>
        /// 异步批量修改数据
        /// </summary>
        /// <param name="dic">要修改的字段</param>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">修改条件</param>
        /// <returns></returns>
        public static async Task<UpdateResult> UpdateManayAsync(MongodbHost host, Dictionary<string, string> dic, FilterDefinition<T> filter)
        {
            try
            {
                var client = MongodbManager<T>.GetMongodbCollection(host);
                T t = new T();
                //要修改的字段
                var list = new List<UpdateDefinition<T>>();
                foreach (var item in t.GetType().GetProperties())
                {
                    if (!dic.ContainsKey(item.Name)) continue;
                    var value = dic[item.Name];
                    list.Add(Builders<T>.Update.Set(item.Name, value));
                }
                var updatefilter = Builders<T>.Update.Combine(list);
                return await client.UpdateManyAsync(filter, updatefilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Delete 删除一条数据
        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="id">objectId</param>
        /// <returns></returns>
        public static DeleteResult Delete(MongodbHost host, string id)
        {
            try
            {
                var client = MongodbManager<T>.GetMongodbCollection(host);
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
                return client.DeleteOne(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region DeleteAsync 异步删除一条数据
        /// <summary>
        /// 异步删除一条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="id">objectId</param>
        /// <returns></returns>
        public static async Task<DeleteResult> DeleteAsync(MongodbHost host, string id)
        {
            try
            {
                var client = MongodbManager<T>.GetMongodbCollection(host);
                //修改条件
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
                return await client.DeleteOneAsync(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region DeleteMany 删除多条数据
        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">删除的条件</param>
        /// <returns></returns>
        public static DeleteResult DeleteMany(MongodbHost host, FilterDefinition<T> filter)
        {
            try
            {
                var client = MongodbManager<T>.GetMongodbCollection(host);
                return client.DeleteMany(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region DeleteManyAsync 异步删除多条数据
        /// <summary>
        /// 异步删除多条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">删除的条件</param>
        /// <returns></returns>
        public static async Task<DeleteResult> DeleteManyAsync(MongodbHost host, FilterDefinition<T> filter)
        {
            try
            {
                var client = MongodbManager<T>.GetMongodbCollection(host);
                return await client.DeleteManyAsync(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region Count 根据条件获取总数
        /// <summary>
        /// 根据条件获取总数
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        public static long Count(MongodbHost host, FilterDefinition<T> filter)
        {
            try
            {
                var client = MongodbManager<T>.GetMongodbCollection(host);
                return client.Count(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region CountAsync 异步根据条件获取总数
        /// <summary>
        /// 异步根据条件获取总数
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        public static async Task<long> CountAsync(MongodbHost host, FilterDefinition<T> filter)
        {
            try
            {
                var client = MongodbManager<T>.GetMongodbCollection(host);
                return await client.CountAsync(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region FindOne 根据id查询一条数据
        /// <summary>
        /// 根据id查询一条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="id">objectid</param>
        /// <param name="field">要查询的字段，不写时查询全部</param>
        /// <returns></returns>
        public static T FindOne(MongodbHost host, string id, string[] field = null)
        {
            try
            {
                var client = MongodbManager<T>.GetMongodbCollection(host);
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    return client.Find(filter).FirstOrDefault<T>();
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList?.Clear();
                return client.Find(filter).Project<T>(projection).FirstOrDefault<T>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region FindOneAsync 异步根据id查询一条数据
        /// <summary>
        /// 异步根据id查询一条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="id">objectid</param>
        /// <returns></returns>
        public static async Task<T> FindOneAsync(MongodbHost host, string id, string[] field = null)
        {
            try
            {
                var client = MongodbManager<T>.GetMongodbCollection(host);
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    return await client.Find(filter).FirstOrDefaultAsync();
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList?.Clear();
                return await client.Find(filter).Project<T>(projection).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region FindList 查询集合
        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">查询条件</param>
        /// <param name="field">要查询的字段,不写时查询全部</param>
        /// <param name="sort">要排序的字段</param>
        /// <returns></returns>
        public static List<T> FindList(MongodbHost host, FilterDefinition<T> filter, string[] field = null, SortDefinition<T> sort = null)
        {
            try
            {
                var client = MongodbManager<T>.GetMongodbCollection(host);
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    if (sort == null) return client.Find(filter).ToList();
                    //进行排序
                    return client.Find(filter).Sort(sort).ToList();
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList?.Clear();
                if (sort == null) return client.Find(filter).Project<T>(projection).ToList();
                //排序查询
                return client.Find(filter).Sort(sort).Project<T>(projection).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region FindListAsync 异步查询集合
        /// <summary>
        /// 异步查询集合
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">查询条件</param>
        /// <param name="field">要查询的字段,不写时查询全部</param>
        /// <param name="sort">要排序的字段</param>
        /// <returns></returns>
        public static async Task<List<T>> FindListAsync(MongodbHost host, FilterDefinition<T> filter, string[] field = null, SortDefinition<T> sort = null)
        {
            try
            {
                var client = MongodbManager<T>.GetMongodbCollection(host);
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    if (sort == null) return await client.Find(filter).ToListAsync();
                    return await client.Find(filter).Sort(sort).ToListAsync();
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList?.Clear();
                if (sort == null) return await client.Find(filter).Project<T>(projection).ToListAsync();
                //排序查询
                return await client.Find(filter).Sort(sort).Project<T>(projection).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region FindListByPage 分页查询集合
        /// <summary>
        /// 分页查询集合
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">查询条件</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="count">总条数</param>
        /// <param name="field">要查询的字段,不写时查询全部</param>
        /// <param name="sort">要排序的字段</param>
        /// <returns></returns>
        public static List<T> FindListByPage(MongodbHost host, FilterDefinition<T> filter, int pageIndex, int pageSize, out long count, string[] field = null, SortDefinition<T> sort = null)
        {
            try
            {
                var client = MongodbManager<T>.GetMongodbCollection(host);
                count = client.Count(filter);
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    if (sort == null) return client.Find(filter).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToList();
                    //进行排序
                    return client.Find(filter).Sort(sort).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToList();
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList?.Clear();

                //不排序
                if (sort == null) return client.Find(filter).Project<T>(projection).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToList();

                //排序查询
                return client.Find(filter).Sort(sort).Project<T>(projection).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region FindListByPageAsync 异步分页查询集合
        /// <summary>
        /// 异步分页查询集合
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">查询条件</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="field">要查询的字段,不写时查询全部</param>
        /// <param name="sort">要排序的字段</param>
        /// <returns></returns>
        public static async Task<List<T>> FindListByPageAsync(MongodbHost host, FilterDefinition<T> filter, int pageIndex, int pageSize, string[] field = null, SortDefinition<T> sort = null)
        {
            try
            {
                var client = MongodbManager<T>.GetMongodbCollection(host);
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    if (sort == null) return await client.Find(filter).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();
                    //进行排序
                    return await client.Find(filter).Sort(sort).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList?.Clear();

                //不排序
                if (sort == null) return await client.Find(filter).Project<T>(projection).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();

                //排序查询
                return await client.Find(filter).Sort(sort).Project<T>(projection).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region File文件类操作
        /// <summary>
        /// 通过字节流上传文件
        /// </summary>
        /// <param name="host">mongo服务器信息</param>
        /// <param name="source">文件字节</param>
        /// <param name="fileName">文件名</param>
        /// <param name="options">扩展属性</param>
        /// <returns>返回ObjectId</returns>
        public static ObjectId UploadFileFromBytes(MongodbHost host, string fileName, byte[] source, GridFSUploadOptions options = null)
        {
            var bucket = MongodbManager<GridFSBucket>.GetGridFSBucket(host);
            return bucket.UploadFromBytes(fileName, source, options);
        }
        /// <summary>
        /// 上传，并附带元数据
        /// </summary>
        /// <param name="host"></param>
        /// <param name="fileName"></param>
        /// <param name="source"></param>
        /// <param name="metadata">元数据，键值对</param>
        /// <returns></returns>
        public static ObjectId UploadFileFromBytes(MongodbHost host, string fileName, byte[] source, IDictionary<string, object> metadata)
        {
            var bucket = MongodbManager<GridFSBucket>.GetGridFSBucket(host);
            GridFSUploadOptions options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument(metadata)
            };
            return bucket.UploadFromBytes(fileName, source, options);
        }
      
        /// <summary>
        /// 异步通过字节流上传文件
        /// </summary>
        /// <param name="host"></param>
        /// <param name="fileName">文件名</param>
        /// <param name="source">文件字节</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static Task<ObjectId> UploadFileFromBytesAsync(MongodbHost host, string fileName, byte[] source, GridFSUploadOptions options = null)
        {
            var bucket = MongodbManager<GridFSBucket>.GetGridFSBucket(host);
            return bucket.UploadFromBytesAsync(fileName, source, options);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="fileName"></param>
        /// <param name="source"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ObjectId UploadFileFromStream(MongodbHost host, string fileName, Stream source, GridFSUploadOptions options = null)
        {
            var bucket = MongodbManager<GridFSBucket>.GetGridFSBucket(host);
            return bucket.UploadFromStream(fileName, source, options);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="filePath">文件完整路径</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ObjectId UploadFileFromPath(MongodbHost host, string filePath, GridFSUploadOptions options = null)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception($"{filePath}  does not exist");
            }
            var bucket = MongodbManager<GridFSBucket>.GetGridFSBucket(host);
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
         
            return bucket.UploadFromStream(Path.GetFileName(filePath), fileStream, options);
        }
        /// <summary>
        /// 上传文件流，并附上元数据
        /// </summary>
        /// <param name="host"></param>
        /// <param name="fileName"></param>
        /// <param name="source"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static ObjectId UploadFileFromStream(MongodbHost host, string fileName, Stream source, IDictionary<string, object> metadata)
        {
            var bucket = MongodbManager<GridFSBucket>.GetGridFSBucket(host);
            GridFSUploadOptions options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument(metadata)
            };
            return bucket.UploadFromStream(fileName, source, options);
        }
        /// <summary>
        /// 从本地路径上传文件
        /// </summary>
        /// <param name="host"></param>
        /// <param name="filePath"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static ObjectId UploadFileFromPath(MongodbHost host, string filePath, IDictionary<string, object> metadata)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception($"{filePath}  does not exist");
            }
            var bucket = MongodbManager<GridFSBucket>.GetGridFSBucket(host);
            GridFSUploadOptions options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument(metadata)
            };
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            return bucket.UploadFromStream(fileStream.Name, fileStream, options);
        }
        /// <summary>
        /// 异步上传
        /// </summary>
        /// <param name="host"></param>
        /// <param name="fileName"></param>
        /// <param name="source"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static Task<ObjectId> UploadFileFromStreamAsync(MongodbHost host, string fileName, Stream source, GridFSUploadOptions options = null)
        {
            var bucket = MongodbManager<GridFSBucket>.GetGridFSBucket(host);
            return bucket.UploadFromStreamAsync(fileName, source, options);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="id">文件id，fs.files中的_id，也对应fs.chunks的files_id</param>
        /// <param name="options">下载选项</param>
        public static Stream DownloadFileToStream(MongodbHost host, BsonValue id, GridFSDownloadOptions options = null)
        {
            Stream destination = new MemoryStream();
            var bucket = MongodbManager<GridFSBucket>.GetGridFSBucket(host);
            bucket.DownloadToStream(id, destination, options);
            return destination;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Stream DownloadFileToStream(MongodbHost host, ObjectId id)
        {
            Stream destination = new MemoryStream();
            var bucket = MongodbManager<GridFSBucket>.GetGridFSBucket(host);
            bucket.DownloadToStream(id, destination);
            return destination;
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="host"></param>
        /// <param name="fileName"></param>
        /// <param name="destination"></param>
        /// <param name="options"></param>
        public static Stream DownloadFileToStreamByName(MongodbHost host, string fileName, GridFSDownloadByNameOptions options = null)
        {
            Stream destination = new MemoryStream();
            var bucket = MongodbManager<GridFSBucket>.GetGridFSBucket(host);
            bucket.DownloadToStreamByName(fileName, destination, options);
            return destination;
        }
        /// <summary>
        /// 根据文件名下载文件字节数组
        /// </summary>
        /// <param name="host"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Byte[] DownloadFileAsBytesByName(MongodbHost host, string fileName)
        {
            var bucket = MongodbManager<GridFSBucket>.GetGridFSBucket(host);
            return bucket.DownloadAsBytesByName(fileName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="id">文件_id值？</param>
        /// <returns></returns>
        public static Byte[] DownloadFileAsByteArray(MongodbHost host, ObjectId id)
        {
            var bucket = MongodbManager<GridFSBucket>.GetGridFSBucket(host);
            Byte[] bytes = bucket.DownloadAsBytes(id);
            return bytes;
        }
        /// <summary>
        /// 查找操作
        /// </summary>
        /// <param name="host">mongodb服务器信息</param>
        /// <param name="filter">查询过滤器</param>
        /// <param name="options">查找选项</param>
        /// <returns></returns>
        public static IAsyncCursor<GridFSFileInfo> FindFiles(MongodbHost host, FilterDefinition<GridFSFileInfo> filter, GridFSFindOptions options = null)
        {
            var bucket = MongodbManager<GridFSBucket>.GetGridFSBucket(host);
            return bucket.Find(filter, options);
        }
        /// <summary>
        /// 根据文件名查找所有匹配的文件
        /// </summary>
        /// <param name="host">mongo服务器信息</param>
        /// <param name="fileName">文件名称</param>
        /// <returns></returns>
        public static IAsyncCursor<GridFSFileInfo> FindFiles(MongodbHost host, string fileName)
        {
            var filter = Builders<GridFSFileInfo>.Filter.And(
            Builders<GridFSFileInfo>.Filter.Eq(x => x.Filename, fileName)
            // Builders<GridFSFileInfo>.Filter.Eq("metadata.UserID", fileName)
            );
            var sort = Builders<GridFSFileInfo>.Sort.Descending(x => x.UploadDateTime);
            var options = new GridFSFindOptions
            {
                // Limit = 1,
                Sort = sort
            };
            var bucket = MongodbManager<GridFSBucket>.GetGridFSBucket(host);
            return bucket.Find(filter, options);
        }
        /// <summary>
        /// 根据元数据查找
        /// </summary>
        /// <param name="host">mongo服务器信息</param>
        /// <param name="metadatas">元数据信息</param>
        /// <returns></returns>
        public static GridFSFileInfo FindSingleFile(MongodbHost host, IDictionary<string, object> metadatas)
        {
            IList<FilterDefinition<GridFSFileInfo>> filters = new List<FilterDefinition<GridFSFileInfo>>();
            foreach (var item in metadatas)
            {
                filters.Add(Builders<GridFSFileInfo>.Filter.Eq(item.Key, item.Value));
            }
            var sort = Builders<GridFSFileInfo>.Sort.Descending(x => x.UploadDateTime);
            var options = new GridFSFindOptions
            {
                Limit = 1,
                Sort = sort
            };
            var bucket = MongodbManager<GridFSBucket>.GetGridFSBucket(host);
            using (var cursor = bucket.Find(Builders<GridFSFileInfo>.Filter.And(filters), options))
            {
                return cursor.ToList().FirstOrDefault();
            }
        }
        /// <summary>
        /// 根据元数据查找文件集合
        /// </summary>
        /// <param name="host"></param>
        /// <param name="metadatas"></param>
        /// <returns></returns>
        public static IAsyncCursor<GridFSFileInfo> FindFiles(MongodbHost host, IDictionary<string, object> metadatas)
        {
            IList<FilterDefinition<GridFSFileInfo>> filters = new List<FilterDefinition<GridFSFileInfo>>();
            foreach (var item in metadatas)
            {
                filters.Add(Builders<GridFSFileInfo>.Filter.Eq(item.Key, item.Value));
            }
            var sort = Builders<GridFSFileInfo>.Sort.Descending(x => x.UploadDateTime);
            var options = new GridFSFindOptions
            {
                Limit = 1,
                Sort = sort
            };
            var bucket = MongodbManager<GridFSBucket>.GetGridFSBucket(host);
            return bucket.Find(Builders<GridFSFileInfo>.Filter.And(filters), options);
        }
        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="host">mongo服务器信息</param>
        /// <param name="id">文件id，fs.files中的_id，也对应fs.chunks的files_id</param>
        /// <param name="newFileName">新文件名称</param>
        public static void RenameFile(MongodbHost host, BsonValue id, string newFileName)
        {
            var bucket = MongodbManager<GridFSBucket>.GetGridFSBucket(host);
            bucket.Rename(id, newFileName);
        }
        /// <summary>
        /// 重命名所有匹配的文件
        /// </summary>
        /// <param name="host"></param>
        /// <param name="oldFilename"></param>
        /// <param name="newFilename"></param>
        public static void RenameAllfile(MongodbHost host, string oldFilename, string newFilename)
        {
            var bucket = MongodbManager<GridFSBucket>.GetGridFSBucket(host);
            var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Filename, oldFilename);
            var filesCursor = bucket.Find(filter);
            var files = filesCursor.ToList();
            foreach (var file in files)
            {
                bucket.Rename(file.Id, newFilename);
            }
        }
        /// <summary>
        /// 根据id删除文件
        /// </summary>
        /// <param name="host"></param>
        /// <param name="id">文件id，fs.files中的_id，也对应fs.chunks的files_id</param>
        public static void DeleteFileById(MongodbHost host, string id)
        {
            var bucket = MongodbManager<GridFSBucket>.GetGridFSBucket(host);
            bucket.Delete(new ObjectId(id));
        }
        /// <summary>
        /// 根据ObjectId删除文件
        /// </summary>
        /// <param name="host"></param>
        /// <param name="id">ObjectId</param>
        public static void DeleteFileById(MongodbHost host, ObjectId id)
        {
            var bucket = MongodbManager<GridFSBucket>.GetGridFSBucket(host);
            bucket.Delete(id);
        }
        /// <summary>
        /// 删除fs数据库
        /// </summary>
        /// <param name="host"></param>
        public static void DropFS(MongodbHost host)
        {
            var bucket = MongodbManager<GridFSBucket>.GetGridFSBucket(host);
            bucket.Drop();
        }
        #endregion
    }
}
