using ArangoDB.Client.ChangeTracking;
using Newtonsoft.Json.Linq;
using ArangoDB.Client.Test.Database;
using ArangoDB.Client.Test.Model;
using ArangoDB.Client.Test.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArangoDB.Client.Test.ChangeTracking
{
    public class SimpleChange
    {
        [Fact]
        public void WithoutIdentifier()
        {
            var db = DatabaseGenerator.Get();
            Person person = new Person();
            var tracker = new DocumentTracker(db);

            tracker.TrackChanges(person, ObjectUtility.CreateJObject(person, db));

            Assert.Throws<KeyNotFoundException>(() => tracker.GetChanges(person));
        }

        [Fact]
        public void None()
        {
            var db = DatabaseGenerator.Get();
            Product product = new Product();
            var tracker = new DocumentTracker(db);

            tracker.TrackChanges(product, ObjectUtility.CreateJObject(product, db));

            var changed = tracker.GetChanges(product);

            Assert.Empty(changed);
        }

        [Fact]
        public void IgnoreKey()
        {
            var db = DatabaseGenerator.Get();
            Product product = new Product();
            var tracker = new DocumentTracker(db);

            tracker.TrackChanges(product, ObjectUtility.CreateJObject(product, db));

            product.Key = "999";

            var changed = tracker.GetChanges(product);

            Assert.Empty(changed);
        }

        [Fact]
        public void OneProperty()
        {
            var db = DatabaseGenerator.Get();
            Product product = new Product();
            var tracker = new DocumentTracker(db);

            tracker.TrackChanges(product, ObjectUtility.CreateJObject(product, db));

            product.Quantity = 5;

            var changed = tracker.GetChanges(product);

            var expectedJson = JObject.Parse("{'Quantity':5}");
            Assert.True(JObject.DeepEquals(expectedJson, changed));
        }

        [Fact]
        public void OnePropertyToNull()
        {
            var db = DatabaseGenerator.Get();
            Product product = new Product() { Title = "Pen" };
            var tracker = new DocumentTracker(db);

            tracker.TrackChanges(product, ObjectUtility.CreateJObject(product, db));

            product.Title = null;

            var changed = tracker.GetChanges(product);

            var expectedJson = JObject.Parse("{'Title':null}");
            Assert.True(JObject.DeepEquals(expectedJson, changed));
        }

        [Fact]
        public void TwoProperty()
        {
            var db = DatabaseGenerator.Get();
            Product product = new Product();
            var tracker = new DocumentTracker(db);

            tracker.TrackChanges(product, ObjectUtility.CreateJObject(product, db));

            product.Quantity = 5;
            product.Title = "Pen";

            var changed = tracker.GetChanges(product);

            var expectedJson = JObject.Parse("{'Quantity':5,'Title':'Pen'}");
            Assert.True(JObject.DeepEquals(expectedJson, changed));
        }

        [Fact]
        public void List()
        {
            var db = DatabaseGenerator.Get();
            Product product = new Product() { Tags = new List<string>() { "Soft" } };
            var tracker = new DocumentTracker(db);

            tracker.TrackChanges(product, ObjectUtility.CreateJObject(product, db));

            product.Tags.Add("Hard");

            var changed = tracker.GetChanges(product);

            var expectedJson = JObject.Parse("{'Tags':['Soft','Hard']}");
            Assert.True(JObject.DeepEquals(expectedJson, changed));
        }

        [Fact]
        public void ListToNull()
        {
            var db = DatabaseGenerator.Get();
            Product product = new Product() { Tags = new List<string>() { "Soft" } };
            var tracker = new DocumentTracker(db);

            tracker.TrackChanges(product, ObjectUtility.CreateJObject(product, db));

            product.Tags = null;

            var changed = tracker.GetChanges(product);

            var expectedJson = JObject.Parse("{'Tags':null}");
            Assert.True(JObject.DeepEquals(expectedJson, changed));
        }

        [Fact]
        public void ListFromNull()
        {
            var db = DatabaseGenerator.Get();
            Product product = new Product();
            var tracker = new DocumentTracker(db);

            tracker.TrackChanges(product, ObjectUtility.CreateJObject(product, db));

            product.Tags = new List<string>() { "Soft" };

            var changed = tracker.GetChanges(product);

            var expectedJson = JObject.Parse("{'Tags':['Soft']}");
            Assert.True(JObject.DeepEquals(expectedJson, changed));
        }

        [Fact]
        public void Dictionary()
        {
            var db = DatabaseGenerator.Get();
            Product product = new Product() { TypeQuantities = new Dictionary<string, int>() { { "Soft", 1 } } };
            var tracker = new DocumentTracker(db);

            tracker.TrackChanges(product, ObjectUtility.CreateJObject(product, db));

            product.TypeQuantities.Add("Hard", 2);

            var changed = tracker.GetChanges(product);

            var expectedJson = JObject.Parse("{'TypeQuantities':{'Hard':2}}");
            Assert.True(JObject.DeepEquals(expectedJson, changed));
        }

        [Fact]
        public void DictionaryToNull()
        {
            var db = DatabaseGenerator.Get();
            Product product = new Product() { TypeQuantities = new Dictionary<string, int>() { { "Soft", 1 } } };
            var tracker = new DocumentTracker(db);

            tracker.TrackChanges(product, ObjectUtility.CreateJObject(product, db));

            product.TypeQuantities = null;

            var changed = tracker.GetChanges(product);

            var expectedJson = JObject.Parse("{'TypeQuantities':null}");
            Assert.True(JObject.DeepEquals(expectedJson, changed));
        }

        [Fact]
        public void DictionaryFromNull()
        {
            var db = DatabaseGenerator.Get();
            Product product = new Product();
            var tracker = new DocumentTracker(db);

            tracker.TrackChanges(product, ObjectUtility.CreateJObject(product, db));

            product.TypeQuantities = new Dictionary<string, int>() { { "Soft", 1 } };

            var changed = tracker.GetChanges(product);

            var expectedJson = JObject.Parse("{'TypeQuantities':{'Soft':1}}");
            Assert.True(JObject.DeepEquals(expectedJson, changed));
        }

        [Fact]
        public void DictionaryTwoProperty()
        {
            var db = DatabaseGenerator.Get();
            Product product = new Product() { TypeQuantities = new Dictionary<string, int>() { { "Soft", 1 } } };
            var tracker = new DocumentTracker(db);

            tracker.TrackChanges(product, ObjectUtility.CreateJObject(product, db));

            product.TypeQuantities["Soft"] = 5;
            product.TypeQuantities.Add("Hard", 2);

            var changed = tracker.GetChanges(product);

            var expectedJson = JObject.Parse("{'TypeQuantities':{'Hard':2,'Soft':5}}");
            Assert.True(JObject.DeepEquals(expectedJson, changed));
        }

        [Fact]
        public void NestedOneProperty()
        {
            var db = DatabaseGenerator.Get();
            Product product = new Product() { Category = new Category() { Title = "Featured" } };
            var tracker = new DocumentTracker(db);

            tracker.TrackChanges(product, ObjectUtility.CreateJObject(product, db));

            product.Category.Title = "Stock";

            var changed = tracker.GetChanges(product);

            var expectedJson = JObject.Parse("{'Category':{'Title':'Stock'}}");
            Assert.True(JObject.DeepEquals(expectedJson, changed));
        }

        [Fact]
        public void NestedFromNull()
        {
            var db = DatabaseGenerator.Get();
            Product product = new Product();
            var tracker = new DocumentTracker(db);

            tracker.TrackChanges(product, ObjectUtility.CreateJObject(product, db));

            product.Category = new Category() { Title = "Featured" };

            var changed = tracker.GetChanges(product);

            var expectedJson = JObject.Parse("{'Category':{'Title':'Featured','Tags':null,'Seller':null}}");
            Assert.True(JObject.DeepEquals(expectedJson, changed));
        }

        [Fact]
        public void NestedToNull()
        {
            var db = DatabaseGenerator.Get();
            Product product = new Product() { Category = new Category() { Title = "Featured" } };
            var tracker = new DocumentTracker(db);

            tracker.TrackChanges(product, ObjectUtility.CreateJObject(product, db));

            product.Category = null;

            var changed = tracker.GetChanges(product);

            var expectedJson = JObject.Parse("{'Category':null}");
            Assert.True(JObject.DeepEquals(expectedJson, changed));
        }

        [Fact]
        public void NestedList()
        {
            var db = DatabaseGenerator.Get();
            Product product = new Product() { Category = new Category() { Tags = new List<string>() { "Soft" } } };
            var tracker = new DocumentTracker(db);

            tracker.TrackChanges(product, ObjectUtility.CreateJObject(product, db));

            product.Category.Tags.Add("Hard");

            var changed = tracker.GetChanges(product);

            var expectedJson = JObject.Parse("{'Category':{'Tags':['Soft','Hard']}}");
            Assert.True(JObject.DeepEquals(expectedJson, changed));
        }

        [Fact]
        public void NestedInNestedDictionary()
        {
            var db = DatabaseGenerator.Get();
            Product product = new Product()
            {
                Category = new Category
                {
                    Seller = new Seller
                    {
                        ProductSells = new Dictionary<string, int>() { { "Pen", 1 } }
                    }
                }
            };
            var tracker = new DocumentTracker(db);

            tracker.TrackChanges(product, ObjectUtility.CreateJObject(product, db));

            product.Category.Seller.ProductSells.Add("Pencil", 3);

            var changed = tracker.GetChanges(product);

            var expectedJson = JObject.Parse("{'Category':{'Seller':{'ProductSells':{'Pencil':3}}}}");
            Assert.True(JObject.DeepEquals(expectedJson, changed));
        }

        [Fact]
        public void NestedInNestedOneProperty()
        {
            var db = DatabaseGenerator.Get();
            Product product = new Product()
            {
                Category = new Category
                {
                    Seller = new Seller
                    {
                        TotalSells = 5000
                    }
                }
            };
            var tracker = new DocumentTracker(db);

            tracker.TrackChanges(product, ObjectUtility.CreateJObject(product, db));

            product.Category.Seller.TotalSells = 11000;

            var changed = tracker.GetChanges(product);

            var expectedJson = JObject.Parse("{'Category':{'Seller':{'TotalSells':11000}}}");
            Assert.True(JObject.DeepEquals(expectedJson, changed));
        }

        [Fact]
        public void OldObjectDontHaveNewObjectToken()
        {
            var db = DatabaseGenerator.Get();

            var oldObject = JObject.Parse("{'Name':'raoof'}");
            var newObject = JObject.Parse("{'Name':'raoof','Family':'hojat'}");

            JObject changedObject = new JObject();

            new DocumentTracker(db).CreateChangedDocument(oldObject, newObject, ref changedObject);

            var expectedJson = JObject.Parse("{'Family':'hojat'}");
            Assert.True(JObject.DeepEquals(expectedJson, changedObject));
        }

        [Fact]
        public void NewObjectDontHaveOldObjectToken()
        {
            var db = DatabaseGenerator.Get();

            var oldObject = JObject.Parse("{'Name':'raoof','Family':'hojat'}");
            var newObject = JObject.Parse("{'Name':'raoof'}");

            JObject changedObject = new JObject();

            new DocumentTracker(db).CreateChangedDocument(oldObject, newObject, ref changedObject);

            var expectedJson = JObject.Parse("{}");
            Assert.True(JObject.DeepEquals(expectedJson, changedObject));
        }
    }
}
