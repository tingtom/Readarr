using System;
using System.Collections;
using System.Collections.Generic;
using NzbDrone.Common.EnvironmentInfo;
using NzbDrone.Common.Http;
using NzbDrone.Core.IndexerSearch.Definitions;

namespace NzbDrone.Core.Indexers.LibGen
{
    public class LibGenRequestGenerator : IIndexerRequestGenerator
    {
        private readonly LibGenSettings _settings;

        public LibGenRequestGenerator(LibGenSettings settings)
        {
            _settings = settings;
        }

        public IndexerPageableRequestChain GetRecentRequests()
        {
            var chain = new IndexerPageableRequestChain();

            chain.Add(GetRequest("test", "Author"));

            return chain;
        }

        public IndexerPageableRequestChain GetSearchRequests(AlbumSearchCriteria searchCriteria)
        {
            var chain = new IndexerPageableRequestChain();

            chain.Add(GetRequest(searchCriteria.AlbumQuery, "Title"));

            return chain;
        }

        public IndexerPageableRequestChain GetSearchRequests(ArtistSearchCriteria searchCriteria)
        {
            var chain = new IndexerPageableRequestChain();

            chain.Add(GetRequest(searchCriteria.ArtistQuery, "Author"));

            return chain;
        }

        private IEnumerable<IndexerRequest> GetRequest(string search, string field, int size = 100)
        {
            var request = new IndexerRequest($"{_settings.BaseUrl}?q={field}:{search}&size={size}", HttpAccept.Json);

            request.HttpRequest.AddBasicAuthentication(_settings.Username, _settings.Password);

            yield return request;
        }
    }
}
