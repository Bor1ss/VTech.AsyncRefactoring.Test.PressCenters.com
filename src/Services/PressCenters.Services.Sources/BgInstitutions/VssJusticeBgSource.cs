namespace PressCenters.Services.Sources.BgInstitutions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using AngleSharp.Dom;

    /// <summary>
    /// Висш съдебен съвет.
    /// </summary>
    public class VssJusticeBgSource : BaseSource
    {
        public override string BaseUrl => "https://vss.justice.bg/";

        public override IEnumerable<RemoteNews> GetLatestPublications()
            => this.GetPublications("page/view/2574", ".right_info .row a", count: 5);

        public override async Task<List<RemoteNews>> GetAllPublications()
        {
            var allNews = new List<RemoteNews>();

            for (var i = 1; i <= 365; i++)
            {
                var document = this.Parser.ParseDocument(this.ReadStringFromUrl($"{this.BaseUrl}page/view/2574?p={i}"));
                var newsElements = document.QuerySelectorAll(".right_info .row");
                var newsCount = 0;
                foreach (var newsElement in newsElements)
                {
                    var url = this.NormalizeUrl(newsElement.QuerySelector("a").Attributes["href"].Value);
                    var dateAsString = newsElement.QuerySelector(".new_date").TextContent.Trim();
                    var date = DateTime.ParseExact(dateAsString, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                    var remoteNews = this.GetPublication(url);
                    if (remoteNews == null)
                    {
                        continue;
                    }

                    newsCount++;
                    remoteNews.PostDate = date;
                    allNews.Add(remoteNews);
                }

                Console.WriteLine($"Page {i} => {newsCount} news");
                await Task.Delay(1500);
            }
            return allNews;
        }

        protected override RemoteNews ParseDocument(IDocument document, string url)
        {
            var titleElement = document.QuerySelector(".right_info .title");
            var title = titleElement?.TextContent?.Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                title = "Прессъобщение";
            }

            var contentElement = document.QuerySelector(".right_info .with_left_menu_content");

            var imageElement = contentElement.QuerySelector("img");
            var imageUrl = imageElement?.GetAttribute("src");
            contentElement.RemoveRecursively(imageElement);

            this.NormalizeUrlsRecursively(contentElement);
            var content = contentElement?.InnerHtml;

            return new RemoteNews(title, content, DateTime.Now, imageUrl);
        }
    }
}
