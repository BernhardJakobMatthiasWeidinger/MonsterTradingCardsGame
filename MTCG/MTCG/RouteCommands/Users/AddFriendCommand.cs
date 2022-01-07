using MTCG.Exceptions;
using MTCG.Models;
using Newtonsoft.Json;
using SWE1HttpServer.Core.Response;
using SWE1HttpServer.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.RouteCommands.Users {
    public class AddFriendCommand : ProtectedRouteCommand {
        private readonly MTCGManager mTCGManager;
        private string other = "";

        public AddFriendCommand(MTCGManager mTCGManager, string other) {
            this.mTCGManager = mTCGManager;
            this.other = JsonConvert.DeserializeObject<string>(other);
        }

        public override Response Execute() {
            Response response = new Response();

            try {
                mTCGManager.AddFriend(User, other);
                response.StatusCode = StatusCode.Created;
            } catch (FriendException) {
                response.StatusCode = StatusCode.Conflict;
            } catch (EntityNotFoundException) {
                response.StatusCode = StatusCode.NotFound;
            }

            return response;
        }
    }
}
