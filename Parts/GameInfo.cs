using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StreamBot.Api.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Games;
using TwitchLib.Api.Helix.Models.Streams.GetStreams;

namespace StreamBot.Api
{
    public partial class BotApi
    {
        private Dictionary<string, string> dicoLangs = new()
        {
            { "fr", "french"  },
            { "en", "english" },
            { "es", "spanish" },
        };

        public async Task<GameInfoDto> GetGameInfos(string channelId, bool WithUrl = false)
        {
            GameInfoDto gameInfoDto = new();

            List<string> channels = new() { channelId };

            GetStreamsResponse streams = await twitchAPI.Helix.Streams.GetStreamsAsync(userIds: channels);

            if (streams.Streams.Count() == 0)
            {
                gameInfoDto.Name = "Offline";
                gameInfoDto.IsOffline = true;
                return gameInfoDto;
            }

            gameInfoDto.IsOffline = false;

            TwitchLib.Api.Helix.Models.Streams.GetStreams.Stream stream = streams.Streams[0];
            
            List<string> gamesIds = new()
            {
                stream.GameId
            };

            GetGamesResponse games = await twitchAPI.Helix.Games.GetGamesAsync(gamesIds);

            if (games.Games.Count() == 0)
            {
                throw new Exception("Cannot get game infos");
            }

            Game game = games.Games[0];

            gameInfoDto.Id = game.Id;
            gameInfoDto.Name = game.Name;
            gameInfoDto.Image = game.BoxArtUrl;

            string? lang;
            _ = dicoLangs.TryGetValue(stream.Language.ToLower(), out lang);

            gameInfoDto.Description = await GetSteamDescription(game.Name, lang ?? "french");

            if (WithUrl && bingSubscriptionKey != null)
            {
                gameInfoDto.Url = await GetInstantGamingLink(game.Name);
            }

            return gameInfoDto;
        }

        public async Task<string> GetSteamDescription(string gameName, string lang)
        {
            string? gameId = await GetSteamGameId(gameName);

            if (gameId == null)
            {
                return "";
            }

            string url = $"https://store.steampowered.com/api/appdetails?l={lang}&appids={gameId}";

            using(HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var jsonData = JsonConvert.DeserializeObject<Dictionary<string, GameResponse>>(jsonString);
                    GameResponse? gResp;
                    if (jsonData != null && jsonData.TryGetValue(gameId, out gResp))
                    {
                        if (gResp != null && gResp.Data != null && gResp.Data.Description != null)
                        {
                            return gResp.Data.Description;
                        }
                    }
                }
            }

            return "";
        }

        private async Task<string?> GetSteamGameId(string gameName)
        {
            string url = "https://api.steampowered.com/ISteamApps/GetAppList/v0002/?format=json";

            SteamResponse? steamResponse = null;

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    steamResponse = JsonConvert.DeserializeObject<SteamResponse>(jsonString);
                }
            }

            if (steamResponse == null || steamResponse.AppList == null || steamResponse.AppList.Apps == null)
            {
                return null;
            }

            return steamResponse.AppList.Apps
                                        .Where(a => a.Name.ToLower() == gameName.ToLower())
                                        .SingleOrDefault()?.AppId;
        }

    }
}
