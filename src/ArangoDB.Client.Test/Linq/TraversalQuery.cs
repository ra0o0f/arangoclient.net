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
    public class TraversalQuery
    {
        [Fact]
        public void Traversal()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .Traversal<Person, Friend>("Person/1234")
                .Depth(1, 5)
                .OutBound()
                .Graph("SocialGraph")
                .Select(g => g)
                .GetQueryData();

            Assert.Equal(query.Query.RemoveSpaces(), @"
for `graph_0_Vertex`, `graph_0_Edge`, `graph_0_Path` 
in 1..5 outbound @P1 graph ""SocialGraph""
return { `vertex` : `graph_0_Vertex`, `edge` : `graph_0_Edge`, `path` : `graph_0_Path` }".RemoveSpaces());

            Assert.Equal("Person/1234", query.BindVars[0].Value);
        }

        [Fact]
        public void TraversalWithoutDepth()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .Traversal<Person, Friend>("Person/1234")
                .OutBound()
                .Graph("SocialGraph")
                .Select(g => g)
                .GetQueryData();

            Assert.Equal(query.Query.RemoveSpaces(), @"
for `graph_0_Vertex`, `graph_0_Edge`, `graph_0_Path` 
in outbound @P1 graph ""SocialGraph""
return { `vertex` : `graph_0_Vertex`, `edge` : `graph_0_Edge`, `path` : `graph_0_Path` }".RemoveSpaces());
        }

        [Fact]
        public void TraversalOutBoundDirection()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .Traversal<Person, Friend>("Person/1234")
                .OutBound()
                .Graph("SocialGraph")
                .Select(g => g)
                .GetQueryData();

            Assert.Equal(query.Query.RemoveSpaces(), @"
for `graph_0_Vertex`, `graph_0_Edge`, `graph_0_Path` 
in outbound @P1 graph ""SocialGraph""
return { `vertex` : `graph_0_Vertex`, `edge` : `graph_0_Edge`, `path` : `graph_0_Path` }".RemoveSpaces());
        }

        [Fact]
        public void TraversalInBoundDirection()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .Traversal<Person, Friend>("Person/1234")
                .InBound()
                .Graph("SocialGraph")
                .Select(g => g)
                .GetQueryData();

            Assert.Equal(query.Query.RemoveSpaces(), @"
for `graph_0_Vertex`, `graph_0_Edge`, `graph_0_Path` 
in inbound @P1 graph ""SocialGraph""
return { `vertex` : `graph_0_Vertex`, `edge` : `graph_0_Edge`, `path` : `graph_0_Path` }".RemoveSpaces());
        }

        [Fact]
        public void TraversalAnyDirection()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .Traversal<Person, Friend>("Person/1234")
                .AnyDirection()
                .Graph("SocialGraph")
                .Select(g => g)
                .GetQueryData();

            Assert.Equal(query.Query.RemoveSpaces(), @"
for `graph_0_Vertex`, `graph_0_Edge`, `graph_0_Path` 
in any @P1 graph ""SocialGraph""
return { `vertex` : `graph_0_Vertex`, `edge` : `graph_0_Edge`, `path` : `graph_0_Path` }".RemoveSpaces());
        }

        [Fact]
        public void TraversalWithoutDirection()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .Traversal<Person, Friend>("Person/1234")
                .Graph("SocialGraph")
                .Select(g => g)
                .GetQueryData();

            Assert.Equal(query.Query.RemoveSpaces(), @"
for `graph_0_Vertex`, `graph_0_Edge`, `graph_0_Path` 
in any @P1 graph ""SocialGraph""
return { `vertex` : `graph_0_Vertex`, `edge` : `graph_0_Edge`, `path` : `graph_0_Path` }".RemoveSpaces());
        }

        [Fact]
        public void TraversalSelectVertexMember()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .Traversal<Person, Friend>("Person/1234")
                .Graph("SocialGraph")
                .Select(g => g.Vertex.Age)
                .GetQueryData();

            Assert.Equal(query.Query.RemoveSpaces(), @"
for `graph_0_Vertex`, `graph_0_Edge`, `graph_0_Path` 
in any @P1 graph ""SocialGraph""
return `graph_0_Vertex`.`Age`".RemoveSpaces());
        }

        [Fact]
        public void TraversalSelectEdgeMember()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .Traversal<Person, Friend>("Person/1234")
                .Graph("SocialGraph")
                .Select(g => g.Edge.Key)
                .GetQueryData();

            Assert.Equal(query.Query.RemoveSpaces(), @"
for `graph_0_Vertex`, `graph_0_Edge`, `graph_0_Path` 
in any @P1 graph ""SocialGraph""
return `graph_0_Edge`.`_key`".RemoveSpaces());
        }

        [Fact]
        public void TraversalSelectPathMember()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .Traversal<Person, Friend>("Person/1234")
                .Graph("SocialGraph")
                .Select(g => g.Path.Vertices[0].Age)
                .GetQueryData();

            Assert.Equal(query.Query.RemoveSpaces(), @"
for `graph_0_Vertex`, `graph_0_Edge`, `graph_0_Path` 
in any @P1 graph ""SocialGraph""
return `graph_0_Path`.`vertices` [ @P2 ] .`Age`".RemoveSpaces());
        }

        [Fact]
        public void TraversalOptions()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .Traversal<Person, Friend>("Person/1234")
                .Depth(1, 5)
                .OutBound()
                .Graph("SocialGraph")
                .Options(new { bfs = true })
                .Select(g => g)
                .GetQueryData();

            Assert.Equal(query.Query.RemoveSpaces(), @"
for `graph_0_Vertex`, `graph_0_Edge`, `graph_0_Path` 
in 1..5 outbound @P1 graph ""SocialGraph""  options {""bfs"":true}
return { `vertex` : `graph_0_Vertex`, `edge` : `graph_0_Edge`, `path` : `graph_0_Path` }".RemoveSpaces());
        }

        [Fact]
        public void TraversalInEdges()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .Traversal<Person, Friend>("Person/1234")
                .Depth(1, 5)
                .OutBound()
                .Edge(db.NameOf<Friend>())
                .Edge(db.NameOf<Flight>())
                .Select(g => g)
                .GetQueryData();

            Assert.Equal(query.Query.RemoveSpaces(), @"
for `graph_0_Vertex`, `graph_0_Edge`, `graph_0_Path` 
in 1..5 outbound @P1 `Friend`, `Flight`
return { `vertex` : `graph_0_Vertex`, `edge` : `graph_0_Edge`, `path` : `graph_0_Path` }".RemoveSpaces());

            Assert.Equal("Person/1234", query.BindVars[0].Value);
        }

        [Fact]
        public void TraversalInEdgesWithDirection()
        {
            var db = DatabaseGenerator.Get();

            var query = db.Query()
                .Traversal<Person, Friend>("Person/1234")
                .Depth(1, 5)
                .OutBound()
                .Edge(db.NameOf<Friend>(), EdgeDirection.Inbound)
                .Edge(db.NameOf<Flight>())
                .Select(g => g)
                .GetQueryData();

            Assert.Equal(query.Query.RemoveSpaces(), @"
for `graph_0_Vertex`, `graph_0_Edge`, `graph_0_Path` 
in 1..5 outbound @P1 inbound `Friend`, `Flight`
return { `vertex` : `graph_0_Vertex`, `edge` : `graph_0_Edge`, `path` : `graph_0_Path` }".RemoveSpaces());

            Assert.Equal("Person/1234", query.BindVars[0].Value);
        }
    }
}
