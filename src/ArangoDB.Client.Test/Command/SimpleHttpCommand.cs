using ArangoDB.Client.Common.Newtonsoft.Json.Linq;
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

            Assert.Equal(result.BaseResult.Code, 200);
            Assert.Equal(result.BaseResult.Error, false);

            Assert.Equal(result.Result.Age, 27);
            Assert.Equal(result.Result.Fullname, "raoof hojat");
            Assert.Equal(result.Result.Height, 172);

            var info = mockDB.Db.FindDocumentInfo(result.Result);
            Assert.Equal(info.Id, "Person/KEY");
            Assert.Equal(info.Key, "KEY");
            Assert.Equal(info.Rev, "REV");
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

            Assert.Equal(result.BaseResult.Code, 200);
            Assert.Equal(result.BaseResult.Error, false);

            Assert.Equal(result.Result.Age, 27);
            Assert.Equal(result.Result.Fullname, "raoof hojat");
            Assert.Equal(result.Result.Height, 172);

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
            Assert.Equal(result.BaseResult.Code, 400);
            Assert.Equal(result.BaseResult.Error, true);
            Assert.Equal(result.BaseResult.ErrorMessage, "ERROR");
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
            Assert.Equal(result.BaseResult.Code, 400);
            Assert.Equal(result.BaseResult.Error, true);
            Assert.Equal(result.BaseResult.ErrorMessage, "ERROR");
        }

        [Fact]
        public async Task RequestGenericSingle()
        {
            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            mockDB.SendCommand(JsonSample.NestedSingleResult);

            HttpCommand command = new HttpCommand(mockDB.Db) { EnableChangeTracking = !mockDB.Db.Setting.DisableChangeTracking };

            var result = await command.RequestGenericSingleResult<Person, DocumentInheritedCommandResult<Person>>();

            Assert.Equal(result.BaseResult.Code, 200);
            Assert.Equal(result.BaseResult.Error, false);

            Assert.Equal(result.Result.Age, 27);
            Assert.Equal(result.Result.Fullname, "raoof hojat");
            Assert.Equal(result.Result.Height, 172);

            var info = mockDB.Db.FindDocumentInfo(result.Result);
            Assert.Equal(info.Id, "Person/KEY");
            Assert.Equal(info.Key, "KEY");
            Assert.Equal(info.Rev, "REV");
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

            Assert.Equal(result.BaseResult.Code, 200);
            Assert.Equal(result.BaseResult.Error, false);

            Assert.Equal(result.Result.Age, 27);
            Assert.Equal(result.Result.Fullname, "raoof hojat");
            Assert.Equal(result.Result.Height, 172);

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
            Assert.Equal(result.BaseResult.Code, 400);
            Assert.Equal(result.BaseResult.Error, true);
            Assert.Equal(result.BaseResult.ErrorMessage, "ERROR");
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
            Assert.Equal(result.BaseResult.Code, 400);
            Assert.Equal(result.BaseResult.Error, true);
            Assert.Equal(result.BaseResult.ErrorMessage, "ERROR");
        }

        [Fact]
        public async Task RequestGenericList()
        {
            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            mockDB.SendCommand(JsonSample.ListResult);

            HttpCommand command = new HttpCommand(mockDB.Db) { EnableChangeTracking = !mockDB.Db.Setting.DisableChangeTracking };

            var result = await command.RequestGenericListResult<Person, DocumentInheritedCommandResult<List<Person>>>();

            Assert.Equal(result.BaseResult.Code, 200);
            Assert.Equal(result.BaseResult.Error, false);

            Assert.Equal(result.Result.Count, 2);

            Assert.Equal(result.Result[0].Age, 27);
            Assert.Equal(result.Result[0].Fullname, "raoof hojat");
            Assert.Equal(result.Result[0].Height, 172);

            var info1 = mockDB.Db.FindDocumentInfo(result.Result[0]);
            Assert.Equal(info1.Id, "Person/KEY1");
            Assert.Equal(info1.Key, "KEY1");
            Assert.Equal(info1.Rev, "REV1");

            Assert.Equal(result.Result[1].Age, 7);
            Assert.Equal(result.Result[1].Fullname, "hojat raoof");
            Assert.Equal(result.Result[1].Height, 721);

            var info2 = mockDB.Db.FindDocumentInfo(result.Result[1]);
            Assert.NotNull(info2.Document);
            Assert.Equal(info2.Id, "Person/KEY2");
            Assert.Equal(info2.Key, "KEY2");
            Assert.Equal(info2.Rev, "REV2");
        }

        [Fact]
        public async Task RequestGenericList_WithoutChangeTracking()
        {
            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            mockDB.SendCommand(JsonSample.ListResult);

            mockDB.Db.Setting.DisableChangeTracking = true;

            HttpCommand command = new HttpCommand(mockDB.Db) { EnableChangeTracking = !mockDB.Db.Setting.DisableChangeTracking };

            var result = await command.RequestGenericListResult<Person, DocumentInheritedCommandResult<List<Person>>>();

            Assert.Equal(result.BaseResult.Code, 200);
            Assert.Equal(result.BaseResult.Error, false);

            Assert.Equal(result.Result.Count, 2);

            Assert.Equal(result.Result[0].Age, 27);
            Assert.Equal(result.Result[0].Fullname, "raoof hojat");
            Assert.Equal(result.Result[0].Height, 172);

            Assert.Throws<Exception>(() => mockDB.Db.FindDocumentInfo(result.Result[0]));

            Assert.Equal(result.Result[1].Age, 7);
            Assert.Equal(result.Result[1].Fullname, "hojat raoof");
            Assert.Equal(result.Result[1].Height, 721);

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

            Assert.Equal(result.Result.Count, 0);
            Assert.Equal(result.BaseResult.Code, 400);
            Assert.Equal(result.BaseResult.Error, true);
            Assert.Equal(result.BaseResult.ErrorMessage, "ERROR");
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

            Assert.Equal(result.Result.Count, 0);
            Assert.Equal(result.BaseResult.Code, 400);
            Assert.Equal(result.BaseResult.Error, true);
            Assert.Equal(result.BaseResult.ErrorMessage, "ERROR");
        }


        [Fact]
        public async Task RequestMergedResult()
        {
            var mockDB = new ArangoDatabaseMock().WithChangeTracking();

            mockDB.SendCommand(JsonSample.MergeResult);

            HttpCommand command = new HttpCommand(mockDB.Db) { EnableChangeTracking = !mockDB.Db.Setting.DisableChangeTracking};

            var result = await command.RequestMergedResult<Flight>();

            Assert.Equal(result.BaseResult.Code, 202);
            Assert.Equal(result.BaseResult.Error, false);

            Assert.Equal(result.Result.Airline, "AIRLINE");
            Assert.Equal(result.Result.FlightNumber, "10012314");

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

            Assert.Null(result.Result);
            Assert.Equal(result.BaseResult.Code, 400);
            Assert.Equal(result.BaseResult.Error, true);
            Assert.Equal(result.BaseResult.ErrorMessage, "ERROR");
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
                    Assert.Equal(persons.Count, 0);
                    Assert.Equal(cursor.Statistics.Code, 400);
                    Assert.Equal(cursor.Statistics.Error, true);
                    Assert.Equal(cursor.Statistics.ErrorMessage, "ERROR");
                }
            }
            else
            {
                var persons = cursor.ToList();

                Assert.Equal(cursor.Statistics.Code, 201);
                Assert.Equal(cursor.Statistics.Count, 2);
                Assert.Equal(cursor.Statistics.Error, false);
                Assert.Equal(cursor.Statistics.Id, "26011191");
                Assert.Equal(cursor.Statistics.RequestCount, hasMore ? 2 : 1);
                Assert.Equal(cursor.Statistics.HasMore, false);
                Assert.Equal(cursor.Statistics.Extra.Stats.FullCount, 6);
                Assert.Equal(cursor.Statistics.Extra.Stats.ScannedFull, 3);
                Assert.Equal(cursor.Statistics.Extra.Stats.ScannedIndex, 4);
                Assert.Equal(cursor.Statistics.Extra.Stats.WritesExecuted, 1);
                Assert.Equal(cursor.Statistics.Extra.Stats.WritesIgnored, 2);

                Assert.Equal(persons.Count, hasMore ? 4 : 2);

                int index = 0;
                if (hasMore)
                {
                    Assert.Equal(persons[index].Age, 27);
                    Assert.Equal(persons[index].Fullname, "raoof hojat2");
                    Assert.Equal(persons[index].Height, 172);

                    if (disableChangeTracking.Value)
                        Assert.Throws<Exception>(() => mockDB.Db.FindDocumentInfo(persons[index]));
                    else
                    {
                        var info3 = mockDB.Db.FindDocumentInfo(persons[index]);
                        Assert.Equal(info3.Id, "Person/KEY3");
                        Assert.Equal(info3.Key, "KEY3");
                        Assert.Equal(info3.Rev, "REV3");
                    }

                    index++;

                    Assert.Equal(persons[index].Age, 7);
                    Assert.Equal(persons[index].Fullname, "hojat raoof2");
                    Assert.Equal(persons[index].Height, 721);

                    if (disableChangeTracking.Value)
                        Assert.Throws<Exception>(() => mockDB.Db.FindDocumentInfo(persons[index]));
                    else
                    {
                        var info4 = mockDB.Db.FindDocumentInfo(persons[index]);
                        Assert.NotNull(info4.Document);
                        Assert.Equal(info4.Id, "Person/KEY4");
                        Assert.Equal(info4.Key, "KEY4");
                        Assert.Equal(info4.Rev, "REV4");
                    }

                    index++;
                }

                Assert.Equal(persons[index].Age, 27);
                Assert.Equal(persons[index].Fullname, "raoof hojat");
                Assert.Equal(persons[index].Height, 172);

                if (disableChangeTracking.Value)
                    Assert.Throws<Exception>(() => mockDB.Db.FindDocumentInfo(persons[index]));
                else
                {
                    var info1 = mockDB.Db.FindDocumentInfo(persons[index]);
                    Assert.Equal(info1.Id, "Person/KEY1");
                    Assert.Equal(info1.Key, "KEY1");
                    Assert.Equal(info1.Rev, "REV1");
                }

                index++;

                Assert.Equal(persons[index].Age, 7);
                Assert.Equal(persons[index].Fullname, "hojat raoof");
                Assert.Equal(persons[index].Height, 721);

                if (disableChangeTracking.Value)
                    Assert.Throws<Exception>(() => mockDB.Db.FindDocumentInfo(persons[index]));
                else
                {
                    var info2 = mockDB.Db.FindDocumentInfo(persons[index]);
                    Assert.NotNull(info2.Document);
                    Assert.Equal(info2.Id, "Person/KEY2");
                    Assert.Equal(info2.Key, "KEY2");
                    Assert.Equal(info2.Rev, "REV2");
                }
            }
        }
    }
}
