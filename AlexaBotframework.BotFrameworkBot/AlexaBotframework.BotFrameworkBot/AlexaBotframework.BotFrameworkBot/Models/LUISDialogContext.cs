using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Luis.Models;
using AlexaBotframework.BotFrameworkBot.Helpers;

namespace AlexaBotframework.BotFrameworkBot.Models
{
    [Serializable]
    public class LUISDialogContext
    {
        private readonly string _customerId;

        public string Intent { get; set; }
        public IntentType PreviousIntentType { get; private set; }
        public IntentType CurrentIntentType { get; private set; }
        public IEnumerable<string> Entities { get; set; }
        public string Query { get; set; }
        public IEnumerable<Question> Questions { get; set; }

        public bool SurveyCompleted => Questions.Count() > 0 && Questions.Count() == Questions.Where(q => !string.IsNullOrEmpty(q.Answer)).Count();

        public Question CurrentQuestion => SurveyCompleted ? null : Questions.Where(q =>  string.IsNullOrEmpty(q.Answer)).FirstOrDefault();

        public LUISDialogContext(string customerId)
        {
            _customerId = customerId;

            Questions = new List<Question>();
        }


        public void Refresh(IntentType? intentType, LuisResult result)
        {
            Intent = result?.Intents.FirstOrDefault().Intent;
            Entities = result?.Entities.Select(e => e.Entity).ToList();
            Query = result?.Query;

            PreviousIntentType = CurrentIntentType;
            if (intentType.HasValue) CurrentIntentType = intentType.Value;
        }

        #region answers

        public void AddAnswer(string text)
        {
            AddAnswer(CurrentQuestion.Id, text);
        }

        private void AddAnswer(int questionId, string text)
        {
            var question = Questions.First(q => q.Id == questionId);
            var answer = question.Answer;
            if (string.IsNullOrEmpty(answer))
            {
                question.Answer = text;
            }
            else
            {
                if (string.IsNullOrEmpty(text))
                    return;

                question.Answer = text;
            }
        }

        public void CheckAnswer(LuisResult result)
        {
            foreach (var entity in result.Entities)
            {
                var entityType = entity.Type;
                if (entity.Type == Helpers.Constants.Entities.Builtin_Age)
                {
                    entityType = Helpers.Constants.Entities.Builtin_Age;
                }
                else if(entity.Type == Helpers.Constants.Entities.Location)
                {
                    entityType = Helpers.Constants.Entities.Location;
                }
                else if (entity.Type.Contains("::"))
                {
                    entityType = entity.Type.Substring(0, entity.Type.IndexOf("::"));
                }

                var question = Questions.FirstOrDefault(e => e.EntityType.Parse() == entityType);
                if (question != null)
                {
                    if (entity.Type.Contains(Constants.Entities.Builtin_Age) && entity.Resolution.Count > 1)
                    {
                        AddAnswer(question.Id, entity.Resolution.LastOrDefault().Value as string);
                    }
                    else
                    {
                        AddAnswer(question.Id, entity.Entity);
                    }
                }
            }
        }

        #endregion

    }
}