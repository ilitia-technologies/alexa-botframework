using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlexaBotFramework.AlexaSkill.Helpers
{
    public class Constants
    {
        public static class Slots
        {
            public const string Phrase = "phrase";
        }

        public static class Intents
        {
            public const string AmazonHelpIntent = "AMAZON.HelpIntent";
        }

        public static class Messages
        {
            public const string Help = "I need help";

            public const string Welcome = "Welcome, talk now with your bot";
            public const string ConversationStarted = Welcome;
            public const string ConversationFailed = "Sorry, there was a problem";
            public const string ActivityFailed = "Sorry, we couldn't send your message to the bot";
        }

        public static class Locales
        {
            public const string USA = "en-US";
        }
    }
}