using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Test.Database
{
    public class DatabaseGenerator
    {
        static Lazy<IArangoDatabase> db = new Lazy<IArangoDatabase>(() => 
        {
            var database = new ArangoDatabase(url:"http://localhost.:8529",database:"bakoot");
            return database;
        });

        public static IArangoDatabase Get()
        {
            return db.Value;
        }
    }
}
