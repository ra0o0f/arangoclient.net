# ArangoDatabase

#### What is ArangoDatabase?
Creating a document, executing a query and all other operation against ArangoDB are accessible through `ArangoDatabase` object. it performs all operations by communicating through ArangoDB Http API.


```csharp
using (IArangoDatabase db = new ArangoDatabase("http://localhost:8529", "SampleDB"))
{
	var info = db.CurrentDatabaseInformation();

	// output: "C:\ArangoDB 2.3.4\var\lib\arangodb\databases\[database]     
	Console.WriteLine(info.Path);
}
```

##### Methods for creating new instance of `ArangoDatabase`


One way to create `ArangoDatabase` object is to pass `database` and `url` arguments like above example. Another way is by passing a `DatabaseSharedSetting` object, This way
you have the ability to set other properties:

```csharp
var sharedSetting = new DatabaseSharedSetting
{
	Database = "SampleDB",
	Url = "http://localhost:8529",
	WaitForSync = true,
	CreateCollectionOnTheFly = false
};

using (IArangoDatabase db = new ArangoDatabase(sharedSetting))
{
}
```

`DatabaseSharedSetting` objects are the way to change client default behavior, you
can define it once and pass it to all places `ArangoDatabase` object is created.
[Database settings in detail](./DatabaseSetting.md)

There is also another way that `ArangoDatabase` can store `DatabaseSharedSetting` for you..

```csharp
// this will create a new SharedSetting
ArangoDatabase.ChangeSetting(s =>
{
	s.Database = "SampleDB";
	s.Url = "http://localhost:8529";
	s.WaitForSync = true;
	s.CreateCollectionOnTheFly = false;
});

// above setting can be used to create new instance of ArangoDatabase
using(var db = ArangoDatabase.CreateWithSetting())
{
}
```

It is also possible to create as many settings as needed

```csharp
ArangoDatabase.ChangeSetting("SampleDBSetting", s =>
{
	s.Database = "SampleDB";
	s.Url = "http://localhost:8529";
	s.WaitForSync = true;
	s.CreateCollectionOnTheFly = false;
});

using(IArangoDatabase db = ArangoDatabase.CreateWithSetting("SampleDBSetting"))
{
}
```

##### Notes on creating new instance of `ArangoDatabase`

* `ArangoDatabase` instance lifetime should be kept short. this is because it's responsible for change tracking where it is able to update documents partially base on changes you make on a document, So storing it for the application lifetime would be a bad idea (for example you can store it for a asp.net request life cycle)

*  `ArangoDatabase` instance members are not guaranteed to be thread safe, So in a multi-threaded application you must use a separate instance of `ArangoDatabase`

* Creating new `ArangoDatabase` does not perform any request against the database, So create as many as you need. You can also pass a `DatabaseSharedSetting` object on creating new instance to share settings in all of them.
