using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.ForeignKey;
using FluentValidation.Results;
using NLog;
using NzbDrone.Common.Disk;
using NzbDrone.Common.Http;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.Indexers;
using NzbDrone.Core.Organizer;
using NzbDrone.Core.Parser.Model;
using NzbDrone.Core.RemotePathMappings;

namespace NzbDrone.Core.Download.Clients.IPFS
{
    public class IPFS : DownloadClientBase<IPFSSettings>
    {
        private readonly IHttpClient _httpClient;

        public IPFS(IConfigService configService,
                    IDiskProvider diskProvider,
                    IRemotePathMappingService remotePathMappingService,
                    IHttpClient httpClient,
                    Logger logger)
            : base(configService, diskProvider, remotePathMappingService, logger)
        {
            _httpClient = httpClient;
        }

        public override string Name => "IPFS Node";

        public override DownloadProtocol Protocol => DownloadProtocol.IPFS;

        public string GetDownloadClientId(string filename)
        {
            return Definition.Name + "_" + Path.GetFileName(filename);
        }

        public override string Download(RemoteAlbum remoteAlbum)
        {
            // Split the download url into hash and extension
            var splitHash = remoteAlbum.Release.DownloadUrl.Split('.');

            // Generate path for this file
            var path = Path.Combine(Settings.IPFSDownloadPath, FileNameBuilder.CleanFileName(remoteAlbum.Release.Title) + "." + splitHash[1]);

            var task = Task.Run(() =>
            {
                // Pin the hash if set in the settings
                if (Settings.PinHash)
                {
                    var pinRequest = new HttpRequest($"{Settings.IPFSNodeUrl}/api/v0/pin/add?arg={splitHash[0]}");
                    var pinResponse = _httpClient.Post(pinRequest);

                    if (pinResponse.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        _logger.Error("Unable to pin hash on IPFS node");
                    }
                }

                // Download the file and store locally
                using (var file = File.Create(path))
                {
                    var request = new HttpRequest($"{Settings.IPFSNodeUrl}/api/v0/cat?arg={splitHash[0]}");
                    var response = _httpClient.Post(request);

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        _logger.Error("Unable to download file from IPFS node");
                    }

                    file.Write(response.ResponseData, 0, response.ResponseData.Length);
                }
            });

            return GetDownloadClientId(path);
        }

        public override IEnumerable<DownloadClientItem> GetItems()
        {
            foreach (var file in _diskProvider.GetFiles(Settings.IPFSDownloadPath, SearchOption.TopDirectoryOnly))
            {
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
            var request = new HttpRequest($"{Settings.IPFSNodeUrl}/api/v0/version");
            var response = _httpClient.Post(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                failures.Add(new ValidationFailure(string.Empty, "Unable to connect to the IPFS node"));
            }
        }
    }
}
