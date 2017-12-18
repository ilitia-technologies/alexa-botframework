using AlexaBotframework.BotFrameworkBot.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using AlexaBotframework.BotFrameworkBot.Utils;

namespace AlexaBotframework.BotFrameworkBot.Services
{
    public class MicrosoftTranslatorService
    {
        /// Header name used to pass the subscription key to translation service
        private const string OcpApimSubscriptionKeyHeader = "Ocp-Apim-Subscription-Key";

        /// Url template to make translate call
        private const string TranslateUrlTemplate = "http://api.microsofttranslator.com/v2/http.svc/translate?text={0}&from={1}&to={2}&category={3}";

        private static readonly string microsoftTranslatorKey;

        static MicrosoftTranslatorService()
        {
            microsoftTranslatorKey = AppSettingsProvider.GetValue("MicrosoftTranslatorKey");
        }

        /// <summary>
        /// Translates text
        /// </summary>
        /// <param name="text">Text to translate</param>
        /// <param name="languageFrom">original language</param>
        /// <param name="languageTo">final language</param>
        /// <returns></returns>
        public static async Task<string> TranslateAsync(string text, string languageFrom = "en", string languageTo = "es")
        {
            var translation = string.Empty;
            try
            {
                var translateResponse = await TranslateRequest(string.Format(TranslateUrlTemplate, text, languageFrom, languageTo, "general"), MicrosoftTranslatorService.microsoftTranslatorKey);

                if (translateResponse.IsSuccessStatusCode)
                {
                    System.Runtime.Serialization.DataContractSerializer dcs = new System.Runtime.Serialization.DataContractSerializer(Type.GetType("System.String"));
                    var stream = await translateResponse.Content.ReadAsStreamAsync();
                    translation = (string)dcs.ReadObject(stream);
                }
                else
                {
                    translation = "Sorry, we couldn't translate your message";
                }
            }
            catch (Exception ex)
            {
                translation = "Sorry, there was an error translating your message";
            }
            DiagnosticsUtil.TraceInformation($"Translating: original {text} - result: {translation}");
            return translation;
        }

        private static async Task<HttpResponseMessage> TranslateRequest(string url, string azureSubscriptionKey)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add(OcpApimSubscriptionKeyHeader, azureSubscriptionKey);
                return await client.GetAsync(url);
            }
        }
    }
}