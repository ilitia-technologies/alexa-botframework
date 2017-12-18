using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlexaBotframework.BotFrameworkBot.Dialogs.SurveyLUISDialog.Helpers
{
    public class Constants
    {
        public static class Messages
        {
            public const string CannotUnderstand = "De veras que lo siento, no he podido enterderte. Ponte en contacto con información.";

            public const string AskForRepetition = "Lo siento, no he entendido. Por favor, ¿puedes explicármelo de otra forma?";

            public static string ThanksForCompletingTheSurvey = "Gracias por completar la encuesta";
        }

        public static class Intents
        {
            public const string Age = "Edad";
            public const string StudiesLocation = "CentroEstudios";
            public const string Opinion = "Opinion";

            public const string None = "None";
            public const string Empty = "";
        }

        public static class Entities
        {
            public const string Builtin_Age = "builtin.age";

            public const string Location = "Ubicacion";
        }
    }
}