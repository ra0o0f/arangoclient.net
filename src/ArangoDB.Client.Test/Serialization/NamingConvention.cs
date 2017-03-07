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

            Assert.Equal(sharedSetting.Collection.ResolveCollectionName<ComplexModel>(), "ComplexModel");
        }

        [Fact]
        public void NameOfNestedCollection()
        {
            DatabaseSharedSetting sharedSetting = new DatabaseSharedSetting();

            Assert.Equal(sharedSetting.Collection.ResolveCollectionName<ComplexModel.NestedModel>(), "nested");
        }

        [Fact]
        public void NameOfMember()
        {
            DatabaseSharedSetting sharedSetting = new DatabaseSharedSetting();

            Assert.Equal(sharedSetting.Collection.ResolveNestedPropertyName<ComplexModel>(x => x.Name), "Name");
        }

        [Fact]
        public void NameOfNestedMember()
        {
            DatabaseSharedSetting sharedSetting = new DatabaseSharedSetting();

            Assert.Equal(sharedSetting.Collection.ResolveNestedPropertyName<ComplexModel>(x => x.Nested.Name), "fullname");
        }

        [Fact]
        public void NameOfNestedNestedMember()
        {
            DatabaseSharedSetting sharedSetting = new DatabaseSharedSetting();

            Assert.Equal(sharedSetting.Collection.ResolveNestedPropertyName<ComplexModel>(x => x.Nested.NestedNested.Name), "name");
        }
    }
}
