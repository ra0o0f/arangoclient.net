using ArangoDB.Client.Test.Database;
using ArangoDB.Client.Test.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ArangoDB.Client.Test.Utility;
using Newtonsoft.Json;

namespace ArangoDB.Client.Test.Linq
{
    public class SimpleQuery
    {
        [Fact]
        public void From()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>();
            Assert.Equal("for `generated_0` in `Person` return `generated_0`", query.GetQueryData().Query.RemoveSpaces());
        }

        [Fact]
        public void FromWithSelect()
        {
            var db = DatabaseGenerator.Get();

            var query = from p in db.Query<Person>() select p;

            Assert.Equal("for `p` in `Person` return `p`", query.GetQueryData().Query.RemoveSpaces());
        }
        
        [Fact]
        public void FromWithSelectNewTernary()
        {
            var db = DatabaseGenerator.Get();
            var query = from p in db.Query<Person>()
                      select new Person
                      {
                          Fullname = p.Age > 10 ? p.Fullname : "",
                          Outfit = p.Age < 10 ? p.Outfit : null
                      };
            var queryText = query.GetQueryData().Query.RemoveSpaces();
            Assert.Equal(
@"for `p` in `Person` 
return {
 `Fullname` : ( `p`.`Age` > @P1 ) ? `p`.`Fullname` : @P2 ,
 `Outfit` : ( `p`.`Age` < @P3 ) ? `p`.`Outfit` : @P4 
}".RemoveSpaces(), queryText);
        }

        [Fact]
        public void FromWithSelectNew()
        {
            var db = DatabaseGenerator.Get();

            var query = from p in db.Query<Person>() select new { p };

            Assert.Equal("for `p` in `Person` return { `p` : `p` }", query.GetQueryData().Query.RemoveSpaces());
        }

        [Fact]
        public void Where()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().Where(x => x.Fullname == x.Fullname);

            Assert.Equal("for `x` in `Person` filter ( `x`.`Fullname` == `x`.`Fullname` ) return `x`", query.GetQueryData().Query.RemoveSpaces());
        }

        [Fact]
        public void Filter()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().Filter(x => x.Fullname == x.Fullname);

            Assert.Equal("for `x` in `Person` filter ( `x`.`Fullname` == `x`.`Fullname` ) return `x`", query.GetQueryData().Query.RemoveSpaces());
        }

        [Fact]
        public void WhereNested()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().Where(x => x.Fullname == x.Fullname && (x.Age != x.Age || x.Age == x.Age));

            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(),
@"for `x` in `Person` 
filter ( ( `x`.`Fullname` == `x`.`Fullname` ) and ( ( `x`.`Age` != `x`.`Age` ) or ( `x`.`Age` == `x`.`Age` ) ) ) return `x`".RemoveSpaces());
        }

        [Fact]
        public void WhereNot()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().Where(x => !(x.Fullname == x.Fullname));

            Assert.Equal("for `x` in `Person` filter not ( `x`.`Fullname` == `x`.`Fullname` ) return `x`", query.GetQueryData().Query.RemoveSpaces());
        }

        [Fact]
        public void Sort()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().OrderBy(x => x.Age).ThenByDescending(x => x.Fullname);

            Assert.Equal("for `x` in `Person` sort `x`.`Age` asc , `x`.`Fullname` desc return `x`", query.GetQueryData().Query.RemoveSpaces());
        }

        [Fact]
        public void Let()
        {
            var db = DatabaseGenerator.Get();

            var query = from p in db.Query<Person>()
                        let a = p.Age
                        let f = p.Fullname
                        select new { p, a, f };

            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(), @"
for `p` in `Person`
let `a` = `p`.`Age`
let `f` = `p`.`Fullname`
return { `p` : `p` , `a` : `a` , `f` : `f` }
".RemoveSpaces());
        }

        [Fact]
        public void LetWithLambda()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>()
                .Let(p => p.Age, (q, a) => q
                .Let(p => p.Fullname, (q2, f) => q2
                .Select(p => new { p, a, f })));

            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(), @"
for `p` in `Person`
let `a` = `p`.`Age`
let `f` = `p`.`Fullname`
return { `p` : `p` , `a` : `a` , `f` : `f` }
".RemoveSpaces());
        }

        [Fact]
        public void Let_WithoutUsingGenericQuery()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .Let(_ => 4, (q, number) => q
                .Let(_ => "Some text", (q2, text) => q
                .Select(_ => new { number, text })));

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"
let `number` = @P1
let `text` = @P2
return { `number` : `number` , `text` : `text` }
".RemoveSpaces());

            Assert.Equal(2, queryData.BindVars.Count);
            Assert.Equal(4, queryData.BindVars[0].Value);
            Assert.Equal("Some text", queryData.BindVars[1].Value);
        }

        [Fact]
        public void LetWithLambda_SelectVersion()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>()
                .Select(p => new { p, a = p.Age })
                .Select(p => new { p, f = p.p.Fullname })
                .Select(p => new { p.p.p, p.p.a, p.f });

            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(), @"
for `p` in `Person`
let `a` = `p`.`Age`
let `f` = `p`.`Fullname`
return { `p` : `p` , `a` : `a` , `f` : `f` }
".RemoveSpaces());
        }

        [Fact]
        public void Take()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().Take(10).Select(x => x);

            Assert.Equal("for `generated_1` in `Person` limit @P1 return `generated_1`", query.GetQueryData().Query.RemoveSpaces());
        }

        [Fact]
        public void TakeSkip()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().Take(10).Skip(20).Select(x => x);

            Assert.Equal("for `generated_2` in `Person` limit @P1 , @P2 return `generated_2`", query.GetQueryData().Query.RemoveSpaces());

            var query2 = db.Query<Person>().Skip(20).Take(10).Select(x => x);

            Assert.Equal("for `generated_2` in `Person` limit @P1 , @P2 return `generated_2`", query2.GetQueryData().Query.RemoveSpaces());

        }

        [Fact]
        public void Skip()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().Skip(20).Select(x => x);

            Assert.Throws<InvalidOperationException>(() => query.GetQueryData());
        }

        [Fact]
        public void GroupBySingleSelectKey()
        {
            var db = DatabaseGenerator.Get();

            db.Setting.Linq.TranslateGroupByIntoName = identifier => identifier;

            var query = db.Query<Person>().GroupBy(x => x.Age).Select(byAge => byAge.Key);

            Assert.Equal("for `x` in `Person` collect `CV1` = `x`.`Age` into `byAge` return `CV1`", query.GetQueryData().Query.RemoveSpaces());

            db.Setting.Linq.TranslateGroupByIntoName = null;
        }

        [Fact]
        public void GroupBySingleSelectExpressionKey()
        {
            var db = DatabaseGenerator.Get();

            db.Setting.Linq.TranslateGroupByIntoName = identifier => identifier;

            var query = db.Query<Person>().GroupBy(x => AQL.ToString(x.Age)).Select(byAge => byAge.Key);

            Assert.Equal("for `x` in `Person` collect `CV1` = to_string( `x`.`Age` ) into `byAge` return `CV1`", query.GetQueryData().Query.RemoveSpaces());

            db.Setting.Linq.TranslateGroupByIntoName = null;
        }

        [Fact]
        public void GroupByNewSingleSelectKey()
        {
            var db = DatabaseGenerator.Get();

            db.Setting.Linq.TranslateGroupByIntoName = identifier => identifier;

            var query = db.Query<Person>().GroupBy(x => new { x.Age }).Select(byAge => byAge.Key);

            Assert.Equal("for `x` in `Person` collect `Age` = `x`.`Age` into `byAge` return { `Age` : `Age` }", query.GetQueryData().Query.RemoveSpaces());

            db.Setting.Linq.TranslateGroupByIntoName = null;
        }

        [Fact]
        public void GroupByNewSingleSelectExpressionKey()
        {
            var db = DatabaseGenerator.Get();

            db.Setting.Linq.TranslateGroupByIntoName = identifier => identifier;

            var query = db.Query<Person>().GroupBy(x => new { cage = AQL.ToString(x.Age) }).Select(byAge => byAge.Key);

            Assert.Equal("for `x` in `Person` collect `cage` = to_string( `x`.`Age` ) into `byAge` return { `cage` : `cage` }", query.GetQueryData().Query.RemoveSpaces());

            db.Setting.Linq.TranslateGroupByIntoName = null;
        }

        [Fact]
        public void GroupByNewSingleSelectExpressionKey_SelectOnlyOneKey()
        {
            var db = DatabaseGenerator.Get();

            db.Setting.Linq.TranslateGroupByIntoName = identifier => identifier;

            var query = db.Query<Person>().GroupBy(x => new { cage = AQL.ToString(x.Age) }).Select(byAge => byAge.Key.cage);

            Assert.Equal("for `x` in `Person` collect `cage` = to_string( `x`.`Age` ) into `byAge` return `cage`", query.GetQueryData().Query.RemoveSpaces());

            db.Setting.Linq.TranslateGroupByIntoName = null;
        }

        [Fact]
        public void GroupByNewMultipleSelectKey()
        {
            var db = DatabaseGenerator.Get();

            db.Setting.Linq.TranslateGroupByIntoName = identifier => identifier;

            var query = db.Query<Person>().GroupBy(x => new { x.Age, x.Fullname }).Select(byAge => byAge.Key);

            Assert.Equal("for `x` in `Person` collect `Age` = `x`.`Age` , `Fullname` = `x`.`Fullname` into `byAge` return { `Age` : `Age` , `Fullname` : `Fullname` }", query.GetQueryData().Query.RemoveSpaces());

            db.Setting.Linq.TranslateGroupByIntoName = null;
        }

        [Fact]
        public void GroupBySelectInto()
        {
            var db = DatabaseGenerator.Get();

            var query = from p in db.Query<Person>()
                        group p by p.Age into byAge
                        select byAge.Select(x => x.Fullname);

            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(), @"for `p` in `Person` collect `CV1` = `p`.`Age` into `C1`
return ( for `x` in `C1` return `x`.`p`.`Fullname` )".RemoveSpaces());
        }

        [Fact]
        public void GroupByGroupBySelectInto()
        {
            var db = DatabaseGenerator.Get();

            db.Setting.Linq.TranslateGroupByIntoName = identifier => identifier;

            var query = (from p in db.Query<Person>()
                         group p by p.Age into byAge
                         select new
                         {
                             byAgebyHeight =
                                 (
                                     from b in byAge
                                     group b by b.Height into byAgebyHeight
                                     select byAgebyHeight.Select(x => new { x.Fullname, byAgebyHeight.Key })
                                 ),
                             age = byAge.Key
                         });

            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(), @"for `p` in `Person` collect `CV1` = `p`.`Age` into `byAge` 
return { `byAgebyHeight` : 
( for `b` in `byAge` collect `CV2` = `b`.`p`.`Height` into `byAgebyHeight` return ( for `x` in `byAgebyHeight` return { `Fullname` : `x`.`b`.`p`.`Fullname` , `Key` : `CV2` } ) ) ,
    `age` : `CV1`
}".RemoveSpaces());

            db.Setting.Linq.TranslateGroupByIntoName = null;
        }

        [Fact]
        public void GroupBySelectAggregation()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().GroupBy(x => x.Age)
                .Select(byAge => new { len = AQL.Length(byAge), byAge.Key, Ages = byAge.Select(x => x.Age) });

            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(),
                @"for `x` in `Person` collect `CV1` = `x`.`Age` into `C1` return { `len` : length( `C1` ) , `Key` : `CV1` , `Ages` : ( for `x` in `C1` return `x`.`x`.`Age` )  }".RemoveSpaces());
        }

        [Fact]
        public void UpdateSingle()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .Update(_ => new { Outfit = new { Color = "red" } }, _ => "123456")
                .In<Person>()
                .IgnoreModificationSelect();

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"update 
@P1 with @P2 in `Person`".RemoveSpaces());

            Assert.Equal("123456", queryData.BindVars[0].Value);
            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[1].Value),
                JsonConvert.SerializeObject(new { Outfit = new { Color = "red" } }));
        }

        [Fact]
        public void UpdateSingle_SelectNew()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .Update(_ => new { Outfit = new { Color = "red" } }, _ => "123456")
                .In<Person>()
                .Select((n, o) => n);

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"update 
@P1 with @P2 in `Person` return `NEW`".RemoveSpaces());

            Assert.Equal("123456", queryData.BindVars[0].Value);
            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[1].Value),
                JsonConvert.SerializeObject(new { Outfit = new { Color = "red" } }));
        }

        [Fact]
        public void Update()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().Update(p => new { Outfit = new { Color = "red" } }).IgnoreModificationSelect();

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `p` in `Person` update 
`p` with @P1 in `Person`".RemoveSpaces());

            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[0].Value),
                JsonConvert.SerializeObject(new { Outfit = new { Color = "red" } }));
        }

        [Fact]
        public void Update_SelectNewOld()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().Update(p => new { Outfit = new { Color = "red" } })
                .Select((n, o) => new { n.Outfit, o.Height });

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `p` in `Person` update 
`p` with @P1 in `Person` return { `Outfit` : `NEW` .`Outfit` , `Height` : `OLD` .`Height` }".RemoveSpaces());

            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[0].Value),
                JsonConvert.SerializeObject(new { Outfit = new { Color = "red" } }));
        }

        [Fact]
        public void Update_WithKey()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Flight>().Update(f => new { Airline = "lufthansa" }, keySelector: k => k.Key)
                .IgnoreModificationSelect();

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `f` in `Flight` update 
`f`.`_key` with @P1 in `Flight`".RemoveSpaces());

            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[0].Value),
                JsonConvert.SerializeObject(new { Airline = "lufthansa" }));
        }

        [Fact]
        public void Update_ToAnotherCollection()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Flight>().Update(f => new { Airline = "lufthansa" }).In<Person>().IgnoreModificationSelect();

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `f` in `Flight` update 
`f` with @P1 in `Person`".RemoveSpaces());

            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[0].Value),
                JsonConvert.SerializeObject(new { Airline = "lufthansa" }));
        }

        [Fact]
        public void RemoveSingle()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query().Remove(_ => "12345").In<Person>().IgnoreModificationSelect();

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"remove @P1 in `Person`".RemoveSpaces());
            Assert.Equal("12345", queryData.BindVars[0].Value);
        }

        [Fact]
        public void RemoveSingle_SelectOldAge()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query().Remove(_ => "12345").In<Person>().Select((n, o) => o.Age);

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"remove @P1 in `Person` return `OLD` .`Age`".RemoveSpaces());
        }

        [Fact]
        public void Remove()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().Remove().IgnoreModificationSelect();

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `x` in `Person` remove `x` in `Person`".RemoveSpaces());
        }

        [Fact]
        public void Remove_NotIgnoringSelect()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().Remove();

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `x` in `Person` remove `x` in `Person` return `x`".RemoveSpaces());
        }

        [Fact]
        public void Remove_WithKey()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Flight>().Remove(keySelector: x => x.Key).IgnoreModificationSelect();

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `x` in `Flight` remove `x`.`_key` in `Flight`".RemoveSpaces());
        }

        [Fact]
        public void Remove_WithKey_SelectOldKey()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Flight>().Remove(keySelector: x => x.Key).Select((n, o) => o.Key);

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `x` in `Flight` remove `x`.`_key` in `Flight` return `OLD` .`_key`".RemoveSpaces());
        }

        [Fact]
        public void Remove_InAnotherCollection()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Flight>().Remove().In<Person>().IgnoreModificationSelect();

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `x` in `Flight` remove 
`x` in `Person`".RemoveSpaces());
        }

        [Fact]
        public void Remove_InAnotherCollection_SelectOld()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Flight>().Remove().In<Person>().Select((n, o) => o.Age);

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `x` in `Flight` remove 
`x` in `Person` return `OLD` .`Age`".RemoveSpaces());
        }

        [Fact]
        public void InsertSingle()
        {
            var db = DatabaseGenerator.Get();

            var person = new Person
            {
                Age = 21
            };

            var query = db.Query().Insert(_ => person).In<Person>().IgnoreModificationSelect();

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"insert @P1 in `Person`".RemoveSpaces());
            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[0].Value), JsonConvert.SerializeObject(person));
        }

        [Fact]
        public void InsertSingle_SelectNewMember()
        {
            var db = DatabaseGenerator.Get();

            var person = new Person
            {
                Age = 21
            };

            var query = db.Query().Insert(_ => person).In<Person>().Select((n, o) => n.Age);

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"insert @P1 in `Person` return `NEW` .`Age`".RemoveSpaces());
            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[0].Value), JsonConvert.SerializeObject(person));
        }

        [Fact]
        public void Insert()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().Insert().IgnoreModificationSelect();

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `x` in `Person` insert `x` in `Person`".RemoveSpaces());
        }

        [Fact]
        public void Insert_NotIgnoringSelect()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().Insert();

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `x` in `Person` insert `x` in `Person` return `x`".RemoveSpaces());
        }

        [Fact]
        public void Insert_New()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().Insert(p => new { Outfit = new { Color = "red" } }).IgnoreModificationSelect();

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `p` in `Person` insert @P1 in `Person`".RemoveSpaces());

            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[0].Value), JsonConvert.SerializeObject(new { Outfit = new { Color = "red" } }));
        }

        [Fact]
        public void Insert_New_SelectNewMember()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>()
                .Insert(p => new { Outfit = new { Color = "red" } })
                .Select((n, o) => n.Age);

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `p` in `Person` insert @P1 in `Person` return `NEW` .`Age`".RemoveSpaces());

            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[0].Value), JsonConvert.SerializeObject(new { Outfit = new { Color = "red" } }));
        }

        [Fact]
        public void Insert_InAnotherCollection()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Flight>().Insert().In<Person>().IgnoreModificationSelect();

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `x` in `Flight` insert 
`x` in `Person`".RemoveSpaces());
        }

        [Fact]
        public void ReplaceSingle()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query().Replace(p => new { Outfit = new { Color = "red" } }, _ => "123456")
                .In<Person>()
                .IgnoreModificationSelect();

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"replace 
@P1 with @P2 in `Person`".RemoveSpaces());

            Assert.Equal("123456", queryData.BindVars[0].Value);
            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[1].Value),
                JsonConvert.SerializeObject(new { Outfit = new { Color = "red" } }));
        }

        [Fact]
        public void ReplaceSingle_SelectOld()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .Replace(p => new { Outfit = new { Color = "red" } }, _ => "123456")
                .In<Person>()
                .Select((n, o) => o);

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"replace 
@P1 with @P2 in `Person` return `OLD`".RemoveSpaces());

            Assert.Equal("123456", queryData.BindVars[0].Value);
            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[1].Value),
                JsonConvert.SerializeObject(new { Outfit = new { Color = "red" } }));
        }

        [Fact]
        public void Replace()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().Replace(p => new { Outfit = new { Color = "red" } }).IgnoreModificationSelect();

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `p` in `Person` replace 
`p` with @P1 in `Person`".RemoveSpaces());

            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[0].Value),
                JsonConvert.SerializeObject(new { Outfit = new { Color = "red" } }));
        }

        [Fact]
        public void Replace_SelectNewOld()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().Replace(p => new { Outfit = new { Color = "red" } })
                .Select((n, o) => new { n.Outfit, o.Height });

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `p` in `Person` replace 
`p` with @P1 in `Person` return { `Outfit` : `NEW` .`Outfit` , `Height` : `OLD` .`Height` }".RemoveSpaces());

            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[0].Value),
                JsonConvert.SerializeObject(new { Outfit = new { Color = "red" } }));
        }

        [Fact]
        public void Replace_WithKey()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Flight>().Replace(f => new { Airline = "lufthansa" }, keySelector: k => k.Key)
                .IgnoreModificationSelect();

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `f` in `Flight` replace 
`f`.`_key` with @P1 in `Flight`".RemoveSpaces());

            dynamic parameter = queryData.BindVars[0].Value;
            Assert.Equal(parameter.Airline, "lufthansa");
        }

        [Fact]
        public void Replace_ToAnotherCollection()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Flight>().Replace(f => new { Airline = "lufthansa" }).In<Person>().IgnoreModificationSelect();

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `f` in `Flight` replace 
`f` with @P1 in `Person`".RemoveSpaces());

            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[0].Value),
                JsonConvert.SerializeObject(new { Airline = "lufthansa" }));
        }

        [Fact]
        public void Subquery()
        {
            var db = DatabaseGenerator.Get();

            var query = from p in db.Query<Person>()
                        let flights = from f in db.Query<Flight>() select f
                        select new { p, flights };

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `p` in `Person` 
let `flights` = ( for `f` in `Flight` return `f` )
return { `p` : `p` , `flights` : `flights` }".RemoveSpaces());
        }

        [Fact]
        public void Join()
        {
            var db = DatabaseGenerator.Get();

            var query = from p in db.Query<Person>()
                        from f in db.Query<Flight>()
                        where p.Age == f.Code
                        select new { p, f };

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `p` in `Person` 
for `f` in `Flight`
filter ( `p`.`Age` == `f`.`Code` )
return { `p` : `p` , `f` : `f` }".RemoveSpaces());
        }

        [Fact]
        public void JoinWithLambda()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>()
                .For(p => db.Query<Flight>()
                .Where(f => p.Age == f.Code)
                .Select(f => new { p, f }));

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `p` in `Person` 
for `f` in `Flight`
filter ( `p`.`Age` == `f`.`Code` )
return { `p` : `p` , `f` : `f` }".RemoveSpaces());
        }

        [Fact]
        public void Join_WithoutUsingGenericQuery()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .For(_ => db.Query<Person>()
                .For(p => db.Query<Flight>()
                .Where(f => p.Age == f.Code)
                .Select(f => new { p, f })));

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `p` in `Person` 
for `f` in `Flight`
filter ( `p`.`Age` == `f`.`Code` )
return { `p` : `p` , `f` : `f` }".RemoveSpaces());
        }

        [Fact]
        public void Upsert()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .Upsert<Category>(_ => new { ip = "192.168.173.13" },
                                  _ => new { ip = "192.168.173.13", name = "flittard" },
                                 (_, o) => new { })
                                 .In<Category>()
                                 .IgnoreModificationSelect();

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"
upsert @P1
insert @P2
update @P3
in `Category`
".RemoveSpaces());

            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[0].Value), JsonConvert.SerializeObject(new { ip = "192.168.173.13" }));
            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[1].Value), JsonConvert.SerializeObject(new { ip = "192.168.173.13", name = "flittard" }));
            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[2].Value), JsonConvert.SerializeObject(new { }));
        }

        [Fact]
        public void Upsert_SelectOld()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .Upsert<Category>(_ => new { ip = "192.168.173.13" },
                                  _ => new { ip = "192.168.173.13", name = "flittard" },
                                 (_, o) => new { })
                                 .In<Category>()
                                 .Select((n, o) => o.Tags);

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"
upsert @P1
insert @P2
update @P3
in `Category`
return `OLD` .`Tags`
".RemoveSpaces());

            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[0].Value), JsonConvert.SerializeObject(new { ip = "192.168.173.13" }));
            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[1].Value), JsonConvert.SerializeObject(new { ip = "192.168.173.13", name = "flittard" }));
            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[2].Value), JsonConvert.SerializeObject(new { }));
        }

        [Fact]
        public void UpsertWithOld()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .Upsert<Category>(_ => new { ip = "192.168.173.13" },
                                  _ => new { ip = "192.168.173.13", name = "flittard" },
                                 (_, o) => new { name = o.Title })
                                 .In<Person>()
                                 .IgnoreModificationSelect();

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"
upsert @P1
insert @P2
update { `name` : `OLD` .`Title` }
in `Person`
".RemoveSpaces());

            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[0].Value), JsonConvert.SerializeObject(new { ip = "192.168.173.13" }));
            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[1].Value), JsonConvert.SerializeObject(new { ip = "192.168.173.13", name = "flittard" }));
        }

        [Fact]
        public void UpsertWithFor()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Host>()
                .Upsert(h => new Host { Key = h.Key },
                h => new Host { },
                (h, old) => new Host { Tags = AQL.Append(h.Tags, old.Tags) })
                .IgnoreModificationSelect();

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"
for `h` in `hosts`
upsert { `_key` : `h`.`_key` }
insert @P1
update { `tags` : append( `h`.`tags` , `OLD` .`tags` ) }
in `hosts`
".RemoveSpaces());

            ObjectUtility.AssertSerialize(queryData.BindVars[0].Value, new { }, db);
        }
    }
}
