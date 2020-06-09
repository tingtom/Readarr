using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.Parser.Model;

namespace NzbDrone.Core.Indexers.LibGen
{
    public class LibGenParser : IParseIndexerResponse
    {
        public IList<ReleaseInfo> ParseResponse(IndexerResponse indexerResponse)
        {
            var response = JsonConvert.DeserializeObject<LibGenSearchResponse>(indexerResponse.Content);

            return response.Hits.Hits.Select(hit => new ReleaseInfo()
            {
                Guid = Guid.NewGuid().ToString(),
                Title = $"{hit.Source.Author} - {hit.Source.Title}.{hit.Source.Extension}",
                DownloadUrl = hit.Source.IPFSBlake2B,
                DownloadProtocol = DownloadProtocol.IPFS,
                PublishDate = hit.Source.TimeAdded
            }).ToList();
        }
    }
}
