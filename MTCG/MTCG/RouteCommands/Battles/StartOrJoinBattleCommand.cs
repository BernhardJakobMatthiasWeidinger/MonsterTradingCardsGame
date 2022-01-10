using MTCG.Exceptions;
using MTCG.Models;
using SWE1HttpServer.Core.Response;
using SWE1HttpServer.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.RouteCommands.Battles {
    public class StartOrJoinBattleCommand : ProtectedRouteCommand {
        private readonly MTCGManager mTCGManager;
        private readonly string username;

        public StartOrJoinBattleCommand(MTCGManager mTCGManager, string username) {
            this.mTCGManager = mTCGManager;
            this.username = username;
        }

        public override Response Execute() {
            Response response = new Response();
            try {
                response.Payload = mTCGManager.GetLogFromBattle(User, username);
            } catch (EntityAlreadyExistsException) {
                response.StatusCode = StatusCode.Conflict;
            } catch (InconsistentNumberException) {
                response.StatusCode = StatusCode.Forbidden;
            } catch (FriendException) {
                response.StatusCode = StatusCode.Forbidden;
            } catch (Exception) {
                response.StatusCode = StatusCode.BadRequest;
            }

            return response;
        }
    }
}
