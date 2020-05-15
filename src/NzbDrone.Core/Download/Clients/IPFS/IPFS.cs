using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.ForeignKey;
using FluentValidation.Results;
using Ipfs.Http;
using NLog;
using NzbDrone.Common.Disk;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.Indexers;
using NzbDrone.Core.Organizer;
using NzbDrone.Core.Parser.Model;
using NzbDrone.Core.RemotePathMappings;

namespace NzbDrone.Core.Download.Clients.IPFS
{
    public class IPFS : DownloadClientBase<IPFSSettings>
    {
        public IPFS(IConfigService configService,
                    IDiskProvider diskProvider,
                    IRemotePathMappingService remotePathMappingService,
                    Logger logger)
            : base(configService, diskProvider, remotePathMappingService, logger)
        {
        }

        public override string Name => "IPFS Node";

        public override DownloadProtocol Protocol => DownloadProtocol.IPFS;

        public string GetDownloadClientId(string filename)
        {
            return Definition.Name + "_" + Path.GetFileName(filename) + "_" + _diskProvider.FileGetLastWrite(filename).Ticks;
        }

        public override string Download(RemoteAlbum remoteAlbum)
        {
            IpfsClient client = new IpfsClient(Settings.IPFSNodeUrl);

            var path = Path.Combine(Settings.IPFSDownloadPath, FileNameBuilder.CleanFileName(remoteAlbum.Release.Title) + ".ipfs");

            var task = Task.Run(() =>
            {
                // Pin the hash if set in the settings
                if (Settings.PinHash)
                {
                    client.Pin.AddAsync(remoteAlbum.Release.DownloadUrl, false).Wait();
                }

                // Download the file and store locally
                using (var file = File.Create(path))
                {
                    var stream = client.FileSystem.ReadFileAsync(remoteAlbum.Release.DownloadUrl).Result;
                    stream.CopyTo(file);
                }
            });

            return GetDownloadClientId(path);
        }

        public override IEnumerable<DownloadClientItem> GetItems()
        {
            foreach (var file in _diskProvider.GetFiles(Settings.IPFSDownloadPath, SearchOption.TopDirectoryOnly))
            {
                if (Path.GetExtension(file) != ".ipfs")
                {
                    continue;
                }

                var title = FileNameBuilder.CleanFileName(Path.GetFileName(file));

                var historyItem = new DownloadClientItem
                {
                    DownloadClient = Definition.Name,
                    DownloadId = GetDownloadClientId(file),
                    Title = title,

                    CanBeRemoved = true,
                    CanMoveFiles = true,

                    TotalSize = _diskProvider.GetFileSize(file),

                    OutputPath = new OsPath(file)
                };

                if (_diskProvider.IsFileLocked(file))
                {
                    historyItem.Status = DownloadItemStatus.Downloading;
                }
                else
                {
                    historyItem.Status = DownloadItemStatus.Completed;
                }

                yield return historyItem;
            }
        }

        public override DownloadClientInfo GetStatus()
        {
            return new DownloadClientInfo()
            {
                IsLocalhost = true
            };
        }

        public override void RemoveItem(string downloadId, bool deleteData)
        {
            throw new NotImplementedException();
        }

        protected override void Test(List<ValidationFailure> failures)
        {
            IpfsClient client = new IpfsClient(Settings.IPFSNodeUrl);

            try
            {
                var version = client.Generic.VersionAsync().Result;

                if (version.Count <= 0)
                {
                    failures.Add(new ValidationFailure(string.Empty, "Unable to retrieve version information for IPFS node"));
                }
            }
            catch
            {
                failures.Add(new ValidationFailure(string.Empty, "Unable to connect to the IPFS node"));
            }
        }
    }
}
