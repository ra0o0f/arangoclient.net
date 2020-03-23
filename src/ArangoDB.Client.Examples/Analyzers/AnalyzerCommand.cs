using System.Threading.Tasks;
using ArangoDB.Client.Data;
using Xunit;

namespace ArangoDB.Client.Examples.Analyzers
{
    public class AnalyzerCommand : TestDatabaseSetup
    {
        [Fact]
        public async Task CreateView()
        {
            var textAnalyzerData = new TextAnalyzerData
            {
                Accent = false,
                Case = "lower",
                Locale = "de.utf-8",
                Stemming = false,
                Stopwords = new[] {"test"},
                EdgeNgram = new NgramData
                {
                    Max = 50,
                    Min = 5,
                    PreserveOriginal = true
                }
            };

            var createResult = await db.CreateTextAnalyzer("analyzertest", textAnalyzerData,
                new[] {"frequency", "norm", "position"});

            Assert.NotNull(createResult.Name);
        }

        [Fact]
        public async Task ListView()
        {
            var searchViewResults = await db.ListAnalyzerAsync();

            Assert.True(searchViewResults.Exists(result => result.Name == "text_en"));
        }

        [Fact]
        public async Task DropView()
        {
            var dropResult = await db.DropAnalyzer("analyzertest");

            Assert.Equal("ExampleDB::analyzertest", dropResult.Name);
        }
    }
}