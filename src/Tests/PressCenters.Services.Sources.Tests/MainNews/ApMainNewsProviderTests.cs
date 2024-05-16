﻿namespace PressCenters.Services.Sources.Tests.MainNews
{
    using PressCenters.Services.Sources.MainNews;

    using Xunit;

    public class ApMainNewsProviderTests
    {
        [Fact]
        public async void GetMainNewsShouldWorkCorrectly()
        {
            var provider = new ApMainNewsProvider();
            var news = await provider.GetMainNews();
            Assert.NotNull(news.Title);
            Assert.NotNull(news.ImageUrl);
            Assert.True(news.Title.Length >= 10);
            Assert.Contains("apnews.com", news.OriginalUrl);
            Assert.StartsWith("https", news.OriginalUrl);
            Assert.StartsWith("https", news.ImageUrl);
        }
    }
}
