using MTCG.Models;
using SWE1HttpServer.Core.Response;
using SWE1HttpServer.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.RouteCommands.Users {
    public class GetUserStatsCommand : ProtectedRouteCommand {
        private readonly MTCGManager mTCGManager;
        private bool json = true;

        public GetUserStatsCommand(MTCGManager mTCGManager, string format) {
            this.mTCGManager = mTCGManager;
            if (!String.IsNullOrWhiteSpace(format)) {
                if (format.Split("=")[1] == "plain") {
                    json = false;
                }
            }
        }

        public override Response Execute() {
            Response response = new Response();

            response.StatusCode = StatusCode.Ok;
            response.Payload = User.GetUserStats(json);

            return response;
        }
    }
}
