using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Test.Database
{
    public class DatabaseGenerator
    {
        static Lazy<ArangoDatabase> db = new Lazy<ArangoDatabase>(() => 
        {
            var database = new ArangoDatabase(url:"http://localhost.:8529",database:"bakoot");
            return database;
        });

        public static ArangoDatabase Get()
        {
            return db.Value;
        }
    }
}
