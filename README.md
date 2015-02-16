### .Net client driver for http://www.arangodb.net
##### written in c# portable class library[.net 4.5, winphone 8.1] <h8><sub>xarmin ios and android apps are probably supported but not tested</sub></h8>
<hr/>

### Main features
* Linq to Aql
* Asynchronous and Synchronous API
* Stream data with IEnumerable or IAsyncEnumerator 
* Change Tracking (Under development will be available soon)

note:
linq-provider now supports several functions ( orderby,take,skip,groupby,join... with aql synonyms sort,filter,collect,for,limit... )
. all api operations performs async internally with sync-over-async api. 
<br/><b>all APIs will be added soon(with documentation), but don't hesitate to open an issue if you need one sooner</b>

### Quick sample
```csharp
class Person
{
    [DocumentProperty(Identifier = IdentifierType.Key)]
    public string Key;

    public string Name;

    public int Age;
}

using (ArangoDatabase db = new ArangoDatabase(url: "http://localhost:8529", database: "SampleDB"))
{
    var person = new Person { Name = "raoof hojat", Age = 27 };

    // saves new document and creates 'Person' collection on the fly
    db.Save<Person>(person);
    // perform operation async
    await db.SaveAsync<Person>(person);

    // returns [27]
    var persons =  db.Query<Person>()
                  .Filter(p => AQL.Contains(x.Name, "raoof"))
                  .Return(p => p.Age)
                  .ToList();
}
```
<hr/>

###Linq Samples
```c#
// join sample
  var query = from p in db.Query<Person>()
              from o in db.Query<Order>()
              where p.Key == o.CustomerKey
              select new { p.Name,o.Title };
      
```
```c#
// lambda sample
var query = db.Query<Order>()
                    .Where(x=>x.Price<10000)
                    .Skip(500)
                    .Take(100)
                    .GroupBy(x => x.CustomerKey)
                    .Select(x => new
                    {
                        Key = x.Key
                    });
          
// aql clauses equivalent
var query = db.Query<Order>()
                    .Filter(x => x.Price < 10000)
                    .Limit(500, 100)
                    .Collect(x => x.CustomerKey)
                    .Return(x => new
                    {
                        Key = x.Key
                    });

// aql query equivalent
var query = db.CreateStatement<Order>(@"for x in Order
                        filter x.Price < 10000
                        limit 500,100
                        collect CustomerKey = x.CustomerKey into C1
                        return { CustomerKey }
                        ");
``` 
<hr/>

### Streaming data

```c#
var cursor = db.ByExample<Person>(new {Age=20});

// fetch cursor result
var list = cursor.ToList();

// stream cursor results
foreach(var p in cursor.AsEnumerable())
    Console.WriteLine(p.Name);

// fetch cursor result async
var list = awiat cursor.ToListAsync();

// stream cursor results async
using(enumerator = cursor.GetAsyncEnumerator())
{
    while(await enumerator.MoveNextAsync())
    {
        Console.WriteLine(enumerator.Current.Name);
    }
}
```

<hr/>

### Database Settings

client behavior could be change by 'db.Settings' property, like:

```c#
// will set default value for waitForSync to true anywhere it is used 
db.Settings.WaitForSync = true;

// waitForSync is true
db.Update<Person>("41234512",new {Name:"hojat"});

// waitForSync will be overridden to false
db.Update<Person>("41234512",new {Name:"hojat"}, waitForSync: false);
```

for ease of changing client behavior, ArangoDatabase could be create with following static methods

```c#
// set setting once
var setting = ArangoDatabase.FindSetting();
setting.WaitForSync = true;

// will create ArangoDatabase with setting defined above
using(ArangoDatabase db = ArangoDatabase.WithSetting())
{
}

// you can define multiple settings by passing an identifier to "FindSetting"
var setting = ArangoDatabase.FindSetting("SampleDbSetting");
setting.WaitForSync = true;

// and use it like this
using(ArangoDatabase db =ArangoDatabase.WithSetting("SampleDbSetting"))
{
}
```
