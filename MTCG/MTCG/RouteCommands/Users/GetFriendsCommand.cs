using MTCG.Models;
using SWE1HttpServer.Core.Response;
using SWE1HttpServer.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.RouteCommands.Users {
    public class GetFriendsCommand : ProtectedRouteCommand {
        private readonly MTCGManager mTCGManager;
        private bool json = true;

        public GetFriendsCommand(MTCGManager mTCGManager, string format) {
            this.mTCGManager = mTCGManager;
            if (format == "plain") { json = false; }
        }

        public override Response Execute() {
            Response response = new Response();

            if (User.Friends.Any()) {
                response.StatusCode = StatusCode.Ok;
                response.Payload = mTCGManager.GetFriends(User, json);
            }
            else {
                response.StatusCode = StatusCode.NoContent;
            }

            return response;
        }
    }
}
