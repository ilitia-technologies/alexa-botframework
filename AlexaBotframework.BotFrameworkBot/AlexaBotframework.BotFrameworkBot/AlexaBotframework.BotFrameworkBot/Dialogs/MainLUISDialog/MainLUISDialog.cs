using AlexaBotframework.BotFrameworkBot.Dialogs.MainLUISDialog.Models;
using AlexaBotframework.BotFrameworkBot.Helpers;
using AlexaBotframework.BotFrameworkBot.Models;
using AlexaBotframework.BotFrameworkBot.Utils;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AlexaBotframework.BotFrameworkBot.Dialogs.MainLUISDialog
{
    [Serializable]
    [LuisModel("YOUR_LUIS_MAIN_DIALOG_MODEL_ID", "YOUR_SUBSCRIPTION_KEY", domain: "YOUR_DOMAIN")]
    public class MainLUISDialog : LuisDialog<object>
    {
        private readonly DialogInfo _info;
        private LUISDialogContext _context;

        private FlowType _flowType;
        private bool _misunderstood;


        public MainLUISDialog(DialogInfo info)
        {
            DiagnosticsUtil.TraceInformation($"MainLUISDialog constructor");
            _info = info;
            _flowType = FlowType.Normal;
        }

        public override Task StartAsync(IDialogContext context)
        {
            _context = new LUISDialogContext(context.Activity.From.Id);
            return base.StartAsync(context);
        }

        #region intents

        [LuisIntent(Helpers.Constants.Intents.StartDialog)]
        public async Task StartDialog(IDialogContext context, LuisResult result)
        {
            DiagnosticsUtil.TraceInformation($"MainLUISDialog- StartDialog");

            _context.Refresh(IntentType.StartDialog, result);
            var response = GetNextResponse();
            await SendResponse(context, response);
        }

        [LuisIntent(Helpers.Constants.Intents.Ready)]
        public async Task ReadyData(IDialogContext context, LuisResult result)
        {
            DiagnosticsUtil.TraceInformation($"MainLUISDialog- ReadyData");

            _context.Refresh(IntentType.Ready, result);

            //var response = GetNextResponse();
            var surveyDialogInfo = new SurveyLUISDialog.Models.SurveyLUISDialogInfo(_info);
            _flowType = FlowType.Survey;
            context.Call(new SurveyLUISDialog.SurveyLUISDialog(surveyDialogInfo), SurveyLUISDialogDone);

            await Task.FromResult(0);
        }

        [LuisIntent(Helpers.Constants.Intents.NotReady)]
        public async Task NotReady(IDialogContext context, LuisResult result)
        {
            DiagnosticsUtil.TraceInformation($"MainLUISDialog- NotReady");

            _context.Refresh(IntentType.NotReady, result);

            var response = GetNextResponse();

            await SendResponse(context, response);
        }


        [LuisIntent(Helpers.Constants.Intents.None)]
        public async Task Empty(IDialogContext context, LuisResult result)
        {
            DiagnosticsUtil.TraceInformation($"MainLUISDialog- Empty");

            _context.Refresh(IntentType.None, result);
            var response = GetNextResponse();

            await SendResponse(context, response);
        }

        #endregion



        #region Response

        // Get next response
        private Response GetNextResponse()
        {
            // Understanding
            if (_context.CurrentIntentType == IntentType.None)
            {
                if (_misunderstood)
                {
                    return new Response(Helpers.Constants.Messages.CannotUnderstand);
                }
                else
                {
                    _misunderstood = true;
                    return new Response(Helpers.Constants.Messages.AskForRepetition);
                }
            }
            else
                _misunderstood = false;

            // Welcomes and None
            switch (_context.CurrentIntentType)
            {
                case IntentType.StartDialog:
                    return new Response(String.Format(Helpers.Constants.Messages.Welcome, _info.UserName));
                case IntentType.Ready:
                    return new Response(Helpers.Constants.Messages.SurveyIsStarting);
                case IntentType.NotReady:
                    return new Response(Helpers.Constants.Messages.SeeYouNextTime, endConversation: true);
                default:
                    return new Response(Helpers.Constants.Messages.SeeYouNextTime, endConversation: true);
            }

        }


        // Send response
        private async Task SendResponse(IDialogContext context, Response response)
        {
            IMessageActivity message;

            // Ensure original translation
            response.Message = await TranslationUtil.ReverseFromSpanishTranslation(response.Message, _info.Locale);

            // Build message
            switch (_info.ChannelId)
            {
                case Constants.Channels.DirectLine:
                    message = await BuildDirectLineMessage(context, response);
                    break;
                case Constants.Channels.WebChat:
                default:
                    message = await BuildDefaultMessage(context, response);
                    break;
            }

            // Send message
            await context.PostAsync(message);

            if (response.EndConversation)
            {
                context.EndConversation(EndOfConversationCodes.CompletedSuccessfully);
            }
            else
            {
                context.Wait(MessageReceived);
            }
        }


        private IMessageActivity BuildMessage(IDialogContext context)
        {
            IMessageActivity message = context.MakeMessage();
            message.Locale = _info.Locale;
            return message;
        }

        private async Task<IMessageActivity> BuildDefaultMessage(IDialogContext context, Response response)
        {
            IMessageActivity message = BuildMessage(context);

            // Set message
            message.Text = response.Message;

            // Conversation summary
            if (response.EndConversation && _context.CurrentIntentType != IntentType.NotReady && _flowType == FlowType.Normal)
            {
                message.Text = await TranslationUtil.ReverseFromSpanishTranslation(GetConversationSummary(), TranslationUtil.GetDefaultLocale(_info.Locale));
            }

            return message;
        }
        private async Task<IMessageActivity> BuildDirectLineMessage(IDialogContext context, Response response)
        {
            JsonResponse jsonResponse = new JsonResponse(response);

            // Set jsonResponse
            jsonResponse.ConversationResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                                                await TranslationUtil.ReverseFromSpanishTranslation(
                                                    JsonConvert.SerializeObject(GetConversationSummaryDictionary()), TranslationUtil.GetDefaultLocale(_info.Locale)));

            jsonResponse.Intent = _context.Intent;
            jsonResponse.Entities = _context.Entities;

            // Set message
            IMessageActivity message = BuildMessage(context);
            message.Text = JsonConvert.SerializeObject(jsonResponse);

            return message;
        }


        private string GetConversationSummary()
        {
            var conversation = GetConversationSummaryDictionary();

            string text = string.Format(Helpers.Constants.Messages.ConversationSummary,
                conversation[Constants.Entities.Builtin_Age],
                conversation[Constants.Entities.Location],
                conversation[Constants.Entities.Opinion]);

            return text;
        }

        private Dictionary<string, string> GetConversationSummaryDictionary()
        {
            var conversation = new Dictionary<string, string>();
            if (_context == null)
                return conversation;

            foreach (var question in _context.Questions)
            {
                var answer = question.Answer;
                if (!string.IsNullOrEmpty(answer))
                    conversation.Add(question.EntityType.Parse(), answer);
            }

            return conversation;
        }


        #endregion


        private async Task SurveyLUISDialogDone(IDialogContext context, IAwaitable<object> result)
        {
            await result;
            context.EndConversation(EndOfConversationCodes.CompletedSuccessfully);
        }
    }
}