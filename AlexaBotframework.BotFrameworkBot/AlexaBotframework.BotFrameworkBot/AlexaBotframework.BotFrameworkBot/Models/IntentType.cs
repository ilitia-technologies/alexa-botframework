using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlexaBotframework.BotFrameworkBot.Models
{
    public enum IntentType
    {
        None,

        //General
        Ready,
        NotReady,
        StartDialog,

        //Survey
        Number,
        Location,
        Opinion
    }
}