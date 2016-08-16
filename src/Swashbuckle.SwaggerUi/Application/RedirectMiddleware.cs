using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.AspNetCore.Http.Extensions;

namespace Swashbuckle.SwaggerUi.Application
{
    public class RedirectMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _toPath;
        private readonly TemplateMatcher _requestMatcher;

        public RedirectMiddleware(
            RequestDelegate next,
            string fromPath,
            string toPath)
        {
            _next = next;
            _toPath = toPath;
            _requestMatcher = new TemplateMatcher(TemplateParser.Parse(fromPath), new RouteValueDictionary());
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (!RequestingFromPath(httpContext.Request))
            {
                await _next(httpContext);
                return;
            }
            string basePath = UriHelper.GetDisplayUrl(httpContext.Request);
            RespondWithRedirect(httpContext.Response, basePath);
        }

        private bool RequestingFromPath(HttpRequest request)
        {
            if (request.Method != "GET") return false;

			return _requestMatcher.TryMatch(request.Path, new RouteValueDictionary());
        }

        private void RespondWithRedirect(HttpResponse response, string pathBase)
        {
            response.Redirect(pathBase + "/" + _toPath);
        }
    }
}
