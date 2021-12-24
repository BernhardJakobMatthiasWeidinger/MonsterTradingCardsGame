using MTCG.Models;
using SWE1HttpServer.Core.Authentication;
using SWE1HttpServer.Core.Response;
using SWE1HttpServer.Core.Routing;

namespace MTCG.RouteCommands {
    public abstract class ProtectedRouteCommand : IProtectedRouteCommand {
        public IIdentity Identity { get; set; }

        public User User => (User)Identity;

        public abstract Response Execute();
    }
}
