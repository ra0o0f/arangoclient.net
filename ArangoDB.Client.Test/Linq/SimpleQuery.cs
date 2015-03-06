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

            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(), "for `x` in `Person` collect `Age` = `x`.`Age` into `byAge` return `Age`");

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

            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(), @"for `p` in `Person` collect `Age` = `p`.`Age` into `C1`
return ( for `x` in `C1` return `x`.`p`.`Fullname` )".RemoveSpaces());
        }

        [Fact]
        public void GroupByGroupBySelectInto()
        {
            var db = DatabaseGenerator.Get();

            db.Setting.Linq.TranslateGroupByIntoName = identifier => identifier;

            var query = from p in db.Query<Person>()
                        group p by p.Age into byAge
                        select new
                            {
                                byAgebyHeight =
                                (
                                    from b in byAge
                                    group b by b.Height into byAgebyHeight
                                    select byAgebyHeight.Select(x => x.Fullname)
                                )
                            };

            Assert.Equal(query.GetQueryData().Query.RemoveSpaces(), @"for `p` in `Person` collect `Age` = `p`.`Age` into `byAge` 
return { `byAgebyHeight` : ( for `b` in `byAge` collect `Height` = `b`.`p`.`Height` into `byAgebyHeight` return ( for `x` in `byAgebyHeight` return `x`.`b`.`p`.`Fullname` ) ) }".RemoveSpaces());

            db.Setting.Linq.TranslateGroupByIntoName = null;
        }


    }
}
