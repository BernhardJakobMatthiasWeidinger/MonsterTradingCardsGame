using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SWE1HttpServer.Core.Response;
using SWE1HttpServer.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MTCG.Models;
using MTCG.Exceptions;

namespace MTCG.RouteCommands.Users {
    public class ConfigureDeckCommand : ProtectedRouteCommand {
        private readonly MTCGManager mTCGManager;
        private readonly string payload;

        public ConfigureDeckCommand(MTCGManager mTCGManager, string payload) {
            this.mTCGManager = mTCGManager;
            this.payload = payload;
        }

        public override Response Execute() {
            Response response = new Response();

            try {
                List<Guid> cardIds = new List<Guid>();
                JArray jsonIds = (JArray)JsonConvert.DeserializeObject(payload);
                foreach (JValue id in jsonIds) {
                    cardIds.Add(Guid.Parse(id.ToString()));
                }

                mTCGManager.ConfigureDeck(User, cardIds);
                response.StatusCode = StatusCode.Ok;
            } catch (Exception ex) {
                if (ex is InconsistentNumberException) {
                    response.StatusCode = StatusCode.Conflict;
                } else if (ex is NotInDeckOrStackException) {
                    response.StatusCode = StatusCode.Forbidden;
                } else {
                    response.StatusCode = StatusCode.BadRequest;
                }

                response.Payload = User.DeckToString(true);
            }

                return response;
        }
    }
}
