using ArangoDB.Client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArangoDB.Client.Examples.Collections
{
    public class CollectionCommand : TestDatabaseSetup
    {
        [Fact]
        public void CreateAndDropCollection()
        {
            var createResult = db.CreateCollection("SampleCollection");

            Assert.NotNull(createResult.Id);

            var dropResult = db.DropCollection("SampleCollection");

            Assert.NotNull(dropResult.Id);
        }

        [Fact]
        public void CreateAndDropCollectionWithOptions()
        {
            var createResult = db.CreateCollection("SampleCollection",
                waitForSync: true,
                doCompact: true,
                IndexBuckets: 64,
                isSystem: false,
                isVolatile: false,
                journalSize: 2048576,
                keyOptions: new CreateCollectionKeyOption
                {
                    AllowUserKeys = true,
                    Increment = 5,
                    Offset = 20,
                    Type = KeyGeneratorType.Autoincrement
                },
                type: CollectionType.Document);

            Assert.NotNull(createResult.Id);

            var dropResult = db.DropCollection("SampleCollection");

            Assert.NotNull(dropResult.Id);
        }
    }
}
