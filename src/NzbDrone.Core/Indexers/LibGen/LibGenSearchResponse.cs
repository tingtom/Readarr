using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NzbDrone.Core.Indexers.LibGen
{
    public partial class LibGenSearchResponse
    {
        public int Took { get; set; }
        public bool TimedOut { get; set; }
        [JsonProperty("_shards")]
        public Shards Shards { get; set; }
        public Results Hits { get; set; }
    }

    public partial class Results
    {
        public Total Total { get; set; }
        public double MaxScore { get; set; }
        public List<Hit> Hits { get; set; }
    }

    public partial class Hit
    {
        [JsonProperty("_index")]
        public string Index { get; set; }
        [JsonProperty("_type")]
        public string Type { get; set; }
        [JsonProperty("_id")]
        public string Id { get; set; }
        [JsonProperty("_score")]
        public double Score { get; set; }
        [JsonProperty("_source")]
        public Source Source { get; set; }
    }

    public partial class Source
    {
        public string Language { get; set; }
        [JsonProperty("@timestamp")]
        public DateTime Timestamp { get; set; }
        public string BTIH { get; set; }
        public string SHA256 { get; set; }
        public string TTH { get; set; }
        public long Subfolder { get; set; }
        public string Identifier { get; set; }
        public long ID { get; set; }
        public string Visible { get; set; }
        public string SHA1 { get; set; }
        public long Version { get; set; }
        public string Generic { get; set; }
        public string Topic { get; set; }
        public string Series { get; set; }
        public string Edition { get; set; }
        public string GooglebookID { get; set; }
        public string Extension { get; set; }
        public DateTime DescriptionTimeLastModified { get; set; }
        public string Pages { get; set; }
        public string MD5 { get; set; }
        public string Publisher { get; set; }
        public string Library { get; set; }
        public string Filename { get; set; }
        public string CRC32 { get; set; }
        public string Index { get; set; }
        public string Cover { get; set; }
        public DateTime TimeAdded { get; set; }
        public string Locator { get; set; }
        public string Title { get; set; }
        public string ASIN { get; set; }
        public string IPFSBlake2B { get; set; }
        public string AICH { get; set; }
        public string Author { get; set; }
        public string Issue { get; set; }
        public string Year { get; set; }
        public long Filesize { get; set; }
        public string Commentary { get; set; }
        public string Edonkey { get; set; }
    }

    public partial class Total
    {
        public int Value { get; set; }
        public string Relation { get; set; }
    }

    public partial class Shards
    {
        public int Total { get; set; }
        public int Successful { get; set; }
        public int Skipped { get; set; }
        public int Failed { get; set; }
    }
}
