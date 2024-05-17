using System.Threading.Tasks;

namespace PressCenters.Services.Sources.MainNews
{
    public class NewsBntBgMainNewsProvider : BaseMainNewsProvider
    {
        public override string BaseUrl { get; } = "https://bntnews.bg";

        public override async Task<RemoteMainNews> GetMainNewsAsync()
        {
            var document = await this.GetDocumentAsync(this.BaseUrl);

            var titleElement = document.QuerySelector(".main-news-title");
            var title = titleElement.TextContent.Trim();

            var urlElement = document.QuerySelector(".main-news-title");
            var url = urlElement.Attributes["href"].Value.Trim();

            var imageElement = document.QuerySelector(".top-img img");
            var imageUrl = imageElement?.Attributes["src"]?.Value?.Trim();

            return new RemoteMainNews(title, url, imageUrl);
        }
    }
}
