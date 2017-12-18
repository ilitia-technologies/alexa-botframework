using AlexaBotframework.BotFrameworkBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlexaBotframework.BotFrameworkBot.Dialogs.SurveyLUISDialog.Models
{
    [Serializable]
    public class SurveyLUISDialogInfo
    {
        public DialogInfo Info { get; set; }

        public SurveyLUISDialogInfo(DialogInfo info)
        {
            Info = info;
        }

    }
}