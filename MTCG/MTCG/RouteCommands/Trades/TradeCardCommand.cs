using MTCG.Exceptions;
using MTCG.Models;
using Newtonsoft.Json;
using SWE1HttpServer.Core.Response;
using SWE1HttpServer.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.RouteCommands.Trades {
    public class TradeCardCommand : ProtectedRouteCommand {
        private readonly MTCGManager mTCGManager;
        private readonly string id;
        private readonly string payload;

        public TradeCardCommand(MTCGManager mTCGManager, string id, string payload) {
            this.mTCGManager = mTCGManager;
            this.id = id;
            this.payload = payload;
        }

        public override Response Execute() {
            Response response = new Response();

            try {
                mTCGManager.TradeCard(User, id, JsonConvert.DeserializeObject<string>(payload));
                response.StatusCode = StatusCode.Ok;
            } catch (EntityNotFoundException) {
                response.StatusCode = StatusCode.NotFound;
            } catch (Exception ex) {
                if (ex is InvalidOperationException || 
                    ex is InDeckException ||
                    ex is NotInDeckOrStackException ||
                    ex is InvalidCardTypeException ||
                    ex is InconsistentNumberException) {
                    response.StatusCode = StatusCode.Conflict;
                } else {
                    response.StatusCode = StatusCode.BadRequest;
                }
            }

            return response;
        }
    }
}
