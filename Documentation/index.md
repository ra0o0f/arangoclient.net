##ArangoDatabase

####What is ArangoDatabase?
Creating a document, executing a query and all other operation against ArangoDB are accessible through `ArangoDatabase` object. it performs all operations by communicating through ArangoDB Http API.


```csharp
using (IArangoDatabase db = new ArangoDatabase("http://localhost:8529", "SampleDB"))
{
	var info = db.CurrentDatabaseInformation();
	
	// output: "C:\ArangoDB 2.3.4\var\lib\arangodb\databases\database-179610003747"     
	Console.WriteLine(info.Path);
}
```

####Creating a new ArangoDatabase object
Creating `ArangoDatabase` object is a cheap operation and it does not perform any expensive action against database, so it can be create anytime database operation is needed.

One way to create `ArangoDatabase` object is to pass `database` and `url` arguments like above example. Another way is by passing a `DatabaseSharedSetting` object, in this way
you can also set other properties:
    
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

`DatabaseSharedSetting` object are the way to change client default behavior, you
can define it once(store it for the application life time) and pass it to all places `ArangoDatabase` is used. 
[Database settings in detail](./DatabaseSetting.md)

`ArangoDatabase` also provide two static methods for storing and using `DatabaseSharedSetting` for application life time to create new `ArangoDatabase`.

```csharp
// this will create a new SharedSetting
ArangoDatabase.ChangeSetting(s =>
{
	s.Database = "SampleDB";
	s.Url = "http://localhost:8529";
	s.WaitForSync = true;
	s.CreateCollectionOnTheFly = false;
});

// above setting can be used to create new ArangoDatabase
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

<div class="document-caution">
Note that `ArangoDatabase` is also responsible for `Change Tracking` that 
tracks documents to perform easy updates of documents. so do not keep
`ArangoDatabase` object for a long time. 
</div>






