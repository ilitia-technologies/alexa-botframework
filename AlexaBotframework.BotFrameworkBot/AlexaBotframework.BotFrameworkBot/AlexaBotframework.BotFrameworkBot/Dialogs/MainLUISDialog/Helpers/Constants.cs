using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlexaBotframework.BotFrameworkBot.Dialogs.MainLUISDialog.Helpers
{
    public static class Constants
    {
        public static class Messages
        {
            public const string Welcome = "Bienvenido, {0}, soy tu ayudante personal, voy a realizar una pequeña encuesta, ¿estás listo?";

            public const string CannotUnderstand = "De veras que lo siento, no he podido enterderte. Ponte en contacto con información.";

            public const string AskForRepetition = "Lo siento, no he entendido. Por favor, ¿puedes explicármelo de otra forma?";

            public const string SeeYouNextTime = "De acuerdo, nos vemos otro día.";

            public static string ConversationSummary = "Tienes {0} años, estudias en {1} y ésta es tu opinión sobre la charla: {2}";

            public static string SurveyIsStarting = "Por favor, contesta a las siguientes preguntas:";
        }

        public static class Intents
        {
            public const string Ready = "Disponible";
            public const string NotReady = "NoDisponible";
            public const string None = "None";
            public const string Empty = "";
            public const string StartDialog = "";
        }
    }
}