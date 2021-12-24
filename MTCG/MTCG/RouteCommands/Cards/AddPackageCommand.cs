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

            response.StatusCode = StatusCode.Ok;
            response.Payload = payload;

            return response;
        }
    }
}
