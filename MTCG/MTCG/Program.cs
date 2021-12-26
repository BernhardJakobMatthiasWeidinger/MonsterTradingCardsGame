using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using SWE1HttpServer.Core.Request;
using SWE1HttpServer.Core.Routing;
using SWE1HttpServer.Core.Server;

using MTCG.Models;
using MTCG.DAL;
using MTCG.RouteCommands.Users;
using System.Net;
using MTCG.RouteCommands.Cards;

namespace MTCG {
    class Program {
        static void Main(string[] args) {
            if (DBConnection.Connect()) {
                DBUserRepository userRepository = new DBUserRepository();
                DBCardRepository cardRepository = new DBCardRepository();
                MTCGManager mtcgManager = new MTCGManager(userRepository, cardRepository);

                var identityProvider = new MTCGIdentityProvider(userRepository);
                var routeParser = new IdRouteParser();

                var router = new Router(routeParser, identityProvider);
                RegisterRoutes(router, mtcgManager);

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
            router.AddProtectedRoute(HttpMethod.Get, "/cards{id}", (r, p) => new GetStackCommand(mtcgManager, p["id"]));
            router.AddProtectedRoute(HttpMethod.Get, "/deck{id}", (r, p) => new GetDeckCommand(mtcgManager, p["id"]));
        }

        private static string getAttribute(string json, string attribute) {
            JObject jobject = JObject.Parse(json);
            return jobject.Value<string>(attribute);
        }

        void testing() {
            //Create cards
            Console.WriteLine("Create Cards:");
            try
            {
                MonsterCard m0 = new MonsterCard(Guid.NewGuid(), "Unicorn", 10.0);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }

            MonsterCard m1 = new MonsterCard(Guid.NewGuid(), "Ork", 10.0);
            MonsterCard m2 = new MonsterCard(Guid.NewGuid(), "FireElf", 12.0);
            MonsterCard m3 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 25.0);
            MonsterCard m4 = new MonsterCard(Guid.NewGuid(), "WaterDragon", 34.0);
            MonsterCard m5 = new MonsterCard(Guid.NewGuid(), "FireKraken", 20.0);
            SpellCard s1 = new SpellCard(Guid.NewGuid(), "RegularSpell", 14.0);
            SpellCard s2 = new SpellCard(Guid.NewGuid(), "WaterSpell", 20.0);
            SpellCard s3 = new SpellCard(Guid.NewGuid(), "FireSpell", 30.0);
            SpellCard s4 = new SpellCard(Guid.NewGuid(), "WaterSpell", 25.0);
            SpellCard s5 = new SpellCard(Guid.NewGuid(), "RegularSpell", 40.0);

            //Create packages
            Console.WriteLine("\nCreate Packages:");
            try
            {
                Package p0 = new Package(new List<Card> { m1, m2, s1, s2 });
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
            Package p1 = new Package(new List<Card> { m1, m2, m3, s1, s2 });
            Package p2 = new Package(new List<Card> { m4, m5, s3, s4, s5 });

            //Create user1
            Console.WriteLine("\nCreate User 1 and add cards to stack and deck:");
            User u1 = new User(Guid.NewGuid(), "maxiiii", "supersecretpassword1");
            p1.AquirePackage(u1);
            try
            {
                u1.ConfigureDeck(new List<Guid> { m1.Id, m2.Id, m3.Id, s1.Id, s2.Id });
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
            try
            {
                u1.ConfigureDeck(new List<Guid> { m1.Id, m2.Id, m3.Id, s3.Id });
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }

            u1.ConfigureDeck(new List<Guid> { m1.Id, m2.Id, m3.Id, s1.Id });

            //Create user2
            User u2 = new User(Guid.NewGuid(), "miniiii", "supersecretpassword2");
            p2.AquirePackage(u2);
            u2.ConfigureDeck(new List<Guid> { m4.Id, m5.Id, s3.Id, s4.Id });

            //Create Trade
            Console.WriteLine("\nCreate Trade:");
            try
            {
                Trade tfail1 = new Trade(Guid.NewGuid(), s4, u1, CardType.spell, ElementType.fire, 15.0);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
            try
            {
                Trade tfail2 = new Trade(Guid.NewGuid(), s1, u1, CardType.spell, ElementType.fire, 15.0);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }

            Trade t1 = new Trade(Guid.NewGuid(), s2, u1, CardType.spell, ElementType.normal, 15.0);

            //Trade with user2
            Console.WriteLine("\nTrade with user2:");
            try
            {
                t1.TradeCard(u1, s5);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
            try
            {
                t1.TradeCard(u2, s4);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Before Trade:");
            Console.WriteLine("User1 has card s5 in stack: " + u1.Stack.Contains(s5));
            Console.WriteLine("User2 has card s2 in stack: " + u2.Stack.Contains(s2));
            t1.TradeCard(u2, s5);
            Console.WriteLine("After Trade:");
            Console.WriteLine("User1 has card s5 in stack: " + u1.Stack.Contains(s5));
            Console.WriteLine("User2 has card s2 in stack: " + u2.Stack.Contains(s2));

            Console.WriteLine("\nBattle:");
            Battle b1 = new Battle(Guid.NewGuid(), u1);
            Console.WriteLine(b1.Play(u2));

            u1.AddFriend(u2);

            Console.WriteLine(u1.StackToString(true));
            Console.WriteLine(u1.DeckToString(true));
            Console.WriteLine(u1.GetUserStats(true));
            Console.WriteLine(u1.GetUserData(true));
            Console.WriteLine();
            Console.WriteLine(u1.StackToString(false));
            Console.WriteLine(u1.DeckToString(false));
            Console.WriteLine(u1.GetUserStats(false));
            Console.WriteLine(u1.GetUserData(false));

            Console.WriteLine();

            JArray u1Deck = (JArray)((JObject)JsonConvert.DeserializeObject(u1.DeckToString(true))).GetValue("Deck");
            JObject firstCard = (JObject)JsonConvert.DeserializeObject(u1Deck[0].ToString());

            Console.WriteLine(firstCard.GetValue("Id"));
            Console.Read();
        }
    }
}
