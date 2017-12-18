using AlexaBotframework.BotFrameworkBot.Dialogs.SurveyLUISDialog.Models;
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

namespace AlexaBotframework.BotFrameworkBot.Dialogs.SurveyLUISDialog
{
    [Serializable]
    [LuisModel("YOUR_LUIS_SURVEY_DIALOG_MODEL_ID", "YOUR_SUBSCRIPTION_KEY", domain: "YOUR_DOMAIN")]
    public class SurveyLUISDialog : LuisDialog<object>
    {
        private readonly SurveyLUISDialogInfo _info;
        private LUISDialogContext _context;

        private FlowType _flowType;
        private bool _misunderstood;

        public SurveyLUISDialog(SurveyLUISDialogInfo info)
        {
            DiagnosticsUtil.TraceInformation($"SurveyLUISDialog constructor");
            _info = info;
        }

        public override async Task StartAsync(IDialogContext context)
        {
            DiagnosticsUtil.TraceInformation($"SurveyLUISDialog- StartAsync");

            _context = new LUISDialogContext(context.Activity.From.Id);
            _context.Refresh(IntentType.StartDialog,  null);
            _context.Questions = new List<Question>()
            {
               new Question(){ Id = 0, Subject = "Años", Text ="¿Cuántos años tienes", EntityType = EntityType.Age },
               new Question(){ Id = 1, Subject = "Lugar de estudios", Text ="¿Dónde estudias", EntityType = EntityType.Location },
               new Question(){ Id = 2, Subject = "Opinion", Text ="¿Te ha gustado la charla?", EntityType = EntityType.Opinion },
            };

            Response response = GetNextResponse();

            await SendResponse(context, response);
        }

        [LuisIntent(Helpers.Constants.Intents.Age)]
        public async Task Number(IDialogContext context, LuisResult result)
        {
            DiagnosticsUtil.TraceInformation($"SurveyLUISDialog- Number");
            _context.Refresh(IntentType.Number, result);
            _context.CheckAnswer(result);

            var response = GetNextResponse();
            await SendResponse(context, response);
        }

        [LuisIntent(Helpers.Constants.Intents.StudiesLocation)]
        public async Task StudiesLocation(IDialogContext context, LuisResult result)
        {
            DiagnosticsUtil.TraceInformation($"SurveyLUISDialog- StudiesLocation");
            _context.Refresh(IntentType.Location, result);
            _context.CheckAnswer(result);

            var response = GetNextResponse();
            await SendResponse(context, response);
        }

        [LuisIntent(Helpers.Constants.Intents.Opinion)]
        public async Task Opinion(IDialogContext context, LuisResult result)
        {
            DiagnosticsUtil.TraceInformation($"SurveyLUISDialog- Opinion");
            _context.Refresh(IntentType.Opinion, result);
            _context.CheckAnswer(result);

            var response = GetNextResponse();
            await SendResponse(context, response);
        }

        //[LuisIntent(Helpers.Constants.Intents.None)]
        [LuisIntent(Helpers.Constants.Intents.Empty)]
        public async Task None(IDialogContext context, LuisResult result)
        {
            DiagnosticsUtil.TraceInformation($"SurveyLUISDialog- Empty");
            _context.Refresh(IntentType.None, result);

            var response = GetNextResponse();
            await SendResponse(context, response);
        }


        #region Response

        private Response GetNextResponse()
        {
            // Understanding
            if (_context.CurrentIntentType == IntentType.None)
            {
                if (_misunderstood)
                    return new Response(Helpers.Constants.Messages.CannotUnderstand);
                else
                {
                    _misunderstood = true;
                    return new Response(Helpers.Constants.Messages.AskForRepetition);
                }
            }
            else
                _misunderstood = false;


            // Get next question
            if (!_context.SurveyCompleted)
            {
                var nextQuestion = _context.CurrentQuestion;
                return new Response(nextQuestion.Text, nextQuestion.SuspendConversation);
            }

            // If no questions left, end questionnaire
            return new Response(Helpers.Constants.Messages.ThanksForCompletingTheSurvey, endConversation: true);
        }

        // Send response
        private async Task SendResponse(IDialogContext context, Response response)
        {
            IMessageActivity activity;

            // Ensure original translation
            DiagnosticsUtil.TraceInformation($"Response locale {_info.Info.Locale}");
            
            response.Message = await TranslationUtil.ReverseFromSpanishTranslation(response.Message, _info.Info.Locale);

            // Build message
            switch (_info.Info.ChannelId)
            {
                case AlexaBotframework.BotFrameworkBot.Helpers.Constants.Channels.DirectLine:
                    activity = BuildDirectLineMessage(context, response);
                    break;
                default:
                    activity = BuildDefaultMessage(context, response);
                    break;
            }

            // Send response
            await context.PostAsync(activity);

            // Check end conversation
            if (response.EndConversation)
            {
                DialogResultType resultType = _context.SurveyCompleted ? DialogResultType.Completed : DialogResultType.Suspended;

                context.Done(resultType);
            }
            else
            {
                context.Wait(MessageReceived);
            }
        }


        private IMessageActivity BuildDefaultMessage(IDialogContext context, Response response)
        {
            IMessageActivity activity = context.MakeMessage();
            activity.Locale = TranslationUtil.GetDefaultLocale(activity.Locale);

            // Set message
            activity.Text = response.Message;

            if (response.EndConversation && _context.SurveyCompleted)
            {
                foreach (var question in _context.Questions)
                {
                    var answer = question.Answer;
                    if (!string.IsNullOrEmpty(answer))
                        activity.Text += $"\n{question.Subject}: {answer}. ";
                }
            }

            return activity;
        }

        private IMessageActivity BuildDirectLineMessage(IDialogContext context, Response response)
        {
            IMessageActivity activity = context.MakeMessage();
            activity.Locale = TranslationUtil.GetDefaultLocale(activity.Locale);
            activity.Text = JsonConvert.SerializeObject(response);

            return activity;
        }

        #endregion
    }
}