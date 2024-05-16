namespace PressCenters.Services.Sources.Tests.MainNews
{
    using PressCenters.Services.Sources.MainNews;
    using System.Threading.Tasks;
    using Xunit;

    public class BtaBgMainNewsProviderTests
    {
        [Fact]
        public async Task GetMainNewsShouldWorkCorrectly()
        {
            var provider = new BtaBgMainNewsProvider();
            var news = await provider.GetMainNews();
            Assert.NotNull(news.Title);
            Assert.True(news.Title.Length >= 10);
            Assert.Contains("bta.bg", news.OriginalUrl);
            Assert.Contains("bta.bg", news.ImageUrl);
        }
    }
}
