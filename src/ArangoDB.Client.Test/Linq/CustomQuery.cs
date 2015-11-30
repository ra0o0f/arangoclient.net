using ArangoDB.Client.Test.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ArangoDB.Client.Test.Utility;
using ArangoDB.Client.Common.Newtonsoft.Json;
using ArangoDB.Client.Test.Model;

namespace ArangoDB.Client.Test.Linq
{
    public class CustomQuery
    {
        [Fact]
        public void ArangoDoc_Return()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query().Return(_ => "this will be returned");

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"return @P1");

            Assert.Equal(queryData.BindVars[0].Value, "this will be returned");
        }

        [Fact]
        public void ArangoDoc_CrossJoin()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .For(_ => new int[] { 2011, 2012, 2013 }
                .For(year => new int[] { 1, 2, 3, 4 }
                .Select(quarter => new
                {
                    y = year,
                    q = quarter,
                    nice = AQL.Concat(AQL.ToString(quarter), "/", AQL.ToString(year))
                })));

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"
for `year` in @P1
for `quarter` in @P2
return { 
    `y` : `year` , 
    `q` : `quarter` , 
    `nice` : concat( [ to_string( `quarter` ) , @P3 , to_string( `year` ) ] ) 
}
".RemoveSpaces());

            Assert.Equal(queryData.BindVars.Count, 3);
            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[0].Value), "[2011,2012,2013]");
            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[1].Value), "[1,2,3,4]");
            Assert.Equal(queryData.BindVars[2].Value, "/");
        }

        [Fact]
        public void UpsertComplexUpdate()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .Upsert<Host>(_ => new Host { Ip = "192.168.173.94" },
                                  _ => new Host { Ip = "192.168.173.94", Name = "chorweiler", Tags = new string[] { "development" } },
                                 (_, o) => new Host { Tags = AQL.Push(o.Tags, "development", true) })
                                 .In<Host>().ReturnResult(true);

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"
upsert @P1
insert @P2
update { `tags` : push( `OLD` .`tags` , @P3 , @P4  )  }
in `hosts`
let crudResult = NEW 
return crudResult
                    ".RemoveSpaces());

            ObjectUtility.AssertSerialize(queryData.BindVars[0].Value, new { ip = "192.168.173.94" },db);
            ObjectUtility.AssertSerialize(queryData.BindVars[1].Value,new { ip = "192.168.173.94", name = "chorweiler", tags = new string[] { "development" } },db);
            Assert.Equal(queryData.BindVars[2].Value, "development");
            Assert.Equal(queryData.BindVars[3].Value, true);
        }
    }
}
