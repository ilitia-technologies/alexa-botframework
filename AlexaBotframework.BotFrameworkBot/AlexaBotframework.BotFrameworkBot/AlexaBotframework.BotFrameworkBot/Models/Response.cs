using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlexaBotframework.BotFrameworkBot.Models
{
    public class Response
    {
        public string Message { get; set; }
        public bool EndConversation { get; set; }

        public Response(string message, bool endConversation = false)
        {
            Message = message;
            EndConversation = endConversation;
        }
    }
}