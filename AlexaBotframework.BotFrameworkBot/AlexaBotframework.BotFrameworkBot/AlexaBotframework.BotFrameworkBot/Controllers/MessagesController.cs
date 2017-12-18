using AlexaBotframework.BotFrameworkBot.Dialogs.MainLUISDialog;
using AlexaBotframework.BotFrameworkBot.Models;
using AlexaBotframework.BotFrameworkBot.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static AlexaBotframework.BotFrameworkBot.Helpers.Constants;

namespace AlexaBotframework.BotFrameworkBot.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {

        private ConnectorClient _connector;
        private string _channelId;
        private string _userName;


        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            HttpResponseMessage response;
            string originalLocale = TranslationUtil.GetDefaultLocale(activity.Locale);

            try
            {
                // Ensure spanish
                await TranslationUtil.EnsureSpanishTranslation(activity);

                _channelId = activity.ChannelId;

                if (!string.IsNullOrEmpty(activity.ServiceUrl))
                {
                    _connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                }

                // Get username
                _userName = GetUserNameOrDefault(activity);

                if (activity.Type == ActivityTypes.Message)
                {
                    // Go to MainDialog
                    var dialogInfo = new DialogInfo(activity.ChannelId, _userName, originalLocale);
                    await Microsoft.Bot.Builder.Dialogs.Conversation.SendAsync(activity, () => new MainLUISDialog(dialogInfo));
                }
                else
                {
                    await HandleSystemMessage(activity);
                }

                response = Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var telemetryClient = new TelemetryClient();
                telemetryClient.TrackException(ex);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, ex.InnerException.ToString());
            }
            return response;
        }

        private async Task HandleSystemMessage(Activity activity)
        {
            if (activity.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels

                //IConversationUpdateActivity update = activity;
                //if (activity.MembersAdded != null && activity.MembersAdded.Any())
                //{
                //    foreach (var newMember in activity.MembersAdded)
                //    {
                //        if (newMember.Id != activity.Recipient.Id)
                //        {
                //            await SendWelcomeMessage(activity);
                //        }
                //    }
                //}

            }
            else if (activity.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
                //if (activity.Action == "add")
                //{
                //    await SendWelcomeMessage(activity);
                //}
            }
            else if (activity.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (activity.Type == ActivityTypes.Ping)
            {
            }
        }

        private string GetUserNameOrDefault(Activity activity)
        {
            String userName = "Ilitio";
            var userInfo = activity.Entities.FirstOrDefault(e => e.Type.Equals("UserInfo"));
            if (userInfo != null)
            {
                var userInfoObj = userInfo.Properties.ToObject<UserInfo>();
                userName = userInfoObj.UserName.GivenName;
            }

            return userName;
        }

        private async Task SendWelcomeMessage(Activity activity)
        {
            Activity message;
            string locale = TranslationUtil.GetDefaultLocale(activity.Locale);

            // Ensure original language
            var welcomeMessage = await TranslationUtil.ReverseFromSpanishTranslation(string.Format(AlexaBotframework.BotFrameworkBot.Dialogs.MainLUISDialog.Helpers.Constants.Messages.Welcome, _userName), locale);

            if (_channelId.Contains(Helpers.Constants.Channels.DirectLine))
            {
                var botResponse = new Dialogs.MainLUISDialog.Models.JsonResponse(new Response(welcomeMessage));
                botResponse.Intent = "";

                var text = JsonConvert.SerializeObject(botResponse);
                message = activity.CreateReply(text, locale);
            }
            else
            {
                message = activity.CreateReply(welcomeMessage, locale);
            }

            await _connector.Conversations.ReplyToActivityAsync(message);
        }

    }
}