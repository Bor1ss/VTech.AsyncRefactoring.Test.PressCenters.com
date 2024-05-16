﻿namespace PressCenters.Services.Sources.MainNews
{
    using System.Text;
    using System.Threading.Tasks;

    public class DnesBgMainNewsProvider : BaseMainNewsProvider
    {
        public override string BaseUrl { get; } = "https://www.dnes.bg";

        public override async Task<RemoteMainNews> GetMainNews()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var document = await this.GetDocument(this.BaseUrl);

            var titleElement = document.QuerySelector(".top-news-wrapper .left .top-news .image-title > a");
            var title = titleElement.TextContent.Trim();
            var url = this.BaseUrl + titleElement.Attributes["href"].Value.Trim();

            var imageElement = document.QuerySelector(".top-news-wrapper .left .top-news .first a img");
            var imageUrl = imageElement?.Attributes["src"]?.Value?.Trim();

            return new RemoteMainNews(title, url, imageUrl);
        }
    }
}
