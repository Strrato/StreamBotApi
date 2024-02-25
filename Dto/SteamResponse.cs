using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamBot.Api.Dto
{
    public class SteamResponse
    {
        [JsonProperty("applist")]
        public AppList? AppList { get; set; }
    }

    public class AppList
    {
        [JsonProperty("apps")]
        public List<App>? Apps;
    }
    public class App
    {
        [JsonProperty("appid")]
        public required string AppId { get; set; }

        [JsonProperty("name")]
        public required string Name { get; set; }
    }

    public class GameResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public required GameResponseData Data { get; set; }
    }

    public class GameResponseData
    {
        [JsonProperty("short_description")]
        public string? Description { get; set; }
    }

}
