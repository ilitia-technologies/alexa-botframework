using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlexaBotframework.BotFrameworkBot.Models
{
    [Serializable]
    public class Question
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Text  { get; set; }
        public EntityType EntityType { get; set; }

        public bool SuspendConversation { get; set; }

        public string Answer { get; set; }

    }
}