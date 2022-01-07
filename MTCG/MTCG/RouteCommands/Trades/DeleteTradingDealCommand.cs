using MTCG.Exceptions;
using MTCG.Models;
using SWE1HttpServer.Core.Response;
using SWE1HttpServer.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.RouteCommands.Trades {
    public class DeleteTradingDealCommand : ProtectedRouteCommand {
        private readonly MTCGManager mTCGManager;
        private readonly string tradeId;

        public DeleteTradingDealCommand(MTCGManager mTCGManager, string tradeId) {
            this.mTCGManager = mTCGManager;
            this.tradeId = tradeId;
        }

        public override Response Execute() {
            Response response = new Response();

            try {
                mTCGManager.DeleteTrade(User, tradeId);
                response.StatusCode = StatusCode.Ok;
            } catch (EntityNotFoundException) {
                response.StatusCode = StatusCode.NotFound;
            } catch (InvalidOperationException) {
                response.StatusCode = StatusCode.Forbidden;
            } catch (Exception) {
                response.StatusCode = StatusCode.BadRequest;
            }

            return response;
        }
    }
}
