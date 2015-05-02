using ArangoDB.Client;
using ArangoDB.Client.Common.Newtonsoft.Json;
using ArangoDB.Client.Data;
using ArangoDB.Client.Test.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            dbtest();
        }

        static void dbtest()
        {
            ArangoDatabase.ChangeSetting(s =>
            {
                s.Database = "SampleDB";
                s.Url = "http://localhost:8529";
            });

            //using (var db = ArangoDatabase.CreateWithSetting())
            //{
            //    //Console.WriteLine(q.Query);

            //    db.CreateGraph("", new List<EdgeDefinitionData>
            //    {
            //        new EdgeDefinitionData
            //        {
            //            Collection = "Col1",
            //            From
            //        }
            //    });

            //}
        }
    }
}
