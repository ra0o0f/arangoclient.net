using ArangoDB.Client.Common.Newtonsoft.Json;
using ArangoDB.Client.Common.Newtonsoft.Json.Linq;
using ArangoDB.Client.Data;
using ArangoDB.Client.Serialization;
using ArangoDB.Client.Test.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArangoDB.Client.Test.Serialization
{
    public class SimpleDocumentParser
    {
        JsonTextReader GenerateReader(string json)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(json);
            writer.Flush();
            stream.Position = 0;

            var streamReader = new StreamReader(stream);
            return new JsonTextReader(streamReader);
        }

        string SingleResultJson
        {
            get
            {
                return @"
{
'error':false,
'code':200,
'Fullname':'raoof hojat',
'Age':27,
'Height':172
}
"; ;
            }
        }

        string BatchDocumentJson
        {
            get
            {
                return @"
{
'error':false,
'code':200,
'document':
{
'Fullname':'raoof hojat',
'Age':27,
'Height':172
}
}
"; ;
            }
        }

        string ErrorJson
        {
            get
            {
                return @"
{
'error' : true, 
'errorMessage' : 'error message', 
'code' : 404, 
'errorNum' : 1202 
}";
            }
        }

        string BatchListJson
        {
            get
            {
                return @"
{
'error':false,
'code':200,
'document':
[{
'Fullname':'raoof hojat',
'Age':27,
'Height':172
}]
}
"; ;
            }
        }

        [Fact]
        public void ParseSingle()
        {
            using(var reader = GenerateReader(SingleResultJson))
            {
                var documentParser = new DocumentParser(new ArangoDatabase());
                JObject jObject=null;
                var person = documentParser.ParseSingleResult<Person>(reader, out jObject, true);

                Assert.Equal(person.Age, 27);
                Assert.Equal(person.Fullname, "raoof hojat");
                Assert.Equal(person.Height, 172);

                Assert.True(JObject.DeepEquals(jObject, JObject.Parse(SingleResultJson)));
            }
        }

        [Fact]
        public void ParseBatchList()
        {
            using (var reader = GenerateReader(BatchListJson))
            {
                var documentParser = new DocumentParser(new ArangoDatabase());

                BaseResult baseResult = null;
                var personList = documentParser.ParseBatchResult<Person>(reader, out baseResult);

                Assert.Equal(personList.Count, 1);

                var person = personList[0];

                Assert.Equal(person.Age, 27);
                Assert.Equal(person.Fullname, "raoof hojat");
                Assert.Equal(person.Height, 172);

                Assert.Equal(baseResult.Code, 200);
                Assert.Equal(baseResult.Error, false);
            }
        }

        [Fact]
        public void ParseBatchDocument()
        {
            using (var reader = GenerateReader(BatchDocumentJson))
            {
                var documentParser = new DocumentParser(new ArangoDatabase());

                BaseResult baseResult = null;
                var personList = documentParser.ParseBatchResult<Person>(reader, out baseResult);

                Assert.Equal(personList.Count, 1);

                var person = personList[0];

                Assert.Equal(person.Age, 27);
                Assert.Equal(person.Fullname, "raoof hojat");
                Assert.Equal(person.Height, 172);

                Assert.Equal(baseResult.Code, 200);
                Assert.Equal(baseResult.Error, false);
            }
        }

        [Fact]
        public void ParseBatchErrorDocument()
        {
            using (var reader = GenerateReader(ErrorJson))
            {
                var documentParser = new DocumentParser(new ArangoDatabase());

                BaseResult baseResult = null;
                Assert.Throws<ArangoServerException>(() => documentParser.ParseBatchResult<Person>(reader, out baseResult));
            }
        }

        [Fact]
        public void ParseBatchErrorDocumentWithNotThrowing()
        {
            using (var reader = GenerateReader(ErrorJson))
            {
                var db = new ArangoDatabase();
                db.Setting.ThrowForServerErrors = false;
                var documentParser = new DocumentParser(db);

                BaseResult baseResult = null;
                var list = documentParser.ParseBatchResult<Person>(reader, out baseResult);

                Assert.Equal(list.Count, 0);

                Assert.Equal(baseResult.Error, true);
                Assert.Equal(baseResult.Code, 404);
                Assert.Equal(baseResult.ErrorMessage, "error message");
                Assert.Equal(baseResult.ErrorNum, 1202);
            }
        }

        [Fact]
        public void ParseSingleError()
        {
            using (var reader = GenerateReader(ErrorJson))
            {
                var documentParser = new DocumentParser(new ArangoDatabase());
                JObject jObject = null;
                var person = documentParser.ParseSingleResult<Person>(reader, out jObject, true);

                // person wont be null here because we have no base result yet 
                Assert.Equal(person.Age, 0);
                Assert.Null(person.Fullname);
                Assert.Equal(person.Height, 0);
            }
        }
    }
}
