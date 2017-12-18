using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlexaBotframework.BotFrameworkBot.Helpers
{
    public static class Constants
    {
        public static class Channels
        {
            public const string DirectLine = "directline";
            public const string Cortana = "cortana";
            public const string WebChat = "webchat";
        }

        public static class Locales
        {
            public const string Spain = "es-ES";
            public const string USA = "en-US";
        }

        public static class Entities
        {
            public const string Builtin_Number = "builtin.number";
            public const string Builtin_Age = "builtin.age";
            public const string Builtin = "builtin";
            public const string Location = "Ubicacion";
            public const string Opinion = "Opinion";
        }
    }
}