﻿namespace PressCenters.Services.Sources.Tests.MainNews
{
    using PressCenters.Services.Sources.MainNews;

    using Xunit;

    public class MediapoolBgMainNewsProviderTests
    {
        [Fact]
        public async void GetMainNewsShouldWorkCorrectly()
        {
            var provider = new MediapoolBgMainNewsProvider();
            var news = await provider.GetMainNews();
            Assert.NotNull(news.Title);
            Assert.True(news.Title.Length >= 10);
            Assert.Contains("mediapool.bg", news.OriginalUrl);
            Assert.Contains("mediapool.bg", news.ImageUrl);
        }
    }
}
