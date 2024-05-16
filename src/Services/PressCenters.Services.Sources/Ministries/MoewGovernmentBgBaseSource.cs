namespace PressCenters.Services.Sources.Ministries
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using AngleSharp.Dom;

    public abstract class MoewGovernmentBgBaseSource : BaseSource
    {
        public override string BaseUrl { get; } = "https://www.moew.government.bg/";

        public override bool UseProxy => true;

        protected abstract string NewsListUrl { get; }

        protected abstract int NewsListPagesCount { get; }

        public override IEnumerable<RemoteNews> GetLatestPublications() =>
            this.GetPublications(this.NewsListUrl, "ul.news-list-internal li a.green-btn", count: 5);

        public override async Task<List<RemoteNews>> GetAllPublications()
        {
            var allNews = new List<RemoteNews>();

            for (var i = 1; i <= this.NewsListPagesCount; i++)
            {
                var news = this.GetPublications($"{this.NewsListUrl}page/{i}/", "ul.news-list-internal li a.green-btn");
                Console.WriteLine($"Page {i} => {news.Count} news");
                allNews.AddRange(news);
            }
            return allNews;
        }

        protected override RemoteNews ParseDocument(IDocument document, string url)
        {
            var titleElement = document.QuerySelector(".content-box h3.green");
            var title = titleElement?.TextContent;

            var timeElement = document.QuerySelector(".content-box .date");
            var timeAsString = timeElement?.TextContent?.Trim();
            var time = DateTime.ParseExact(timeAsString, "dd MMMM yyyy | HH:mm", CultureInfo.GetCultureInfo("bg-BG"));

            var imageElement = document.QuerySelector(".content-box .image-container img");
            var imageUrl = imageElement?.GetAttribute("src");

            var contentElement = document.QuerySelector(".description_holder_div");
            this.NormalizeUrlsRecursively(contentElement);
            var content = contentElement?.InnerHtml;

            return new RemoteNews(title, content, time, imageUrl);
        }
    }
}
