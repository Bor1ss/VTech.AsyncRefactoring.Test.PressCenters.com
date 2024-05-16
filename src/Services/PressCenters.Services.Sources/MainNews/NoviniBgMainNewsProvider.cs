using System.Threading.Tasks;

namespace PressCenters.Services.Sources.MainNews
{
    public class NoviniBgMainNewsProvider : BaseMainNewsProvider
    {
        public override string BaseUrl { get; } = "https://novini.bg";

        public override async Task<RemoteMainNews> GetMainNewsAsync()
        {
            var document = await this.GetDocumentAsync(this.BaseUrl);

            var titleElement = document.QuerySelector(".g-grid__item h2");
            var title = titleElement.TextContent.Trim();

            var urlElement = document.QuerySelector(".g-grid__item a");
            var url = urlElement.Attributes["href"].Value.Trim();

            var imageElement = document.QuerySelector(".g-grid__item img");
            var imageUrl = imageElement?.Attributes["src"]?.Value?.Trim();

            return new RemoteMainNews(title, url, imageUrl);
        }
    }
}
