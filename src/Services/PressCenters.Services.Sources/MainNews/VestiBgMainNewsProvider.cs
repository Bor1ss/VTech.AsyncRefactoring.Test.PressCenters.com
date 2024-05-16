using System.Threading.Tasks;

namespace PressCenters.Services.Sources.MainNews
{
    public class VestiBgMainNewsProvider : BaseMainNewsProvider
    {
        public override string BaseUrl { get; } = "https://www.vesti.bg";

        public override async Task<RemoteMainNews> GetMainNewsAsync()
        {
            var document = await this.GetDocumentAsync(this.BaseUrl);

            var titleElement = document.QuerySelector(".leading h2");
            var title = titleElement.TextContent.Trim();

            var urlElement = document.QuerySelector(".leading a");
            var url = urlElement.Attributes["href"].Value.Trim();

            var imageElement = document.QuerySelector(".leading img");
            var imageUrl = imageElement?.Attributes["data-original"]?.Value?.Trim();

            return new RemoteMainNews(title, url, imageUrl);
        }
    }
}
