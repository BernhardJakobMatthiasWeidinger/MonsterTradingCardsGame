using MTCG.Models;
using SWE1HttpServer.Core.Response;
using SWE1HttpServer.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.RouteCommands.Users {
    public class GetScoreboardCommand : ProtectedRouteCommand {
        private readonly MTCGManager mTCGManager;
        private bool json = true;

        public GetScoreboardCommand(MTCGManager mTCGManager, string format) {
            this.mTCGManager = mTCGManager;
            if (!String.IsNullOrWhiteSpace(format)) {
                if (format.Split("=")[1] == "plain") {
                    json = false;
                }
            }
        }

        public override Response Execute() {
            Response response = new Response();

            string scoreboard = mTCGManager.GetScoreboard(json);
            response.StatusCode = StatusCode.Ok;
            response.Payload = scoreboard;

            return response;
        }
    }
}
