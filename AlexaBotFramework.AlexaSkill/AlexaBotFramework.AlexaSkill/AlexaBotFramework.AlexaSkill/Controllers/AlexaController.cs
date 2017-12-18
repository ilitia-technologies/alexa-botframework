using AlexaBotFramework.AlexaSkill.Speechlet;
using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace AlexaBotFramework.AlexaSkill.Controllers
{
    public class AlexaController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> ConnectWithBotFramework()
        {
            try
            {
                var speechlet = new BotFrameworkSpeechletAsync();

                return await speechlet.GetResponseAsync(Request);
            }
            catch(Exception ex)
            {
                var telemetryClient = new TelemetryClient();
                telemetryClient.TrackException(ex);

                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}