using ArangoDB.Client.Test.Database;
using ArangoDB.Client.Test.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ArangoDB.Client.Test.Utility;

namespace ArangoDB.Client.Test.Linq
{
    public class ShortestPathQuery
    {
        [Fact]
        public void ShortestPath()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .ShortestPath<Person, Friend>("Person/1234", "Person/4321")
                .OutBound()
                .Graph("SocialGraph")
                .Select(g => g)
                .GetQueryData();

            Assert.Equal(query.Query.RemoveSpaces(), @"
for `graph_0_Vertex`, `graph_0_Edge` 
in outbound shortest_path @P1 to @P2 graph ""SocialGraph""
return { `vertex` : `graph_0_Vertex`, `edge` : `graph_0_Edge` }".RemoveSpaces());

            Assert.Equal(query.BindVars[0].Value, "Person/1234");
            Assert.Equal(query.BindVars[1].Value, "Person/4321");
        }

        [Fact]
        public void ShortestPathOutBoundDirection()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .ShortestPath<Person, Friend>("Person/1234", "Person/4321")
                .OutBound()
                .Graph("SocialGraph")
                .Select(g => g)
                .GetQueryData();

            Assert.Equal(query.Query.RemoveSpaces(), @"
for `graph_0_Vertex`, `graph_0_Edge`
in outbound shortest_path @P1 to @P2 graph ""SocialGraph""
return { `vertex` : `graph_0_Vertex`, `edge` : `graph_0_Edge` }".RemoveSpaces());
        }

        [Fact]
        public void ShortestPathInBoundDirection()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .ShortestPath<Person, Friend>("Person/1234", "Person/4321")
                .InBound()
                .Graph("SocialGraph")
                .Select(g => g)
                .GetQueryData();

            Assert.Equal(query.Query.RemoveSpaces(), @"
for `graph_0_Vertex`, `graph_0_Edge`
in inbound shortest_path @P1 to @P2 graph ""SocialGraph""
return { `vertex` : `graph_0_Vertex`, `edge` : `graph_0_Edge` }".RemoveSpaces());
        }

        [Fact]
        public void ShortestPathAnyDirection()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .ShortestPath<Person, Friend>("Person/1234", "Person/4321")
                .AnyDirection()
                .Graph("SocialGraph")
                .Select(g => g)
                .GetQueryData();

            Assert.Equal(query.Query.RemoveSpaces(), @"
for `graph_0_Vertex`, `graph_0_Edge` 
in any shortest_path @P1 to @P2 graph ""SocialGraph""
return { `vertex` : `graph_0_Vertex`, `edge` : `graph_0_Edge` }".RemoveSpaces());
        }

        [Fact]
        public void ShortestPathWithoutDirection()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .ShortestPath<Person, Friend>("Person/1234", "Person/4321")
                .Graph("SocialGraph")
                .Select(g => g)
                .GetQueryData();

            Assert.Equal(query.Query.RemoveSpaces(), @"
for `graph_0_Vertex`, `graph_0_Edge` 
in any shortest_path @P1 to @P2 graph ""SocialGraph""
return { `vertex` : `graph_0_Vertex`, `edge` : `graph_0_Edge` }".RemoveSpaces());
        }

        [Fact]
        public void ShortestPathSelectVertexMember()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .ShortestPath<Person, Friend>("Person/1234", "Person/4321")
                .Graph("SocialGraph")
                .Select(g => g.Vertex.Age)
                .GetQueryData();

            Assert.Equal(query.Query.RemoveSpaces(), @"
for `graph_0_Vertex`, `graph_0_Edge`
in any shortest_path @P1 to @P2 graph ""SocialGraph""
return `graph_0_Vertex`.`Age`".RemoveSpaces());
        }

        [Fact]
        public void ShortestPathSelectEdgeMember()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .ShortestPath<Person, Friend>("Person/1234", "Person/4321")
                .Graph("SocialGraph")
                .Select(g => g.Edge.Key)
                .GetQueryData();

            Assert.Equal(query.Query.RemoveSpaces(), @"
for `graph_0_Vertex`, `graph_0_Edge` 
in any shortest_path @P1 to @P2 graph ""SocialGraph""
return `graph_0_Edge`.`_key`".RemoveSpaces());
        }

        [Fact]
        public void ShortestPathOptions()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .ShortestPath<Person, Friend>("Person/1234", "Person/4321")
                .OutBound()
                .Graph("SocialGraph")
                .Options(new { bfs = true })
                .Select(g => g)
                .GetQueryData();

            Assert.Equal(query.Query.RemoveSpaces(), @"
for `graph_0_Vertex`, `graph_0_Edge`
in outbound shortest_path @P1 to @P2 graph ""SocialGraph""  options {""bfs"":true}
return { `vertex` : `graph_0_Vertex`, `edge` : `graph_0_Edge` }".RemoveSpaces());
        }

        [Fact]
        public void ShortestPathInEdges()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .ShortestPath<Person, Friend>("Person/1234", "Person/4321")
                .OutBound()
                .Edge(db.NameOf<Friend>())
                .Edge(db.NameOf<Flight>())
                .Select(g => g)
                .GetQueryData();

            Assert.Equal(query.Query.RemoveSpaces(), @"
for `graph_0_Vertex`, `graph_0_Edge` 
in outbound shortest_path @P1 to @P2 `Friend`, `Flight`
return { `vertex` : `graph_0_Vertex`, `edge` : `graph_0_Edge` }".RemoveSpaces());

            Assert.Equal(query.BindVars[0].Value, "Person/1234");
        }

        [Fact]
        public void ShortestPathInEdgesWithDirection()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .ShortestPath<Person, Friend>("Person/1234", "Person/4321")
                .OutBound()
                .Edge(db.NameOf<Friend>(), EdgeDirection.Inbound)
                .Edge(db.NameOf<Flight>())
                .Select(g => g)
                .GetQueryData();

            Assert.Equal(query.Query.RemoveSpaces(), @"
for `graph_0_Vertex`, `graph_0_Edge`
in outbound shortest_path @P1 to @P2 inbound `Friend`, `Flight`
return { `vertex` : `graph_0_Vertex`, `edge` : `graph_0_Edge` }".RemoveSpaces());

            Assert.Equal(query.BindVars[0].Value, "Person/1234");
        }
    }
}
