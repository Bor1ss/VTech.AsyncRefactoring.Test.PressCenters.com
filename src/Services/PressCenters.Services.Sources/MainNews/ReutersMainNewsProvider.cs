namespace PressCenters.Services.Sources.MainNews
{
    using System.Linq;
    using System.Threading.Tasks;
    using AngleSharp;

    using PressCenters.Common;

    public class ReutersMainNewsProvider : BaseMainNewsProvider
    {
        public override string BaseUrl { get; } = "https://www.reuters.com";

        public override async Task<RemoteMainNews> GetMainNewsAsync()
        {
            var document = await this.GetDocumentAsync(this.BaseUrl);

            var titleElement = document.GetElementsByTagName("h3").FirstOrDefault(x => x.ClassName.Contains("MediaStoryCard__heading__"));
            var title = titleElement?.TextContent.Trim();

            var url = this.BaseUrl + document.GetElementsByTagName("a").FirstOrDefault(x => x.ClassName.Contains("MediaStoryCard__basic_hero___"))?.Attributes["href"].Value.Trim();

            var imageUrl = document.ToHtml().GetStringBetween("\",\"url\":\"", "\"");
            if (!imageUrl.StartsWith("http"))
            {
                imageUrl = null;
            }

            return new RemoteMainNews(title, url, imageUrl);
        }
    }
}
