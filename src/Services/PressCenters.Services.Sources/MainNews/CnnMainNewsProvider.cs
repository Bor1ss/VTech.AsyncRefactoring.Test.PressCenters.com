namespace PressCenters.Services.Sources.MainNews
{ 
    using System.Threading.Tasks;


    public class CnnMainNewsProvider : BaseMainNewsProvider
    {
        public override string BaseUrl => "https://edition.cnn.com";

        public override async Task<RemoteMainNews> GetMainNewsAsync()
        {
            var document = await this.GetDocumentAsync(this.BaseUrl);

            var titleElement = document.QuerySelector(".container_lead-package__title_url-text");
            var title = titleElement.TextContent.Trim().Trim('.').Trim();

            var url =titleElement.ParentElement.Attributes["href"].Value.Trim();

            var imageElement = document.QuerySelector(".container_lead-package__cards-wrapper img");
            var imageUrl = imageElement?.Attributes["src"]?.Value?.Trim();

            return new RemoteMainNews(title, url, imageUrl);
        }

        public class JsonDataObject
        {
            public string Html { get; set; }
        }
    }
}
