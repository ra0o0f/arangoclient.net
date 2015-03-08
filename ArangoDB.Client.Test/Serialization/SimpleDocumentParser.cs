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

        

        [Fact]
        public void ParseSingle()
        {
            using(var reader = GenerateReader(JsonSample.SingleResult))
            {
                var documentParser = new DocumentParser(new ArangoDatabase());
                JObject jObject=null;
                var person = documentParser.ParseSingleResult<Person>(reader, out jObject, true);

                Assert.Equal(person.Age, 27);
                Assert.Equal(person.Fullname, "raoof hojat");
                Assert.Equal(person.Height, 172);

                Assert.True(JObject.DeepEquals(jObject, JObject.Parse(JsonSample.SingleResult)));
            }
        }

        [Fact]
        public void ParseBatchList()
        {
            using (var reader = GenerateReader(JsonSample.ListResult))
            {
                var db = new ArangoDatabase();

                var documentParser = new DocumentParser(db);

                BaseResult baseResult = null;
                var personList = documentParser.ParseBatchResult<Person>(reader, out baseResult);

                Assert.Equal(personList.Count, 2);

                Assert.Equal(personList[0].Age, 27);
                Assert.Equal(personList[0].Fullname, "raoof hojat");
                Assert.Equal(personList[0].Height, 172);

                Assert.Equal(personList[1].Age, 7);
                Assert.Equal(personList[1].Fullname, "hojat raoof");
                Assert.Equal(personList[1].Height, 721);

                var info1 = db.FindDocumentInfo(personList[0]);
                Assert.NotNull(info1.Document);
                Assert.Equal(info1.Id, "Person/KEY1");
                Assert.Equal(info1.Key, "KEY1");
                Assert.Equal(info1.Rev, "REV1");

                var info2 = db.FindDocumentInfo(personList[1]);
                Assert.NotNull(info2.Document);
                Assert.Equal(info2.Id, "Person/KEY2");
                Assert.Equal(info2.Key, "KEY2");
                Assert.Equal(info2.Rev, "REV2");

                Assert.Equal(baseResult.Code, 200);
                Assert.Equal(baseResult.Error, false);
            }
        }

        [Fact]
        public void ParseBatchDocument()
        {
            using (var reader = GenerateReader(JsonSample.NestedSingleResult))
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
            using (var reader = GenerateReader(JsonSample.Error))
            {
                var documentParser = new DocumentParser(new ArangoDatabase());

                BaseResult baseResult = null;
                Assert.Throws<ArangoServerException>(() => documentParser.ParseBatchResult<Person>(reader, out baseResult));
            }
        }

        [Fact]
        public void ParseBatchErrorDocumentWithNotThrowing()
        {
            using (var reader = GenerateReader(JsonSample.Error))
            {
                var db = new ArangoDatabase();
                db.Setting.ThrowForServerErrors = false;
                var documentParser = new DocumentParser(db);

                BaseResult baseResult = null;
                var list = documentParser.ParseBatchResult<Person>(reader, out baseResult);

                Assert.Equal(list.Count, 0);

                Assert.Equal(baseResult.Error, true);
                Assert.Equal(baseResult.Code, 400);
                Assert.Equal(baseResult.ErrorMessage, "ERROR");
                Assert.Equal(baseResult.ErrorNum, 1202);
            }
        }

        [Fact]
        public void ParseSingleError()
        {
            using (var reader = GenerateReader(JsonSample.Error))
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
