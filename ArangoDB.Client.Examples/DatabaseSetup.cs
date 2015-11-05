using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Examples
{
    public abstract class TestDatabaseSetup : IDisposable
    {
        protected IArangoDatabase db;

        Lazy<DatabaseSharedSetting> SharedSetting = new Lazy<DatabaseSharedSetting>(() =>
        {
            var sharedSetting = new DatabaseSharedSetting();
            sharedSetting.Database = "ExampleDB";
            sharedSetting.Url = "http://localhost:8529";

            sharedSetting.Collection.ChangeIdentifierDefaultName(IdentifierType.Key, "Key");

            using (var _db = new ArangoDatabase(sharedSetting))
            {
                if (!_db.ListDatabases().Contains(sharedSetting.Database))
                    _db.CreateDatabase(sharedSetting.Database);

                var collections = _db.ListCollections().Select(c=>c.Name).ToArray();
                var collectionsToCreate = new string[]{ "Person" };

                foreach (var c in collectionsToCreate.Except(collections))
                    _db.CreateCollection(c);
            }

            return sharedSetting;
        });
        
        public TestDatabaseSetup()
        {
            var sharedSetting = SharedSetting.Value;

            db = new ArangoDatabase(sharedSetting);
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
