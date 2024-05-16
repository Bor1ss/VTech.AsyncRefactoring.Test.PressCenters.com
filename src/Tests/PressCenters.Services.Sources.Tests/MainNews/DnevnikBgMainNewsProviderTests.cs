namespace PressCenters.Services.Sources.Tests.MainNews
{
    using PressCenters.Services.Sources.MainNews;
    using System.Threading.Tasks;
    using Xunit;

    public class DnevnikBgMainNewsProviderTests
    {
        [Fact]
        public async Task GetMainNewsShouldWorkCorrectly()
        {
            var provider = new DnevnikBgMainNewsProvider();
            var news = await provider.GetMainNews();
            Assert.NotNull(news.Title);
            Assert.True(news.Title.Length >= 10);
            Assert.Contains("dnevnik.bg", news.OriginalUrl);
            Assert.StartsWith("https", news.OriginalUrl);
            Assert.StartsWith("https", news.ImageUrl);
        }
    }
}
