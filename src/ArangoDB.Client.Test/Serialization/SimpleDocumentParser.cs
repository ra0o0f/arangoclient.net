using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ArangoDB.Client.Data;
using ArangoDB.Client.Serialization;
using ArangoDB.Client.Test.Model;
using System;
using System.IO;
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
            return new ArangoJsonTextReader(streamReader);
        }


        public void ParseSingleResult<T>(string value, Action<JObject, T> assert)
        {
            using (var reader = GenerateReader(value))
            {
                var documentParser = new DocumentParser(new ArangoDatabase());
                JObject jObject = null;
                var result = documentParser.ParseSingleResult<T>(reader, out jObject, true);

                assert(jObject, result);
            }
        }

        [Fact]
        public void ParseSingle()
        {
            ParseSingleResult<Person>(JsonSample.SingleResult, (jObject, person) =>
            {
                Assert.Equal(27, person.Age);
                Assert.Equal("raoof hojat", person.Fullname);
                Assert.Equal(172, person.Height);

                Assert.True(JObject.DeepEquals(jObject, JObject.Parse(JsonSample.SingleResult)));
            });
        }


        [Fact]
        public void ParseSingle_IsoDate_ToString()
        {
            ParseSingleResult<DateModel>(DateJson.SingleDateModelResult, (jObject, result) =>
            {
                // ISO Date to String
                Assert.Equal(JTokenType.String, jObject["IsoDateAsString"].Type);
                Assert.Equal("1983-10-20", result.IsoDateAsString);

            });
        }

        [Fact]
        public void ParseSingle_IsoDate_ToDateTime()
        {
            ParseSingleResult<DateModel>(DateJson.SingleDateModelResult, (jObject, result) =>
            {
                Assert.Equal(JTokenType.String, jObject["IsoDateAsDateTime"].Type);
                Assert.Equal(new DateTime(1983, 10, 20), result.IsoDateAsDateTime);

            });
        }

        [Fact]
        public void ParseSingle_IsoDateTime_ToString()
        {
            ParseSingleResult<DateModel>(DateJson.SingleDateModelResult, (jObject, result) =>
            {
                Assert.Equal(JTokenType.String, jObject["IsoDateTimeAsString"].Type);
                Assert.Equal("2017-11-16T00:00:00Z", result.IsoDateTimeAsString);

            });
        }

        [Fact]
        public void ParseSingle_IsoDateTimeInUtc_ToDateTimeUtcKind()
        {
            ParseSingleResult<DateModel>(DateJson.SingleDateModelResult, (jObject, result) =>
            {
                Assert.Equal(new DateTime(2017, 11, 16, 02, 30, 15), result.IsoDateTimeAsDateTime);
                Assert.Equal(DateTimeKind.Utc, result.IsoDateTimeAsDateTime.Kind);

            });
        }

        [Fact]
        public void ParseSingle_IsoDateTimeInUtc_ToDateTimeOffset()
        {
            ParseSingleResult<DateModel>(DateJson.SingleDateModelResult, (jObject, result) =>
            {
                Assert.Equal(new DateTimeOffset(2017, 11, 16, 01, 00, 00, TimeSpan.Zero), result.IsoDateTimeAsDateTimeOffset);
            });
        }

        [Fact]
        public void ParseSingle_IsoDateTimeWithOffset_ToDateTimeOffset()
        {
            ParseSingleResult<DateModel>(DateJson.SingleDateModelResult, (jObject, result) =>
            {
                // "2018-02-14T13:43:12+02:30"
                Assert.Equal(new DateTimeOffset(2018, 2, 14, 13, 43, 12, new TimeSpan(2,30,0)), result.IsoDateTimeWithOffsetAsDateTimeOffset);
            });
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
