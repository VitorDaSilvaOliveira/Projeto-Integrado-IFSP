using System.Security.Claims;
using Estoque.Domain.Entities;
using Estoque.Infrastructure.Data;
using Estoque.Infrastructure.Utils;
using JJMasterData.Core.UI.Components;
using Microsoft.AspNetCore.Http;

namespace Estoque.Infrastructure.Services;

public class AuditLogService(EstoqueDbContext context, IHttpContextAccessor httpContextAccessor, IComponentFactory componentFactory)
{
    public async Task<JJFormView> GetFormViewAuditLogAsync()
    {
        var formView = await componentFactory.FormView.CreateAsync("AuditLogs");
        formView.ShowTitle = true;
        return formView;
    }
    
    public async Task LogAsync(string area, string action, string details = "", string? userId = null, string? userName = null)
    {
        var httpContext = httpContextAccessor.HttpContext;
        var user = httpContext?.User;

        if (string.IsNullOrEmpty(userId))
            userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userName))
            userName = user?.Identity?.Name;

        var ip = httpContext?.Connection.RemoteIpAddress?.ToString();

        var log = new AuditLog
        {
            UserId = userId,
            UserName = userName,
            IpAddress = ip,
            AccessedAt = LocalTime.Now(),
            Area = area,
            Action = action,
            Details = details
        };

        context.AuditLogs.Add(log);
        await context.SaveChangesAsync();
    }

}