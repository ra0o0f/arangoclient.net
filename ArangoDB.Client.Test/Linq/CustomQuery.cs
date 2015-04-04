using ArangoDB.Client.Test.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ArangoDB.Client.Test.Utility;
using ArangoDB.Client.Common.Newtonsoft.Json;

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
    }
}
