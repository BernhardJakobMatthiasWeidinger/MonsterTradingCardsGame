using MTCG.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SWE1HttpServer.Core.Response;
using SWE1HttpServer.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.RouteCommands.Users {
    public class SetUserDataCommand : ProtectedRouteCommand {
        private readonly MTCGManager mTCGManager;
        private readonly string username;
        private readonly string payload;

        public SetUserDataCommand(MTCGManager mTCGManager, string username, string payload) {
            this.mTCGManager = mTCGManager;
            this.username = username;
            this.payload = payload;
        }

        public override Response Execute() {
            Response response = new Response();

            try {
                if (User.Username == username) {
                    mTCGManager.SetUserData(User, payload);
                    response.StatusCode = StatusCode.Ok;
                } else {
                    response.StatusCode = StatusCode.Unauthorized;
                }
            } catch (Exception) {
                response.StatusCode = StatusCode.BadRequest;
            }
            return response;
        }
    }
}
