namespace PressCenters.Services.Sources.Ministries
{
    using System.Threading.Tasks;

    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using AngleSharp.Dom;

    /// <summary>
    /// Министерство на транспорта, информационните технологии и съобщенията.
    /// </summary>
    public class MtitcGovernmentBgSource : BaseSource
    {
        public override string BaseUrl { get; } = "https://www.mtc.government.bg/";

        public override bool UseProxy => true;

        public override IEnumerable<RemoteNews> GetLatestPublications() =>
            this.GetPublications("bg/category/1", "#main .views-field-title a", "bg/category/1", 5);

        public override async Task<IEnumerable<RemoteNews>> GetAllPublicationsAsync()
        {
            List<RemoteNews>GetAllPublicationsResult = new List<RemoteNews>();
            for (var i = 0; i <= 25; i++)
            {
                var news = this.GetPublications(
                    $"bg/category/1?page={i}",
                    "#main .views-field-title a",
                    "bg/category/1");
                Console.WriteLine($"Page {i} => {news.Count} news");
                foreach (var remoteNews in news)
                {
                    GetAllPublicationsResult.Add(remoteNews);
                }
            }
            return GetAllPublicationsResult;
        }

        internal override string ExtractIdFromUrl(string url)
        {
            var uri = new Uri(url.Trim().Trim('/'));
            return uri.Segments[^2] + uri.Segments[^1];
        }

        protected override RemoteNews ParseDocument(IDocument document, string url)
        {
            var titleElement = document.QuerySelector("#main h1");
            if (titleElement == null)
            {
                return null;
            }

            var title = titleElement.TextContent;

            var timeElement = document.QuerySelector("#main .content time.datetime");
            var time = DateTime.Parse(timeElement?.Attributes["datetime"]?.Value, CultureInfo.InvariantCulture);

            var imageElement = document.QuerySelector("#main .content .field--name-field-image a");
            var imageUrl = imageElement?.GetAttribute("href");

            var contentElement = document.QuerySelector("#main .content .field--name-body");
            this.NormalizeUrlsRecursively(contentElement);
            var content = contentElement?.InnerHtml;

            return new RemoteNews(title, content, time, imageUrl);
        }
    }
}
