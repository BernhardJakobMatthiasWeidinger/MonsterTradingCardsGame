using MTCG.Models;
using SWE1HttpServer.Core.Response;
using SWE1HttpServer.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.RouteCommands.Cards {
    public class AddPackageCommand : ProtectedRouteCommand {
        private readonly MTCGManager mTCGManager;
        private readonly string payload;

        public AddPackageCommand(MTCGManager mTCGManager, string payload) {
            this.mTCGManager = mTCGManager;
            this.payload = payload;
        }

        public override Response Execute() {
            Response response = new Response();
            try {
                Package package = mTCGManager.CreatePackage(User.Username, payload);

                if (User.Username == "admin") {
                    if (package != null) {
                        DBConnection.InsertPackage(package);
                        response.StatusCode = StatusCode.Created;
                    } else {
                        response.StatusCode = StatusCode.BadRequest;
                    }
                }
                else {
                    response.StatusCode = StatusCode.Unauthorized;
                }
            } catch (ArgumentException) {
                response.StatusCode = StatusCode.Conflict;
            } catch (Exception) {
                response.StatusCode = StatusCode.BadRequest;
            }

            return response;
        }
    }
}
