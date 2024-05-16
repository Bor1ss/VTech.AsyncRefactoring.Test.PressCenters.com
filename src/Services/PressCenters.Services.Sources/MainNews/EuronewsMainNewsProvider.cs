using System.Threading.Tasks;

namespace PressCenters.Services.Sources.MainNews
{
    public class EuronewsMainNewsProvider : BaseMainNewsProvider
    {
        public override string BaseUrl { get; } = "https://www.euronews.com";

        public override async Task<RemoteMainNews> GetMainNewsAsync()
        {
            var document = await this.GetDocumentAsync(this.BaseUrl + "/?PageSpeed=noscript");

            var titleElement = document.QuerySelector(".c-first-topstory .media__main h1.media__body__title a")
                               ?? document.QuerySelector(".m-object__title__link--big-title");

            var title = titleElement.TextContent.Trim();

            var url = this.BaseUrl + titleElement.Attributes["href"].Value.Trim();

            var imageElement = document.QuerySelector(".media__img__link img");
            var imageUrl = imageElement?.Attributes["data-src"]?.Value ?? imageElement?.Attributes["src"]?.Value;
            imageUrl = imageUrl?.Trim();

            return new RemoteMainNews(title, url, imageUrl);
        }
    }
}
