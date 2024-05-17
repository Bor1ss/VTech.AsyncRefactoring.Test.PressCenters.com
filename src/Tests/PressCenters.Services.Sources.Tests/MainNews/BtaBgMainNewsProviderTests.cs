namespace PressCenters.Services.Sources.Tests.MainNews
{
    using PressCenters.Services.Sources.MainNews;

    using Xunit;

    public class BtaBgMainNewsProviderTests
    {
        [Fact]
        public void GetMainNewsShouldWorkCorrectly()
        {
            var provider = new BtaBgMainNewsProvider();
            var news = provider.GetMainNewsAsync().GetAwaiter().GetResult();
            Assert.NotNull(news.Title);
            Assert.True(news.Title.Length >= 10);
            Assert.Contains("bta.bg", news.OriginalUrl);
            Assert.Contains("bta.bg", news.ImageUrl);
        }
    }
}
