namespace PressCenters.Services.Sources
{
    using System.Threading.Tasks;

    using System.Collections.Generic;

    public interface ISource
    {
        string BaseUrl { get; }

        IEnumerable<RemoteNews> GetLatestPublications();

        Task<IEnumerable<RemoteNews>> GetAllPublicationsAsync();

        RemoteNews GetPublication(string url);
    }
}
