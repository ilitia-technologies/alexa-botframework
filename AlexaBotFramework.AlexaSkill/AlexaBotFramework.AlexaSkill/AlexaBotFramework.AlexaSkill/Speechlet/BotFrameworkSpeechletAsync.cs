using AlexaBotFramework.AlexaSkill.Helpers;
using AlexaBotFramework.AlexaSkill.Services.BotFrameworkService;
using AlexaSkillsKit.Speechlet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AlexaBotFramework.AlexaSkill.Speechlet
{
    public class BotFrameworkSpeechletAsync : SpeechletAsyncBase
    {
        private AlexaBotFramework.AlexaSkill.Services.BotFrameworkService.BotFrameworkService _botFrameworkService;

        public async override Task<SpeechletResponse> OnLaunchAsync(LaunchRequest request, Session session)
        {
            EnsureServiceCreated(session.User.Id);

            // Start conversation
            var success = await _botFrameworkService.StartConversationAsync();
            var response = success
                ? Constants.Messages.ConversationStarted
                : Constants.Messages.ConversationFailed;

            // Welcome message
            return BuildSpeechletResponse(Constants.Messages.Welcome, response, false);
        }

        public async override Task OnSessionStartedAsync(SessionStartedRequest sessionStartedRequest, Session session)
        {
            //Ensure service and conversation are already started
            EnsureServiceCreated(session.User.Id);

            if (!_botFrameworkService.IsConversationStarted)
                await _botFrameworkService.StartConversationAsync();
        }


        public async override Task<SpeechletResponse> OnIntentAsync(IntentRequest request, Session session)
        {

            // Get intent from the request object.
            AlexaSkillsKit.Slu.Intent intent = request.Intent;
            string intentName = (intent != null) ? intent.Name : null;

            // Get user phrase
            var userPhrase = string.Empty;
            if (request != null)
            {
                if (request.Intent.Name == Helpers.Constants.Intents.AmazonHelpIntent)
                {
                    userPhrase = Helpers.Constants.Messages.Help;
                }
                else
                {
                    AlexaSkillsKit.Slu.Slot slotValue;
                    if (request.Intent.Slots.TryGetValue(Constants.Slots.Phrase, out slotValue))
                    {
                        userPhrase = slotValue.Value;
                    }
                }
            }

            EnsureServiceCreated(session.User.Id);

            // Ensure conversation is already started
            if (!_botFrameworkService.IsConversationStarted)
            {
                if (!await _botFrameworkService.StartConversationAsync())
                {
                    return BuildSpeechletResponse(intent.Name, Constants.Messages.ConversationFailed, false);
                }
            }

            // Send activity and receive response
            var shouldEndSession = false;
            var response = string.Empty;
            var success = await _botFrameworkService.SendActivityAsync(userPhrase);

            if (success)
            {
                var directLineActivity = (await _botFrameworkService.ReceiveActivityAsync()).First();
                shouldEndSession = directLineActivity.EndConversation;
                if (shouldEndSession) await _botFrameworkService.EndConversationAsync();
                response = directLineActivity.Message;
            }
            else
            {
                response = Constants.Messages.ActivityFailed;
            }

            return BuildSpeechletResponse(intent.Name, response, shouldEndSession);
        }

        public async override Task OnSessionEndedAsync(SessionEndedRequest request, Session session)
        {
            await _botFrameworkService.EndConversationAsync();
        }

        private void EnsureServiceCreated(string userId)
        {
            string prefix = "amzn1.ask.account.";

            if (_botFrameworkService == null)
                _botFrameworkService = new BotFrameworkService(userId.Substring(prefix.Length, 15));
        }
    }
}