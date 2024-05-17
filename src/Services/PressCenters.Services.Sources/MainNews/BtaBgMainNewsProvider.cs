namespace PressCenters.Services.Sources.MainNews
{
    using System.Threading.Tasks;

    using System;

    public class BtaBgMainNewsProvider : BaseMainNewsProvider
    {
        public override string BaseUrl { get; } = "https://www.bta.bg";

        public override bool UseProxy => true;

        public override async Task<RemoteMainNews> GetMainNewsAsync()
        {
            var document = await this.GetDocumentAsync(this.BaseUrl);

            var titleElement = document.QuerySelector(".leading-section .news-card__title a");
            var title = titleElement.TextContent.Trim().Trim('.').Trim();

            var url = this.BaseUrl + titleElement.Attributes["href"].Value.Trim();

            var imageElement = document.QuerySelector(".leading-section .news-card__image img");
            var imageUrl = this.BaseUrl + imageElement?.Attributes["data-src"]?.Value?.Trim();
            if (this.UseProxy)
            {
                imageUrl = new Uri(imageUrl).GetLeftPart(UriPartial.Query); // Remove hash fragment
                imageUrl = imageUrl.Replace("https://", "https://proxy.presscenters.com/https/")
                        .Replace("http://", "https://proxy.presscenters.com/http/");
                imageUrl = imageUrl.Replace("+", "%20");
            }

            return new RemoteMainNews(title, url, imageUrl);
        }
    }
}
