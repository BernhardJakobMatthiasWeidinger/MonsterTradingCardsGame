﻿using MTCG.Models;
using SWE1HttpServer.Core.Response;
using SWE1HttpServer.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.RouteCommands.Users {
    public class GetDeckCommand : ProtectedRouteCommand {
        private readonly MTCGManager mTCGManager;
        private bool json = true;

        public GetDeckCommand(MTCGManager mTCGManager, string format) {
            this.mTCGManager = mTCGManager;
            if (format == "plain") { json = false; }
        }

        public override Response Execute() {
            Response response = new Response();

            if (User.Deck.Any()) {
                response.StatusCode = StatusCode.Ok;
                response.Payload = User.DeckToString(json);
            } else {
                response.StatusCode = StatusCode.NoContent;
            }

            return response;
        }
    }
}
