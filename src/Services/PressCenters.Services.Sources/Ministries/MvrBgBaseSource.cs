namespace PressCenters.Services.Sources.Ministries
{
    using System.Threading.Tasks;

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;

    using AngleSharp.Dom;
    using AngleSharp.Html.Parser;

    public abstract class MvrBgBaseSource : BaseSource
    {
        public override bool UseProxy => true;

        public override string BaseUrl { get; } = "https://www.mvr.bg/";

        public abstract string NewsListUrl { get; }

        public abstract string NewsLinkSelector { get; }

        public abstract int NewsListPagesCount { get; }

        public override IEnumerable<RemoteNews> GetLatestPublications() =>
            this.GetPublications(this.NewsListUrl, this.NewsLinkSelector, count: 5);

        public override async Task<IEnumerable<RemoteNews>> GetAllPublicationsAsync()
        {
            List<RemoteNews>GetAllPublicationsResult = new List<RemoteNews>();
            var address = $"{this.BaseUrl}{this.NewsListUrl}";
            var parser = new HtmlParser();
            var httpClient = new HttpClient();
            for (var i = 1; i <= this.NewsListPagesCount; i++)
            {
                var response = await httpClient.PostAsync(
                    address,
                    new FormUrlEncodedContent(
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("page_no", i.ToString()),
                            new KeyValuePair<string, string>("page_size", "24"),
                        }));
                var content = await response.Content.ReadAsStringAsync();
                var document = parser.ParseDocument(content);
                var links = document.QuerySelectorAll(this.NewsLinkSelector)
                    .Select(x => this.NormalizeUrl(x.Attributes["href"].Value)).Where(x => !new Uri(x).PathAndQuery.Contains(":")).Distinct()
                    .ToList();
                var news = links.Select(this.GetPublication).Where(x => x != null).ToList();
                Console.WriteLine($"№{i} => {news.Count} news ({news.DefaultIfEmpty().Min(x => x?.PostDate)} - {news.DefaultIfEmpty().Max(x => x?.PostDate)})");
                foreach (var remoteNews in news)
                {
                    GetAllPublicationsResult.Add(remoteNews);
                }
            }
            return GetAllPublicationsResult;
        }

        internal override string ExtractIdFromUrl(string url)
        {
            // Get last 2 url segments
            var uri = new Uri(url.Trim().Trim('/'));
            return WebUtility.UrlDecode(uri.Segments[^2] + uri.Segments[^1]);
        }

        protected override RemoteNews ParseDocument(IDocument document, string url)
        {
            var titleElement = document.QuerySelector(".article__description h1");
            if (titleElement == null)
            {
                return null;
            }

            var title = titleElement.TextContent.Trim();

            var timeElement = document.QuerySelector(".article__description h5");
            var timeAsString = timeElement?.TextContent?.Trim();
            if (!DateTime.TryParseExact(timeAsString, "dd MMM yyyy", CultureInfo.GetCultureInfo("bg-BG"), DateTimeStyles.None, out var time))
            {
                timeElement = document.QuerySelector(".article__description .timestamp");
                timeAsString = timeElement?.TextContent?.Trim();

                if (!DateTime.TryParseExact(timeAsString, "dd MMMM yyyy | HH:mm", CultureInfo.GetCultureInfo("bg-BG"), DateTimeStyles.None, out time))
                {
                    time = DateTime.ParseExact(timeAsString, "dd MMMM yyyy", CultureInfo.GetCultureInfo("bg-BG"));
                }
            }

            var imageElement = document.QuerySelector("#image_source");
            var imageUrl = imageElement?.GetAttribute("src");

            // Try to get exact time
            var modifiedTimeElement = document.QuerySelector(".article__container .timestamp");
            var modifiedTimeText = modifiedTimeElement?.TextContent?.Trim();
            if (!string.IsNullOrWhiteSpace(modifiedTimeText))
            {
                if (DateTime.TryParseExact(
                    modifiedTimeText,
                    "dd MMMM yyyy | HH:mm",
                    CultureInfo.GetCultureInfo("bg-BG"),
                    DateTimeStyles.AllowWhiteSpaces,
                    out var modifiedTime))
                {
                    if (time.Date == modifiedTime.Date)
                    {
                        time = modifiedTime;
                    }
                }
            }

            var contentElement = document.QuerySelector(".article__container");
            contentElement.RemoveRecursively(timeElement);
            contentElement.RemoveRecursively(document.QuerySelector(".article__container div.row"));
            contentElement.RemoveRecursively(document.QuerySelector(".article__container script"));
            contentElement.RemoveRecursively(document.QuerySelector(".article__container h1")); // title
            contentElement.RemoveRecursively(document.QuerySelector(".article__container .pull-right"));
            this.NormalizeUrlsRecursively(contentElement);
            var content = contentElement.InnerHtml.Trim();

            return new RemoteNews(title, content, time, imageUrl);
        }
    }
}
