using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Test.Model
{
    public class Product
    {
        public Product()
        {
            this.Key = "123";
            this.Id = "Product/123";
            this.Rev = "12345";
        }

        [DocumentProperty(Identifier = IdentifierType.Key)]
        public string Key { get; set; }

        [DocumentProperty(Identifier = IdentifierType.Revision)]
        public string Rev { get; set; }

        [DocumentProperty(Identifier = IdentifierType.Handle)]
        public string Id { get; set; }

        public string Title { get; set; }

        public double Price { get; set; }

        public int Quantity { get; set; }

        public List<string> Tags { get; set; }

        public Dictionary<string, int> TypeQuantities { get; set; }

        public Category Category { get; set; }
    }

    public class Category
    {
        public string Title { get; set; }

        public List<string> Tags { get; set; }

        public Seller Seller { get; set; }
    }

    public class Seller
    {
        public int TotalSells { get; set; }

        public Dictionary<string, int>  ProductSells { get; set; }
    }
}
