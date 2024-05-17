namespace PressCenters.Services.Sources.Tests.MainNews
{
    using System.Threading.Tasks;

    using PressCenters.Services.Sources.MainNews;

    using Xunit;

    public class DnesBgMainNewsProviderTests
    {
        [Fact]
        public async Task GetMainNewsShouldWorkCorrectlyAsync()
        {
            var provider = new DnesBgMainNewsProvider();
            var news = await provider.GetMainNewsAsync();
            Assert.NotNull(news.Title);
            Assert.True(news.Title.Length >= 10);
            Assert.Contains("dnes.bg", news.OriginalUrl);
            Assert.StartsWith("http", news.OriginalUrl);
            Assert.StartsWith("http", news.ImageUrl);
        }
    }
}
