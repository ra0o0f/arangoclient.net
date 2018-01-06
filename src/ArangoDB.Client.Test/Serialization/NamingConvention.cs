using ArangoDB.Client.Test.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArangoDB.Client.Test.Serialization
{
    public class NamingConvention
    {
        [Fact]
        public void NameOfCollection()
        {
            DatabaseSharedSetting sharedSetting = new DatabaseSharedSetting();

            Assert.Equal("ComplexModel", sharedSetting.Collection.ResolveCollectionName<ComplexModel>());
        }

        [Fact]
        public void NameOfNestedCollection()
        {
            DatabaseSharedSetting sharedSetting = new DatabaseSharedSetting();

            Assert.Equal("nested", sharedSetting.Collection.ResolveCollectionName<ComplexModel.NestedModel>());
        }

        [Fact]
        public void NameOfMember()
        {
            DatabaseSharedSetting sharedSetting = new DatabaseSharedSetting();

            Assert.Equal("Name", sharedSetting.Collection.ResolveNestedPropertyName<ComplexModel>(x => x.Name));
        }

        [Fact]
        public void NameOfNestedMember()
        {
            DatabaseSharedSetting sharedSetting = new DatabaseSharedSetting();

            Assert.Equal("fullname", sharedSetting.Collection.ResolveNestedPropertyName<ComplexModel>(x => x.Nested.Name));
        }

        [Fact]
        public void NameOfNestedNestedMember()
        {
            DatabaseSharedSetting sharedSetting = new DatabaseSharedSetting();

            Assert.Equal("name", sharedSetting.Collection.ResolveNestedPropertyName<ComplexModel>(x => x.Nested.NestedNested.Name));
        }
    }
}
