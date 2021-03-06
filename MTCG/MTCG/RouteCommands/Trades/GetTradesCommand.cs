using MTCG.Models;
using SWE1HttpServer.Core.Response;
using SWE1HttpServer.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.RouteCommands.Trades {
    public class GetTradesCommand : ProtectedRouteCommand {
        private readonly MTCGManager mTCGManager;
        private bool json = true;

        public GetTradesCommand(MTCGManager mTCGManager, string format) {
            this.mTCGManager = mTCGManager;
            if (format == "plain") { json = false; }
        }

        public override Response Execute() {
            Response response = new Response();

            response.StatusCode = StatusCode.Ok;
            response.Payload = mTCGManager.GetTrades(User, json);

            return response;
        }
    }
}
