using MTCG.Models;
using SWE1HttpServer.Core.Response;
using SWE1HttpServer.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.RouteCommands.Users {
    public class GetUserDataCommand : ProtectedRouteCommand {
        private readonly MTCGManager mTCGManager;
        private readonly string username;

        public GetUserDataCommand(MTCGManager mTCGManager, string username) {
            this.mTCGManager = mTCGManager;
            this.username = username;
        }

        public override Response Execute() {
            Response response = new Response();

            if (User.Username == username) {
                response.StatusCode = StatusCode.Ok;
                response.Payload = User.GetUserData(true);
            } else {
                response.StatusCode = StatusCode.Forbidden;
            }

            return response;
        }
    }
}
