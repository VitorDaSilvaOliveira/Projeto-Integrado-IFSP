using System.Security.Claims;
using Estoque.Domain.Entities;
using Estoque.Infrastructure.Data;
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
        if (string.IsNullOrEmpty(userId))
            userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        var httpContext = httpContextAccessor.HttpContext;
        var user = httpContext?.User;

        var ip = httpContext?.Connection.RemoteIpAddress?.ToString();

        var log = new AuditLog
        {
            UserId = userId,
            UserName = userName,
            IpAddress = ip,
            AccessedAt = DateTime.Now,
            Area = area,
            Action = action,
            Details = details
        };

        context.AuditLogs.Add(log);
        await context.SaveChangesAsync();
    }

}