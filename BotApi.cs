using StreamBot.Api.Dto;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Games;
using TwitchLib.Api.Helix.Models.Streams.GetStreams;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace StreamBot.Api
{
    public partial class BotApi
    {

        protected TwitchAPI twitchAPI;
        protected string? bingSubscriptionKey;


        public BotApi(string clientId, string secret)
        {
            twitchAPI = new();
            twitchAPI.Settings.ClientId = clientId;
            twitchAPI.Settings.Secret = secret;
        }

        public BotApi(string clientId, string secret, string bingKey) : this(clientId, secret)
        {
            bingSubscriptionKey = bingKey;
        }

        public async Task<List<UserInfoDto>> GetUsersPPAsync(List<string> userIds)
        {

            GetUsersResponse userInfos = await twitchAPI.Helix.Users.GetUsersAsync(userIds);

            List<UserInfoDto> usersDto = new();

            foreach(User? user in userInfos.Users)
            {
                if (user == null) continue;
                usersDto.Add(new UserInfoDto()
                {
                    Id = user.Id,
                    ImageUrl = user.ProfileImageUrl
                });
            }


            return usersDto;
        }

        public async Task<string> GetUserId(string username)
        {
            List<string> userNames = new() { username };
            GetUsersResponse users = await twitchAPI.Helix.Users.GetUsersAsync(logins: userNames);

            if (users != null && users.Users.Count() > 0)
            {
                return users.Users[0].Id;
            }

            throw new Exception("Invalid username");
        }
       

    }
}
