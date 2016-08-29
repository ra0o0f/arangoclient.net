using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArangoDB.Client.Examples.Databases
{
    public class DatabaseCommand : TestDatabaseSetup
    {
        [Fact]
        public void CurrentDatabaseInformation()
        {
            var dbInfo = db.CurrentDatabaseInformation();

            Assert.False(dbInfo.IsSystem);
            Assert.Equal(dbInfo.Name, db.SharedSetting.Database);
            Assert.NotNull(dbInfo.Path);
            Assert.NotNull(dbInfo.Id);
        }

        [Fact]
        public void ListAccessibleDatabases()
        {
            var dbs = db.ListAccessibleDatabases();

            Assert.Equal(dbs.Count, 1);
            Assert.Equal(dbs.Except(new string[] { db.SharedSetting.Database }).Count(), 0);
        }

        [Fact]
        public void ListDatabases()
        {
            var dbs = db.ListDatabases();

            Assert.True(dbs.Count >= 2);
            Assert.Equal(dbs.Except(new string[] { "_system", db.SharedSetting.Database }).Count(), dbs.Count - 2);
        }

        [Fact]
        public void CreateAndDropDatabase()
        {
            db.CreateDatabase("JustForTest");

            db.DropDatabase("JustForTest");
        }
    }
}
