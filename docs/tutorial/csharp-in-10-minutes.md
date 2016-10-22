# Tutorial: C# in 10 Minutes

### Install the .NET Client

ArangoDB .NET client could be installed from [Nuget](https://www.nuget.org/packages/ArangoDB.Client). It supports variants of runtimes including  .NET 4.5, .NET Core 1.0, Mono/Xamarin, UAP 10.0, WPA 8.1, WIN 8 ... .

So let's Install the latest version of the client by:

``` javascript
Install-Package ArangoDB.Client
```

### Create a Database Setting

First of all we need to tell the client how to connect to ArangoDB server. We do this by creating a new database setting using `ChangeSetting` static method:

```csharp
ArangoDatabase.ChangeSetting(s =>
{
    s.Database = "TutorialDotNet";
    s.Url = "http://localhost:8529";

    // you can set other settings if you need
    s.Credential = new NetworkCredential("root", "123456");
    s.SystemDatabaseCredential = new NetworkCredential("root", "123456");
});
```
By this you will create(or change if exists) a database setting named `default`. If you need to connect to multiple databases you can create other settings by specifying an identifier:
 ```csharp
 ArangoDatabase.ChangeSetting("setting-identifier", s => {});
 ```

### Work with Database

Creating a document, executing a query and all other operations against ArangoDB server are accessible through `ArangoDatabase` object.

Let's create an `ArangoDatabase` object and tell it to use the `default` setting that we created before. To do this we can use `CreateWithSetting` static method:

```csharp
using (var db = ArangoDatabase.CreateWithSetting())
{
}
```

If we create other settings, we can use them by specify the setting identifier:

```csharp
using (var db = ArangoDatabase.CreateWithSetting("setting-identifier"))
{
}
```

### Create a Database

Let's start by creating a new database:

```csharp
db.CreateDatabase("TutorialDotNet");
```

You can now get information about the new created database:

```csharp
var currentDatabaseInfo = db.CurrentDatabaseInformation();

// path of the created database
Console.WriteLine(currentDatabaseInfo.Path);
```

### Create a collection

Collections in ArangoDB are similar to Tables in 
a relational databases like Microsoft SQL Server. 

Let's create a collection named `Person` in our database.

```csharp
db.CreateCollection("Person");
```

ArangoDB has another type of collections called `Edge` collections.
By creating edge collection we can use graph functionality 
that ArangoDB provides. To create edge collection:

```csharp
db.CreateCollection("Friend", type: CollectionType.Edge);
```

Create a document

Now that we created our collection we can insert documents in it.

First Let's define a class Named `Person` to use it for declare our documents:   

```csharp
public class Person
{
    [DocumentProperty(Identifier = IdentifierType.Key)]
    public string Key { get; set; }

    public string Fullname { get; set; }

    public int Age { get; set; }
}
```

Then to insert new document:

```csharp
var person = new Person
{
    Fullname = "raoof hojat",
    Age = 28
};

db.Insert<Person>(person);

// this will print the new inserted document key
Console.WriteLine(person.Key);
```

Note that when you use `db.Insert<T>`, collection name will be resolved
from type of `T` which in this case is `Person` collection. you can
read about naming convetions and how types are resolved later in this tutorial.

You can always use strings instead of types for resolving collection names:

```csharp
db.Collection("Person").Insert(new Person
{
    Fullname = "raoof hojat",
    Age = 28
});
```

### Update a document

We inserted a document, Let's update a member of it.

ArangoDB .NET client can track changes of your documents. So to
update the document that we insert before we can apply changes 
and pass it to `Update` method:

```c#
person.Age = 35;

// partially updates person document, only `Age` attribute will be updated
db.Update<Person>(person);
```

Note that `db.Update` method can update `inserted`/`loaded` documents
like above. If you want to update a document without loading it you
should use `db.UpdateById` instead:

```csharp
db.UpdateById<Person>("document-key", new { Age = 40 });
```

### Read a document

Read a document can be done by:

```csharp
var loadedPeson = db.Document<Person>("document-key");
```

Note that if document does not exists `db.Document` will return `null`.

### Query database by LINQ 

ArangoDB .NET client also supports LINQ queries 
for load/update/insert/remove documents.

Let's retrieve our inserted document by LINQ:

```csharp
var result = db.Query<Person>()
    .Where(p => p.Age == 35)
    .Select(p => p.Fullname)
    .ToList();
``` 

