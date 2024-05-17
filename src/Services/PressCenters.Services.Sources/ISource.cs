namespace PressCenters.Services.Sources
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISource
    {
        string BaseUrl { get; }

        IEnumerable<RemoteNews> GetLatestPublications();

        IAsyncEnumerable<RemoteNews> GetAllPublicationsAsync();

        RemoteNews GetPublication(string url);

    }
}
