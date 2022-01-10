using System;
using System.Collections.Generic;
using System.Net;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using SWE1HttpServer.Core.Request;
using SWE1HttpServer.Core.Routing;
using SWE1HttpServer.Core.Server;

using MTCG.Models;
using MTCG.DAL;
using MTCG.RouteCommands.Users;
using MTCG.RouteCommands.Trades;
using MTCG.RouteCommands.Packages;
using MTCG.RouteCommands.Battles;
using Newtonsoft.Json.Converters;

namespace MTCG {
    class Program {
        static void Main(string[] args) {
            if (DBConnection.Connect()) {
                DBUserRepository userRepository = new DBUserRepository();
                DBPackageRepository cardRepository = new DBPackageRepository();
                DBTradeRepository tradeRepository = new DBTradeRepository();
                DBBattleRepository battleRepository = new DBBattleRepository();
                MTCGManager mtcgManager = new MTCGManager(userRepository, cardRepository, tradeRepository, battleRepository);

                var identityProvider = new MTCGIdentityProvider(userRepository);
                var routeParser = new IdRouteParser();

                var router = new Router(routeParser, identityProvider);
                RegisterRoutes(router, mtcgManager);
                JsonSettings();

                var httpServer = new HttpServer(IPAddress.Any, 10001, router);
                httpServer.Start();
            } else {
                Console.WriteLine("Cannot connect to DB");
            }
        }
        private static void RegisterRoutes(Router router, MTCGManager mtcgManager) {
            // public routes
            router.AddRoute(HttpMethod.Post, "/users", (r, p) => new RegisterCommand(mtcgManager, getAttribute(r.Payload, "Username"), getAttribute(r.Payload, "Password")));
            router.AddRoute(HttpMethod.Post, "/sessions", (r, p) => new LoginCommand(mtcgManager, getAttribute(r.Payload, "Username"), getAttribute(r.Payload, "Password")));

            // protected routes
            router.AddProtectedRoute(HttpMethod.Post, "/packages", (r, p) => new AddPackageCommand(mtcgManager, r.Payload));
            router.AddProtectedRoute(HttpMethod.Post, "/transactions/packages", (r, p) => new GetPackageCommand(mtcgManager));

            router.AddProtectedRoute(HttpMethod.Get, "/cards", (r, p) => new GetStackCommand(mtcgManager, ""));
            router.AddProtectedRoute(HttpMethod.Get, "/cards?format={id}", (r, p) => new GetStackCommand(mtcgManager, p["id"]));
            router.AddProtectedRoute(HttpMethod.Get, "/deck", (r, p) => new GetDeckCommand(mtcgManager, ""));
            router.AddProtectedRoute(HttpMethod.Get, "/deck?format={id}", (r, p) => new GetDeckCommand(mtcgManager, p["id"]));
            router.AddProtectedRoute(HttpMethod.Put, "/deck", (r, p) => new ConfigureDeckCommand(mtcgManager, r.Payload));

            router.AddProtectedRoute(HttpMethod.Get, "/users/{id}", (r, p) => new GetUserDataCommand(mtcgManager, p["id"]));
            router.AddProtectedRoute(HttpMethod.Put, "/users/{id}", (r, p) => new SetUserDataCommand(mtcgManager, p["id"], r.Payload));
            router.AddProtectedRoute(HttpMethod.Get, "/stats", (r, p) => new GetUserStatsCommand(mtcgManager, ""));
            router.AddProtectedRoute(HttpMethod.Get, "/stats?format={id}", (r, p) => new GetUserStatsCommand(mtcgManager, p["id"]));
            router.AddProtectedRoute(HttpMethod.Get, "/score", (r, p) => new GetScoreboardCommand(mtcgManager, ""));
            router.AddProtectedRoute(HttpMethod.Get, "/score?format={id}", (r, p) => new GetScoreboardCommand(mtcgManager, p["id"]));

            router.AddProtectedRoute(HttpMethod.Post, "/battles", (r, p) => new StartOrJoinBattleCommand(mtcgManager, ""));
            router.AddProtectedRoute(HttpMethod.Post, "/battles/{id}", (r, p) => new StartOrJoinBattleCommand(mtcgManager, p["id"]));

            router.AddProtectedRoute(HttpMethod.Get, "/tradings", (r, p) => new GetTradesCommand(mtcgManager, ""));
            router.AddProtectedRoute(HttpMethod.Get, "/tradings?format={id}", (r, p) => new GetTradesCommand(mtcgManager, p["id"]));
            router.AddProtectedRoute(HttpMethod.Post, "/tradings", (r, p) => new AddTradingDealCommand(mtcgManager, r.Payload));
            router.AddProtectedRoute(HttpMethod.Post, "/tradings/{id}", (r, p) => new TradeCardCommand(mtcgManager, p["id"], r.Payload));
            router.AddProtectedRoute(HttpMethod.Delete, "/tradings/{id}", (r, p) => new DeleteTradingDealCommand(mtcgManager, p["id"]));

            router.AddProtectedRoute(HttpMethod.Get, "/friends", (r, p) => new GetFriendsCommand(mtcgManager, ""));
            router.AddProtectedRoute(HttpMethod.Get, "/friends?format={id}", (r, p) => new GetFriendsCommand(mtcgManager, p["id"]));
            router.AddProtectedRoute(HttpMethod.Post, "/friends", (r, p) => new AddFriendCommand(mtcgManager, r.Payload));
            router.AddProtectedRoute(HttpMethod.Delete, "/friends", (r, p) => new DeleteFriendCommand(mtcgManager, r.Payload));
        }

        private static string getAttribute(string json, string attribute) {
            JObject jobject = JObject.Parse(json);
            return jobject.Value<string>(attribute);
        }

        private static void JsonSettings() {
            //Convert enum numbers to string in json
            JsonConvert.DefaultSettings = (() => {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
                return settings;
            });
        }
    }
}
