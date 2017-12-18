using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlexaBotframework.BotFrameworkBot.Models
{
    [Serializable]
    public enum DialogResultType
    {
        Completed,
        Suspended,
        Posponed
    }
}