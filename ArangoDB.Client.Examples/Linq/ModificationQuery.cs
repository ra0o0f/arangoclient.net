using ArangoDB.Client.Examples.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArangoDB.Client.Examples.Linq
{
    public class ModificationQuery : TestDatabaseSetup
    {
        [Fact]
        public void Upsert()
        {
            // first remove all hosts
            db.Query<Host>().Remove().Execute();

            var query = db.Query()
                .Upsert<Host>(_ => new Host { Ip = "192.168.173.94" },
                                  _ => new Host { Ip = "192.168.173.94", Name = "chorweiler", Tags = new string[] { "development" } },
                                 (_, o) => new Host { Tags = AQL.Push(o.Tags, "production", true) })
                                 .In<Host>();

            // execute query for the first time
            var firstResult = query.ToList();

            Assert.Equal(firstResult.Count, 1);
            Assert.Equal(firstResult[0].Tags.Count, 1);
            Assert.Equal(firstResult[0].Tags[0], "development");

            // execute query for the second time
            var secondResult = query.ToList();

            Assert.Equal(secondResult.Count, 1);
            Assert.Equal(secondResult[0].Tags.Count, 2);
            Assert.Equal(secondResult[0].Tags[0], "development");
            Assert.Equal(secondResult[0].Tags[1], "production");
        }

        [Fact]
        public void UpsertOnCollection()
        {
            // first remove all hosts
            db.Query<Host>().Remove().Execute();

            // insert a host
            db.Insert<Host>(new Host { Key = "123", Tags = new string[] { "1", "2", "3" } });

            var result = db.Query<Host>()
                .Upsert(h => new Host { Key = h.Key },
                h => new Host { },
                (h, old) => new Host { Tags = AQL.Append(h.Tags, old.Tags) }).ToList();

            Assert.Equal(result.Count, 1);
            Assert.Equal(result[0].Tags.Count, 6);
            Assert.Equal(result[0].Tags.Count(t => t == "2"), 2);
        }
    }
}
