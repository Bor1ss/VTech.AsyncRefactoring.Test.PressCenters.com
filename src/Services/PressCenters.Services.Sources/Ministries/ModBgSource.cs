﻿namespace PressCenters.Services.Sources.Ministries
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using AngleSharp;
    using AngleSharp.Dom;

    using PressCenters.Common;

    /// <summary>
    /// Министерство на отбраната.
    /// </summary>
    public class ModBgSource : BaseSource
    {
        public override string BaseUrl => "https://mod.bg/";

        public override IEnumerable<RemoteNews> GetLatestPublications() =>
            this.GetNews(
                $"{this.BaseUrl}bg/news_archive.php?fn_month={DateTime.UtcNow.Month}&fn_year={DateTime.UtcNow.Year}");

        public override IEnumerable<RemoteNews> GetAllPublications()
        {
            for (var date = DateTime.UtcNow; date >= new DateTime(2010, 1, 1); date = date.AddMonths(-1))
            {
                var news = this.GetNews($"{this.BaseUrl}bg/news_archive.php?fn_month={date.Month}&fn_year={date.Year}");
                Console.WriteLine($"{date:yyyy, MMM} => {news.Count} news");
                foreach (var remoteNews in news)
                {
                    yield return remoteNews;
                }
            }
        }

        internal override string ExtractIdFromUrl(string url) => new Uri(url).Fragment.Trim('#');

        protected override RemoteNews ParseDocument(IDocument document, string url)
        {
            var id = this.ExtractIdFromUrl(url);
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            var newsElement = document.QuerySelector($".tablelist2:has(div[id={id}])");

            // Title
            var title = newsElement?.QuerySelector("h2").TextContent;
            if (title == null)
            {
                return null;
            }

            // Time
            var timeAsString = newsElement.PreviousElementSibling?.TextContent?.Trim();
            var time = DateTime.ParseExact(timeAsString, "dd.MM.yyyy", CultureInfo.InvariantCulture);

            // Image
            var imageElement = newsElement.QuerySelector("a[rel^='lightbox'] img");
            var imageUrl = imageElement?.GetAttribute("src") ?? "/images/sources/mod.bg.jpg";

            // Content
            var contentElement = newsElement.QuerySelector($"div[id={id}]");
            var images = newsElement.QuerySelectorAll("a[rel^='lightbox']");
            foreach (var image in images)
            {
                contentElement.RemoveRecursively(image);
            }

            this.NormalizeUrlsRecursively(contentElement);
            var content = contentElement?.InnerHtml;

            return new RemoteNews(title, content, time, imageUrl);
        }

        private IList<RemoteNews> GetNews(string address)
        {
            var document = this.BrowsingContext.OpenAsync(address).GetAwaiter().GetResult();
            var links = document.QuerySelectorAll(".tablelist2 a").Select(x => x?.Attributes["href"]?.Value)
                .Where(x => x?.Contains("show(") == true).Select(x => $"{address}#{x.GetStringBetween("show(", ");")}")
                .ToList();
            var news = links.Select(this.GetPublication).Where(x => x != null).ToList();
            return news;
        }
    }
}
