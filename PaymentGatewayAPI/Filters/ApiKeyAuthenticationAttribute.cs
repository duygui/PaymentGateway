using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using PaymentGatewayAPI.DataAccess;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGatewayAPI.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthenticationAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "ApiKey";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!IsKeyValidated(context))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var potentialApiKey);
            var userIdClaim = new Claim(ClaimTypes.Name, potentialApiKey);
            var identity = new ClaimsIdentity(new[] { userIdClaim }, "ApiKey");
            var principal = new ClaimsPrincipal(identity);
            Thread.CurrentPrincipal = principal;

            await next();
        }

        private bool IsKeyValidated(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var potentialApiKey))
            {
                if (Guid.TryParse(potentialApiKey, out Guid potentialApiGuid))
                {
                    var dataOperations = context.HttpContext.RequestServices.GetRequiredService<IDataOperations>();
                    if (dataOperations.GetTenants().Any(p => p.ApiKey == potentialApiGuid))
                        return true;
                }
            }
            return false;
        }
    }
}
