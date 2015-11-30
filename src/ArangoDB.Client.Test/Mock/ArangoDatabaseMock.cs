using ArangoDB.Client.ChangeTracking;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Test.Mock
{
    public class ArangoDatabaseMock
    {
        Mock<IArangoDatabase> mockDB = new Mock<IArangoDatabase>();

        public IArangoDatabase Db
        {
            get { return mockDB.Object; }
        }

        public ArangoDatabaseMock()
        {
            WithDefaultSetting();
        }

        public Mock<IArangoDatabase> GetMock()
        {
            return mockDB;
        }

        public ArangoDatabaseMock CallBase()
        {
            mockDB.CallBase = true;
            return this;
        }

        public ArangoDatabaseMock WithDefaultSetting()
        {
            mockDB.Setup(x => x.SharedSetting).Returns(new DatabaseSharedSetting());
            mockDB.Setup(x => x.Setting).Returns(new DatabaseSetting(Db.SharedSetting));

            return this;
        }

        public ArangoDatabaseMock WithChangeTracking()
        {
            mockDB.Setup(x => x.ChangeTracker).Returns(new DocumentTracker(Db));

            Func<object, DocumentContainer> valueFunction = x => Db.ChangeTracker.FindDocumentInfo(x);
            mockDB.Setup(x => x.FindDocumentInfo(It.IsAny<object>())).Returns(valueFunction);

            return this;
        }

        public ArangoDatabaseMock SendCommand(string json, HttpStatusCode? statusCode = HttpStatusCode.OK)
        {
            mockDB.Setup(x => x.Connection
                .SendCommandAsync(It.IsAny<HttpMethod>(), It.IsAny<Uri>(), It.IsAny<object>(), It.IsAny<NetworkCredential>()))
                .ReturnsAsync(new HttpResponseMessage() { StatusCode = statusCode.Value, Content = new StringContent(json) });

            return this;
        }

        public ArangoDatabaseMock SendCommandSequence(IList<string> json, HttpStatusCode? statusCode = HttpStatusCode.OK)
        {
            var setup = mockDB.SetupSequence(x => x.Connection
                .SendCommandAsync(It.IsAny<HttpMethod>(), It.IsAny<Uri>(), It.IsAny<object>(), It.IsAny<NetworkCredential>()));


            foreach(var j in json)
                setup.Returns(Task.FromResult(new HttpResponseMessage() { StatusCode = statusCode.Value, Content = new StringContent(j) }));

            return this;
        }
    }
}
