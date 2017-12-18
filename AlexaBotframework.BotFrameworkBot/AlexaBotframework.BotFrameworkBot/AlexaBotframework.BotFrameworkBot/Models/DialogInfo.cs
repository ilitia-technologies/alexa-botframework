using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlexaBotframework.BotFrameworkBot.Models
{
    [Serializable]
    public class DialogInfo
    {
        public string ChannelId { get; set; }
        public string UserName { get; set; }
        public string Locale { get; set; }

        public DialogInfo(string channelId, string username, string locale)
        {
            ChannelId = channelId;
            UserName = username;
            Locale = locale;
        }

        public string GetUserNameOrDefault()
        {
            return !string.IsNullOrEmpty(UserName) ? UserName : "compañero";
        }

    }
}