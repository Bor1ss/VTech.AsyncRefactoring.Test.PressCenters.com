namespace PressCenters.Services.Sources.Tests.MainNews
{
    using System.Threading.Tasks;

    using PressCenters.Services.Sources.MainNews;

    using Xunit;

    public class NoviniBgMainNewsProviderTests
    {
        [Fact]
        public async Task GetMainNewsShouldWorkCorrectlyAsync()
        {
            var provider = new NoviniBgMainNewsProvider();
            var news = await provider.GetMainNewsAsync();
            Assert.NotNull(news.Title);
            Assert.True(news.Title.Length >= 10);
            Assert.Contains("novini.bg", news.OriginalUrl);
            Assert.StartsWith("https", news.OriginalUrl);
            Assert.StartsWith("https", news.ImageUrl);
        }
    }
}
