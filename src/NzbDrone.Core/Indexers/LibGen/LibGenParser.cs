using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using HtmlAgilityPack;
using NzbDrone.Common.EnvironmentInfo;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.Parser.Model;

namespace NzbDrone.Core.Indexers.LibGen
{
    public class LibGenParser : IParseIndexerResponse
    {
        private readonly DbConnection _connection;

        public LibGenParser(IAppFolderInfo appFolderInfo)
        {
            string path = Path.Combine(appFolderInfo.GetAppDataPath(), "hashes.db");

            //Open hash database
            if (File.Exists(path))
            {
                _connection = SQLiteFactory.Instance.CreateConnection();
                _connection.ConnectionString = GetConnectionString(path);
                _connection.Open();
            }
        }

        private string GetConnectionString(string path)
        {
            var connectionBuilder = new SQLiteConnectionStringBuilder();
            connectionBuilder.DataSource = path;
            connectionBuilder.CacheSize = -10000;
            connectionBuilder.DateTimeKind = DateTimeKind.Utc;
            connectionBuilder.JournalMode = OsInfo.IsOsx ? SQLiteJournalModeEnum.Truncate : SQLiteJournalModeEnum.Wal;
            connectionBuilder.Pooling = true;
            connectionBuilder.Version = 3;
            return connectionBuilder.ConnectionString;
        }

        private string MapMD5(string md5)
        {
            return _connection.QueryFirstOrDefault<string>($"SELECT ipfs_blake2b FROM ipfs_fiction_hashes WHERE md5_file_name = '{md5}'");
        }

        public IList<ReleaseInfo> ParseResponse(IndexerResponse indexerResponse)
        {
            //Parse HTML
            var html = new HtmlDocument();
            html.LoadHtml(indexerResponse.Content);

            //Get all books in the catalog table
            var rows = html.DocumentNode.SelectNodes("//table[@class='catalog']/tbody/tr");

            return rows.Select(row =>
            {
                var cols = row.ChildNodes.Where(n => n.NodeType == HtmlNodeType.Element).ToArray();
                var editLink = row.SelectSingleNode("//td[last()]/a");

                //Must be able to get the MD5 for the book
                if (editLink != null)
                {
                    //Get the MD5 from the edit link
                    var md5 = editLink.Attributes["href"].Value.Split('/').Last();

                    //Get the extension from the file column
                    var ext = cols[4].InnerText.Split('/').First().Trim().ToLower();

                    //Try to map the MD5 to IPFS hash
                    var hash = MapMD5($"{md5.ToLower()}.{ext}");

                    if (hash == null)
                    {
                        return null;
                    }

                    var publishDate = cols[4].Attributes["title"].Value.Replace("Uploaded at", "");

                    var author = cols[0].InnerText.Trim().Split(',').Reverse().Join(" ");

                    return new ReleaseInfo()
                    {
                        Guid = Guid.NewGuid().ToString(),
                        Title = new[] { author, cols[1].InnerText, cols[2].InnerText }.Where(part => !string.IsNullOrWhiteSpace(part)).Join(" - ") + "." + ext,
                        DownloadUrl = $"{hash}.{ext}",
                        DownloadProtocol = DownloadProtocol.IPFS,
                        PublishDate = DateTime.Parse(publishDate)
                    };
                }

                return null;
            }).Where(a => a != null).ToList();
        }
    }
}
