namespace PressCenters.Services.Sources.Tests.MainNews
{
    using System.Threading.Tasks;

    using PressCenters.Services.Sources.MainNews;

    using Xunit;

    public class BtaBgMainNewsProviderTests
    {
        [Fact]
        public async Task GetMainNewsShouldWorkCorrectlyAsync()
        {
            var provider = new BtaBgMainNewsProvider();
            var news = await provider.GetMainNewsAsync();
            Assert.NotNull(news.Title);
            Assert.True(news.Title.Length >= 10);
            Assert.Contains("bta.bg", news.OriginalUrl);
            Assert.Contains("bta.bg", news.ImageUrl);
        }
    }
}
