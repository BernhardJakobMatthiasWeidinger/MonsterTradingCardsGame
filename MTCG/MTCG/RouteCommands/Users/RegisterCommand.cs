using MTCG.Models;
using SWE1HttpServer.Core.Response;
using SWE1HttpServer.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.RouteCommands.Users {
    public class RegisterCommand : IRouteCommand {
        private readonly MTCGManager mTCGManager;
        private readonly string username;
        private readonly string password;

        public RegisterCommand(MTCGManager mTCGManager, string username, string password) {
            this.mTCGManager = mTCGManager;
            this.username = username;
            this.password = password;
        }
        public Response Execute() {
            Response response = new Response();

            if (mTCGManager.RegisterUser(username, password)) {
                response.StatusCode = StatusCode.Created;
                response.Payload = username + "-mtcgToken";
            } else {
                response.StatusCode = StatusCode.Conflict;
            }

            return response;
        }
    }
}
