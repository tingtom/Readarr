using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NzbDrone.Common.Http;
using NzbDrone.Core.IndexerSearch.Definitions;

namespace NzbDrone.Core.Indexers.LibGen
{
    public class LibGenRequestGenerator : IIndexerRequestGenerator
    {
        public IndexerPageableRequestChain GetRecentRequests()
        {
            var chain = new IndexerPageableRequestChain();

            chain.Add(GetRequest("test"));

            return chain;
        }

        public IndexerPageableRequestChain GetSearchRequests(AlbumSearchCriteria searchCriteria)
        {
            var chain = new IndexerPageableRequestChain();

            chain.Add(GetRequest(searchCriteria.AlbumQuery));

            return chain;
        }

        public IndexerPageableRequestChain GetSearchRequests(ArtistSearchCriteria searchCriteria)
        {
            var chain = new IndexerPageableRequestChain();

            chain.Add(GetRequest(searchCriteria.ArtistQuery, "authors"));

            return chain;
        }

        private IEnumerable<IndexerRequest> GetRequest(string search, string criteria = "", string language = "", string format = "", int page = 1)
        {
            var request = new IndexerRequest($"http://gen.lib.rus.ec/fiction/?q={search}&criteria={criteria}&language={language}&format={format}&page={page}", HttpAccept.Json);

            yield return request;
        }
    }
}
