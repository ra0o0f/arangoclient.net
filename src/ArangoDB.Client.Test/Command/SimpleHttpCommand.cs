using Newtonsoft.Json.Linq;
using ArangoDB.Client.Data;
using ArangoDB.Client.Http;
using ArangoDB.Client.Test.Mock;
using ArangoDB.Client.Test.Model;
using ArangoDB.Client.Test.Serialization;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArangoDB.Client.Test.Command
{
    public class SimpleHttpCommand
    {
        [Fact]
        public async Task DistinctResult()
        {
            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            mockDB.SendCommand(JsonSample.SingleResult);

            HttpCommand command = new HttpCommand(mockDB.Db) { EnableChangeTracking = !mockDB.Db.Setting.DisableChangeTracking };

            var result = await command.RequestDistinctResult<Person>();

            Assert.Null(result.BaseResult.Code);
            Assert.False(result.BaseResult.HasError());

            Assert.Equal(27, result.Result.Age);
            Assert.Equal("raoof hojat", result.Result.Fullname);
            Assert.Equal(172, result.Result.Height);

            var info = mockDB.Db.FindDocumentInfo(result.Result);
            Assert.Equal("Person/KEY", info.Id);
            Assert.Equal("KEY", info.Key);
            Assert.Equal("REV", info.Rev);
            Assert.True(JObject.DeepEquals(info.Document, JObject.Parse(JsonSample.SingleResult)));
        }

        [Fact]
        public async Task DistinctResult_WithoutChangeTracking()
        {
            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            mockDB.SendCommand(JsonSample.SingleResult);

            mockDB.Db.Setting.DisableChangeTracking = true;

            HttpCommand command = new HttpCommand(mockDB.Db) { EnableChangeTracking = !mockDB.Db.Setting.DisableChangeTracking };

            var result = await command.RequestDistinctResult<Person>();

            Assert.Null(result.BaseResult.Code);
            Assert.False(result.BaseResult.HasError());

            Assert.Equal(27, result.Result.Age);
            Assert.Equal("raoof hojat", result.Result.Fullname);
            Assert.Equal(172, result.Result.Height);

            Assert.Throws<Exception>(() => mockDB.Db.FindDocumentInfo(result.Result));
        }

        [Fact]
        public async Task DistinctResult_BadRequest()
        {
            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            mockDB.SendCommand(JsonSample.Error, statusCode: HttpStatusCode.NotFound);

            HttpCommand command = new HttpCommand(mockDB.Db) { EnableChangeTracking = !mockDB.Db.Setting.DisableChangeTracking };

            await Assert.ThrowsAsync<ArangoServerException>(() => command.RequestDistinctResult<Person>());
        }

        [Fact]
        public async Task DistinctResult_WithoutChangeTracking_BadRequest()
        {
            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            mockDB.SendCommand(JsonSample.Error, statusCode: HttpStatusCode.NotFound);

            mockDB.Db.Setting.DisableChangeTracking = true;

            HttpCommand command = new HttpCommand(mockDB.Db) { EnableChangeTracking = !mockDB.Db.Setting.DisableChangeTracking };

            await Assert.ThrowsAsync<ArangoServerException>(() => command.RequestDistinctResult<Person>());
        }

        [Fact]
        public async Task DistinctResult_BadRequest_DontThrow()
        {
            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            mockDB.SendCommand(JsonSample.Error, statusCode: HttpStatusCode.NotFound);

            mockDB.Db.Setting.ThrowForServerErrors = false;

            HttpCommand command = new HttpCommand(mockDB.Db) { EnableChangeTracking = !mockDB.Db.Setting.DisableChangeTracking };

            var result = await command.RequestDistinctResult<Person>();

            Assert.Null(result.Result);
            Assert.Equal(400, result.BaseResult.Code);
            Assert.True(result.BaseResult.Error);
            Assert.Equal("ERROR", result.BaseResult.ErrorMessage);
        }

        [Fact]
        public async Task DistinctResult_WithoutChangeTracking_BadRequest_DontThrow()
        {
            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            mockDB.SendCommand(JsonSample.Error, statusCode: HttpStatusCode.NotFound);

            mockDB.Db.Setting.DisableChangeTracking = true;
            mockDB.Db.Setting.ThrowForServerErrors = false;

            HttpCommand command = new HttpCommand(mockDB.Db) { EnableChangeTracking = !mockDB.Db.Setting.DisableChangeTracking };

            var result = await command.RequestDistinctResult<Person>();

            Assert.Null(result.Result);
            Assert.Equal(400, result.BaseResult.Code);
            Assert.True(result.BaseResult.Error);
            Assert.Equal("ERROR", result.BaseResult.ErrorMessage);
        }

        [Fact]
        public async Task RequestGenericSingle()
        {
            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            mockDB.SendCommand(JsonSample.NestedSingleResult);

            HttpCommand command = new HttpCommand(mockDB.Db) { EnableChangeTracking = !mockDB.Db.Setting.DisableChangeTracking };

            var result = await command.RequestGenericSingleResult<Person, DocumentInheritedCommandResult<Person>>();

            Assert.Equal(200, result.BaseResult.Code);
            Assert.False(result.BaseResult.Error);

            Assert.Equal(27, result.Result.Age);
            Assert.Equal("raoof hojat", result.Result.Fullname);
            Assert.Equal(172, result.Result.Height);

            var info = mockDB.Db.FindDocumentInfo(result.Result);
            Assert.Equal("Person/KEY", info.Id);
            Assert.Equal("KEY", info.Key);
            Assert.Equal("REV", info.Rev);
            Assert.True(JObject.DeepEquals(info.Document, JObject.Parse(JsonSample.SingleResult)));
        }

        [Fact]
        public async Task RequestGenericSingle_WithoutChangeTracking()
        {
            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            mockDB.SendCommand(JsonSample.NestedSingleResult);

            mockDB.Db.Setting.DisableChangeTracking = true;

            HttpCommand command = new HttpCommand(mockDB.Db) { EnableChangeTracking = !mockDB.Db.Setting.DisableChangeTracking };

            var result = await command.RequestGenericSingleResult<Person, DocumentInheritedCommandResult<Person>>();

            Assert.Equal(200, result.BaseResult.Code);
            Assert.False(result.BaseResult.Error);

            Assert.Equal(27, result.Result.Age);
            Assert.Equal("raoof hojat", result.Result.Fullname);
            Assert.Equal(172, result.Result.Height);

            Assert.Throws<Exception>(() => mockDB.Db.FindDocumentInfo(result.Result));
        }

        [Fact]
        public async Task RequestGenericSingle_BadRequest()
        {
            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            mockDB.SendCommand(JsonSample.Error, statusCode: HttpStatusCode.NotFound);

            HttpCommand command = new HttpCommand(mockDB.Db) { EnableChangeTracking = !mockDB.Db.Setting.DisableChangeTracking };

            await Assert.ThrowsAsync<ArangoServerException>(() => command.RequestGenericSingleResult<Person, DocumentInheritedCommandResult<Person>>());
        }

        [Fact]
        public async Task RequestGenericSingle_WithoutChangeTracking_BadRequest()
        {
            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            mockDB.SendCommand(JsonSample.Error, statusCode: HttpStatusCode.NotFound);

            mockDB.Db.Setting.DisableChangeTracking = true;

            HttpCommand command = new HttpCommand(mockDB.Db) { EnableChangeTracking = !mockDB.Db.Setting.DisableChangeTracking };

            await Assert.ThrowsAsync<ArangoServerException>(() => command.RequestGenericSingleResult<Person, DocumentInheritedCommandResult<Person>>());
        }

        [Fact]
        public async Task RequestGenericSingle_BadRequest_DontThrow()
        {
            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            mockDB.SendCommand(JsonSample.Error, statusCode: HttpStatusCode.NotFound);

            mockDB.Db.Setting.ThrowForServerErrors = false;

            HttpCommand command = new HttpCommand(mockDB.Db) { EnableChangeTracking = !mockDB.Db.Setting.DisableChangeTracking };

            var result = await command.RequestGenericSingleResult<Person, DocumentInheritedCommandResult<Person>>();

            Assert.Null(result.Result);
            Assert.Equal(400, result.BaseResult.Code);
            Assert.True(result.BaseResult.Error);
            Assert.Equal("ERROR", result.BaseResult.ErrorMessage);
        }

        [Fact]
        public async Task RequestGenericSingle_WithoutChangeTracking_BadRequest_DontThrow()
        {
            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            mockDB.SendCommand(JsonSample.Error, statusCode: HttpStatusCode.NotFound);

            mockDB.Db.Setting.DisableChangeTracking = true;
            mockDB.Db.Setting.ThrowForServerErrors = false;

            HttpCommand command = new HttpCommand(mockDB.Db) { EnableChangeTracking = !mockDB.Db.Setting.DisableChangeTracking };

            var result = await command.RequestGenericSingleResult<Person, DocumentInheritedCommandResult<Person>>();

            Assert.Null(result.Result);
            Assert.Equal(400, result.BaseResult.Code);
            Assert.True(result.BaseResult.Error);
            Assert.Equal("ERROR", result.BaseResult.ErrorMessage);
        }

        [Fact]
        public async Task RequestGenericList()
        {
            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            mockDB.SendCommand(JsonSample.ListResult);

            HttpCommand command = new HttpCommand(mockDB.Db) { EnableChangeTracking = !mockDB.Db.Setting.DisableChangeTracking };

            var result = await command.RequestGenericListResult<Person, DocumentInheritedCommandResult<List<Person>>>();

            Assert.Equal(200, result.BaseResult.Code);
            Assert.False(result.BaseResult.Error);

            Assert.Equal(2, result.Result.Count);

            Assert.Equal(27, result.Result[0].Age);
            Assert.Equal("raoof hojat", result.Result[0].Fullname);
            Assert.Equal(172, result.Result[0].Height);

            var info1 = mockDB.Db.FindDocumentInfo(result.Result[0]);
            Assert.Equal("Person/KEY1", info1.Id);
            Assert.Equal("KEY1", info1.Key);
            Assert.Equal("REV1", info1.Rev);

            Assert.Equal(7, result.Result[1].Age);
            Assert.Equal("hojat raoof", result.Result[1].Fullname);
            Assert.Equal(721, result.Result[1].Height);

            var info2 = mockDB.Db.FindDocumentInfo(result.Result[1]);
            Assert.NotNull(info2.Document);
            Assert.Equal("Person/KEY2", info2.Id);
            Assert.Equal("KEY2", info2.Key);
            Assert.Equal("REV2", info2.Rev);
        }

        [Fact]
        public async Task RequestGenericList_WithoutChangeTracking()
        {
            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            mockDB.SendCommand(JsonSample.ListResult);

            mockDB.Db.Setting.DisableChangeTracking = true;

            HttpCommand command = new HttpCommand(mockDB.Db) { EnableChangeTracking = !mockDB.Db.Setting.DisableChangeTracking };

            var result = await command.RequestGenericListResult<Person, DocumentInheritedCommandResult<List<Person>>>();

            Assert.Equal(200, result.BaseResult.Code);
            Assert.False(result.BaseResult.Error);

            Assert.Equal(2, result.Result.Count);

            Assert.Equal(27, result.Result[0].Age);
            Assert.Equal("raoof hojat", result.Result[0].Fullname);
            Assert.Equal(172, result.Result[0].Height);

            Assert.Throws<Exception>(() => mockDB.Db.FindDocumentInfo(result.Result[0]));

            Assert.Equal(7, result.Result[1].Age);
            Assert.Equal("hojat raoof", result.Result[1].Fullname);
            Assert.Equal(721, result.Result[1].Height);

            Assert.Throws<Exception>(() => mockDB.Db.FindDocumentInfo(result.Result[1]));
        }

        [Fact]
        public async Task RequestGenericList_BadRequest()
        {
            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            mockDB.SendCommand(JsonSample.Error, statusCode: HttpStatusCode.NotFound);

            HttpCommand command = new HttpCommand(mockDB.Db) { EnableChangeTracking = !mockDB.Db.Setting.DisableChangeTracking };

            await Assert.ThrowsAsync<ArangoServerException>(() => command.RequestGenericListResult<Person, DocumentInheritedCommandResult<List<Person>>>());
        }

        [Fact]
        public async Task RequestGenericList_WithoutChangeTracking_BadRequest()
        {
            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            mockDB.SendCommand(JsonSample.Error, statusCode: HttpStatusCode.NotFound);

            mockDB.Db.Setting.DisableChangeTracking = true;

            HttpCommand command = new HttpCommand(mockDB.Db) { EnableChangeTracking = !mockDB.Db.Setting.DisableChangeTracking };

            await Assert.ThrowsAsync<ArangoServerException>(() => command.RequestGenericListResult<Person, DocumentInheritedCommandResult<List<Person>>>());
        }

        [Fact]
        public async Task RequestGenericList_BadRequest_DontThrow()
        {
            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            mockDB.SendCommand(JsonSample.Error, statusCode: HttpStatusCode.NotFound);

            mockDB.Db.Setting.ThrowForServerErrors = false;

            HttpCommand command = new HttpCommand(mockDB.Db) { EnableChangeTracking = !mockDB.Db.Setting.DisableChangeTracking };

            var result = await command.RequestGenericListResult<Person, DocumentInheritedCommandResult<List<Person>>>();

            Assert.Empty(result.Result);
            Assert.Equal(400, result.BaseResult.Code);
            Assert.True(result.BaseResult.Error);
            Assert.Equal("ERROR", result.BaseResult.ErrorMessage);
        }

        [Fact]
        public async Task RequestGenericList_WithoutChangeTracking_BadRequest_DontThrow()
        {
            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            mockDB.SendCommand(JsonSample.Error, statusCode: HttpStatusCode.NotFound);

            mockDB.Db.Setting.DisableChangeTracking = true;
            mockDB.Db.Setting.ThrowForServerErrors = false;

            HttpCommand command = new HttpCommand(mockDB.Db) { EnableChangeTracking = !mockDB.Db.Setting.DisableChangeTracking };

            var result = await command.RequestGenericListResult<Person, DocumentInheritedCommandResult<List<Person>>>();

            Assert.Empty(result.Result);
            Assert.Equal(400, result.BaseResult.Code);
            Assert.True(result.BaseResult.Error);
            Assert.Equal("ERROR", result.BaseResult.ErrorMessage);
        }


        [Fact]
        public async Task RequestMergedResult()
        {
            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            mockDB.SendCommand(JsonSample.MergeResult);

            HttpCommand command = new HttpCommand(mockDB.Db) { EnableChangeTracking = !mockDB.Db.Setting.DisableChangeTracking };

            var result = await command.RequestMergedResult<Flight>();

            Assert.Equal(202, result.BaseResult.Code);
            Assert.False(result.BaseResult.Error);

            Assert.Equal("AIRLINE", result.Result.Airline);
            Assert.Equal("10012314", result.Result.FlightNumber);

            Assert.Throws<Exception>(() => mockDB.Db.FindDocumentInfo(result.Result));
        }

        [Fact]
        public async Task RequestMerged_BadRequest()
        {
            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            mockDB.SendCommand(JsonSample.Error, statusCode: HttpStatusCode.NotFound);

            HttpCommand command = new HttpCommand(mockDB.Db) { EnableChangeTracking = !mockDB.Db.Setting.DisableChangeTracking };

            await Assert.ThrowsAsync<ArangoServerException>(() => command.RequestMergedResult<Flight>());
        }

        [Fact]
        public async Task RequestMerged_BadRequest_DontThrow()
        {
            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            mockDB.SendCommand(JsonSample.Error, statusCode: HttpStatusCode.NotFound);

            mockDB.Db.Setting.ThrowForServerErrors = false;

            HttpCommand command = new HttpCommand(mockDB.Db) { EnableChangeTracking = !mockDB.Db.Setting.DisableChangeTracking };

            var result = await command.RequestMergedResult<Flight>();

            Assert.NotNull(result.Result);
            Assert.Equal(400, result.BaseResult.Code);
            Assert.True(result.BaseResult.Error);
            Assert.Equal("ERROR", result.BaseResult.ErrorMessage);
        }

        [Fact]
        public void RequestCursor_WithStatsComesFirst()
        {
            CreateRequestCursor(json: JsonSample.CursorLastResult_WithStatsComesFirst);
        }

        [Fact]
        public void RequestCursor_WithStatsInTheMiddle()
        {
            CreateRequestCursor(json: JsonSample.CursorLastResult_WithStatsInTheMiddle);
        }

        [Fact]
        public void RequestCursor_WithStatsComesLast()
        {
            CreateRequestCursor(json: JsonSample.CursorLastResult_WithStatsComesLast);
        }

        [Fact]
        public void RequestCursor_WithoutChangeTracking_WithStatsComesFirst()
        {
            CreateRequestCursor(json: JsonSample.CursorLastResult_WithStatsComesFirst, disableChangeTracking: true);
        }

        [Fact]
        public void RequestCursor_WithoutChangeTracking_WithStatsInTheMiddle()
        {
            CreateRequestCursor(json: JsonSample.CursorLastResult_WithStatsInTheMiddle, disableChangeTracking: true);
        }

        [Fact]
        public void RequestCursor_WithoutChangeTracking_WithStatsComesLast()
        {
            CreateRequestCursor(json: JsonSample.CursorLastResult_WithStatsComesLast, disableChangeTracking: true);
        }

        [Fact]
        public void RequestCursorWithMoreResult()
        {
            CreateRequestCursor(jsonList: new string[]{
                JsonSample.CursorFirstResult_WithStatsComesFirst,
                JsonSample.CursorLastResult_WithStatsComesFirst
            });
        }

        [Fact]
        public void RequestCursorWithMoreResult_WithoutChangeTracking()
        {
            CreateRequestCursor(jsonList: new string[]{
                JsonSample.CursorFirstResult_WithStatsComesFirst,
                JsonSample.CursorLastResult_WithStatsComesFirst
            });
        }

        [Fact]
        public void RequestCursorError()
        {
            CreateRequestCursor(json: JsonSample.Error, error: true);
        }

        [Fact]
        public void RequestCursorError_DontThrow()
        {
            CreateRequestCursor(json: JsonSample.Error, error: true, throwOnError: false);
        }

        void CreateRequestCursor(string json = null, IList<string> jsonList = null, bool? disableChangeTracking = false, bool? error = false, bool? throwOnError = true)
        {
            bool hasMore = string.IsNullOrEmpty(json);

            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            if (!hasMore)
                mockDB.SendCommand(json, statusCode: error.Value ? HttpStatusCode.NotFound : HttpStatusCode.OK);
            else
                mockDB.SendCommandSequence(jsonList, statusCode: error.Value ? HttpStatusCode.NotFound : HttpStatusCode.OK);

            mockDB.Db.Setting.DisableChangeTracking = disableChangeTracking.Value;
            mockDB.Db.Setting.ThrowForServerErrors = throwOnError.Value;

            HttpCommand command = new HttpCommand(mockDB.Db);

            var cursor = command.CreateCursor<Person>();

            if (error.Value)
            {
                if (throwOnError.Value)
                    Assert.Throws<ArangoServerException>(() => cursor.ToList());
                else
                {
                    var persons = cursor.ToList();
                    Assert.Empty(persons);
                    Assert.Equal(400, cursor.Statistics.Code);
                    Assert.True(cursor.Statistics.Error);
                    Assert.Equal("ERROR", cursor.Statistics.ErrorMessage);
                }
            }
            else
            {
                var persons = cursor.ToList();

                Assert.Equal(201, cursor.Statistics.Code);
                Assert.Equal(2, cursor.Statistics.Count);
                Assert.False(cursor.Statistics.Error);
                Assert.Equal("26011191", cursor.Statistics.Id);
                Assert.Equal(cursor.Statistics.RequestCount, hasMore ? 2 : 1);
                Assert.False(cursor.Statistics.HasMore);
                Assert.Equal(6, cursor.Statistics.Extra.Stats.FullCount);
                Assert.Equal(3, cursor.Statistics.Extra.Stats.ScannedFull);
                Assert.Equal(4, cursor.Statistics.Extra.Stats.ScannedIndex);
                Assert.Equal(1, cursor.Statistics.Extra.Stats.WritesExecuted);
                Assert.Equal(2, cursor.Statistics.Extra.Stats.WritesIgnored);

                Assert.Equal(persons.Count, hasMore ? 4 : 2);

                int index = 0;
                if (hasMore)
                {
                    Assert.Equal(27, persons[index].Age);
                    Assert.Equal("raoof hojat2", persons[index].Fullname);
                    Assert.Equal(172, persons[index].Height);

                    if (disableChangeTracking.Value)
                        Assert.Throws<Exception>(() => mockDB.Db.FindDocumentInfo(persons[index]));
                    else
                    {
                        var info3 = mockDB.Db.FindDocumentInfo(persons[index]);
                        Assert.Equal("Person/KEY3", info3.Id);
                        Assert.Equal("KEY3", info3.Key);
                        Assert.Equal("REV3", info3.Rev);
                    }

                    index++;

                    Assert.Equal(7, persons[index].Age);
                    Assert.Equal("hojat raoof2", persons[index].Fullname);
                    Assert.Equal(721, persons[index].Height);

                    if (disableChangeTracking.Value)
                        Assert.Throws<Exception>(() => mockDB.Db.FindDocumentInfo(persons[index]));
                    else
                    {
                        var info4 = mockDB.Db.FindDocumentInfo(persons[index]);
                        Assert.NotNull(info4.Document);
                        Assert.Equal("Person/KEY4", info4.Id);
                        Assert.Equal("KEY4", info4.Key);
                        Assert.Equal("REV4", info4.Rev);
                    }

                    index++;
                }

                Assert.Equal(27, persons[index].Age);
                Assert.Equal("raoof hojat", persons[index].Fullname);
                Assert.Equal(172, persons[index].Height);

                if (disableChangeTracking.Value)
                    Assert.Throws<Exception>(() => mockDB.Db.FindDocumentInfo(persons[index]));
                else
                {
                    var info1 = mockDB.Db.FindDocumentInfo(persons[index]);
                    Assert.Equal("Person/KEY1", info1.Id);
                    Assert.Equal("KEY1", info1.Key);
                    Assert.Equal("REV1", info1.Rev);
                }

                index++;

                Assert.Equal(7, persons[index].Age);
                Assert.Equal("hojat raoof", persons[index].Fullname);
                Assert.Equal(721, persons[index].Height);

                if (disableChangeTracking.Value)
                    Assert.Throws<Exception>(() => mockDB.Db.FindDocumentInfo(persons[index]));
                else
                {
                    var info2 = mockDB.Db.FindDocumentInfo(persons[index]);
                    Assert.NotNull(info2.Document);
                    Assert.Equal("Person/KEY2", info2.Id);
                    Assert.Equal("KEY2", info2.Key);
                    Assert.Equal("REV2", info2.Rev);
                }
            }
        }
    }
}
