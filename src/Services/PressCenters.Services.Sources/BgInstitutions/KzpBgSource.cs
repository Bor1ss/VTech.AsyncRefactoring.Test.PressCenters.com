﻿namespace PressCenters.Services.Sources.BgInstitutions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using AngleSharp.Dom;

    /// <summary>
    /// Комисия за защита на потребителите.
    /// </summary>
    public class KzpBgSource : BaseSource
    {
        public override string BaseUrl { get; } = "https://kzp.bg/";

        public override bool UseProxy => true;

        public override IEnumerable<RemoteNews> GetLatestPublications() =>
            this.GetPublications("novini/1", ".blog-list li h3 a", count: 5);

        public override async System.Threading.Tasks.Task<IEnumerable<RemoteNews>> GetAllPublications()
        {
            var result = new List<RemoteNews>();
            for (var page = 1; page <= 145; page++)
            {
                var news = this.GetPublications($"novini/{page}", ".blog-list li h3 a");
                Console.WriteLine($"Page {page} => {news.Count} news");
                foreach (var remoteNews in news)
                {
                    result.Add(remoteNews);
                }
            }

            return result;
        }

        protected override RemoteNews ParseDocument(IDocument document, string url)
        {
            var titleElement = document.QuerySelector("h1.page-inner-title");
            if (titleElement == null)
            {
                return null;
            }

            var title = titleElement.TextContent.Trim();

            var timeElement = document.QuerySelector(".blog-post .date");
            var timeAsString = timeElement?.TextContent?.Trim();
            if (string.IsNullOrWhiteSpace(timeAsString))
            {
                return null;
            }

            var time = DateTime.ParseExact(timeAsString, "dd.MM.yyyy", CultureInfo.InvariantCulture);

            var imageElement = document.QuerySelector(".blog-post img");
            var imageUrl = imageElement?.GetAttribute("src");

            var contentElement = document.QuerySelector(".blog-post article");
            this.NormalizeUrlsRecursively(contentElement);
            var content = contentElement?.InnerHtml;

            return new RemoteNews(title, content, time, imageUrl);
        }
    }
}
