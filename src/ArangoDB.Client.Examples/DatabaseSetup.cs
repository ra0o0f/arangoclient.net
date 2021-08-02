using ArangoDB.Client.Data;
using ArangoDB.Client.Examples.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Examples
{
    public abstract class TestDatabaseSetup : IDisposable
    {
        protected IArangoDatabase db;

        static NetworkCredential GetCredential()
        {
            return new NetworkCredential("root", "123456");
        }

        static Lazy<DatabaseSharedSetting> SharedSetting = new Lazy<DatabaseSharedSetting>(() =>
        {
            // Uncomment if you want your requests goes through a proxy
            // ArangoDatabase.ClientSetting.Proxy = new System.Net.WebProxy("http://localhost:19777");

            var sharedSetting = new DatabaseSharedSetting();
            sharedSetting.Database = "ExampleDB";
            sharedSetting.Url = "http://localhost.:8529";

            var credential = GetCredential();

            sharedSetting.SystemDatabaseCredential = new NetworkCredential(
                credential.UserName,
                credential.Password);
            sharedSetting.Credential = new NetworkCredential(
                credential.UserName,
                credential.Password);

            sharedSetting.Collection.ChangeIdentifierDefaultName(IdentifierType.Key, "Key");

            using (var _db = new ArangoDatabase(sharedSetting))
            {
                if (!_db.ListDatabases().Contains(sharedSetting.Database))
                    _db.CreateDatabase(sharedSetting.Database, new List<DatabaseUser>
                    {
                        new DatabaseUser
                        {
                            Username = credential.UserName,
                            Passwd = credential.Password
                        }
                    });

                var collections = _db.ListCollections().Select(c => c.Name).ToArray();
                var collectionsToCreate = new Tuple<string, CollectionType>[] {
                    new Tuple<string, CollectionType>("Person", CollectionType.Document),
                    new Tuple<string, CollectionType>("hosts", CollectionType.Document),
                    new Tuple<string, CollectionType>("Follow", CollectionType.Edge),
                    new Tuple<string, CollectionType>("Relation", CollectionType.Edge)
                };

                foreach (var c in collectionsToCreate)
                    if (collections.Contains(c.Item1) == false)
                        _db.CreateCollection(c.Item1, type: c.Item2);
            }

            return sharedSetting;
        });

        public TestDatabaseSetup()
        {
            var sharedSetting = SharedSetting.Value;

            db = new ArangoDatabase(sharedSetting);

            ClearDatabase();
        }

        private void ClearDatabase()
        {
            db.Query<Person>().Remove().Execute();
            db.Query<Follow>().Remove().Execute();
            db.Query<Host>().Remove().Execute();

            if (db.ListGraphs().Count(x => x.Key == "SocialGraph") != 0)
                db.Graph("SocialGraph").Drop();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
