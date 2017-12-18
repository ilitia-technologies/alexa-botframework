using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AlexaBotframework.BotFrameworkBot.Providers
{
    public static class AppSettingsProvider
    {
        public static string GetValue(string key, bool required = true)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            string value = ConfigurationManager.AppSettings[key];

            if (required && String.IsNullOrWhiteSpace(value))
            {
                throw new Exception("Required configuration value for '" + key + "' is missing. Add it to config.");
            }

            return value;
        }
    }
}