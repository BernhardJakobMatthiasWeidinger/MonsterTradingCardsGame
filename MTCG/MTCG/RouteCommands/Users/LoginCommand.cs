using MTCG.Models;
using SWE1HttpServer.Core.Response;
using SWE1HttpServer.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.RouteCommands.Users {
    public class LoginCommand : IRouteCommand {
        private readonly MTCGManager mTCGManager;
        private readonly string username;
        private readonly string password;

        public LoginCommand(MTCGManager mTCGManager, string username, string password) {
            this.mTCGManager = mTCGManager;
            this.username = username;
            this.password = password;
        }
        public Response Execute() {
            User user = mTCGManager.LoginUser(username, password);
            Response response = new Response();

            if (user == null) {
                response.StatusCode = StatusCode.Unauthorized;
            } else {
                response.StatusCode = StatusCode.Ok;
                response.Payload = user.Username + "-mtcgToken";
            }

            return response;
        }
    }
}
