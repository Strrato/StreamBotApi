using Newtonsoft.Json;
using StreamBot.Api.Cache;
using StreamBot.Api.Dto;
using StreamBot.Api.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StreamBot.Api
{
    public partial class BotApi
    {
        public static readonly string CacheName = "UrlCache.json";

        private static readonly string endpoint = "https://api.bing.microsoft.com/v7.0/search";

        public async Task<string?> GetInstantGamingLink(string gameName)
        {

            if (bingSubscriptionKey == null)
            {
                throw new Exception("Bing api key is missing");
            }

            GameCache? cache = CacheUtils.GetCache<GameCache>(CacheName).Where(c => c.Game == gameName).FirstOrDefault();
            if (cache != null)
            {
                return cache.Url;
            }

            
            string? url = await BingSearch($"site:instant-gaming.com pc acheter \"{gameName}\"");

            if (url != null) 
            { 
                GameCache newEntry = new() { Game = gameName, Url = url };
                CacheUtils.AddToCache(CacheName, newEntry);
            }

            return url;
        }

        private async Task<string?> BingSearch(string query)
        {
            var uriQuery = endpoint + "?q=" + Uri.EscapeDataString(query);
            List<bingValue>? bingValues = null;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", bingSubscriptionKey);
                HttpResponseMessage response = await client.GetAsync(uriQuery);
                string jsonString = await response.Content.ReadAsStringAsync();
                BingResponse? bingResponse = JsonConvert.DeserializeObject<BingResponse>(jsonString);

                if (bingResponse != null && bingResponse.WebPages != null)
                {
                    bingValues = bingResponse.WebPages.Value;
                }
            }
            string? url = null;
            if (bingValues != null && bingValues.Count() > 0) 
            {
                string instantRegex = @"(https?:\/\/www.instant-gaming.com\/[A-z]{2}\/\d+-[A-Za-z0-9\-\/]+)";
                url = bingValues.Where(u => Regex.IsMatch(u.Url, instantRegex, RegexOptions.IgnoreCase)).FirstOrDefault()?.Url;
            }

            return url;
        }

    }
}
