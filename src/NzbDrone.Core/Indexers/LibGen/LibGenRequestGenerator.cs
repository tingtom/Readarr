using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NzbDrone.Common.EnvironmentInfo;
using NzbDrone.Common.Extensions;
using NzbDrone.Common.Http;
using NzbDrone.Core.IndexerSearch.Definitions;

namespace NzbDrone.Core.Indexers.LibGen
{
    public class LibGenRequestGenerator : IIndexerRequestGenerator
    {
        private readonly IAppFolderInfo _appFolderInfo;
        private readonly LibGenSettings _settings;

        public LibGenRequestGenerator(IAppFolderInfo appFolderInfo, LibGenSettings settings)
        {
            _appFolderInfo = appFolderInfo;
            _settings = settings;
        }

        public IndexerPageableRequestChain GetRecentRequests()
        {
            var chain = new IndexerPageableRequestChain();

            string path = Path.Combine(_appFolderInfo.GetAppDataPath(), "hashes.db");

            // Make sure the hashes exist for this indexer
            if (File.Exists(path))
            {
                chain.Add(GetRequest("test"));
            }

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
            var request = new IndexerRequest($"{_settings.BaseUrl}?q={search}&criteria={criteria}&language={language}&format={format}&page={page}", HttpAccept.Json);

            yield return request;
        }
    }
}
