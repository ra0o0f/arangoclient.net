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
using ArangoDB.Client.Common.Newtonsoft.Json;

namespace ArangoDB.Client.Test.Linq
{
    public class SimpleQuery
    {
        [Fact]
        public void From()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>();
            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(), "for `generated_0` in `Person` return `generated_0`");
        }

        [Fact]
        public void FromWithSelect()
        {
            var db = DatabaseGenerator.Get();

            var query = from p in db.Query<Person>() select p;

            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(), "for `p` in `Person` return `p`");
        }

        [Fact]
        public void FromWithSelectNew()
        {
            var db = DatabaseGenerator.Get();

            var query = from p in db.Query<Person>() select new { p };

            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(), "for `p` in `Person` return { `p` : `p` }");
        }

        [Fact]
        public void Where()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().Where(x => x.Fullname == x.Fullname);

            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(), "for `x` in `Person` filter ( `x`.`Fullname` == `x`.`Fullname` ) return `x`");
        }

        [Fact]
        public void Filter()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().Filter(x => x.Fullname == x.Fullname);

            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(), "for `x` in `Person` filter ( `x`.`Fullname` == `x`.`Fullname` ) return `x`");
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

            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(), "for `x` in `Person` filter not ( `x`.`Fullname` == `x`.`Fullname` ) return `x`");
        }

        [Fact]
        public void Sort()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().OrderBy(x => x.Age).ThenByDescending(x => x.Fullname);

            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(), "for `x` in `Person` sort `x`.`Age` asc , `x`.`Fullname` desc return `x`");
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

            Assert.Equal(queryData.BindVars.Count, 2);
            Assert.Equal(queryData.BindVars[0].Value, 4);
            Assert.Equal(queryData.BindVars[1].Value, "Some text");
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

            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(), "for `generated_1` in `Person` limit @P1 return `generated_1`");
        }

        [Fact]
        public void TakeSkip()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().Take(10).Skip(20).Select(x => x);

            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(), "for `generated_2` in `Person` limit @P1 , @P2 return `generated_2`");

            var query2 = db.Query<Person>().Skip(20).Take(10).Select(x => x);

            Assert.Equal(query2.GetQueryData().Query.RemoveSpaces(), "for `generated_2` in `Person` limit @P1 , @P2 return `generated_2`");

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

            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(), "for `x` in `Person` collect `CV1` = `x`.`Age` into `byAge` return `CV1`");

            db.Setting.Linq.TranslateGroupByIntoName = null;
        }

        [Fact]
        public void GroupBySingleSelectExpressionKey()
        {
            var db = DatabaseGenerator.Get();

            db.Setting.Linq.TranslateGroupByIntoName = identifier => identifier;

            var query = db.Query<Person>().GroupBy(x => AQL.ToString(x.Age)).Select(byAge => byAge.Key);

            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(), "for `x` in `Person` collect `CV1` = to_string( `x`.`Age` ) into `byAge` return `CV1`");

            db.Setting.Linq.TranslateGroupByIntoName = null;
        }

        [Fact]
        public void GroupByNewSingleSelectKey()
        {
            var db = DatabaseGenerator.Get();

            db.Setting.Linq.TranslateGroupByIntoName = identifier => identifier;

            var query = db.Query<Person>().GroupBy(x => new { x.Age }).Select(byAge => byAge.Key);

            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(), "for `x` in `Person` collect `Age` = `x`.`Age` into `byAge` return { `Age` : `Age` }");

            db.Setting.Linq.TranslateGroupByIntoName = null;
        }

        [Fact]
        public void GroupByNewSingleSelectExpressionKey()
        {
            var db = DatabaseGenerator.Get();

            db.Setting.Linq.TranslateGroupByIntoName = identifier => identifier;

            var query = db.Query<Person>().GroupBy(x => new { cage = AQL.ToString(x.Age) }).Select(byAge => byAge.Key);

            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(), "for `x` in `Person` collect `cage` = to_string( `x`.`Age` ) into `byAge` return { `cage` : `cage` }");

            db.Setting.Linq.TranslateGroupByIntoName = null;
        }

        [Fact]
        public void GroupByNewSingleSelectExpressionKey_SelectOnlyOneKey()
        {
            var db = DatabaseGenerator.Get();

            db.Setting.Linq.TranslateGroupByIntoName = identifier => identifier;

            var query = db.Query<Person>().GroupBy(x => new { cage = AQL.ToString(x.Age) }).Select(byAge => byAge.Key.cage);

            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(), "for `x` in `Person` collect `cage` = to_string( `x`.`Age` ) into `byAge` return `cage`");

            db.Setting.Linq.TranslateGroupByIntoName = null;
        }

        [Fact]
        public void GroupByNewMultipleSelectKey()
        {
            var db = DatabaseGenerator.Get();

            db.Setting.Linq.TranslateGroupByIntoName = identifier => identifier;

            var query = db.Query<Person>().GroupBy(x => new { x.Age, x.Fullname }).Select(byAge => byAge.Key);

            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(), "for `x` in `Person` collect `Age` = `x`.`Age` , `Fullname` = `x`.`Fullname` into `byAge` return { `Age` : `Age` , `Fullname` : `Fullname` }");

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
                                    select byAgebyHeight.Select(x => new { x.Fullname,byAgebyHeight.Key })
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
                .Select(byAge => new { len = AQL.Length(byAge), byAge.Key , Ages = byAge.Select(x => x.Age) });

            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(), 
                @"for `x` in `Person` collect `CV1` = `x`.`Age` into `C1` return { `len` : length( `C1` ) , `Key` : `CV1` , `Ages` : ( for `x` in `C1` return `x`.`x`.`Age` )  }".RemoveSpaces());
        }

        [Fact]
        public void Update()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().Update(p => new { Outfit = new { Color = "red" } });

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `p` in `Person` update 
`p` with @P1 in `Person`".RemoveSpaces());
            
            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[0].Value),
                JsonConvert.SerializeObject(new { Outfit = new { Color = "red" } } ));
        }

        [Fact]
        public void Update_WithKey()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Flight>().Update(f => new { Airline = "lufthansa" }, keySelector: k => k.Key);

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `f` in `Flight` update 
`f`.`_key` with @P1 in `Flight`".RemoveSpaces());

            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[0].Value),
                JsonConvert.SerializeObject(new { Airline = "lufthansa" }));
        }

        [Fact]
        public void Update_WithResult()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Flight>().Update(f => new { Airline = "lufthansa" }).ReturnResult(true);

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `f` in `Flight` update 
`f` with @P1 in `Flight` let crudResult = new return crudResult".RemoveSpaces());

            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[0].Value),
                JsonConvert.SerializeObject(new { Airline = "lufthansa" }));
        }

        [Fact]
        public void Update_WithOldResult()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Flight>().Where(x => x.Code < 100).Update(f => new { Airline = "lufthansa" }).ReturnResult(false);

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `x` in `Flight` filter ( `x`.`Code` < @P1 ) update 
`x` with @P2 in `Flight` let crudResult = old return crudResult".RemoveSpaces());

            Assert.Equal(queryData.BindVars[0].Value, 100);
            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[1].Value),
                JsonConvert.SerializeObject(new { Airline = "lufthansa" }));
        }

        [Fact]
        public void Update_ToAnotherCollection()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Flight>().Update(f => new { Airline = "lufthansa" }).In<Person>().ReturnResult(false);

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `f` in `Flight` update 
`f` with @P1 in `Person` let crudResult = old return crudResult".RemoveSpaces());
            
            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[0].Value),
                JsonConvert.SerializeObject(new { Airline = "lufthansa" }));
        }

        [Fact]
        public void Remove()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().Remove();

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `x` in `Person` remove `x` in `Person`".RemoveSpaces());
        }

        [Fact]
        public void Remove_WithKey()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Flight>().Remove(keySelector: x => x.Key);

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `x` in `Flight` remove `x`.`_key` in `Flight`".RemoveSpaces());
        }

        [Fact]
        public void Remove_WithResult()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Flight>().Remove().ReturnResult(false);

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `x` in `Flight` remove `x` in `Flight` let crudResult = old return crudResult".RemoveSpaces());
        }

        [Fact]
        public void Remove_InAnotherCollection()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Flight>().Remove().In<Person>().ReturnResult(false);

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `x` in `Flight` remove 
`x` in `Person` let crudResult = old return crudResult".RemoveSpaces());
        }

        [Fact]
        public void Insert()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().Insert();

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `x` in `Person` insert `x` in `Person`".RemoveSpaces());
        }

        [Fact]
        public void Insert_New()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().Insert(p => new { Outfit = new { Color = "red" } });

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `p` in `Person` insert @P1 in `Person`".RemoveSpaces());

            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[0].Value), JsonConvert.SerializeObject(new { Outfit = new { Color = "red" } }));
        }

        [Fact]
        public void Insert_WithResult()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Flight>().Insert().ReturnResult(false);

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `x` in `Flight` insert `x` in `Flight` let crudResult = new return crudResult".RemoveSpaces());
        }

        [Fact]
        public void Insert_InAnotherCollection()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Flight>().Insert().In<Person>().ReturnResult(false);

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `x` in `Flight` insert 
`x` in `Person` let crudResult = new return crudResult".RemoveSpaces());
        }

        [Fact]
        public void Replace()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Person>().Replace(p => new { Outfit = new { Color = "red" } });

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `p` in `Person` replace 
`p` with @P1 in `Person`".RemoveSpaces());

            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[0].Value),
                JsonConvert.SerializeObject(new { Outfit = new { Color = "red" } }));
        }

        [Fact]
        public void Replace_WithKey()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Flight>().Replace(f => new { Airline = "lufthansa" }, keySelector: k => k.Key);

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `f` in `Flight` replace 
`f`.`_key` with @P1 in `Flight`".RemoveSpaces());

            dynamic parameter = queryData.BindVars[0].Value;
            Assert.Equal(parameter.Airline, "lufthansa");
        }

        [Fact]
        public void Replace_WithResult()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Flight>().Replace(f => new { Airline = "lufthansa" }).ReturnResult(true);

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `f` in `Flight` replace 
`f` with @P1 in `Flight` let crudResult = new return crudResult".RemoveSpaces());
            
            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[0].Value),
                JsonConvert.SerializeObject(new { Airline = "lufthansa" }));
        }

        [Fact]
        public void Replace_WithOldResult()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Flight>().Where(x => x.Code < 100).Replace(f => new { Airline = "lufthansa" }).ReturnResult(false);

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `x` in `Flight` filter ( `x`.`Code` < @P1 ) replace 
`x` with @P2 in `Flight` let crudResult = old return crudResult".RemoveSpaces());

            Assert.Equal(queryData.BindVars[0].Value, 100);
            Assert.Equal(JsonConvert.SerializeObject(queryData.BindVars[1].Value),
                JsonConvert.SerializeObject(new { Airline = "lufthansa" }));
        }

        [Fact]
        public void Replace_ToAnotherCollection()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query<Flight>().Replace(f => new { Airline = "lufthansa" }).In<Person>().ReturnResult(false);

            var queryData = query.GetQueryData();

            Assert.Equal(queryData.Query.RemoveSpaces(), @"for `f` in `Flight` replace 
`f` with @P1 in `Person` let crudResult = old return crudResult".RemoveSpaces());
            
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
    }
}
