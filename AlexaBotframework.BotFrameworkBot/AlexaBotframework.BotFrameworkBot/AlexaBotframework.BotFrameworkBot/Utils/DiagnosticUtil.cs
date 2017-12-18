using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace AlexaBotframework.BotFrameworkBot.Utils
{
    public static class DiagnosticsUtil
    {
        public static void TraceError(string message, string origin = "Unknown")
        {
            var builtMessage = BuildMessage(message, origin);
            System.Diagnostics.Trace.TraceError(builtMessage);
        }

        public static void TraceInformation(string message, [CallerMemberName]string origin = "Unknown")
        {
            var builtMessage = BuildMessage(message, origin);
            System.Diagnostics.Trace.TraceInformation(builtMessage);
        }

        private static string BuildMessage(string message, string origin)
        {
            var dateNow = DateTime.Now.ToString("g", DateTimeFormatInfo.InvariantInfo);
            return $"[{dateNow}]\t({origin})\t{message}";
        }
    }
}