using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NzbDrone.Core.Parser.Model;

namespace NzbDrone.Core.Indexers.LibGen
{
    public class LibGenParser : IParseIndexerResponse
    {
        public IList<ReleaseInfo> ParseResponse(IndexerResponse indexerResponse)
        {
            //Parse HTML
            var html = new HtmlDocument();
            html.LoadHtml(indexerResponse.Content);

            //Get all books in the catalog table
            var rows = html.DocumentNode.SelectNodes("//table[@class='catalog']/tbody/tr");

            return rows.Select(row =>
            {
                var editLink = row.SelectSingleNode("//td[last()]/a");

                //Must be able to get the MD5 for the book
                if (editLink != null)
                {
                    var md5 = editLink.Attributes["href"].Value.Split('/').Last();

                    return new ReleaseInfo()
                    {
                        Artist = row.ChildNodes[0].InnerText,
                        Album = row.ChildNodes[1].InnerText,
                        Title = row.ChildNodes[2].InnerText,
                        DownloadUrl = md5,
                        DownloadProtocol = DownloadProtocol.IPFS
                    };
                }

                return null;
            }).Where(a => a != null).ToList();
        }
    }
}
