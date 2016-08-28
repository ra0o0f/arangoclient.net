using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Examples
{
    public abstract class TestDatabaseSetup : IDisposable
    {
        protected IArangoDatabase db;

        static Lazy<DatabaseSharedSetting> SharedSetting = new Lazy<DatabaseSharedSetting>(() =>
        {
            // Uncomment if you want your requests goes through proxy
            //ArangoDatabase.ClientSetting.Proxy = new System.Net.WebProxy("http://localhost:8888");

            var sharedSetting = new DatabaseSharedSetting();
            sharedSetting.Database = "ExampleDB";
            sharedSetting.Url = "http://localhost.:8529";

            sharedSetting.SystemDatabaseCredential = new System.Net.NetworkCredential(
                ConfigurationManager.AppSettings["dbSystemUser"],
                ConfigurationManager.AppSettings["dbSystemPass"]);
            sharedSetting.Credential = new System.Net.NetworkCredential(
                ConfigurationManager.AppSettings["dbSystemUser"],
                ConfigurationManager.AppSettings["dbSystemPass"]);

            sharedSetting.Collection.ChangeIdentifierDefaultName(IdentifierType.Key, "Key");

            using (var _db = new ArangoDatabase(sharedSetting))
            {
                if (!_db.ListDatabases().Contains(sharedSetting.Database))
                    _db.CreateDatabase(sharedSetting.Database);

                var collections = _db.ListCollections().Select(c => c.Name).ToArray();
                var collectionsToCreate = new string[] { "Person", "hosts" };

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
