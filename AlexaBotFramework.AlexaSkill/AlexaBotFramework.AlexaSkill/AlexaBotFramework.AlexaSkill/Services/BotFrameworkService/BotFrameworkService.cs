using AlexaBotFramework.AlexaSkill.Configuration;
using AlexaBotFramework.AlexaSkill.Helpers;
using AlexaBotFramework.AlexaSkill.Services.BotFrameworkService.Models;
using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AlexaBotFramework.AlexaSkill.Services.BotFrameworkService
{
    public class BotFrameworkService
    {
        private readonly string _userId;
        private readonly DirectLineClient _client;

        public static Dictionary<string, ConversationInfo> ConversationIdUserIdDictionary = new Dictionary<string, ConversationInfo>();
        public bool IsConversationStarted => BotFrameworkService.ConversationIdUserIdDictionary.ContainsKey(_userId);


        public BotFrameworkService(string userId)
        {
            _userId = userId;
            _client = new DirectLineClient(ConfigurationManager.AppSettings[BotFrameworkSettings.Bot_DIRECTLINE_SECRET]);
        }

        public async Task<bool> StartConversationAsync()
        {
            if (!IsConversationStarted)
            {
                var conversation = await _client.Conversations.StartConversationAsync();
                if (conversation != null)
                {
                    if (BotFrameworkService.ConversationIdUserIdDictionary.ContainsKey(_userId))
                        BotFrameworkService.ConversationIdUserIdDictionary.Remove(_userId);

                    BotFrameworkService.ConversationIdUserIdDictionary.Add(_userId,
                        new ConversationInfo
                        {
                            ConnectionId = conversation.ConversationId,
                            Watermark = null
                        }
                    );

                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> SendActivityAsync(string userPhrase)
        {
            var activity = CreateActivity(ActivityTypes.Message);
            activity.Text = userPhrase;

            bool success = await _client.Conversations.PostActivityAsync(BotFrameworkService.ConversationIdUserIdDictionary[_userId].ConnectionId, activity) != null;

            return success;
        }

        public async Task<IEnumerable<DirectLineActivityResponse>> ReceiveActivityAsync()
        {
            var activitySet = await _client.Conversations.GetActivitiesAsync(
                BotFrameworkService.ConversationIdUserIdDictionary[_userId].ConnectionId,
                BotFrameworkService.ConversationIdUserIdDictionary[_userId].Watermark
            );
            BotFrameworkService.ConversationIdUserIdDictionary[_userId].Watermark = activitySet?.Watermark;

            var botName = ConfigurationManager.AppSettings[BotFrameworkSettings.BOT_NAME];
            var activities = activitySet.Activities.Where(a => a.From.Id == botName).Select(a => JsonConvert.DeserializeObject<DirectLineActivityResponse>(a.Text));

            return activities;
        }

        public async Task<bool> EndConversationAsync()
        {
            if (IsConversationStarted)
            {
                var activity = CreateActivity(ActivityTypes.EndOfConversation);
                var response = await _client.Conversations.PostActivityAsync(BotFrameworkService.ConversationIdUserIdDictionary[_userId].ConnectionId, activity);

                if (response.Id != null)
                {
                    BotFrameworkService.ConversationIdUserIdDictionary.Remove(_userId);
                    return true;
                }
                return false;
            }

            return true;
        }

        private Activity CreateActivity(string type)
        {
            return new Activity()
            {
                From = new ChannelAccount(_userId),
                Type = type,
                Locale = Constants.Locales.USA
            };
        }
    }
}