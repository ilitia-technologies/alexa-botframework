using AlexaBotframework.BotFrameworkBot.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace AlexaBotframework.BotFrameworkBot.Dialogs.MainLUISDialog.Models
{
    [Serializable]
    public class JsonResponse
    {
        public string Message { get; set; }
        public bool EndConversation { get; set; }

        public string Intent { get; set; }
        public IEnumerable<string> Entities { get; set; }
        public IDictionary<string, string> ConversationResult { get; set; }


        public JsonResponse(Response response)
        {
            Message = response.Message;
            EndConversation = response.EndConversation;

            this.Entities = new Collection<string>();
        }


    }
}