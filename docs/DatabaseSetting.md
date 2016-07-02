# What is Database Setting?

Database settings are responsible for client API behaviors and they do this by setting default properties, naming conventions or handle events.

#### Setting Default Properties

The client provides a way to set several properties like the cursor `BatchSize`, `DisableChangeTracking` or `CreateCollectionOnTheFly`. Changing these properties affects all `ArangoDatabase` objects that are using same `DatabaseSharedSetting`.

```csharp
var sharedSetting = new DatabaseSharedSetting
{
    // Sets the default database for this setting
    Database = "SampleDB",
    // Endpoint of the database server
    Url = "http://localhost:8529",
    // This will tell ArangoDB to create collections automatically
    // when inserting new documents
    CreateCollectionOnTheFly = true,
    // Batch size determines how many results are at most transferred
    // from the server to the client in one chunk
    Cursor.BatchSize = 5000
};

using (IArangoDatabase db = new ArangoDatabase(sharedSetting))
{
}
```

var person = new Person { Age = 30 };
db.Insert<Person>(person, waitForSync: true);
Now if we want to insert multiple documents, instead of assign WaitForSync on each db.Insert API method, we can use database setting to set a default value for WaitForSync:

```csharp
using (IArangoDatabase db = new ArangoDatabase("http://localhost:8529", "SampleDB"))
{
    // set default value for WaitForSync
  db.Setting.WaitForSync = true;

    // insert will use the default WaitForSync defined by db.Setting
    db.Insert<Person>(person);

    // assign waitForSync would override default value
    db.Remove<Person>(person, waitForSync: false);
}
```

However changing settings through db.Setting would just affect the ArangoDatabase object that is changing it. So for share settings among multiple ArangoDatabase objects we should create a SharedDatabaseSetting object:

var sharedSetting = new DatabaseSharedSetting
{
    Database = "SampleDB",
    Url = "http://localhost:8529",
    WaitForSync = true
};

using (IArangoDatabase db = new ArangoDatabase(sharedSetting))
{
    // insert will use the default WaitForSync defined by sharedSetting
    db.Insert<Person>(person);

    // default value defined by sharedSetting can be overriden by db.Setting
  db.Setting.WaitForSync = false;
    db.Update<Person>(person);

    // assign waitForSync would override default value
    db.Remove<Person>(person, waitForSync: true);
}
As you can see by last example, settings can be overridden in following manner:

Default settings can be set for all ArangoDatabase objects that use same DatabaseSharedSetting object.
db.Setting can be used to override the shared setting without affecting it and provide default settings for ArangoDatabase object that is changing it.

If settings properties are accessible through the API method parameters, it can override the default settings.
