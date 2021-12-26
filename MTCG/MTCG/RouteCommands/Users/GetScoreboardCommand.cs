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

        public GetScoreboardCommand(MTCGManager mTCGManager) {
            this.mTCGManager = mTCGManager;
        }

        public override Response Execute() {
            Response response = new Response();

            string scoreboard = mTCGManager.GetScoreboard();
            response.StatusCode = StatusCode.Ok;
            response.Payload = scoreboard;

            return response;
        }
    }
}
