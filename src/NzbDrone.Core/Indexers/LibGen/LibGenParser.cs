using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NzbDrone.Core.Parser.Model;

namespace NzbDrone.Core.Indexers.LibGen
{
    public class LibGenParser : IParseIndexerResponse
    {
        public IList<ReleaseInfo> ParseResponse(IndexerResponse indexerResponse)
        {
            return new List<ReleaseInfo>()
            {
                new ReleaseInfo()
                {
                    Title = "Test book",
                    DownloadProtocol = DownloadProtocol.IPFS,
                    DownloadUrl = "bafykbzacedpdzcfe5e6xfwp2bjks63kvkcky7lmuum7f3ldtekmdwla67bz2s"
                }
            };
        }
    }
}
