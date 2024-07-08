using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MarketMate.Api.Filters;

public class WithVkAccessTokenFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var authorizationHeader = context.HttpContext.Request.Headers["VkAccessToken"].FirstOrDefault();

        if (string.IsNullOrEmpty(authorizationHeader))
        {
            context.Result = new UnauthorizedResult();
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
