using Newtonsoft.Json.Converters;
using ArangoDB.Client.Serialization;
using ArangoDB.Client.Test.Database;
using ArangoDB.Client.Test.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArangoDB.Client.Test.Setting
{
    public class SerializationSetting
    {
        [Fact]
        public void EnumConvertion()
        {
            var db = DatabaseGenerator.Get();

            db.Setting.Serialization.SerializeEnumAsInteger = false;

            var stringValue = new DocumentSerializer(db).SerializeWithoutReader(ProductStatus.Available);

            Assert.Equal("\"Available\"", stringValue);

            db.Setting.Serialization.SerializeEnumAsInteger = true;

            var integerValue = new DocumentSerializer(db).SerializeWithoutReader(ProductStatus.SoldOut);

            Assert.Equal("1", integerValue);
        }

        [Fact]
        public void CustomConverter()
        {
            var db = DatabaseGenerator.Get();

            db.Setting.Serialization.Converters.Add(new StringEnumConverter());

            var stringValue = new DocumentSerializer(db).SerializeWithoutReader(ProductStatus.Available);

            Assert.Equal("\"Available\"", stringValue);

            db.Setting.Serialization.Converters.Clear();

            var integerValue = new DocumentSerializer(db).SerializeWithoutReader(ProductStatus.Available);

            Assert.Equal("0", integerValue);
        }
    }
}
