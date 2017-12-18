using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlexaBotFramework.AlexaSkill.Services.BotFrameworkService.Models
{
    public class DirectLineActivityResponse
    {
        public string Message { get; set; }
        public bool EndConversation { get; set; }
    }
}