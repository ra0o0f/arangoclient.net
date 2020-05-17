using System.Threading.Tasks;
using ArangoDB.Client.Data;
using Xunit;

namespace ArangoDB.Client.Examples.Views
{
    public class ViewCommand : TestDatabaseSetup
    {
        [Fact]
        public async Task CreateView()
        {
            var viewLinkData = new ViewLinkData
            {
                Collections =
                {
                    {
                        "Person",
                        new ViewLinkItemData
                        {
                            Fields = new FieldData
                            {
                                Fields =
                                {
                                    {
                                        "Name",
                                        new ViewLinkItemData
                                        {
                                            Analyzers = new[] {"text_de"}
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };


            var createResult = await db.CreateSearchViewAsync("test", viewLinkData);

            Assert.NotNull(createResult.Id);
        }
        
        [Fact]
        public async Task ListView()
        {
            var searchViewResults = await db.ListSearchViewAsync();

            Assert.True(searchViewResults.Exists(result => result.Name == "test"));
        }
        
        [Fact]
        public async Task DropView()
        {
            var dropResult = await db.DropSearchViewAsync("test");

            Assert.True(dropResult.Result);
        }
    }
}