using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamBot.Api.Dto
{
    public class BingResponse
    {
        [JsonProperty("webPages")]
        public BingWebPages? WebPages { get; set; }
    }

    public class BingWebPages
    {
        [JsonProperty("value")]
        public required List<bingValue> Value;
    }

    public class bingValue
    {
        [JsonProperty("url")]
        public required string Url { get; set; }
    }
}
