using MTCG.DAL;
using MTCG.Models;
using SWE1HttpServer.Core.Authentication;
using SWE1HttpServer.Core.Request;

namespace MTCG {
    internal class MTCGIdentityProvider : IIdentityProvider {
        private DBUserRepository userRepository;

        public MTCGIdentityProvider(DBUserRepository userRepository) {
            this.userRepository = userRepository;
        }

        public IIdentity GetIdentyForRequest(RequestContext request) {
            User currentUser = null;

            if (request.Header.TryGetValue("Authorization", out string authToken)) {
                const string prefix = "Basic ";
                if (authToken.StartsWith(prefix)) {
                    currentUser = userRepository.GetUserByAuthToken(authToken.Substring(prefix.Length));
                }
            }

            return currentUser;
        }
    }
}