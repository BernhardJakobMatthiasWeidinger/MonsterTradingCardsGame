using SWE1HttpServer.Core.Request;
using SWE1HttpServer.Core.Routing;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MTCG {
    class IdRouteParser : IRouteParser {
        public bool IsMatch(RequestContext request, HttpMethod method, string routePattern) {
            if (method != request.Method) {
                return false;
            }

            var pattern = "^" + routePattern.Replace("{id}", ".*").Replace("/", "\\/") + "$";
            return Regex.IsMatch(request.ResourcePath, pattern);
        }

        public Dictionary<string, string> ParseParameters(RequestContext request, string routePattern) {
            var parameters = new Dictionary<string, string>();
            var pattern = "^" + routePattern.Replace("{id}", "(?<id>.*)").Replace("/", "\\/") + "$";

            var result = Regex.Match(request.ResourcePath, pattern);
            if (result.Groups["id"].Success) {
                parameters["id"] = result.Groups["id"].Captures[0].Value;
            }

            return parameters;
        }
    }
}