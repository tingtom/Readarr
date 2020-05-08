using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;
using Ipfs.Http;
using NLog;
using NzbDrone.Common.Disk;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.Indexers;
using NzbDrone.Core.Parser.Model;
using NzbDrone.Core.RemotePathMappings;

namespace NzbDrone.Core.Download.Clients.IPFS
{
    public class IPFS : DownloadClientBase<IPFSSettings>
    {
        public IPFS(
            IConfigService configService,
            IDiskProvider diskProvider,
            IRemotePathMappingService remotePathMappingService,
            Logger logger)
            : base(configService, diskProvider, remotePathMappingService, logger)
        {
        }

        public override string Name => "IPFS Node";

        public override DownloadProtocol Protocol => DownloadProtocol.IPFS;

        public override string Download(RemoteAlbum remoteAlbum)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<DownloadClientItem> GetItems()
        {
            throw new NotImplementedException();
        }

        public override DownloadClientInfo GetStatus()
        {
            throw new NotImplementedException();
        }

        public override void RemoveItem(string downloadId, bool deleteData)
        {
            throw new NotImplementedException();
        }

        protected async override void Test(List<ValidationFailure> failures)
        {
            IpfsClient client = new IpfsClient(Settings.IPFSNodeUrl);
            var versionInformation = await client.Generic.VersionAsync();
        }
    }
}
