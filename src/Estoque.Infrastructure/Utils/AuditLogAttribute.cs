namespace Estoque.Infrastructure.Utils;

using Estoque.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

public class AuditLogAttribute : Attribute, IAsyncActionFilter
{
    private readonly string _area;
    private readonly string _action;
    private readonly string _details;

    public AuditLogAttribute(string area, string action, string details = "")
    {
        _area = area;
        _action = action;
        _details = details;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        if (resultContext.Exception == null)
        {
            var auditLogService = context.HttpContext.RequestServices.GetRequiredService<AuditLogService>();

            await auditLogService.LogAsync(
                _area,
                _action,
                _details
            );
        }
    }
}
