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
        private readonly string id;

        public StartOrJoinBattleCommand(MTCGManager mTCGManager, string id) {
            this.mTCGManager = mTCGManager;
            this.id = id;
        }

        public override Response Execute() {
            Response response = new Response();
            try {
                mTCGManager.GetLogFromBattle(User, id);
            } catch (ArgumentException) {
                response.StatusCode = StatusCode.Conflict;
            } catch (Exception) {
                response.StatusCode = StatusCode.BadRequest;
            }

            return response;
        }
    }
}
