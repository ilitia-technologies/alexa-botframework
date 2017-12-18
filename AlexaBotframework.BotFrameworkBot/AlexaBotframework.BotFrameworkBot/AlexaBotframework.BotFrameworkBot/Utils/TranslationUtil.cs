using AlexaBotframework.BotFrameworkBot.Services;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using static AlexaBotframework.BotFrameworkBot.Helpers.Constants;

namespace AlexaBotframework.BotFrameworkBot.Utils
{
    public static class TranslationUtil
    {

        /// <summary>
        /// Ensure activity language is spanish.
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <returns></returns>
        public static async Task EnsureSpanishTranslation(IMessageActivity activity)
        {
            if (string.IsNullOrEmpty(activity.Locale))
                return;

            if (activity.Locale.Substring(0, 2) != Helpers.Constants.Locales.Spain.Substring(0, 2))
            {
                activity.Text = await MicrosoftTranslatorService.TranslateAsync(activity.Text, activity.Locale, Helpers.Constants.Locales.Spain);

                activity.Locale = Helpers.Constants.Locales.Spain;
            }
        }

        /// <summary>
        /// Revert response from spanish language to a specified locale.
        /// </summary>
        /// <param name="message">Message in spanish language</param>
        /// <param name="locale">The locale.</param>
        /// <returns></returns>
        public static async Task<string> ReverseFromSpanishTranslation(string message, string locale)
        {
            string translatedMessage = message;

            if (string.IsNullOrEmpty(locale))
                return translatedMessage;

            if (locale.Substring(0, 2) != Helpers.Constants.Locales.Spain.Substring(0, 2))
            {
                translatedMessage = await MicrosoftTranslatorService.TranslateAsync(message, Helpers.Constants.Locales.Spain, locale);
            }

            return translatedMessage;
        }

        public static string GetDefaultLocale(string locale)
        {
            return !string.IsNullOrEmpty(locale) ? locale : Locales.USA;
        }
    }
}