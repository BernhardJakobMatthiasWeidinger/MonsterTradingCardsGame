using MTCG.Exceptions;
using MTCG.Models;
using SWE1HttpServer.Core.Response;
using SWE1HttpServer.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.RouteCommands.Trades {
    public class AddTradingDealCommand : ProtectedRouteCommand {
        private readonly MTCGManager mTCGManager;
        private readonly string payload;

        public AddTradingDealCommand(MTCGManager mTCGManager, string payload) {
            this.mTCGManager = mTCGManager;
            this.payload = payload;
        }

        public override Response Execute() {
            Response response = new Response();

            try {
                mTCGManager.CreateTrade(User, payload);
                response.StatusCode = StatusCode.Created;
            } catch (Exception ex) {
                if (ex is EntityAlreadyExistsException ||
                    ex is InDeckException ||
                    ex is NotInDeckOrStackException) {
                    response.StatusCode = StatusCode.Forbidden;
                } else {
                    response.StatusCode = StatusCode.BadRequest;
                }
            }

            return response;
        }
    }
}
