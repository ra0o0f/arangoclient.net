using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            using (var reader = GenerateReader(JsonSample.SingleResult))
            {
                var documentParser = new DocumentParser(new ArangoDatabase());
                JObject jObject = null;
                var person = documentParser.ParseSingleResult<Person>(reader, out jObject, true);

                Assert.Equal(27, person.Age);
                Assert.Equal("raoof hojat", person.Fullname);
                Assert.Equal(172, person.Height);

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

                Assert.Equal(2, personList.Count);

                Assert.Equal(27, personList[0].Age);
                Assert.Equal("raoof hojat", personList[0].Fullname);
                Assert.Equal(172, personList[0].Height);

                Assert.Equal(7, personList[1].Age);
                Assert.Equal("hojat raoof", personList[1].Fullname);
                Assert.Equal(721, personList[1].Height);

                var info1 = db.FindDocumentInfo(personList[0]);
                Assert.NotNull(info1.Document);
                Assert.Equal("Person/KEY1", info1.Id);
                Assert.Equal("KEY1", info1.Key);
                Assert.Equal("REV1", info1.Rev);

                var info2 = db.FindDocumentInfo(personList[1]);
                Assert.NotNull(info2.Document);
                Assert.Equal("Person/KEY2", info2.Id);
                Assert.Equal("KEY2", info2.Key);
                Assert.Equal("REV2", info2.Rev);

                Assert.Equal(200, baseResult.Code);
                Assert.False(baseResult.Error);
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

                Assert.Single(personList);

                var person = personList[0];

                Assert.Equal(27, person.Age);
                Assert.Equal("raoof hojat", person.Fullname);
                Assert.Equal(172, person.Height);

                Assert.Equal(200, baseResult.Code);
                Assert.False(baseResult.Error);
            }
        }

        [Fact]
        public void ParseBatchErrorDocument()
        {
            using (var reader = GenerateReader(JsonSample.Error))
            {
                var documentParser = new DocumentParser(new ArangoDatabase());

                BaseResult baseResult = null;

                var result = documentParser.ParseBatchResult<Person>(reader, out baseResult);

                Assert.NotNull(result);
                Assert.Empty(result);

                // old behavior
                //Assert.Throws<ArangoServerException>(() => documentParser.ParseBatchResult<Person>(reader, out baseResult));
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

                Assert.Empty(list);

                Assert.True(baseResult.Error);
                Assert.Equal(400, baseResult.Code);
                Assert.Equal("ERROR", baseResult.ErrorMessage);
                Assert.Equal(1202, baseResult.ErrorNum);
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
                Assert.Equal(0, person.Age);
                Assert.Null(person.Fullname);
                Assert.Equal(0, person.Height);
            }
        }
    }
}
