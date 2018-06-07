using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dist.Dme.DisFS.Adapters.Mongo.Mongo
{
    public class MongoTest
    {
        public static void UnitTest()
        {
            MongodbHost host = null;
            //1.批量修改,修改的条件
            var time = DateTime.Now;
            var list = new List<FilterDefinition<PhoneEntity>>
            {
                Builders<PhoneEntity>.Filter.Lt("AddTime", time.AddDays(5)),
                Builders<PhoneEntity>.Filter.Gt("AddTime", time)
            };
            var filter = Builders<PhoneEntity>.Filter.And(list);

            //2.要修改的字段内容
            var dic = new Dictionary<string, string>
            {
                { "UseAge", "168" },
                { "Name", "朝阳" }
            };
            //3.批量修改
            var kk = MongodbHelper<PhoneEntity>.UpdateManay(host, dic, filter);


            //根据条件查询集合
            time = DateTime.Now;
            list = new List<FilterDefinition<PhoneEntity>>
            {
                Builders<PhoneEntity>.Filter.Lt("AddTime", time.AddDays(20)),
                Builders<PhoneEntity>.Filter.Gt("AddTime", time)
            };
            filter = Builders<PhoneEntity>.Filter.And(list);
            //2.查询字段
            var field = new[] { "Name", "Price", "AddTime" };
            //3.排序字段
            var sort = Builders<PhoneEntity>.Sort.Descending("AddTime");
            var res = MongodbHelper<PhoneEntity>.FindList(host, filter, field, sort);


            //分页查询，查询条件
            time = DateTime.Now;
            list = new List<FilterDefinition<PhoneEntity>>
            {
                Builders<PhoneEntity>.Filter.Lt("AddTime", time.AddDays(400)),
                Builders<PhoneEntity>.Filter.Gt("AddTime", time)
            };
            filter = Builders<PhoneEntity>.Filter.And(list);
            //排序条件
            sort = Builders<PhoneEntity>.Sort.Descending("AddTime");

            res = MongodbHelper<PhoneEntity>.FindListByPage(host, filter, 2, 10, out long count, null, sort);
        }
    }

    internal class PhoneEntity
    {
    }
}
