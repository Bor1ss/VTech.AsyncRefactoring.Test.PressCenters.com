namespace PressCenters.Services.Sources.Tests.MainNews
{
    using System.Threading.Tasks;

    using PressCenters.Services.Sources.MainNews;

    using Xunit;

    public class ReutersMainNewsProviderTests
    {
        [Fact]
        public async Task GetMainNewsShouldWorkCorrectlyAsync()
        {
            var provider = new ReutersMainNewsProvider();
            var news = await provider.GetMainNewsAsync();
            Assert.NotNull(news.Title);
            Assert.True(news.Title.Length >= 10);
            Assert.Contains("reuters.com", news.OriginalUrl);
            Assert.StartsWith("https", news.OriginalUrl);
            Assert.StartsWith("https", news.ImageUrl);
        }
    }
}
