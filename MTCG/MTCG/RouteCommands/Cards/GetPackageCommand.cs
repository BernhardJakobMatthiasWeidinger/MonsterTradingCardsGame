using MTCG.Models;
using SWE1HttpServer.Core.Response;
using SWE1HttpServer.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.RouteCommands.Cards {
    public class GetPackageCommand : ProtectedRouteCommand {
        private readonly MTCGManager mTCGManager;

        public GetPackageCommand(MTCGManager mTCGManager) {
            this.mTCGManager = mTCGManager;
        }

        public override Response Execute() {
            Response response = new Response();

            if (mTCGManager.AcquirePackage(User)) {
                response.StatusCode = StatusCode.Ok;
            } else {
                response.StatusCode = StatusCode.Forbidden;
            }

            return response;
        }
    }
}
