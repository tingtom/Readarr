using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NzbDrone.Common.EnvironmentInfo;
using NzbDrone.Common.Http;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.Parser;

namespace NzbDrone.Core.Indexers.LibGen
{
    public class LibGen : HttpIndexerBase<LibGenSettings>
    {
        public LibGen(IHttpClient httpClient,
                      IIndexerStatusService indexerStatusService,
                      IConfigService configService,
                      IParsingService parsingService,
                      IAppFolderInfo appFolderInfo,
                      Logger logger)
            : base(httpClient, indexerStatusService, configService, parsingService, logger)
        {
        }

        public override string Name => "Library Genesis";
        public override bool SupportsRss => false;

        public override DownloadProtocol Protocol => DownloadProtocol.IPFS;

        public override IParseIndexerResponse GetParser()
        {
            return new LibGenParser();
        }

        public override IIndexerRequestGenerator GetRequestGenerator()
        {
            return new LibGenRequestGenerator(Settings);
        }
    }
}
