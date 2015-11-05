using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArangoDB.Client.Examples.SetupClient
{
    public class DatabaseSetting
    {
        [Fact]
        public void SimpleSetting()
        {
            // you can setup a database-setting with just url and database-name
            using (IArangoDatabase db = new ArangoDatabase(url: "http://localhost:8529", database:"ExampleDB"))
            {
                Assert.Equal(db.SharedSetting.Database, "ExampleDB");

                Assert.Equal(db.SharedSetting.Url, "http://localhost:8529");
            }
        }

        [Fact]
        public void SharedSetting()
        {
            // you can setup a database-setting with DatabaseSharedSetting object and use it
            // everytime you create a ArangoDatabase object

            var sharedSetting = new DatabaseSharedSetting();

            // set database name
            // default: null
            sharedSetting.Database = "ExampleDB";

            // set ArangoDB server endpoint 
            // default: "http://localhost:8529"
            sharedSetting.Url = "http://localhost:8529";

            // credential for database, you dont need it if authentication is disabled
            // default: new NetworkCredential("root", "")
            sharedSetting.Credential = new NetworkCredential("user", "123456");

            // credential for _system database, for operations that can only be used through
            // _system database like creation or droping database
            // you dont need it if authentication is disabled
            // default: new NetworkCredential("root", "")
            sharedSetting.SystemDatabaseCredential = new NetworkCredential("system_user", "123456");

            // set default WaitForSync for all operations
            // default: false
            sharedSetting.WaitForSync = true;

            // if set to false, invalid operations won't cause an exception and you can
            // proccess the response yourself
            // default: true
            sharedSetting.ThrowForServerErrors = false;

            // if set to false, collections won't be created when inserting documents
            // default: true
            sharedSetting.CreateCollectionOnTheFly = false;

            // set default BatchSize for cursor all operations
            // default: null
            sharedSetting.Cursor.BatchSize = 1024;

            // set default Count for cursor all operations
            // default: null
            sharedSetting.Cursor.Count = true;

            // set default MaxPlans for cursor all operations
            // default: null
            sharedSetting.Cursor.MaxPlans = 10;

            // add default rule for cursor all operations
            // default: empty list
            sharedSetting.Cursor.Rules.Add("rule-name");


        }
    }
}
